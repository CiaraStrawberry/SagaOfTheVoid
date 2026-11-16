namespace TrueSync.Physics3D;

public class PointOnPoint : Constraint
{
	[AddTracking]
	private TSVector localAnchor1;

	[AddTracking]
	private TSVector localAnchor2;

	[AddTracking]
	private TSVector r1;

	[AddTracking]
	private TSVector r2;

	[AddTracking]
	private FP biasFactor = 5 * FP.EN2;

	[AddTracking]
	private FP softness = FP.EN2;

	[AddTracking]
	private FP effectiveMass = FP.Zero;

	[AddTracking]
	private FP accumulatedImpulse = FP.Zero;

	[AddTracking]
	private FP bias;

	[AddTracking]
	private FP softnessOverDt;

	[AddTracking]
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

	public PointOnPoint(RigidBody body1, RigidBody body2, TSVector anchor)
		: base(body1, body2)
	{
		TSVector.Subtract(ref anchor, ref body1.position, out localAnchor1);
		TSVector.Subtract(ref anchor, ref body2.position, out localAnchor2);
		TSVector.Transform(ref localAnchor1, ref body1.invOrientation, out localAnchor1);
		TSVector.Transform(ref localAnchor2, ref body2.invOrientation, out localAnchor2);
	}

	public override void PrepareForIteration(FP timestep)
	{
		TSVector.Transform(ref localAnchor1, ref body1.orientation, out r1);
		TSVector.Transform(ref localAnchor2, ref body2.orientation, out r2);
		TSVector.Add(ref body1.position, ref r1, out var result);
		TSVector.Add(ref body2.position, ref r2, out var result2);
		TSVector.Subtract(ref result2, ref result, out var result3);
		FP magnitude = result3.magnitude;
		TSVector tSVector = result2 - result;
		if (tSVector.sqrMagnitude != FP.Zero)
		{
			tSVector.Normalize();
		}
		ref TSVector reference = ref jacobian[0];
		reference = -FP.One * tSVector;
		ref TSVector reference2 = ref jacobian[1];
		reference2 = -FP.One * (r1 % tSVector);
		ref TSVector reference3 = ref jacobian[2];
		reference3 = FP.One * tSVector;
		ref TSVector reference4 = ref jacobian[3];
		reference4 = r2 % tSVector;
		effectiveMass = body1.inverseMass + body2.inverseMass + TSVector.Transform(jacobian[1], body1.invInertiaWorld) * jacobian[1] + TSVector.Transform(jacobian[3], body2.invInertiaWorld) * jacobian[3];
		softnessOverDt = softness / timestep;
		effectiveMass += softnessOverDt;
		effectiveMass = FP.One / effectiveMass;
		bias = magnitude * biasFactor * (FP.One / timestep);
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
		drawer.DrawLine(body1.position, body1.position + r1);
		drawer.DrawLine(body2.position, body2.position + r2);
	}
}
