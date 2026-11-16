using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class Body : IDisposable, IBody2D, IBody
{
	[ThreadStatic]
	internal static int _bodyIdCounter;

	internal BodySpecialSensor _specialSensor;

	public int SpecialSensorMask = -1;

	public List<Body> _specialSensorResults;

	internal FP _angularDamping;

	internal BodyType _bodyType;

	internal FP _inertia;

	internal FP _linearDamping;

	internal FP _mass;

	internal bool _sleepingAllowed = true;

	internal bool _awake = true;

	internal bool _fixedRotation;

	internal bool _enabled = true;

	internal FP _angularVelocity;

	internal TSVector2 _linearVelocity;

	internal TSVector2 _force;

	internal FP _invI;

	internal FP _invMass;

	internal FP _sleepTime;

	internal Sweep _sweep;

	internal FP _torque;

	internal World _world;

	internal Transform _xf;

	internal bool _island;

	internal bool disabled;

	internal FP prevKinematicMass;

	internal FP prevKinematicInvMass;

	internal FP prevKinematicInertia;

	internal FP prevKinematicInvI;

	internal Sweep prevKinematicSweep;

	public List<IBodyConstraint> bodyConstraints;

	public PhysicsLogicFilter PhysicsLogicFilter;

	public ControllerFilter ControllerFilter;

	public BodySpecialSensor SpecialSensor
	{
		get
		{
			return _specialSensor;
		}
		set
		{
			_specialSensor = value;
			if (value != BodySpecialSensor.None)
			{
				_specialSensorResults = new List<Body>();
			}
		}
	}

	public int BodyId { get; private set; }

	public int IslandIndex { get; set; }

	public FP GravityScale { get; set; }

	public object UserData { get; set; }

	public FP Revolutions => Rotation / FP.Pi;

	public BodyType BodyType
	{
		get
		{
			return _bodyType;
		}
		set
		{
			if (_bodyType == value)
			{
				return;
			}
			BodyType bodyType = _bodyType;
			_bodyType = value;
			if (bodyType == BodyType.Kinematic)
			{
				_mass = prevKinematicMass;
				_invMass = prevKinematicInvMass;
				_inertia = prevKinematicInertia;
				_invI = prevKinematicInvI;
				_sweep = prevKinematicSweep;
			}
			else
			{
				prevKinematicMass = _mass;
				prevKinematicInvMass = _invMass;
				prevKinematicInertia = _inertia;
				prevKinematicInvI = _invI;
				prevKinematicSweep = _sweep;
				ResetMassData();
			}
			if (_bodyType != BodyType.Dynamic)
			{
				_linearVelocity = TSVector2.zero;
				_angularVelocity = 0f;
				_sweep.A0 = _sweep.A;
				_sweep.C0 = _sweep.C;
				SynchronizeFixtures();
			}
			Awake = true;
			_force = TSVector2.zero;
			_torque = 0f;
			ContactEdge contactEdge = ContactList;
			while (contactEdge != null)
			{
				ContactEdge contactEdge2 = contactEdge;
				contactEdge = contactEdge.Next;
				_world.ContactManager.Destroy(contactEdge2.Contact);
			}
			ContactList = null;
			IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
			foreach (Fixture fixture in FixtureList)
			{
				int proxyCount = fixture.ProxyCount;
				for (int i = 0; i < proxyCount; i++)
				{
					broadPhase.TouchProxy(fixture.Proxies[i].ProxyId);
				}
			}
		}
	}

	public TSVector2 LinearVelocity
	{
		get
		{
			return _linearVelocity;
		}
		set
		{
			if (_bodyType != BodyType.Static)
			{
				if (TSVector2.Dot(value, value) > 0f)
				{
					Awake = true;
				}
				_linearVelocity = value;
			}
		}
	}

	public FP AngularVelocity
	{
		get
		{
			return _angularVelocity;
		}
		set
		{
			if (_bodyType != BodyType.Static)
			{
				if (value * value > 0f)
				{
					Awake = true;
				}
				_angularVelocity = value;
			}
		}
	}

	public FP LinearDamping
	{
		get
		{
			return _linearDamping;
		}
		set
		{
			_linearDamping = value;
		}
	}

	public FP AngularDamping
	{
		get
		{
			return _angularDamping;
		}
		set
		{
			_angularDamping = value;
		}
	}

	public bool IsBullet { get; set; }

	public bool SleepingAllowed
	{
		get
		{
			return _sleepingAllowed;
		}
		set
		{
			if (!value)
			{
				Awake = true;
			}
			_sleepingAllowed = value;
		}
	}

	public bool Awake
	{
		get
		{
			return _awake;
		}
		set
		{
			if (value)
			{
				if (!_awake)
				{
					_sleepTime = 0f;
					_world.ContactManager.UpdateContacts(ContactList, value: true);
				}
			}
			else
			{
				ResetDynamics();
				_sleepTime = 0f;
				_world.ContactManager.UpdateContacts(ContactList, value: false);
			}
			_awake = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			if (value == _enabled)
			{
				return;
			}
			if (value)
			{
				IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
				for (int i = 0; i < FixtureList.Count; i++)
				{
					FixtureList[i].CreateProxies(broadPhase, ref _xf);
				}
			}
			else
			{
				IBroadPhase broadPhase2 = _world.ContactManager.BroadPhase;
				for (int j = 0; j < FixtureList.Count; j++)
				{
					FixtureList[j].DestroyProxies(broadPhase2);
				}
				ContactEdge contactEdge = ContactList;
				while (contactEdge != null)
				{
					ContactEdge contactEdge2 = contactEdge;
					contactEdge = contactEdge.Next;
					_world.ContactManager.Destroy(contactEdge2.Contact);
				}
				ContactList = null;
			}
			_enabled = value;
		}
	}

	public bool FixedRotation
	{
		get
		{
			return _fixedRotation;
		}
		set
		{
			if (_fixedRotation != value)
			{
				_fixedRotation = value;
				_angularVelocity = 0f;
				ResetMassData();
			}
		}
	}

	public List<Fixture> FixtureList { get; internal set; }

	public JointEdge JointList { get; internal set; }

	public ContactEdge ContactList { get; internal set; }

	public TSVector2 Position
	{
		get
		{
			return _xf.p;
		}
		set
		{
			SetTransform(ref value, Rotation);
		}
	}

	public FP Rotation
	{
		get
		{
			return _sweep.A;
		}
		set
		{
			SetTransform(ref _xf.p, value);
		}
	}

	public bool IsStatic
	{
		get
		{
			return _bodyType == BodyType.Static;
		}
		set
		{
			BodyType = ((!value) ? BodyType.Dynamic : BodyType.Static);
		}
	}

	public bool TSIsStatic
	{
		get
		{
			return IsStatic;
		}
		set
		{
			IsStatic = value;
		}
	}

	public bool IsKinematic
	{
		get
		{
			return _bodyType == BodyType.Kinematic;
		}
		set
		{
			BodyType = (value ? BodyType.Kinematic : BodyType.Dynamic);
		}
	}

	public bool IgnoreGravity { get; set; }

	public TSVector2 WorldCenter => _sweep.C;

	public TSVector2 LocalCenter
	{
		get
		{
			return _sweep.LocalCenter;
		}
		set
		{
			if (_bodyType == BodyType.Dynamic)
			{
				TSVector2 c = _sweep.C;
				_sweep.LocalCenter = value;
				_sweep.C0 = (_sweep.C = MathUtils.Mul(ref _xf, ref _sweep.LocalCenter));
				TSVector2 tSVector = _sweep.C - c;
				_linearVelocity += new TSVector2(-_angularVelocity * tSVector.y, _angularVelocity * tSVector.x);
			}
		}
	}

	public FP Mass
	{
		get
		{
			return _mass;
		}
		set
		{
			if (_bodyType == BodyType.Dynamic)
			{
				_mass = value;
				if (_mass <= 0f)
				{
					_mass = 1f;
				}
				_invMass = 1f / _mass;
				prevKinematicMass = _mass;
				prevKinematicInvMass = _invMass;
			}
		}
	}

	public FP Inertia
	{
		get
		{
			return _inertia + Mass * TSVector2.Dot(_sweep.LocalCenter, _sweep.LocalCenter);
		}
		set
		{
			if (_bodyType == BodyType.Dynamic && value > 0f && !_fixedRotation)
			{
				_inertia = value - Mass * TSVector2.Dot(LocalCenter, LocalCenter);
				_invI = 1f / _inertia;
			}
		}
	}

	public FP Restitution
	{
		get
		{
			FP fP = 0;
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fP += fixture.Restitution;
			}
			return (FixtureList.Count <= 0) ? ((FP)0) : (fP / FixtureList.Count);
		}
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.Restitution = value;
			}
		}
	}

	public FP Friction
	{
		get
		{
			FP fP = 0;
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fP += fixture.Friction;
			}
			return (FixtureList.Count <= 0) ? ((FP)0) : (fP / FixtureList.Count);
		}
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.Friction = value;
			}
		}
	}

	public Category CollisionCategories
	{
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.CollisionCategories = value;
			}
		}
	}

	public Category CollidesWith
	{
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.CollidesWith = value;
			}
		}
	}

	public Category IgnoreCCDWith
	{
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.IgnoreCCDWith = value;
			}
		}
	}

	public short CollisionGroup
	{
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.CollisionGroup = value;
			}
		}
	}

	public bool IsSensor
	{
		get
		{
			return FixtureList[0].IsSensor;
		}
		set
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.IsSensor = value;
			}
		}
	}

	public bool IgnoreCCD { get; set; }

	public bool IsDisposed { get; set; }

	public bool TSAffectedByGravity
	{
		get
		{
			return !IgnoreGravity;
		}
		set
		{
			IgnoreGravity = !value;
		}
	}

	public TSVector2 TSLinearVelocity
	{
		get
		{
			return LinearVelocity;
		}
		set
		{
			LinearVelocity = value;
		}
	}

	public FP TSAngularVelocity
	{
		get
		{
			return AngularVelocity;
		}
		set
		{
			AngularVelocity = value;
		}
	}

	public bool TSIsKinematic
	{
		get
		{
			return IsKinematic;
		}
		set
		{
			IsKinematic = value;
		}
	}

	public bool TSDisabled
	{
		get
		{
			return disabled;
		}
		set
		{
			disabled = value;
		}
	}

	public TSVector2 TSPosition
	{
		get
		{
			return Position;
		}
		set
		{
			Position = value;
		}
	}

	public FP TSOrientation
	{
		get
		{
			return Rotation;
		}
		set
		{
			Rotation = value;
		}
	}

	public FP TSLinearDrag
	{
		get
		{
			return LinearDamping;
		}
		set
		{
			LinearDamping = value;
		}
	}

	public FP TSAngularDrag
	{
		get
		{
			return AngularDamping;
		}
		set
		{
			AngularDamping = value;
		}
	}

	public FP TSFriction
	{
		get
		{
			return FixtureList[0].Friction;
		}
		set
		{
			List<Fixture> fixtureList = FixtureList;
			int i = 0;
			for (int count = fixtureList.Count; i < count; i++)
			{
				fixtureList[i].Friction = value;
			}
		}
	}

	public FP TSRestitution
	{
		get
		{
			return FixtureList[0].Restitution;
		}
		set
		{
			List<Fixture> fixtureList = FixtureList;
			int i = 0;
			for (int count = fixtureList.Count; i < count; i++)
			{
				fixtureList[i].Restitution = value;
			}
		}
	}

	public event OnCollisionEventHandler OnCollision
	{
		add
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.OnCollision = (OnCollisionEventHandler)Delegate.Combine(fixture.OnCollision, value);
			}
		}
		remove
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.OnCollision = (OnCollisionEventHandler)Delegate.Remove(fixture.OnCollision, value);
			}
		}
	}

	public event OnSeparationEventHandler OnSeparation
	{
		add
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.OnSeparation = (OnSeparationEventHandler)Delegate.Combine(fixture.OnSeparation, value);
			}
		}
		remove
		{
			for (int i = 0; i < FixtureList.Count; i++)
			{
				Fixture fixture = FixtureList[i];
				fixture.OnSeparation = (OnSeparationEventHandler)Delegate.Remove(fixture.OnSeparation, value);
			}
		}
	}

	public Body(World world, TSVector2? position, FP rotation, object userdata = null)
	{
		FixtureList = new List<Fixture>();
		bodyConstraints = new List<IBodyConstraint>();
		BodyId = _bodyIdCounter++;
		_world = world;
		UserData = userdata;
		GravityScale = 1f;
		BodyType = BodyType.Static;
		Enabled = true;
		_xf.q.Set(rotation);
		if (position.HasValue)
		{
			_xf.p = position.Value;
			_sweep.C0 = _xf.p;
			_sweep.C = _xf.p;
			_sweep.A0 = rotation;
			_sweep.A = rotation;
		}
		world.AddBody(this);
	}

	public void ResetDynamics()
	{
		_torque = 0;
		_angularVelocity = 0;
		_force = TSVector2.zero;
		_linearVelocity = TSVector2.zero;
	}

	public Fixture CreateFixture(Shape shape, object userData = null)
	{
		return new Fixture(this, shape, userData);
	}

	public void DestroyFixture(Fixture fixture)
	{
		ContactEdge contactEdge = ContactList;
		while (contactEdge != null)
		{
			Contact contact = contactEdge.Contact;
			contactEdge = contactEdge.Next;
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			if (fixture == fixtureA || fixture == fixtureB)
			{
				_world.ContactManager.Destroy(contact);
			}
		}
		if (_enabled)
		{
			IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
			fixture.DestroyProxies(broadPhase);
		}
		FixtureList.Remove(fixture);
		fixture.Destroy();
		fixture.Body = null;
		ResetMassData();
	}

	public void SetTransform(ref TSVector2 position, FP rotation)
	{
		SetTransformIgnoreContacts(ref position, rotation);
		_world.ContactManager.FindNewContacts();
	}

	public void SetTransform(TSVector2 position, FP rotation)
	{
		SetTransform(ref position, rotation);
	}

	public void SetTransformIgnoreContacts(ref TSVector2 position, FP angle)
	{
		_xf.q.Set(angle);
		_xf.p = position;
		_sweep.C = MathUtils.Mul(ref _xf, _sweep.LocalCenter);
		_sweep.A = angle;
		_sweep.C0 = _sweep.C;
		_sweep.A0 = angle;
		prevKinematicSweep = _sweep;
		IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
		for (int i = 0; i < FixtureList.Count; i++)
		{
			FixtureList[i].Synchronize(broadPhase, ref _xf, ref _xf);
		}
	}

	public void GetTransform(out Transform transform)
	{
		transform = _xf;
	}

	public void ApplyForce(TSVector2 force, TSVector2 point)
	{
		ApplyForce(ref force, ref point);
	}

	public void ApplyForce(ref TSVector2 force)
	{
		ApplyForce(ref force, ref _xf.p);
	}

	public void ApplyForce(TSVector2 force)
	{
		ApplyForce(ref force, ref _xf.p);
	}

	public void ApplyForce(ref TSVector2 force, ref TSVector2 point)
	{
		if (_bodyType == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			_force += force;
			_torque += (point.x - _sweep.C.x) * force.y - (point.y - _sweep.C.y) * force.x;
		}
	}

	public void ApplyTorque(FP torque)
	{
		if (_bodyType == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			_torque += torque;
		}
	}

	public void ApplyLinearImpulse(TSVector2 impulse)
	{
		ApplyLinearImpulse(ref impulse);
	}

	public void ApplyLinearImpulse(TSVector2 impulse, TSVector2 point)
	{
		ApplyLinearImpulse(ref impulse, ref point);
	}

	public void ApplyLinearImpulse(ref TSVector2 impulse)
	{
		if (_bodyType == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			_linearVelocity += _invMass * impulse;
		}
	}

	public void ApplyLinearImpulse(ref TSVector2 impulse, ref TSVector2 point)
	{
		if (_bodyType == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			_linearVelocity += _invMass * impulse;
			_angularVelocity += _invI * ((point.x - _sweep.C.x) * impulse.y - (point.y - _sweep.C.y) * impulse.x);
		}
	}

	public void ApplyAngularImpulse(FP impulse)
	{
		if (_bodyType == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			_angularVelocity += _invI * impulse;
		}
	}

	public void ResetMassData()
	{
		_mass = 0f;
		_invMass = 0f;
		_inertia = 0f;
		_invI = 0f;
		_sweep.LocalCenter = TSVector2.zero;
		if (BodyType == BodyType.Kinematic)
		{
			_sweep.C0 = _xf.p;
			_sweep.C = _xf.p;
			_sweep.A0 = _sweep.A;
			return;
		}
		TSVector2 zero = TSVector2.zero;
		foreach (Fixture fixture in FixtureList)
		{
			if (!(fixture.Shape._density == 0))
			{
				MassData massData = fixture.Shape.MassData;
				_mass += massData.Mass;
				zero += massData.Mass * massData.Centroid;
				_inertia += massData.Inertia;
			}
		}
		if (BodyType == BodyType.Static)
		{
			_sweep.C0 = (_sweep.C = _xf.p);
			return;
		}
		if (_mass > 0f)
		{
			_invMass = 1f / _mass;
			zero *= _invMass;
		}
		else
		{
			_mass = 1f;
			_invMass = 1f;
		}
		if (_inertia > 0f && !_fixedRotation)
		{
			_inertia -= _mass * TSVector2.Dot(zero, zero);
			_invI = 1f / _inertia;
		}
		else
		{
			_inertia = 0f;
			_invI = 0f;
		}
		TSVector2 c = _sweep.C;
		_sweep.LocalCenter = zero;
		_sweep.C0 = (_sweep.C = MathUtils.Mul(ref _xf, ref _sweep.LocalCenter));
		TSVector2 tSVector = _sweep.C - c;
		_linearVelocity += new TSVector2(-_angularVelocity * tSVector.y, _angularVelocity * tSVector.x);
	}

	public TSVector2 GetWorldPoint(ref TSVector2 localPoint)
	{
		return MathUtils.Mul(ref _xf, ref localPoint);
	}

	public TSVector2 GetWorldPoint(TSVector2 localPoint)
	{
		return GetWorldPoint(ref localPoint);
	}

	public TSVector2 GetWorldVector(ref TSVector2 localVector)
	{
		return MathUtils.Mul(_xf.q, localVector);
	}

	public TSVector2 GetWorldVector(TSVector2 localVector)
	{
		return GetWorldVector(ref localVector);
	}

	public TSVector2 GetLocalPoint(ref TSVector2 worldPoint)
	{
		return MathUtils.MulT(ref _xf, worldPoint);
	}

	public TSVector2 GetLocalPoint(TSVector2 worldPoint)
	{
		return GetLocalPoint(ref worldPoint);
	}

	public TSVector2 GetLocalVector(ref TSVector2 worldVector)
	{
		return MathUtils.MulT(_xf.q, worldVector);
	}

	public TSVector2 GetLocalVector(TSVector2 worldVector)
	{
		return GetLocalVector(ref worldVector);
	}

	public TSVector2 GetLinearVelocityFromWorldPoint(TSVector2 worldPoint)
	{
		return GetLinearVelocityFromWorldPoint(ref worldPoint);
	}

	public TSVector2 GetLinearVelocityFromWorldPoint(ref TSVector2 worldPoint)
	{
		return _linearVelocity + new TSVector2(-_angularVelocity * (worldPoint.y - _sweep.C.y), _angularVelocity * (worldPoint.x - _sweep.C.x));
	}

	public TSVector2 GetLinearVelocityFromLocalPoint(TSVector2 localPoint)
	{
		return GetLinearVelocityFromLocalPoint(ref localPoint);
	}

	public TSVector2 GetLinearVelocityFromLocalPoint(ref TSVector2 localPoint)
	{
		return GetLinearVelocityFromWorldPoint(GetWorldPoint(ref localPoint));
	}

	internal void SynchronizeFixtures()
	{
		Transform transform = default;
		transform.q.Set(_sweep.A0);
		transform.p = _sweep.C0 - MathUtils.Mul(transform.q, _sweep.LocalCenter);
		IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
		for (int i = 0; i < FixtureList.Count; i++)
		{
			FixtureList[i].Synchronize(broadPhase, ref transform, ref _xf);
		}
	}

	internal void SynchronizeTransform()
	{
		_xf.q.Set(_sweep.A);
		_xf.p = _sweep.C - MathUtils.Mul(_xf.q, _sweep.LocalCenter);
	}

	internal bool ShouldCollide(Body other)
	{
		if (_specialSensor != BodySpecialSensor.None || other._specialSensor != BodySpecialSensor.None)
		{
			return true;
		}
		if (_bodyType == BodyType.Static && other._bodyType == BodyType.Static)
		{
			return false;
		}
		for (JointEdge jointEdge = JointList; jointEdge != null; jointEdge = jointEdge.Next)
		{
			if (jointEdge.Other == other && !jointEdge.Joint.CollideConnected)
			{
				return false;
			}
		}
		return true;
	}

	internal void Advance(FP alpha)
	{
		_sweep.Advance(alpha);
		_sweep.C = _sweep.C0;
		_sweep.A = _sweep.A0;
		_xf.q.Set(_sweep.A);
		_xf.p = _sweep.C - MathUtils.Mul(_xf.q, _sweep.LocalCenter);
	}

	public void IgnoreCollisionWith(Body other)
	{
		for (int i = 0; i < FixtureList.Count; i++)
		{
			for (int j = 0; j < other.FixtureList.Count; j++)
			{
				FixtureList[i].IgnoreCollisionWith(other.FixtureList[j]);
			}
		}
	}

	public void RestoreCollisionWith(Body other)
	{
		for (int i = 0; i < FixtureList.Count; i++)
		{
			for (int j = 0; j < other.FixtureList.Count; j++)
			{
				FixtureList[i].RestoreCollisionWith(other.FixtureList[j]);
			}
		}
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			_world.RemoveBody(this);
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	public Body Clone(World world = null)
	{
		Body body = new Body(world ?? _world, Position, Rotation, UserData);
		body._bodyType = _bodyType;
		body._linearVelocity = _linearVelocity;
		body._angularVelocity = _angularVelocity;
		body.GravityScale = GravityScale;
		body.UserData = UserData;
		body._enabled = _enabled;
		body._fixedRotation = _fixedRotation;
		body._sleepingAllowed = _sleepingAllowed;
		body._linearDamping = _linearDamping;
		body._angularDamping = _angularDamping;
		body._awake = _awake;
		body.IsBullet = IsBullet;
		body.IgnoreCCD = IgnoreCCD;
		body.IgnoreGravity = IgnoreGravity;
		body._torque = _torque;
		return body;
	}

	public Body DeepClone(World world = null)
	{
		Body body = Clone(world ?? _world);
		int count = FixtureList.Count;
		for (int i = 0; i < count; i++)
		{
			FixtureList[i].CloneOnto(body);
		}
		return body;
	}

	public string Checkum()
	{
		return string.Concat(Position, "|", Rotation);
	}

	public void TSApplyForce(TSVector2 force)
	{
		ApplyForce(force);
	}

	public void TSApplyForce(TSVector2 force, TSVector2 position)
	{
		ApplyForce(force, position);
	}

	public void TSApplyImpulse(TSVector2 force)
	{
		ApplyLinearImpulse(force, force);
	}

	public void TSApplyImpulse(TSVector2 force, TSVector2 position)
	{
		ApplyLinearImpulse(force, position);
	}

	public void TSApplyTorque(TSVector2 force)
	{
		throw new NotImplementedException();
	}

	public void TSUpdate()
	{
	}
}
