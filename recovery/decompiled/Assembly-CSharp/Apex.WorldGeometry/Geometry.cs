using UnityEngine;

namespace Apex.WorldGeometry;

public static class Geometry
{
	public static bool DoLinesIntersect(Vector3 line1p1, Vector3 line1p2, Vector3 line2p1, Vector3 line2p2)
	{
		Vector3? vector = LineIntersection(line1p1, line1p2, line2p1, line2p2);
		if (!vector.HasValue)
		{
			return false;
		}
		if (IsPointBetweenPointsX(vector.Value, line1p1, line1p2))
		{
			return IsPointBetweenPointsX(vector.Value, line2p1, line2p2);
		}
		return false;
	}

	public static bool DoLinesIntersect(Vector3 line1p1, Vector3 line1p2, Vector3 line2p1, Vector3 line2p2, out Vector3? intersectionPoint)
	{
		intersectionPoint = LineIntersection(line1p1, line1p2, line2p1, line2p2);
		if (!intersectionPoint.HasValue)
		{
			return false;
		}
		if (IsPointBetweenPointsX(intersectionPoint.Value, line1p1, line1p2))
		{
			return IsPointBetweenPointsX(intersectionPoint.Value, line2p1, line2p2);
		}
		return false;
	}

	public static Vector3? LineIntersection(Vector3 line1p1, Vector3 line1p2, Vector3 line2p1, Vector3 line2p2)
	{
		float num = (line1p1.z - line1p2.z) / (line1p1.x - line1p2.x);
		float num2 = line1p1.z - num * line1p1.x;
		float num3 = (line2p1.z - line2p2.z) / (line2p1.x - line2p2.x);
		float num4 = line2p1.z - num3 * line2p1.x;
		if (num == num3)
		{
			return null;
		}
		float num5 = (num2 - num4) / (num3 - num);
		float z = num * num5 + num2;
		return new Vector3(num5, 0f, z);
	}

	public static bool DoesLineIntersectSphere(Vector3 linep1, Vector3 linep2, Vector3 sphereCenter, float sphereRadius)
	{
		Vector3 normalized = (linep2 - linep1).normalized;
		Vector3 rhs = linep1 - sphereCenter;
		float num = Vector3.Dot(normalized, rhs);
		float num2 = num * num - rhs.sqrMagnitude + sphereRadius * sphereRadius;
		return num2 >= 0f;
	}

	private static bool IsPointBetweenPointsX(Vector3 point, Vector3 p1, Vector3 p2)
	{
		float num = Mathf.Max(p1.x, p2.x);
		float num2 = Mathf.Min(p1.x, p2.x);
		return point.x <= num && point.x >= num2;
	}
}
