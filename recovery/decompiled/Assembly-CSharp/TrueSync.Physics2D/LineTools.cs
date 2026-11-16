namespace TrueSync.Physics2D;

public static class LineTools
{
	public static FP DistanceBetweenPointAndLineSegment(ref TSVector2 point, ref TSVector2 start, ref TSVector2 end)
	{
		if (start == end)
		{
			return TSVector2.Distance(point, start);
		}
		TSVector2 tSVector = TSVector2.Subtract(end, start);
		TSVector2 value = TSVector2.Subtract(point, start);
		FP fP = TSVector2.Dot(value, tSVector);
		if (fP <= 0)
		{
			return TSVector2.Distance(point, start);
		}
		FP fP2 = TSVector2.Dot(tSVector, tSVector);
		if (fP2 <= fP)
		{
			return TSVector2.Distance(point, end);
		}
		FP scaleFactor = fP / fP2;
		TSVector2 value2 = TSVector2.Add(start, TSVector2.Multiply(tSVector, scaleFactor));
		return TSVector2.Distance(point, value2);
	}

	public static bool LineIntersect2(ref TSVector2 a0, ref TSVector2 a1, ref TSVector2 b0, ref TSVector2 b1, out TSVector2 intersectionPoint)
	{
		intersectionPoint = TSVector2.zero;
		if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
		{
			return false;
		}
		FP x = a0.x;
		FP y = a0.y;
		FP x2 = a1.x;
		FP y2 = a1.y;
		FP x3 = b0.x;
		FP y3 = b0.y;
		FP x4 = b1.x;
		FP y4 = b1.y;
		if (TSMath.Max(x, x2) < TSMath.Min(x3, x4) || TSMath.Max(x3, x4) < TSMath.Min(x, x2))
		{
			return false;
		}
		if (TSMath.Max(y, y2) < TSMath.Min(y3, y4) || TSMath.Max(y3, y4) < TSMath.Min(y, y2))
		{
			return false;
		}
		FP fP = (x4 - x3) * (y - y3) - (y4 - y3) * (x - x3);
		FP fP2 = (x2 - x) * (y - y3) - (y2 - y) * (x - x3);
		FP fP3 = (y4 - y3) * (x2 - x) - (x4 - x3) * (y2 - y);
		if (FP.Abs(fP3) < Settings.Epsilon)
		{
			return false;
		}
		fP /= fP3;
		fP2 /= fP3;
		if (0 < fP && fP < 1 && 0 < fP2 && fP2 < 1)
		{
			intersectionPoint.x = x + fP * (x2 - x);
			intersectionPoint.y = y + fP * (y2 - y);
			return true;
		}
		return false;
	}

	public static TSVector2 LineIntersect(TSVector2 p1, TSVector2 p2, TSVector2 q1, TSVector2 q2)
	{
		TSVector2 zero = TSVector2.zero;
		FP fP = p2.y - p1.y;
		FP fP2 = p1.x - p2.x;
		FP fP3 = fP * p1.x + fP2 * p1.y;
		FP fP4 = q2.y - q1.y;
		FP fP5 = q1.x - q2.x;
		FP fP6 = fP4 * q1.x + fP5 * q1.y;
		FP fP7 = fP * fP5 - fP4 * fP2;
		if (!MathUtils.FPEquals(fP7, 0))
		{
			zero.x = (fP5 * fP3 - fP2 * fP6) / fP7;
			zero.y = (fP * fP6 - fP4 * fP3) / fP7;
		}
		return zero;
	}

	public static bool LineIntersect(ref TSVector2 point1, ref TSVector2 point2, ref TSVector2 point3, ref TSVector2 point4, bool firstIsSegment, bool secondIsSegment, out TSVector2 point)
	{
		point = default;
		FP fP = point4.y - point3.y;
		FP fP2 = point2.x - point1.x;
		FP fP3 = point4.x - point3.x;
		FP fP4 = point2.y - point1.y;
		FP fP5 = fP * fP2 - fP3 * fP4;
		if (!(fP5 >= -Settings.Epsilon) || !(fP5 <= Settings.Epsilon))
		{
			FP fP6 = point1.y - point3.y;
			FP fP7 = point1.x - point3.x;
			FP fP8 = 1f / fP5;
			FP fP9 = fP3 * fP6 - fP * fP7;
			fP9 *= fP8;
			if (!firstIsSegment || (fP9 >= 0f && fP9 <= 1f))
			{
				FP fP10 = fP2 * fP6 - fP4 * fP7;
				fP10 *= fP8;
				if ((!secondIsSegment || (fP10 >= 0f && fP10 <= 1f)) && (fP9 != 0f || fP10 != 0f))
				{
					point.x = point1.x + fP9 * fP2;
					point.y = point1.y + fP9 * fP4;
					return true;
				}
			}
		}
		return false;
	}

	public static bool LineIntersect(TSVector2 point1, TSVector2 point2, TSVector2 point3, TSVector2 point4, bool firstIsSegment, bool secondIsSegment, out TSVector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment, secondIsSegment, out intersectionPoint);
	}

	public static bool LineIntersect(ref TSVector2 point1, ref TSVector2 point2, ref TSVector2 point3, ref TSVector2 point4, out TSVector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, out intersectionPoint);
	}

	public static bool LineIntersect(TSVector2 point1, TSVector2 point2, TSVector2 point3, TSVector2 point4, out TSVector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, out intersectionPoint);
	}

	public static Vertices LineSegmentVerticesIntersect(ref TSVector2 point1, ref TSVector2 point2, Vertices vertices)
	{
		Vertices vertices2 = new Vertices();
		for (int i = 0; i < vertices.Count; i++)
		{
			if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)], point1, point2, firstIsSegment: true, secondIsSegment: true, out var intersectionPoint))
			{
				vertices2.Add(intersectionPoint);
			}
		}
		return vertices2;
	}

	public static Vertices LineSegmentAABBIntersect(ref TSVector2 point1, ref TSVector2 point2, AABB aabb)
	{
		return LineSegmentVerticesIntersect(ref point1, ref point2, aabb.Vertices);
	}
}
