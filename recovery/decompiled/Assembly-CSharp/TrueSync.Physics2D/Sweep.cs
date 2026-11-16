namespace TrueSync.Physics2D;

public struct Sweep
{
	public FP A;

	public FP A0;

	public FP Alpha0;

	public TSVector2 C;

	public TSVector2 C0;

	public TSVector2 LocalCenter;

	public void GetTransform(out Transform xfb, FP beta)
	{
		xfb = default;
		xfb.p.x = (1f - beta) * C0.x + beta * C.x;
		xfb.p.y = (1f - beta) * C0.y + beta * C.y;
		FP angle = (1f - beta) * A0 + beta * A;
		xfb.q.Set(angle);
		xfb.p -= MathUtils.Mul(xfb.q, LocalCenter);
	}

	public void Advance(FP alpha)
	{
		FP fP = (alpha - Alpha0) / (1f - Alpha0);
		C0 += fP * (C - C0);
		A0 += fP * (A - A0);
		Alpha0 = alpha;
	}

	public void Normalize()
	{
		FP fP = FP.PiTimes2 * FP.Floor(A0 / FP.PiTimes2);
		A0 -= fP;
		A -= fP;
	}
}
