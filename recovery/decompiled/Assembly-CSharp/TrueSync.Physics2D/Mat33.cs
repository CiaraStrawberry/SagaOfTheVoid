namespace TrueSync.Physics2D;

public struct Mat33(TSVector c1, TSVector c2, TSVector c3)
{
	public TSVector ex = c1;

	public TSVector ey = c2;

	public TSVector ez = c3;

	public void SetZero()
	{
		ex = TSVector.zero;
		ey = TSVector.zero;
		ez = TSVector.zero;
	}

	public TSVector Solve33(TSVector b)
	{
		FP fP = TSVector.Dot(ex, TSVector.Cross(ey, ez));
		if (fP != 0f)
		{
			fP = 1f / fP;
		}
		return new TSVector(fP * TSVector.Dot(b, TSVector.Cross(ey, ez)), fP * TSVector.Dot(ex, TSVector.Cross(b, ez)), fP * TSVector.Dot(ex, TSVector.Cross(ey, b)));
	}

	public TSVector2 Solve22(TSVector2 b)
	{
		FP x = ex.x;
		FP x2 = ey.x;
		FP y = ex.y;
		FP y2 = ey.y;
		FP fP = x * y2 - x2 * y;
		if (fP != 0f)
		{
			fP = 1f / fP;
		}
		return new TSVector2(fP * (y2 * b.x - x2 * b.y), fP * (x * b.y - y * b.x));
	}

	public void GetInverse22(ref Mat33 M)
	{
		FP x = ex.x;
		FP x2 = ey.x;
		FP y = ex.y;
		FP y2 = ey.y;
		FP fP = x * y2 - x2 * y;
		if (fP != 0f)
		{
			fP = 1f / fP;
		}
		M.ex.x = fP * y2;
		M.ey.x = -fP * x2;
		M.ex.z = 0f;
		M.ex.y = -fP * y;
		M.ey.y = fP * x;
		M.ey.z = 0f;
		M.ez.x = 0f;
		M.ez.y = 0f;
		M.ez.z = 0f;
	}

	public void GetSymInverse33(ref Mat33 M)
	{
		FP fP = MathUtils.Dot(ex, MathUtils.Cross(ey, ez));
		if (fP != 0f)
		{
			fP = 1f / fP;
		}
		FP x = ex.x;
		FP x2 = ey.x;
		FP x3 = ez.x;
		FP y = ey.y;
		FP y2 = ez.y;
		FP z = ez.z;
		M.ex.x = fP * (y * z - y2 * y2);
		M.ex.y = fP * (x3 * y2 - x2 * z);
		M.ex.z = fP * (x2 * y2 - x3 * y);
		M.ey.x = M.ex.y;
		M.ey.y = fP * (x * z - x3 * x3);
		M.ey.z = fP * (x3 * x2 - x * y2);
		M.ez.x = M.ex.z;
		M.ez.y = M.ey.z;
		M.ez.z = fP * (x * y - x2 * x2);
	}
}
