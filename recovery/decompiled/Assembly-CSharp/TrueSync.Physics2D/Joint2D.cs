using System;

namespace TrueSync.Physics2D;

public abstract class Joint2D
{
	private FP _breakpoint;

	private FP _breakpointSquared;

	public bool Enabled = true;

	internal JointEdge EdgeA = new JointEdge();

	internal JointEdge EdgeB = new JointEdge();

	internal bool IslandFlag;

	public JointType JointType { get; protected set; }

	public Body BodyA { get; internal set; }

	public Body BodyB { get; internal set; }

	public abstract TSVector2 WorldAnchorA { get; set; }

	public abstract TSVector2 WorldAnchorB { get; set; }

	public object UserData { get; set; }

	public bool CollideConnected { get; set; }

	public FP Breakpoint
	{
		get
		{
			return _breakpoint;
		}
		set
		{
			_breakpoint = value;
			_breakpointSquared = _breakpoint * _breakpoint;
		}
	}

	public event Action<Joint2D, FP> Broke;

	protected Joint2D()
	{
		Breakpoint = FP.MaxValue;
		CollideConnected = false;
	}

	protected Joint2D(Body bodyA, Body bodyB)
		: this()
	{
		BodyA = bodyA;
		BodyB = bodyB;
	}

	protected Joint2D(Body body)
		: this()
	{
		BodyA = body;
	}

	public abstract TSVector2 GetReactionForce(FP invDt);

	public abstract FP GetReactionTorque(FP invDt);

	protected void WakeBodies()
	{
		if (BodyA != null)
		{
			BodyA.Awake = true;
		}
		if (BodyB != null)
		{
			BodyB.Awake = true;
		}
	}

	public bool IsFixedType()
	{
		return JointType == JointType.FixedRevolute || JointType == JointType.FixedDistance || JointType == JointType.FixedPrismatic || JointType == JointType.FixedLine || JointType == JointType.FixedMouse || JointType == JointType.FixedAngle || JointType == JointType.FixedFriction;
	}

	internal abstract void InitVelocityConstraints(ref SolverData data);

	internal void Validate(FP invDt)
	{
		if (!Enabled)
		{
			return;
		}
		FP fP = GetReactionForce(invDt).LengthSquared();
		if (!(FP.Abs(fP) <= _breakpointSquared))
		{
			Enabled = false;
			if (Broke != null)
			{
				Broke(this, FP.Sqrt(fP));
			}
		}
	}

	internal abstract void SolveVelocityConstraints(ref SolverData data);

	internal abstract bool SolvePositionConstraints(ref SolverData data);
}
