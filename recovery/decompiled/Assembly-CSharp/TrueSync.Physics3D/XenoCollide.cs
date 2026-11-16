using UnityEngine;

namespace TrueSync.Physics3D;

public sealed class XenoCollide
{
	private static FP CollideEpsilon = FP.EN4;

	private const int MaximumIterations = 34;

	private static void SupportMapTransformed(ISupportMappable support, ref TSMatrix orientation, ref TSVector position, ref TSVector direction, out TSVector result)
	{
		result.x = direction.x * orientation.M11 + direction.y * orientation.M12 + direction.z * orientation.M13;
		result.y = direction.x * orientation.M21 + direction.y * orientation.M22 + direction.z * orientation.M23;
		result.z = direction.x * orientation.M31 + direction.y * orientation.M32 + direction.z * orientation.M33;
		support.SupportMapping(ref result, out result);
		FP fP = result.x * orientation.M11 + result.y * orientation.M21 + result.z * orientation.M31;
		FP fP2 = result.x * orientation.M12 + result.y * orientation.M22 + result.z * orientation.M32;
		FP fP3 = result.x * orientation.M13 + result.y * orientation.M23 + result.z * orientation.M33;
		result.x = position.x + fP;
		result.y = position.y + fP2;
		result.z = position.z + fP3;
	}

