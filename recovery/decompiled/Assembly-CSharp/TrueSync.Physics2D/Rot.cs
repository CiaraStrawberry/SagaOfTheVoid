namespace TrueSync.Physics2D;

public struct Rot(FP angle)
{
	public FP s = FP.Sin(angle);

	public FP c = FP.Cos(angle);

	public void Set(FP angle)
	{
		s = FP.Sin(angle);
		c = FP.Cos(angle);
	}

	public void SetIdentity()
	{
		s = 0f;
		c = 1f;
	}

	public FP GetAngle()
	{
		return FP.Atan2(s, c);
	}

	public TSVector2 GetXAxis()
	{
		return new TSVector2(c, s);
	}

	public TSVector2 GetYAxis()
	{
		return new TSVector2(-s, c);
	}
}
