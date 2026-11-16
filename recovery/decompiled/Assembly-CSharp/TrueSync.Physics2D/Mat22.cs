namespace TrueSync.Physics2D;

public struct Mat22
{
	public TSVector2 ex;

	public TSVector2 ey;

	public Mat22 Inverse
	{
		get
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
			return new Mat22
			{
				ex = 
				{
					x = fP * y2,
					y = -fP * y
				},
				ey = 
				{
					x = -fP * x2,
					y = fP * x
				}
			};
		}
	}

	public Mat22(TSVector2 c1, TSVector2 c2)
	{
		ex = c1;
		ey = c2;
	}

	public Mat22(FP a11, FP a12, FP a21, FP a22)
	{
		ex = new TSVector2(a11, a21);
		ey = new TSVector2(a12, a22);
	}

	public void Set(TSVector2 c1, TSVector2 c2)
	{
		ex = c1;
		ey = c2;
	}

	public void SetIdentity()
	{
		ex.x = 1f;
		ey.x = 0f;
		ex.y = 0f;
		ey.y = 1f;
	}

	public void SetZero()
	{
		ex.x = 0f;
		ey.x = 0f;
		ex.y = 0f;
		ey.y = 0f;
	}

	public TSVector2 Solve(TSVector2 b)
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

	public static void Add(ref Mat22 A, ref Mat22 B, out Mat22 R)
	{
		R.ex = A.ex + B.ex;
		R.ey = A.ey + B.ey;
	}
}
