namespace TrueSync.Physics3D;

public class FixedAngle : Constraint
{
	private FP biasFactor = 5 * FP.EN2;

	private FP softness = FP.Zero;

	private TSVector accumulatedImpulse;

	private TSMatrix initialOrientation1;

	private TSMatrix initialOrientation2;

	private TSMatrix effectiveMass;

	private TSVector bias;

	private FP softnessOverDt;

	public TSVector AppliedImpulse => accumulatedImpulse;

	public TSMatrix InitialOrientationBody1
	{
		get
		{
			return initialOrientation1;
		}
		set
		{
			initialOrientation1 = value;
		}
	}

	public TSMatrix InitialOrientationBody2
	{
		get
		{
			return initialOrientation2;
		}
		set
		{
			initialOrientation2 = value;
		}
	}

	public FP Softness
	{
		get
		{
			return softness;
		}
		set
		{
			softness = value;
		}
	}

	public FP BiasFactor
	{
		get
		{
			return biasFactor;
		}
		set
		{
			biasFactor = value;
		}
	}

	public FixedAngle(RigidBody body1, RigidBody body2)
		: base(body1, body2)
	{
		initialOrientation1 = body1.orientation;
		initialOrientation2 = body2.orientation;
	}

	public override void PrepareForIteration(FP timestep)
	{
		effectiveMass = body1.invInertiaWorld + body2.invInertiaWorld;
		softnessOverDt = softness / timestep;
		effectiveMass.M11 += softnessOverDt;
		effectiveMass.M22 += softnessOverDt;
		effectiveMass.M33 += softnessOverDt;
		TSMatrix.Inverse(ref effectiveMass, out effectiveMass);
		TSMatrix.Multiply(ref initialOrientation1, ref initialOrientation2, out var result);
		TSMatrix.Transpose(ref result, out result);
		TSMatrix tSMatrix = result * body2.invOrientation * body1.orientation;
		FP fP = tSMatrix.M32 - tSMatrix.M23;
		FP fP2 = tSMatrix.M13 - tSMatrix.M31;
		FP fP3 = tSMatrix.M21 - tSMatrix.M12;
		FP fP4 = TSMath.Sqrt(fP * fP + fP2 * fP2 + fP3 * fP3);
		FP fP5 = tSMatrix.M11 + tSMatrix.M22 + tSMatrix.M33;
		FP fP6 = FP.Atan2(fP4, fP5 - 1);
		TSVector tSVector = new TSVector(fP, fP2, fP3) * fP6;
		if (fP4 != FP.Zero)
		{
			tSVector *= FP.One / fP4;
		}
		bias = tSVector * biasFactor * (-FP.One / timestep);
		if (!body1.IsStatic)
		{
			body1.angularVelocity += TSVector.Transform(accumulatedImpulse, body1.invInertiaWorld);
		}
		if (!body2.IsStatic)
		{
			body2.angularVelocity += TSVector.Transform(-FP.One * accumulatedImpulse, body2.invInertiaWorld);
		}
	}

	public override void Iterate()
	{
		TSVector tSVector = body1.angularVelocity - body2.angularVelocity;
		TSVector tSVector2 = accumulatedImpulse * softnessOverDt;
		TSVector tSVector3 = -FP.One * TSVector.Transform(tSVector + bias + tSVector2, effectiveMass);
		accumulatedImpulse += tSVector3;
		if (!body1.IsStatic)
		{
			body1.angularVelocity += TSVector.Transform(tSVector3, body1.invInertiaWorld);
		}
		if (!body2.IsStatic)
		{
			body2.angularVelocity += TSVector.Transform(-FP.One * tSVector3, body2.invInertiaWorld);
		}
	}
}
