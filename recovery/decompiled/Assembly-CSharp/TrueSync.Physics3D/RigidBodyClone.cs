using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class RigidBodyClone
{
	public static ResourcePoolGenericShapeClone poolGenericShapeClone = new ResourcePoolGenericShapeClone();

	public TSVector position;

	public TSMatrix orientation;

	public TSVector linearVelocity;

	public TSVector angularVelocity;

	public TSMatrix inertia;

	public TSMatrix invInertia;

	public TSMatrix invInertiaWorld;

	public TSMatrix invOrientation;

	public TSVector force;

	public TSVector torque;

	public GenericShapeClone shapeClone;

	public List<RigidBody> connections = new List<RigidBody>();

	public List<Constraint> constraints = new List<Constraint>();

	public bool isActive;

	public FP inactiveTime;

	public int marker;

	public bool affectedByGravity;

	public TSBBox boundingBox;

	public int internalIndex;

	public FP inverseMass;

	public bool isColliderOnly;

	public bool isStatic;

	public bool isKinematic;

	public TSVector sweptDirection;

	public bool disabled;

	public TSRigidBodyConstraints freezeConstraint;

	public TSVector _freezePosition;

	public TSMatrix _freezeRotation;

	public TSQuaternion _freezeRotationQuat;

	public bool prevKinematicGravity;

	public FP staticFriction;

	public FP restitution;

	public FP linearDrag;

	public FP angularDrag;

	private int index;

	private int length;

	public void Reset()
	{
		if (shapeClone != null)
		{
			poolGenericShapeClone.GiveBack(shapeClone);
		}
	}

	public void Clone(RigidBody rb)
	{
		marker = rb.marker;
		affectedByGravity = rb.affectedByGravity;
		boundingBox = rb.boundingBox;
		internalIndex = rb.internalIndex;
		inverseMass = rb.inverseMass;
		isColliderOnly = rb.isColliderOnly;
		isStatic = rb.isStatic;
		isKinematic = rb.isKinematic;
		sweptDirection = rb.sweptDirection;
		position = rb.Position;
		orientation = rb.Orientation;
		linearVelocity = rb.LinearVelocity;
		angularVelocity = rb.AngularVelocity;
		inertia = rb.Inertia;
		invInertia = rb.InverseInertia;
		invInertiaWorld = rb.InverseInertiaWorld;
		invOrientation = rb.invOrientation;
		force = rb.Force;
		torque = rb.Torque;
		shapeClone = poolGenericShapeClone.GetNew();
		shapeClone.Clone(rb.Shape);
		connections.Clear();
		index = 0;
		for (length = rb.connections.Count; index < length; index++)
		{
			connections.Add(rb.connections[index]);
		}
		constraints.Clear();
		index = 0;
		for (length = rb.constraints.Count; index < length; index++)
		{
			constraints.Add(rb.constraints[index]);
		}
		isActive = rb.IsActive;
		inactiveTime = rb.inactiveTime;
		marker = rb.marker;
		disabled = rb.disabled;
		freezeConstraint = rb._freezeConstraints;
		_freezePosition = rb._freezePosition;
		_freezeRotation = rb._freezeRotation;
		_freezeRotationQuat = rb._freezeRotationQuat;
		prevKinematicGravity = rb.prevKinematicGravity;
		linearDrag = rb.linearDrag;
		angularDrag = rb.angularDrag;
		staticFriction = rb.staticFriction;
		restitution = rb.restitution;
	}

	public void Restore(World world, RigidBody rb)
	{
		rb.marker = marker;
		rb.affectedByGravity = affectedByGravity;
		rb.boundingBox = boundingBox;
		rb.internalIndex = internalIndex;
		rb.inverseMass = inverseMass;
		rb.isColliderOnly = isColliderOnly;
		rb.isStatic = isStatic;
		rb.isKinematic = isKinematic;
		rb.sweptDirection = sweptDirection;
		rb.position = position;
		rb.orientation = orientation;
		rb.inertia = inertia;
		rb.invInertia = invInertia;
		rb.invInertiaWorld = invInertiaWorld;
		rb.invOrientation = invOrientation;
		rb.force = force;
		rb.torque = torque;
		shapeClone.Restore(rb.Shape);
		rb.connections.Clear();
		rb.connections.AddRange(connections);
		rb.arbiters.Clear();
		rb.constraints.Clear();
		rb.constraints.AddRange(constraints);
		rb.isActive = isActive;
		rb.inactiveTime = inactiveTime;
		rb.marker = marker;
		bool flag = rb.disabled;
		rb.disabled = disabled;
		rb._freezeConstraints = freezeConstraint;
		rb._freezePosition = _freezePosition;
		rb._freezeRotation = _freezeRotation;
		rb._freezeRotationQuat = _freezeRotationQuat;
		rb.prevKinematicGravity = prevKinematicGravity;
		rb.linearDrag = linearDrag;
		rb.angularDrag = angularDrag;
		rb.linearVelocity = linearVelocity;
		rb.angularVelocity = angularVelocity;
		rb.staticFriction = staticFriction;
		rb.restitution = restitution;
		if (flag && !rb.disabled)
		{
			world.physicsManager.GetGameObject(rb).SetActive(value: true);
		}
	}
}
