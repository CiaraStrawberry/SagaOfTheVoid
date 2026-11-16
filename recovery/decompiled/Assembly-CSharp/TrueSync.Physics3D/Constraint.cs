using System;

namespace TrueSync.Physics3D;

public abstract class Constraint : IConstraint, IDebugDrawable, IComparable
{
	internal RigidBody body1;

	internal RigidBody body2;

	internal static int instanceCount;

	private int instance;

	public RigidBody Body1 => body1;

	public RigidBody Body2 => body2;

	public Constraint(RigidBody body1, RigidBody body2)
	{
		this.body1 = body1;
		this.body2 = body2;
		instanceCount++;
		instance = instanceCount;
		body1?.Update();
		body2?.Update();
	}

	public virtual void PrepareForIteration(FP timestep)
	{
	}

	public virtual void Iterate()
	{
	}

	public virtual void PostStep()
	{
	}

	public int CompareTo(object otherObj)
	{
		Constraint constraint = (Constraint)otherObj;
		if (constraint.instance < instance)
		{
			return -1;
		}
		if (constraint.instance > instance)
		{
			return 1;
		}
		return 0;
	}

	public virtual void DebugDraw(IDebugDrawer drawer)
	{
	}
}