	public static bool Detect(ISupportMappable support1, ISupportMappable support2, ref TSMatrix orientation1, ref TSMatrix orientation2, ref TSVector position1, ref TSVector position2, out TSVector point, out TSVector normal, out FP penetration)
	{
		TSVector result = TSVector.zero;
		TSVector result2 = TSVector.zero;
		TSVector result3 = TSVector.zero;
		point = (normal = TSVector.zero);
		penetration = FP.Zero;
		support1.SupportCenter(out var center);
		TSVector.Transform(ref center, ref orientation1, out center);
		TSVector.Add(ref position1, ref center, out center);
		support2.SupportCenter(out var center2);
		TSVector.Transform(ref center2, ref orientation2, out center2);
		TSVector.Add(ref position2, ref center2, out center2);
		TSVector.Subtract(ref center2, ref center, out var result4);
		if (result4.IsNearlyZero())
		{
			result4 = new TSVector(FP.EN4, 0, 0);
		}
		TSVector direction = result4;
		TSVector.Negate(ref result4, out normal);
		SupportMapTransformed(support1, ref orientation1, ref position1, ref direction, out var result5);
		SupportMapTransformed(support2, ref orientation2, ref position2, ref normal, out var result6);
		TSVector.Subtract(ref result6, ref result5, out var result7);
		if (TSVector.Dot(ref result7, ref normal) <= FP.Zero)
		{
			return false;
		}
		TSVector.Cross(ref result7, ref result4, out normal);
		TSVector result8;
		if (normal.IsNearlyZero())
		{
			TSVector.Subtract(ref result7, ref result4, out normal);
			normal.Normalize();
			point = result5;
			TSVector.Add(ref point, ref result6, out point);
			TSVector.Multiply(ref point, FP.Half, out point);
			TSVector.Subtract(ref result6, ref result5, out result8);
			penetration = TSVector.Dot(ref result8, ref normal);
			return true;
		}
		TSVector.Negate(ref normal, out direction);
		SupportMapTransformed(support1, ref orientation1, ref position1, ref direction, out var result9);
		SupportMapTransformed(support2, ref orientation2, ref position2, ref normal, out var result10);
		TSVector.Subtract(ref result10, ref result9, out var result11);
		if (TSVector.Dot(ref result11, ref normal) <= FP.Zero)
		{
			return false;
		}
		TSVector.Subtract(ref result7, ref result4, out result8);
		TSVector.Subtract(ref result11, ref result4, out var result12);
		TSVector.Cross(ref result8, ref result12, out normal);
		FP fP = TSVector.Dot(ref normal, ref result4);
		if (fP > FP.Zero)
		{
			TSVector.Swap(ref result7, ref result11);
			TSVector.Swap(ref result5, ref result9);
			TSVector.Swap(ref result6, ref result10);
			TSVector.Negate(ref normal, out normal);
			Debug.Log("normal: " + normal);
		}
		int num = 0;
		int num2 = 0;
		bool flag = false;
		TSVector result13;
		TSVector result14;
		TSVector result15;
		while (true)
		{
			if (num2 > 34)
			{
				return false;
			}
			num2++;
			TSVector.Negate(ref normal, out direction);
			SupportMapTransformed(support1, ref orientation1, ref position1, ref direction, out result13);
			SupportMapTransformed(support2, ref orientation2, ref position2, ref normal, out result14);
			TSVector.Subtract(ref result14, ref result13, out result15);
			if (TSVector.Dot(ref result15, ref normal) <= FP.Zero)
			{
				return false;
			}
			TSVector.Cross(ref result7, ref result15, out result8);
			if (TSVector.Dot(ref result8, ref result4) < FP.Zero)
			{
				result11 = result15;
				result9 = result13;
				result10 = result14;
				TSVector.Subtract(ref result7, ref result4, out result8);
				TSVector.Subtract(ref result15, ref result4, out result12);
				TSVector.Cross(ref result8, ref result12, out normal);
				continue;
			}
			TSVector.Cross(ref result15, ref result11, out result8);
			if (!(TSVector.Dot(ref result8, ref result4) < FP.Zero))
			{
				break;
			}
			result7 = result15;
			result5 = result13;
			result6 = result14;
			TSVector.Subtract(ref result15, ref result4, out result8);
			TSVector.Subtract(ref result11, ref result4, out result12);
			TSVector.Cross(ref result8, ref result12, out normal);
		}
		while (true)
		{
			num++;
			TSVector.Subtract(ref result11, ref result7, out result8);
			TSVector.Subtract(ref result15, ref result7, out result12);
			TSVector.Cross(ref result8, ref result12, out normal);
			if (normal.IsNearlyZero())
			{
				return true;
			}
			normal.Normalize();
			FP fP2 = TSVector.Dot(ref normal, ref result7);
			if (fP2 >= 0 && !flag)
			{
				flag = true;
			}
			TSVector.Negate(ref normal, out direction);
			SupportMapTransformed(support1, ref orientation1, ref position1, ref direction, out result);
			SupportMapTransformed(support2, ref orientation2, ref position2, ref normal, out result2);
			TSVector.Subtract(ref result2, ref result, out result3);
			TSVector.Subtract(ref result3, ref result15, out result8);
			FP fP3 = TSVector.Dot(ref result8, ref normal);
			penetration = TSVector.Dot(ref result3, ref normal);
			if (fP3 <= CollideEpsilon || penetration <= FP.Zero || num > 34)
			{
				break;
			}
			TSVector.Cross(ref result3, ref result4, out result8);
			FP fP4 = TSVector.Dot(ref result8, ref result7);
			if (fP4 >= FP.Zero)
			{
				fP4 = TSVector.Dot(ref result8, ref result11);
				if (fP4 >= FP.Zero)
				{
					result7 = result3;
					result5 = result;
					result6 = result2;
				}
				else
				{
					result15 = result3;
					result13 = result;
					result14 = result2;
				}
			}
			else
			{
				fP4 = TSVector.Dot(ref result8, ref result15);
				if (fP4 >= FP.Zero)
				{
					result11 = result3;
					result9 = result;
					result10 = result2;
				}
				else
				{
					result7 = result3;
					result5 = result;
					result6 = result2;
				}
			}
		}
		if (flag)
		{
			TSVector.Cross(ref result7, ref result11, out result8);
			FP fP5 = TSVector.Dot(ref result8, ref result15);
			TSVector.Cross(ref result15, ref result11, out result8);
			FP fP6 = TSVector.Dot(ref result8, ref result4);
			TSVector.Cross(ref result4, ref result7, out result8);
			FP fP7 = TSVector.Dot(ref result8, ref result15);
			TSVector.Cross(ref result11, ref result7, out result8);
			FP fP8 = TSVector.Dot(ref result8, ref result4);
			FP fP9 = fP5 + fP6 + fP7 + fP8;
			if (fP9 <= 0)
			{
				fP5 = 0;
				TSVector.Cross(ref result11, ref result15, out result8);
				fP6 = TSVector.Dot(ref result8, ref normal);
				TSVector.Cross(ref result15, ref result7, out result8);
				fP7 = TSVector.Dot(ref result8, ref normal);
				TSVector.Cross(ref result7, ref result11, out result8);
				fP8 = TSVector.Dot(ref result8, ref normal);
				fP9 = fP6 + fP7 + fP8;
			}
			FP fP10 = FP.One / fP9;
			TSVector.Multiply(ref center, fP5, out point);
			TSVector.Multiply(ref result5, fP6, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref result9, fP7, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref result13, fP8, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref center2, fP5, out result12);
			TSVector.Add(ref result12, ref point, out point);
			TSVector.Multiply(ref result6, fP6, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref result10, fP7, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref result14, fP8, out result8);
			TSVector.Add(ref point, ref result8, out point);
			TSVector.Multiply(ref point, fP10 * FP.Half, out point);
		}
		return flag;
	}
}
