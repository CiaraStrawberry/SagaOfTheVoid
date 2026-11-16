using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class RigidBody : IBroadphaseEntity, IDebugDrawable, IEquatable<RigidBody>, IComparable, IBody3D, IBody
{
	[Flags]
	public enum DampingType
	{
		None = 0,
		Angular = 1,
		Linear = 2
	}

	internal TSMatrix inertia;

	internal TSMatrix invInertia;

	internal TSMatrix invInertiaWorld;

	internal TSMatrix orientation;

	internal TSMatrix invOrientation;

	internal TSVector position;

	internal TSVector linearVelocity;

	internal TSVector angularVelocity;

	internal FP staticFriction;

	internal FP restitution;

	internal TSBBox boundingBox;

	internal FP inactiveTime = FP.Zero;

	internal bool isActive = true;

	internal bool isStatic;

	internal bool isKinematic;

	internal bool affectedByGravity = true;

	internal bool isColliderOnly;

	internal CollisionIsland island;

	internal FP inverseMass;

	internal TSVector force;

	internal TSVector torque;

	private int hashCode;

	internal int internalIndex;

	private ShapeUpdatedHandler updatedHandler;

	internal List<RigidBody> connections = new List<RigidBody>();

	internal HashList<Arbiter> arbiters = new HashList<Arbiter>();

	internal HashList<Arbiter> arbitersTrigger = new HashList<Arbiter>();

	internal HashList<Constraint> constraints = new HashList<Constraint>();

	private ReadOnlyHashset<Arbiter> readOnlyArbiters;

	private ReadOnlyHashset<Constraint> readOnlyConstraints;

	internal int marker;

	internal bool disabled;

	internal TSRigidBodyConstraints _freezeConstraints;

	internal TSVector _freezePosition = TSVector.zero;

	internal TSMatrix _freezeRotation = TSMatrix.Identity;

	internal TSQuaternion _freezeRotationQuat = TSQuaternion.identity;

	internal bool prevKinematicGravity;

	internal FP linearDrag;

	internal FP angularDrag;

	internal bool isParticle;

	internal static int instanceCount;

	private int instance;

	protected bool useShapeMassProperties = true;

	internal Shape shape;

	private DampingType damping = DampingType.Angular | DampingType.Linear;

	internal TSVector sweptDirection = TSVector.zero;

	private bool enableDebugDraw;

	private List<TSVector> hullPoints = new List<TSVector>();

	public TSRigidBodyConstraints FreezeConstraints
	{
		get
		{
			return _freezeConstraints;
		}
		set
		{
			if (_freezeConstraints != value)
			{
				_freezeConstraints = value;
				_freezePosition = position;
				if (_freezeRotation != orientation)
				{
					_freezeRotation = orientation;
					_freezeRotationQuat = TSQuaternion.CreateFromMatrix(_freezeRotation);
				}
			}
		}
	}

	public bool Disabled => disabled;

	public bool IsParticle
	{
		get
		{
			return isParticle;
		}
		set
		{
			if (isParticle && !value)
			{
				updatedHandler = ShapeUpdated;
				Shape.ShapeUpdated += updatedHandler;
				SetMassProperties();
				isParticle = false;
			}
			else if (!isParticle && value)
			{
				inertia = TSMatrix.Zero;
				invInertia = (invInertiaWorld = TSMatrix.Zero);
				invOrientation = (orientation = TSMatrix.Identity);
				inverseMass = FP.One;
				Shape.ShapeUpdated -= updatedHandler;
				isParticle = true;
			}
			Update();
		}
	}

	internal ReadOnlyHashset<Arbiter> Arbiters => readOnlyArbiters;

	internal ReadOnlyHashset<Constraint> Constraints => readOnlyConstraints;

	internal bool AllowDeactivation { get; set; }

	internal bool EnableSpeculativeContacts { get; set; }

	public TSBBox BoundingBox => boundingBox;

	internal CollisionIsland CollisionIsland => island;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			if (!isActive && value)
			{
				inactiveTime = FP.Zero;
			}
			else if (isActive && !value)
			{
				inactiveTime = FP.PositiveInfinity;
				angularVelocity.MakeZero();
				linearVelocity.MakeZero();
			}
			isActive = value;
		}
	}

	public bool IsColliderOnly
	{
		get
		{
			return isColliderOnly;
		}
		set
		{
			isColliderOnly = value;
		}
	}

	public TSVector Torque => torque;

	public TSVector Force => force;

	public Shape Shape
	{
		get
		{
			return shape;
		}
		set
		{
			if (shape != null)
			{
				shape.ShapeUpdated -= updatedHandler;
			}
			shape = value;
			shape.ShapeUpdated += ShapeUpdated;
		}
	}

	public DampingType Damping
	{
		get
		{
			return damping;
		}
		set
		{
			damping = value;
		}
	}

	public TSMatrix Inertia => inertia;

	public TSMatrix InverseInertia => invInertia;

	public TSVector LinearVelocity
	{
		get
		{
			return linearVelocity;
		}
		set
		{
			if (!isStatic)
			{
				linearVelocity = value;
			}
		}
	}

	public TSVector AngularVelocity
	{
		get
		{
			return angularVelocity;
		}
		set
		{
			if (!isStatic)
			{
				angularVelocity = value;
			}
		}
	}

	public TSVector Position
	{
		get
		{
			return position;
		}
		set
		{
			if ((_freezeConstraints & TSRigidBodyConstraints.FreezePosition) > TSRigidBodyConstraints.None)
			{
				_freezePosition = value;
			}
			position = value;
			Update();
		}
	}

	public TSMatrix Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			if ((_freezeConstraints & TSRigidBodyConstraints.FreezeRotation) > TSRigidBodyConstraints.None)
			{
				_freezeRotation = value;
				_freezeRotationQuat = TSQuaternion.CreateFromMatrix(_freezeRotation);
			}
			orientation = value;
			Update();
		}
	}

	public bool IsStatic
	{
		get
		{
			return isStatic;
		}
		set
		{
			if (value && !isStatic)
			{
				if (island != null)
				{
					island.islandManager.MakeBodyStatic(this);
				}
				angularVelocity.MakeZero();
				linearVelocity.MakeZero();
			}
			isStatic = value;
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
			return isKinematic;
		}
		set
		{
			if (isKinematic != value)
			{
				isKinematic = value;
				if (isKinematic)
				{
					prevKinematicGravity = affectedByGravity;
					affectedByGravity = false;
					IsStatic = true;
				}
				else
				{
					affectedByGravity = prevKinematicGravity;
					IsStatic = false;
				}
			}
		}
	}

	public bool AffectedByGravity
	{
		get
		{
			return affectedByGravity;
		}
		set
		{
			affectedByGravity = value;
			prevKinematicGravity = value;
		}
	}

	public TSMatrix InverseInertiaWorld => invInertiaWorld;

	public FP Mass
	{
		get
		{
			return FP.One / inverseMass;
		}
		set
		{
			if (value <= FP.Zero)
			{
				throw new ArgumentException("Mass can't be less or equal zero.");
			}
			if (!isParticle)
			{
				TSMatrix.Multiply(ref Shape.inertia, value / Shape.mass, out inertia);
				TSMatrix.Inverse(ref inertia, out invInertia);
			}
			inverseMass = FP.One / value;
		}
	}

	internal int BroadphaseTag { get; set; }

	public bool IsStaticOrInactive => !isActive || isStatic;

	public bool IsStaticNonKinematic => !isActive || (isStatic && !isKinematic);

	internal bool EnableDebugDraw
	{
		get
		{
			return enableDebugDraw;
		}
		set
		{
			enableDebugDraw = value;
			UpdateHullData();
		}
	}

	public bool TSAffectedByGravity
	{
		get
		{
			return AffectedByGravity;
		}
		set
		{
			AffectedByGravity = value;
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

	public TSVector TSLinearVelocity
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

	public TSVector TSAngularVelocity
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

	public bool TSDisabled
	{
		get
		{
			return Disabled;
		}
		set
		{
			disabled = true;
		}
	}

	public TSVector TSPosition
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

	public TSMatrix TSOrientation
	{
		get
		{
			return Orientation;
		}
		set
		{
			Orientation = value;
		}
	}

	public FP TSLinearDrag
	{
		get
		{
			return linearDrag;
		}
		set
		{
			linearDrag = value;
		}
	}

	public FP TSAngularDrag
	{
		get
		{
			return angularDrag;
		}
		set
		{
			angularDrag = value;
		}
	}

	public FP TSFriction
	{
		get
		{
			return staticFriction;
		}
		set
		{
			staticFriction = value;
		}
	}

	public FP TSRestitution
	{
		get
		{
			return restitution;
		}
		set
		{
			restitution = value;
		}
	}

	public RigidBody(Shape shape)
		: this(shape, new BodyMaterial(), isParticle: false)
	{
	}

	public RigidBody(Shape shape, BodyMaterial material)
		: this(shape, material, isParticle: false)
	{
	}

	public RigidBody(Shape shape, BodyMaterial material, bool isParticle)
	{
		readOnlyArbiters = new ReadOnlyHashset<Arbiter>(arbiters);
		readOnlyConstraints = new ReadOnlyHashset<Constraint>(constraints);
		instanceCount++;
		instance = instanceCount;
		hashCode = CalculateHash(instance);
		Shape = shape;
		orientation = TSMatrix.Identity;
		if (!isParticle)
		{
			updatedHandler = ShapeUpdated;
			Shape.ShapeUpdated += updatedHandler;
			SetMassProperties();
		}
		else
		{
			inertia = TSMatrix.Zero;
			invInertia = (invInertiaWorld = TSMatrix.Zero);
			invOrientation = (orientation = TSMatrix.Identity);
			inverseMass = FP.One;
		}
		AllowDeactivation = true;
		EnableSpeculativeContacts = false;
		this.isParticle = isParticle;
		Update();
	}

	public override int GetHashCode()
	{
		return hashCode;
	}

	private int CalculateHash(int a)
	{
		a = a ^ 0x3D ^ (a >> 16);
		a += a << 3;
		a ^= a >> 4;
		a *= 668265261;
		a ^= a >> 15;
		return a;
	}

	public void ApplyImpulse(TSVector impulse)
	{
		if (!isStatic)
		{
			TSVector.Multiply(ref impulse, inverseMass, out var result);
			TSVector.Add(ref linearVelocity, ref result, out linearVelocity);
		}
	}

	public void ApplyImpulse(TSVector impulse, TSVector relativePosition)
	{
		if (!isStatic)
		{
			TSVector.Multiply(ref impulse, inverseMass, out var result);
			TSVector.Add(ref linearVelocity, ref result, out linearVelocity);
			TSVector.Cross(ref relativePosition, ref impulse, out result);
			TSVector.Transform(ref result, ref invInertiaWorld, out result);
			TSVector.Add(ref angularVelocity, ref result, out angularVelocity);
		}
	}

	public void AddForce(TSVector force)
	{
		TSVector.Add(ref force, ref this.force, out this.force);
	}

	public void AddForce(TSVector force, TSVector pos)
	{
		TSVector.Add(ref this.force, ref force, out this.force);
		TSVector.Subtract(ref pos, ref position, out pos);
		TSVector.Cross(ref pos, ref force, out pos);
		TSVector.Add(ref pos, ref torque, out torque);
	}

	public void AddTorque(TSVector torque)
	{
		TSVector.Add(ref torque, ref this.torque, out this.torque);
	}

	public void SetMassProperties()
	{
		inertia = Shape.inertia;
		TSMatrix.Inverse(ref inertia, out invInertia);
		inverseMass = FP.One / Shape.mass;
		useShapeMassProperties = true;
	}

	public void SetMassProperties(TSMatrix inertia, FP mass, bool setAsInverseValues)
	{
		if (setAsInverseValues)
		{
			if (!isParticle)
			{
				invInertia = inertia;
				TSMatrix.Inverse(ref inertia, out this.inertia);
			}
			inverseMass = mass;
		}
		else
		{
			if (!isParticle)
			{
				this.inertia = inertia;
				TSMatrix.Inverse(ref inertia, out invInertia);
			}
			inverseMass = FP.One / mass;
		}
		useShapeMassProperties = false;
		Update();
	}

	private void ShapeUpdated()
	{
		if (useShapeMassProperties)
		{
			SetMassProperties();
		}
		Update();
		UpdateHullData();
	}

	internal void SweptExpandBoundingBox(FP timestep)
	{
		sweptDirection = linearVelocity * timestep;
		if (sweptDirection.x < FP.Zero)
		{
			boundingBox.min.x += sweptDirection.x;
		}
		else
		{
			boundingBox.max.x += sweptDirection.x;
		}
		if (sweptDirection.y < FP.Zero)
		{
			boundingBox.min.y += sweptDirection.y;
		}
		else
		{
			boundingBox.max.y += sweptDirection.y;
		}
		if (sweptDirection.z < FP.Zero)
		{
			boundingBox.min.z += sweptDirection.z;
		}
		else
		{
			boundingBox.max.z += sweptDirection.z;
		}
	}

	public virtual void Update()
	{
		if (isParticle)
		{
			inertia = TSMatrix.Zero;
			invInertia = (invInertiaWorld = TSMatrix.Zero);
			invOrientation = (orientation = TSMatrix.Identity);
			boundingBox = shape.boundingBox;
			TSVector.Add(ref boundingBox.min, ref position, out boundingBox.min);
			TSVector.Add(ref boundingBox.max, ref position, out boundingBox.max);
			angularVelocity.MakeZero();
			return;
		}
		TSMatrix.Transpose(ref orientation, out invOrientation);
		Shape.GetBoundingBox(ref orientation, out boundingBox);
		TSVector.Add(ref boundingBox.min, ref position, out boundingBox.min);
		TSVector.Add(ref boundingBox.max, ref position, out boundingBox.max);
		if (!isStatic)
		{
			TSMatrix.Multiply(ref invOrientation, ref invInertia, out invInertiaWorld);
			TSMatrix.Multiply(ref invInertiaWorld, ref orientation, out invInertiaWorld);
		}
	}

	public bool Equals(RigidBody other)
	{
		return other.instance == instance;
	}

	public int CompareTo(object otherObj)
	{
		RigidBody rigidBody = (RigidBody)otherObj;
		if (rigidBody.instance < instance)
		{
			return -1;
		}
		if (rigidBody.instance > instance)
		{
			return 1;
		}
		return 0;
	}

	private void UpdateHullData()
	{
		hullPoints.Clear();
		if (enableDebugDraw)
		{
			shape.MakeHull(ref hullPoints, 3);
		}
	}

	public void DebugDraw(IDebugDrawer drawer)
	{
		for (int i = 0; i < hullPoints.Count; i += 3)
		{
			TSVector result = hullPoints[i];
			TSVector result2 = hullPoints[i + 1];
			TSVector result3 = hullPoints[i + 2];
			TSVector.Transform(ref result, ref orientation, out result);
			TSVector.Add(ref result, ref position, out result);
			TSVector.Transform(ref result2, ref orientation, out result2);
			TSVector.Add(ref result2, ref position, out result2);
			TSVector.Transform(ref result3, ref orientation, out result3);
			TSVector.Add(ref result3, ref position, out result3);
			drawer.DrawTriangle(result, result2, result3);
		}
	}

	internal int GetInstance()
	{
		return instance;
	}

	internal void PostStep()
	{
		if (_freezeConstraints <= TSRigidBodyConstraints.None)
		{
			return;
		}
		bool flag = (_freezeConstraints & TSRigidBodyConstraints.FreezePositionX) == TSRigidBodyConstraints.FreezePositionX;
		bool flag2 = (_freezeConstraints & TSRigidBodyConstraints.FreezePositionY) == TSRigidBodyConstraints.FreezePositionY;
		bool flag3 = (_freezeConstraints & TSRigidBodyConstraints.FreezePositionZ) == TSRigidBodyConstraints.FreezePositionZ;
		if (flag)
		{
			position.x = _freezePosition.x;
		}
		if (flag2)
		{
			position.y = _freezePosition.y;
		}
		if (flag3)
		{
			position.z = _freezePosition.z;
		}
		bool flag4 = (_freezeConstraints & TSRigidBodyConstraints.FreezeRotationX) == TSRigidBodyConstraints.FreezeRotationX;
		bool flag5 = (_freezeConstraints & TSRigidBodyConstraints.FreezeRotationY) == TSRigidBodyConstraints.FreezeRotationY;
		bool flag6 = (_freezeConstraints & TSRigidBodyConstraints.FreezeRotationZ) == TSRigidBodyConstraints.FreezeRotationZ;
		if (flag4 || flag5 || flag6)
		{
			TSQuaternion quaternion = TSQuaternion.CreateFromMatrix(Orientation);
			if (flag4)
			{
				quaternion.x = _freezeRotationQuat.x;
			}
			if (flag5)
			{
				quaternion.y = _freezeRotationQuat.y;
			}
			if (flag6)
			{
				quaternion.z = _freezeRotationQuat.z;
			}
			quaternion.Normalize();
			Orientation = TSMatrix.CreateFromQuaternion(quaternion);
		}
	}

	public string Checkum()
	{
		return $"{position}|{orientation}";
	}

	public void TSApplyForce(TSVector force)
	{
		AddForce(force);
	}

	public void TSApplyForce(TSVector force, TSVector position)
	{
		AddForce(force, position);
	}

	public void TSApplyImpulse(TSVector force)
	{
		ApplyImpulse(force);
	}

	public void TSApplyImpulse(TSVector force, TSVector position)
	{
		ApplyImpulse(force, position);
	}

	public void TSApplyTorque(TSVector force)
	{
		AddTorque(force);
	}
}
