namespace TrueSync;

public struct TSBBox
{
	public enum ContainmentType
	{
		Disjoint,
		Contains,
		Intersects
	}

	public TSVector min;

	public TSVector max;

	public static readonly TSBBox LargeBox;

	public static readonly TSBBox SmallBox;

	public TSVector center => (min + max) * FP.Half;

	public TSVector size => max - min;

	public TSVector extents => size * FP.Half;

	internal FP Perimeter => 2 * FP.One * ((max.x - min.x) * (max.y - min.y) + (max.x - min.x) * (max.z - min.z) + (max.z - min.z) * (max.y - min.y));

	static TSBBox()
	{
		LargeBox.min = new TSVector(FP.MinValue);
		LargeBox.max = new TSVector(FP.MaxValue);
		SmallBox.min = new TSVector(FP.MaxValue);
		SmallBox.max = new TSVector(FP.MinValue);
	}

	public TSBBox(TSVector min, TSVector max)
	{
		this.min = min;
		this.max = max;
	}

	internal void InverseTransform(ref TSVector position, ref TSMatrix orientation)
	{
		TSVector.Subtract(ref max, ref position, out max);
		TSVector.Subtract(ref min, ref position, out min);
		TSVector.Add(ref max, ref min, out var result);
		result.x *= FP.Half;
		result.y *= FP.Half;
		result.z *= FP.Half;
		TSVector.Subtract(ref max, ref min, out var result2);
		result2.x *= FP.Half;
		result2.y *= FP.Half;
		result2.z *= FP.Half;
		TSVector.TransposedTransform(ref result, ref orientation, out result);
		TSMath.Absolute(ref orientation, out var result3);
		TSVector.TransposedTransform(ref result2, ref result3, out result2);
		TSVector.Add(ref result, ref result2, out max);
		TSVector.Subtract(ref result, ref result2, out min);
	}

	public void Transform(ref TSMatrix orientation)
	{
		TSVector position = FP.Half * (max - min);
		TSVector position2 = FP.Half * (max + min);
		TSVector.Transform(ref position2, ref orientation, out position2);
		TSMath.Absolute(ref orientation, out var result);
		TSVector.Transform(ref position, ref result, out position);
		max = position2 + position;
		min = position2 - position;
	}

	private bool Intersect1D(FP start, FP dir, FP min, FP max, ref FP enter, ref FP exit)
	{
		if (dir * dir < TSMath.Epsilon * TSMath.Epsilon)
		{
			return start >= min && start <= max;
		}
		FP fP = (min - start) / dir;
		FP fP2 = (max - start) / dir;
		if (fP > fP2)
		{
			FP fP3 = fP;
			fP = fP2;
			fP2 = fP3;
		}
		if (fP > exit || fP2 < enter)
		{
			return false;
		}
		if (fP > enter)
		{
			enter = fP;
		}
		if (fP2 < exit)
		{
			exit = fP2;
		}
		return true;
	}

	public bool SegmentIntersect(ref TSVector origin, ref TSVector direction)
	{
		FP enter = FP.Zero;
		FP exit = FP.One;
		if (!Intersect1D(origin.x, direction.x, min.x, max.x, ref enter, ref exit))
		{
			return false;
		}
		if (!Intersect1D(origin.y, direction.y, min.y, max.y, ref enter, ref exit))
		{
			return false;
		}
		if (!Intersect1D(origin.z, direction.z, min.z, max.z, ref enter, ref exit))
		{
			return false;
		}
		return true;
	}

	public bool RayIntersect(ref TSVector origin, ref TSVector direction)
	{
		FP enter = FP.Zero;
		FP exit = FP.MaxValue;
		if (!Intersect1D(origin.x, direction.x, min.x, max.x, ref enter, ref exit))
		{
			return false;
		}
		if (!Intersect1D(origin.y, direction.y, min.y, max.y, ref enter, ref exit))
		{
			return false;
		}
		if (!Intersect1D(origin.z, direction.z, min.z, max.z, ref enter, ref exit))
		{
			return false;
		}
		return true;
	}

	public bool SegmentIntersect(TSVector origin, TSVector direction)
	{
		return SegmentIntersect(ref origin, ref direction);
	}

	public bool RayIntersect(TSVector origin, TSVector direction)
	{
		return RayIntersect(ref origin, ref direction);
	}

	public ContainmentType Contains(TSVector point)
	{
		return Contains(ref point);
	}

	public ContainmentType Contains(ref TSVector point)
	{
		return (min.x <= point.x && point.x <= max.x && min.y <= point.y && point.y <= max.y && min.z <= point.z && point.z <= max.z) ? ContainmentType.Contains : ContainmentType.Disjoint;
	}

	public void GetCorners(TSVector[] corners)
	{
		corners[0].Set(min.x, max.y, max.z);
		corners[1].Set(max.x, max.y, max.z);
		corners[2].Set(max.x, min.y, max.z);
		corners[3].Set(min.x, min.y, max.z);
		corners[4].Set(min.x, max.y, min.z);
		corners[5].Set(max.x, max.y, min.z);
		corners[6].Set(max.x, min.y, min.z);
		corners[7].Set(min.x, min.y, min.z);
	}

	public void AddPoint(TSVector point)
	{
		AddPoint(ref point);
	}

	public void AddPoint(ref TSVector point)
	{
		TSVector.Max(ref max, ref point, out max);
		TSVector.Min(ref min, ref point, out min);
	}

	public static TSBBox CreateFromPoints(TSVector[] points)
	{
		TSVector value = new TSVector(FP.MaxValue);
		TSVector value2 = new TSVector(FP.MinValue);
		for (int i = 0; i < points.Length; i++)
		{
			TSVector.Min(ref value, ref points[i], out value);
			TSVector.Max(ref value2, ref points[i], out value2);
		}
		return new TSBBox(value, value2);
	}

	public ContainmentType Contains(TSBBox box)
	{
		return Contains(ref box);
	}

	public ContainmentType Contains(ref TSBBox box)
	{
		ContainmentType result = ContainmentType.Disjoint;
		if (max.x >= box.min.x && min.x <= box.max.x && max.y >= box.min.y && min.y <= box.max.y && max.z >= box.min.z && min.z <= box.max.z)
		{
			result = ((min.x <= box.min.x && box.max.x <= max.x && min.y <= box.min.y && box.max.y <= max.y && min.z <= box.min.z && box.max.z <= max.z) ? ContainmentType.Contains : ContainmentType.Intersects);
		}
		return result;
	}

	public static TSBBox CreateFromCenter(TSVector center, TSVector size)
	{
		TSVector tSVector = size * FP.Half;
		return new TSBBox(center - tSVector, center + tSVector);
	}

	public static TSBBox CreateMerged(TSBBox original, TSBBox additional)
	{
		CreateMerged(ref original, ref additional, out var result);
		return result;
	}

	public static void CreateMerged(ref TSBBox original, ref TSBBox additional, out TSBBox result)
	{
		TSVector.Min(ref original.min, ref additional.min, out var result2);
		TSVector.Max(ref original.max, ref additional.max, out var result3);
		result.min = result2;
		result.max = result3;
	}

	public override string ToString()
	{
		return string.Concat(min, "|", max);
	}
}
