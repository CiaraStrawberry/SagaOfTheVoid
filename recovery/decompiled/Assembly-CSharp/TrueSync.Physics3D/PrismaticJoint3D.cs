namespace TrueSync.Physics3D;

public class PrismaticJoint3D : Joint3D
{
	private FixedAngle fixedAngle;

	private PointOnLine pointOnLine;

	private PointPointDistance minDistance;

	private PointPointDistance maxDistance;

	public PointPointDistance MaximumDistanceConstraint => maxDistance;

	public PointPointDistance MinimumDistanceConstraint => minDistance;

	public FixedAngle FixedAngleConstraint => fixedAngle;

	public PointOnLine PointOnLineConstraint => pointOnLine;

	public PrismaticJoint3D(World world, RigidBody body1, RigidBody body2)
		: base(world)
	{
		fixedAngle = new FixedAngle(body1, body2);
		pointOnLine = new PointOnLine(body1, body2, body1.position, body2.position);
	}

	public PrismaticJoint3D(World world, RigidBody body1, RigidBody body2, FP minimumDistance, FP maximumDistance)
		: base(world)
	{
		fixedAngle = new FixedAngle(body1, body2);
		pointOnLine = new PointOnLine(body1, body2, body1.position, body2.position);
		minDistance = new PointPointDistance(body1, body2, body1.position, body2.position);
		minDistance.Behavior = PointPointDistance.DistanceBehavior.LimitMinimumDistance;
		minDistance.Distance = minimumDistance;
		maxDistance = new PointPointDistance(body1, body2, body1.position, body2.position);
		maxDistance.Behavior = PointPointDistance.DistanceBehavior.LimitMaximumDistance;
		maxDistance.Distance = maximumDistance;
	}

	public PrismaticJoint3D(World world, RigidBody body1, RigidBody body2, TSVector pointOnBody1, TSVector pointOnBody2)
		: base(world)
	{
		fixedAngle = new FixedAngle(body1, body2);
		pointOnLine = new PointOnLine(body1, body2, pointOnBody1, pointOnBody2);
	}

	public PrismaticJoint3D(World world, RigidBody body1, RigidBody body2, TSVector pointOnBody1, TSVector pointOnBody2, FP maximumDistance, FP minimumDistance)
		: base(world)
	{
		fixedAngle = new FixedAngle(body1, body2);
		pointOnLine = new PointOnLine(body1, body2, pointOnBody1, pointOnBody2);
	}

	public override void Activate()
	{
		if (maxDistance != null)
		{
			World.AddConstraint(maxDistance);
		}
		if (minDistance != null)
		{
			World.AddConstraint(minDistance);
		}
		World.AddConstraint(fixedAngle);
		World.AddConstraint(pointOnLine);
	}

	public override void Deactivate()
	{
		if (maxDistance != null)
		{
			World.RemoveConstraint(maxDistance);
		}
		if (minDistance != null)
		{
			World.RemoveConstraint(minDistance);
		}
		World.RemoveConstraint(fixedAngle);
		World.RemoveConstraint(pointOnLine);
	}
}
