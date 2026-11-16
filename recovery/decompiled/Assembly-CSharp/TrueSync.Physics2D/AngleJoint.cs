namespace TrueSync.Physics2D;

public class AngleJoint : Joint2D
{
	private FP _bias;

	private FP _jointError;

	private FP _massFactor;

	private FP _targetAngle;

	public override TSVector2 WorldAnchorA
	{
		get
		{
			return BodyA.Position;
		}
		set
		{
		}
	}

	public override TSVector2 WorldAnchorB
	{
		get
		{
			return BodyB.Position;
		}
		set
		{
		}
	}

	public FP TargetAngle
	{
		get
		{
			return _targetAngle;
		}
		set
		{
			if (value != _targetAngle)
			{
				_targetAngle = value;
				WakeBodies();
			}
		}
	}

	public FP BiasFactor { get; set; }

	public FP MaxImpulse { get; set; }

	public FP Softness { get; set; }

	internal AngleJoint()
	{
		JointType = JointType.Angle;
	}

	public AngleJoint(Body bodyA, Body bodyB)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Angle;
		BiasFactor = 0.2f;
		MaxImpulse = FP.MaxValue;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return TSVector2.zero;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return 0;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		int islandIndex = BodyA.IslandIndex;
		int islandIndex2 = BodyB.IslandIndex;
		FP a = data.positions[islandIndex].a;
		FP a2 = data.positions[islandIndex2].a;
		_jointError = a2 - a - TargetAngle;
		_bias = -BiasFactor * data.step.inv_dt * _jointError;
		_massFactor = (1 - Softness) / (BodyA._invI + BodyB._invI);
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		int islandIndex = BodyA.IslandIndex;
		int islandIndex2 = BodyB.IslandIndex;
		FP value = (_bias - data.velocities[islandIndex2].w + data.velocities[islandIndex].w) * _massFactor;
		data.velocities[islandIndex].w -= BodyA._invI * FP.Sign(value) * TSMath.Min(FP.Abs(value), MaxImpulse);
		data.velocities[islandIndex2].w += BodyB._invI * FP.Sign(value) * TSMath.Min(FP.Abs(value), MaxImpulse);
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		return true;
	}
}
