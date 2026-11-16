using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class BodyClone2D
{
	private static ResourcePoolShapeClone2D poolClone2D = new ResourcePoolShapeClone2D();

	public FP _angularDamping;

	public BodyType _bodyType;

	public FP _inertia;

	public FP _linearDamping;

	public FP _mass;

	public bool _sleepingAllowed = true;

	public bool _awake = true;

	public bool _fixedRotation;

	public bool _enabled = true;

	public FP _angularVelocity;

	public TSVector2 _linearVelocity;

	public TSVector2 _force;

	public FP _invI;

	public FP _invMass;

	public FP _sleepTime;

	public Sweep _sweep = default;

	public FP _torque;

	public Transform _xf = default;

	public bool _island;

	public bool disabled;

	public FP GravityScale;

	public bool IsBullet;

	public bool IgnoreCCD;

	public bool IgnoreGravity;

	public FP prevKinematicMass;

	public FP prevKinematicInvMass;

	public FP prevKinematicInertia;

	public FP prevKinematicInvI;

	public Sweep prevKinematicSweep;

	public ContactEdgeClone2D contactEdgeClone;

	public List<GenericShapeClone2D> shapesClone = new List<GenericShapeClone2D>();

	public void Reset()
	{
		if (contactEdgeClone != null)
		{
			WorldClone2D.poolContactEdgeClone.GiveBack(contactEdgeClone);
		}
	}

	public void Clone(Body body)
	{
		_angularDamping = body._angularDamping;
		_bodyType = body._bodyType;
		_inertia = body._inertia;
		_linearDamping = body._linearDamping;
		_mass = body._mass;
		_sleepingAllowed = body._sleepingAllowed;
		_awake = body._awake;
		_fixedRotation = body._fixedRotation;
		_enabled = body._enabled;
		_angularVelocity = body._angularVelocity;
		_linearVelocity = body._linearVelocity;
		_force = body._force;
		_invI = body._invI;
		_invMass = body._invMass;
		_sleepTime = body._sleepTime;
		_sweep.A = body._sweep.A;
		_sweep.A0 = body._sweep.A0;
		_sweep.Alpha0 = body._sweep.Alpha0;
		_sweep.C = body._sweep.C;
		_sweep.C0 = body._sweep.C0;
		_sweep.LocalCenter = body._sweep.LocalCenter;
		_torque = body._torque;
		_xf.p = body._xf.p;
		_xf.q = body._xf.q;
		_island = body._island;
		disabled = body.disabled;
		GravityScale = body.GravityScale;
		IsBullet = body.IsBullet;
		IgnoreCCD = body.IgnoreCCD;
		IgnoreGravity = body.IgnoreGravity;
		prevKinematicMass = body.prevKinematicMass;
		prevKinematicInvMass = body.prevKinematicInvMass;
		prevKinematicInertia = body.prevKinematicInertia;
		prevKinematicInvI = body.prevKinematicInvI;
		prevKinematicSweep = body.prevKinematicSweep;
		int i = 0;
		for (int count = shapesClone.Count; i < count; i++)
		{
			poolClone2D.GiveBack(shapesClone[i]);
		}
		shapesClone.Clear();
		List<Fixture> fixtureList = body.FixtureList;
		int j = 0;
		for (int count2 = fixtureList.Count; j < count2; j++)
		{
			GenericShapeClone2D genericShapeClone2D = poolClone2D.GetNew();
			genericShapeClone2D.Clone(body.FixtureList[j].Shape);
			shapesClone.Add(genericShapeClone2D);
		}
		if (body.ContactList == null)
		{
			contactEdgeClone = null;
			return;
		}
		contactEdgeClone = WorldClone2D.poolContactEdgeClone.GetNew();
		contactEdgeClone.Clone(body.ContactList);
	}

	public void Restore(Body body)
	{
		body._angularDamping = _angularDamping;
		body._bodyType = _bodyType;
		body._inertia = _inertia;
		body._linearDamping = _linearDamping;
		body._mass = _mass;
		body._sleepingAllowed = _sleepingAllowed;
		body._awake = _awake;
		body._fixedRotation = _fixedRotation;
		body._enabled = _enabled;
		body._angularVelocity = _angularVelocity;
		body._linearVelocity = _linearVelocity;
		body._force = _force;
		body._invI = _invI;
		body._invMass = _invMass;
		body._sleepTime = _sleepTime;
		body._sweep = _sweep;
		body._torque = _torque;
		body._xf.p = _xf.p;
		body._xf.q = _xf.q;
		body._island = _island;
		bool flag = body.disabled;
		body.disabled = disabled;
		if (flag && !body.disabled)
		{
			ContactManager.physicsManager.GetGameObject(body).SetActive(value: true);
		}
		body.GravityScale = GravityScale;
		body.IsBullet = IsBullet;
		body.IgnoreCCD = IgnoreCCD;
		body.IgnoreGravity = IgnoreGravity;
		body.prevKinematicMass = prevKinematicMass;
		body.prevKinematicInvMass = prevKinematicInvMass;
		body.prevKinematicInertia = prevKinematicInertia;
		body.prevKinematicInvI = prevKinematicInvI;
		body.prevKinematicSweep = prevKinematicSweep;
		List<Fixture> fixtureList = body.FixtureList;
		int i = 0;
		for (int count = shapesClone.Count; i < count; i++)
		{
			shapesClone[i].Restore(fixtureList[i].Shape);
		}
	}
}
