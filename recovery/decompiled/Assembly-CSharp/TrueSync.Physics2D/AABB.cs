namespace TrueSync.Physics2D;

public struct AABB
{
	public TSVector2 LowerBound;

	public TSVector2 UpperBound;

	public FP Width => UpperBound.x - LowerBound.x;

	public FP Height => UpperBound.y - LowerBound.y;

	public TSVector2 Center => 0.5f * (LowerBound + UpperBound);

	public TSVector2 Extents => 0.5f * (UpperBound - LowerBound);

	public FP Perimeter
	{
		get
		{
			FP fP = UpperBound.x - LowerBound.x;
			FP fP2 = UpperBound.y - LowerBound.y;
			return 2f * (fP + fP2);
		}
	}

	public Vertices Vertices
	{
		get
		{
			Vertices vertices = new Vertices(4);
			vertices.Add(UpperBound);
			vertices.Add(new TSVector2(UpperBound.x, LowerBound.y));
			vertices.Add(LowerBound);
			vertices.Add(new TSVector2(LowerBound.x, UpperBound.y));
			return vertices;
		}
	}

	public AABB Q1 => new AABB(Center, UpperBound);

	public AABB Q2 => new AABB(new TSVector2(LowerBound.x, Center.y), new TSVector2(Center.x, UpperBound.y));

	public AABB Q3 => new AABB(LowerBound, Center);

	public AABB Q4 => new AABB(new TSVector2(Center.x, LowerBound.y), new TSVector2(UpperBound.x, Center.y));

	public AABB(TSVector2 min, TSVector2 max)
		: this(ref min, ref max)
	{
	}

	public AABB(ref TSVector2 min, ref TSVector2 max)
	{
		LowerBound = min;
		UpperBound = max;
	}

	public AABB(TSVector2 center, FP width, FP height)
	{
		LowerBound = center - new TSVector2(width / 2, height / 2);
		UpperBound = center + new TSVector2(width / 2, height / 2);
	}

	public bool IsValid()
	{
		TSVector2 tSVector = UpperBound - LowerBound;
		return tSVector.x >= 0f && tSVector.y >= 0f && LowerBound.IsValid() && UpperBound.IsValid();
	}

	public void Combine(ref AABB aabb)
	{
		TSVector2.Min(ref LowerBound, ref aabb.LowerBound, out LowerBound);
		TSVector2.Max(ref UpperBound, ref aabb.UpperBound, out UpperBound);
	}

	public void Combine(ref AABB aabb1, ref AABB aabb2)
	{
		TSVector2.Min(ref aabb1.LowerBound, ref aabb2.LowerBound, out LowerBound);
		TSVector2.Max(ref aabb1.UpperBound, ref aabb2.UpperBound, out UpperBound);
	}

	public bool Contains(ref AABB aabb)
	{
		return true && LowerBound.x <= aabb.LowerBound.x && LowerBound.y <= aabb.LowerBound.y && aabb.UpperBound.x <= UpperBound.x && aabb.UpperBound.y <= UpperBound.y;
	}

	public bool Contains(ref TSVector2 point)
	{
		return point.x > LowerBound.x + Settings.Epsilon && point.x < UpperBound.x - Settings.Epsilon && point.y > LowerBound.y + Settings.Epsilon && point.y < UpperBound.y - Settings.Epsilon;
	}

	public static bool TestOverlap(ref AABB a, ref AABB b)
	{
		TSVector2 tSVector = b.LowerBound - a.UpperBound;
		TSVector2 tSVector2 = a.LowerBound - b.UpperBound;
		if (tSVector.x > 0f || tSVector.y > 0f)
		{
			return false;
		}
		if (tSVector2.x > 0f || tSVector2.y > 0f)
		{
			return false;
		}
		return true;
	}

	public bool RayCast(out RayCastOutput output, ref RayCastInput input, bool doInteriorCheck = true)
	{
		output = default;
		FP fP = -Settings.MaxFP;
		FP fP2 = Settings.MaxFP;
		TSVector2 point = input.Point1;
		TSVector2 v = input.Point2 - input.Point1;
		TSVector2 tSVector = MathUtils.Abs(v);
		TSVector2 zero = TSVector2.zero;
		for (int i = 0; i < 2; i++)
		{
			FP fP3 = ((i != 0) ? tSVector.y : tSVector.x);
			FP fP4 = ((i != 0) ? LowerBound.y : LowerBound.x);
			FP fP5 = ((i != 0) ? UpperBound.y : UpperBound.x);
			FP fP6 = ((i != 0) ? point.y : point.x);
			if (fP3 < Settings.Epsilon)
			{
				if (fP6 < fP4 || fP5 < fP6)
				{
					return false;
				}
				continue;
			}
			FP fP7 = ((i != 0) ? v.y : v.x);
			FP fP8 = 1f / fP7;
			FP a = (fP4 - fP6) * fP8;
			FP b = (fP5 - fP6) * fP8;
			FP fP9 = -1f;
			if (a > b)
			{
				MathUtils.Swap(ref a, ref b);
				fP9 = 1f;
			}
			if (a > fP)
			{
				if (i == 0)
				{
					zero.x = fP9;
				}
				else
				{
					zero.y = fP9;
				}
				fP = a;
			}
			fP2 = TSMath.Min(fP2, b);
			if (fP > fP2)
			{
				return false;
			}
		}
		if (doInteriorCheck && (fP < 0f || input.MaxFraction < fP))
		{
			return false;
		}
		output.Fraction = fP;
		output.Normal = zero;
		return true;
	}
}
