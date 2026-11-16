namespace TrueSync.Physics3D;

public class PointOnLine : Constraint
{
	private TSVector lineNormal;

	private TSVector localAnchor1;

	private TSVector localAnchor2;

	private TSVector r1;

	private TSVector r2;

	private FP biasFactor = FP.Half;

	private FP softness = FP.Zero;

	private FP effectiveMass = FP.Zero;

	private FP accumulatedImpulse = FP.Zero;

	private FP bias;

	private FP softnessOverDt;

	private TSVector[] jacobian = new TSVector[4];

	public FP AppliedImpulse => accumulatedImpulse;

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

	public PointOnLine(RigidBody body1, RigidBody body2, TSVector lineStartPointBody1, TSVector pointBody2)
		: base(body1, body2)
	{
		TSVector.Subtract(ref lineStartPointBody1, ref body1.position, out localAnchor1);
		TSVector.Subtract(ref pointBody2, ref body2.position, out localAnchor2);
		TSVector.Transform(ref localAnchor1, ref body1.invOrientation, out localAnchor1);
		TSVector.Transform(ref localAnchor2, ref body2.invOrientation, out localAnchor2);
		lineNormal = TSVector.Normalize(lineStartPointBody1 - pointBody2);
	}

	public override void PrepareForIteration(FP timestep)
	{
		TSVector.Transform(ref localAnchor1, ref body1.orientation, out r1);
		TSVector.Transform(ref localAnchor2, ref body2.orientation, out r2);
		TSVector.Add(ref body1.position, ref r1, out var result);
		TSVector.Add(ref body2.position, ref r2, out var result2);
		TSVector.Subtract(ref result2, ref result, out var _);
		TSVector tSVector = TSVector.Transform(lineNormal, body1.orientation);
		tSVector.Normalize();
		TSVector tSVector2 = (result - result2) % tSVector;
		if (tSVector2.sqrMagnitude != FP.Zero)
		{
			tSVector2.Normalize();
		}
		tSVector2 %= tSVector;
		jacobian[0] = tSVector2;
		ref TSVector reference = ref jacobian[1];
		reference = (r1 + result2 - result) % tSVector2;
		ref TSVector reference2 = ref jacobian[2];
		reference2 = -FP.One * tSVector2;
		ref TSVector reference3 = ref jacobian[3];
		reference3 = -FP.One * r2 % tSVector2;
		effectiveMass = body1.inverseMass + body2.inverseMass + TSVector.Transform(jacobian[1], body1.invInertiaWorld) * jacobian[1] + TSVector.Transform(jacobian[3], body2.invInertiaWorld) * jacobian[3];
		softnessOverDt = softness / timestep;
		effectiveMass += softnessOverDt;
		if (effectiveMass != 0)
		{
			effectiveMass = FP.One / effectiveMass;
		}
		bias = -(tSVector % (result2 - result)).magnitude * biasFactor * (FP.One / timestep);
		if (!body1.isStatic)
		{
			body1.linearVelocity += body1.inverseMass * accumulatedImpulse * jacobian[0];
			body1.angularVelocity += TSVector.Transform(accumulatedImpulse * jacobian[1], body1.invInertiaWorld);
		}
		if (!body2.isStatic)
		{
			body2.linearVelocity += body2.inverseMass * accumulatedImpulse * jacobian[2];
			body2.angularVelocity += TSVector.Transform(accumulatedImpulse * jacobian[3], body2.invInertiaWorld);
		}
	}

	public override void Iterate()
	{
		FP fP = body1.linearVelocity * jacobian[0] + body1.angularVelocity * jacobian[1] + body2.linearVelocity * jacobian[2] + body2.angularVelocity * jacobian[3];
		FP fP2 = accumulatedImpulse * softnessOverDt;
		FP fP3 = -effectiveMass * (fP + bias + fP2);
		accumulatedImpulse += fP3;
		if (!body1.isStatic)
		{
			body1.linearVelocity += body1.inverseMass * fP3 * jacobian[0];
			body1.angularVelocity += TSVector.Transform(fP3 * jacobian[1], body1.invInertiaWorld);
		}
		if (!body2.isStatic)
		{
			body2.linearVelocity += body2.inverseMass * fP3 * jacobian[2];
			body2.angularVelocity += TSVector.Transform(fP3 * jacobian[3], body2.invInertiaWorld);
		}
	}

	public override void DebugDraw(IDebugDrawer drawer)
	{
		drawer.DrawLine(body1.position + r1, body1.position + r1 + TSVector.Transform(lineNormal, body1.orientation) * 100);
	}
}
