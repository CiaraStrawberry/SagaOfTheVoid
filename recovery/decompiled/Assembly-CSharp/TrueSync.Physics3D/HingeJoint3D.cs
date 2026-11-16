namespace TrueSync.Physics3D;

public class HingeJoint3D : Joint3D
{
	private PointOnPoint[] worldPointConstraint;

	public PointOnPoint PointOnPointConstraint1 => worldPointConstraint[0];

	public PointOnPoint PointOnPointConstraint2 => worldPointConstraint[1];

	public FP AppliedImpulse => worldPointConstraint[0].AppliedImpulse + worldPointConstraint[1].AppliedImpulse;

	private HingeJoint3D(IWorld world, IBody3D body1, IBody3D body2, TSVector position, TSVector hingeAxis)
		: base((World)world)
	{
		worldPointConstraint = new PointOnPoint[2];
		hingeAxis *= FP.Half;
		TSVector value = position;
		TSVector.Add(ref value, ref hingeAxis, out value);
		TSVector value2 = position;
		TSVector.Subtract(ref value2, ref hingeAxis, out value2);
		worldPointConstraint[0] = new PointOnPoint((RigidBody)body1, (RigidBody)body2, value);
		worldPointConstraint[1] = new PointOnPoint((RigidBody)body1, (RigidBody)body2, value2);
		StateTracker.AddTracking(worldPointConstraint[0]);
		StateTracker.AddTracking(worldPointConstraint[1]);
		Activate();
	}

	public static void Create(IWorld world, IBody3D body1, IBody3D body2, TSVector position, TSVector hingeAxis)
	{
		new HingeJoint3D(world, body1, body2, position, hingeAxis);
	}

	public override void Activate()
	{
		World.AddConstraint(worldPointConstraint[0]);
		World.AddConstraint(worldPointConstraint[1]);
	}

	public override void Deactivate()
	{
		World.RemoveConstraint(worldPointConstraint[0]);
		World.RemoveConstraint(worldPointConstraint[1]);
	}
}
