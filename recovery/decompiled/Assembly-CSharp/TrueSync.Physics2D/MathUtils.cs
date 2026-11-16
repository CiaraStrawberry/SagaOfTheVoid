using System;
using System.Runtime.InteropServices;

namespace TrueSync.Physics2D;

public static class MathUtils
{
	[StructLayout(LayoutKind.Explicit)]
	private struct FPConverter
	{
		[FieldOffset(0)]
		public FP x;

		[FieldOffset(0)]
		public int i;
	}

	public static FP Cross(ref TSVector2 a, ref TSVector2 b)
	{
		return a.x * b.y - a.y * b.x;
	}

	public static FP Cross(TSVector2 a, TSVector2 b)
	{
		return Cross(ref a, ref b);
	}

	public static TSVector Cross(TSVector a, TSVector b)
	{
		return new TSVector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	}

	public static TSVector2 Cross(TSVector2 a, FP s)
	{
		return new TSVector2(s * a.y, -s * a.x);
	}

	public static TSVector2 Cross(FP s, TSVector2 a)
	{
		return new TSVector2(-s * a.y, s * a.x);
	}

	public static TSVector2 Abs(TSVector2 v)
	{
		return new TSVector2(FP.Abs(v.x), FP.Abs(v.y));
	}

	public static TSVector2 Mul(ref Mat22 A, TSVector2 v)
	{
		return Mul(ref A, ref v);
	}

	public static TSVector2 Mul(ref Mat22 A, ref TSVector2 v)
	{
		return new TSVector2(A.ex.x * v.x + A.ey.x * v.y, A.ex.y * v.x + A.ey.y * v.y);
	}

	public static TSVector2 Mul(ref Transform T, TSVector2 v)
	{
		return Mul(ref T, ref v);
	}

	public static TSVector2 Mul(ref Transform T, ref TSVector2 v)
	{
		FP x = T.q.c * v.x - T.q.s * v.y + T.p.x;
		FP y = T.q.s * v.x + T.q.c * v.y + T.p.y;
		return new TSVector2(x, y);
	}

	public static TSVector2 MulT(ref Mat22 A, TSVector2 v)
	{
		return MulT(ref A, ref v);
	}

	public static TSVector2 MulT(ref Mat22 A, ref TSVector2 v)
	{
		return new TSVector2(v.x * A.ex.x + v.y * A.ex.y, v.x * A.ey.x + v.y * A.ey.y);
	}

	public static TSVector2 MulT(ref Transform T, TSVector2 v)
	{
		return MulT(ref T, ref v);
	}

	public static TSVector2 MulT(ref Transform T, ref TSVector2 v)
	{
		FP fP = v.x - T.p.x;
		FP fP2 = v.y - T.p.y;
		FP x = T.q.c * fP + T.q.s * fP2;
		FP y = -T.q.s * fP + T.q.c * fP2;
		return new TSVector2(x, y);
	}

	public static void MulT(ref Mat22 A, ref Mat22 B, out Mat22 C)
	{
		C = default;
		C.ex.x = A.ex.x * B.ex.x + A.ex.y * B.ex.y;
		C.ex.y = A.ey.x * B.ex.x + A.ey.y * B.ex.y;
		C.ey.x = A.ex.x * B.ey.x + A.ex.y * B.ey.y;
		C.ey.y = A.ey.x * B.ey.x + A.ey.y * B.ey.y;
	}

	public static TSVector Mul(Mat33 A, TSVector v)
	{
		return v.x * A.ex + v.y * A.ey + v.z * A.ez;
	}

	public static Transform Mul(Transform A, Transform B)
	{
		return new Transform
		{
			q = Mul(A.q, B.q),
			p = Mul(A.q, B.p) + A.p
		};
	}

	public static void MulT(ref Transform A, ref Transform B, out Transform C)
	{
		C = default;
		C.q = MulT(A.q, B.q);
		C.p = MulT(A.q, B.p - A.p);
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static TSVector2 Mul22(Mat33 A, TSVector2 v)
	{
		return new TSVector2(A.ex.x * v.x + A.ey.x * v.y, A.ex.y * v.x + A.ey.y * v.y);
	}

	public static Rot Mul(Rot q, Rot r)
	{
		Rot result = default;
		result.s = q.s * r.c + q.c * r.s;
		result.c = q.c * r.c - q.s * r.s;
		return result;
	}

	public static TSVector2 MulT(Transform T, TSVector2 v)
	{
		FP fP = v.x - T.p.x;
		FP fP2 = v.y - T.p.y;
		FP x = T.q.c * fP + T.q.s * fP2;
		FP y = -T.q.s * fP + T.q.c * fP2;
		return new TSVector2(x, y);
	}

	public static Rot MulT(Rot q, Rot r)
	{
		Rot result = default;
		result.s = q.c * r.s - q.s * r.c;
		result.c = q.c * r.c + q.s * r.s;
		return result;
	}

	public static Transform MulT(Transform A, Transform B)
	{
		return new Transform
		{
			q = MulT(A.q, B.q),
			p = MulT(A.q, B.p - A.p)
		};
	}

	public static TSVector2 Mul(Rot q, TSVector2 v)
	{
		return new TSVector2(q.c * v.x - q.s * v.y, q.s * v.x + q.c * v.y);
	}

	public static TSVector2 MulT(Rot q, TSVector2 v)
	{
		return new TSVector2(q.c * v.x + q.s * v.y, -q.s * v.x + q.c * v.y);
	}

	public static TSVector2 Skew(TSVector2 input)
	{
		return new TSVector2(-input.y, input.x);
	}

	public static bool IsValid(FP x)
	{
		if (FP.IsNaN(x))
		{
			return false;
		}
		return !FP.IsInfinity(x);
	}

	public static bool IsValid(this TSVector2 x)
	{
		return IsValid(x.x) && IsValid(x.y);
	}

	public static FP InvSqrt(FP x)
	{
		FPConverter fPConverter = default;
		fPConverter.x = x;
		FP fP = 0.5f * x;
		fPConverter.i = 1597463007 - (fPConverter.i >> 1);
		x = fPConverter.x;
		x *= 1.5f - fP * x * x;
		return x;
	}

	public static int Clamp(int a, int low, int high)
	{
		return Math.Max(low, Math.Min(a, high));
	}

	public static FP Clamp(FP a, FP low, FP high)
	{
		return TSMath.Max(low, TSMath.Min(a, high));
	}

	public static TSVector2 Clamp(TSVector2 a, TSVector2 low, TSVector2 high)
	{
		return TSVector2.Max(low, TSVector2.Min(a, high));
	}

	public static void Cross(ref TSVector2 a, ref TSVector2 b, out FP c)
	{
		c = a.x * b.y - a.y * b.x;
	}

	public static FP VectorAngle(ref TSVector2 p1, ref TSVector2 p2)
	{
		FP fP = FP.Atan2(p1.y, p1.x);
		FP fP2 = FP.Atan2(p2.y, p2.x);
		FP fP3;
		for (fP3 = fP2 - fP; fP3 > FP.Pi; fP3 -= 2 * FP.Pi)
		{
		}
		for (; fP3 < -FP.Pi; fP3 += 2 * FP.Pi)
		{
		}
		return fP3;
	}

	public static FP Dot(TSVector a, TSVector b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static FP VectorAngle(TSVector2 p1, TSVector2 p2)
	{
		return VectorAngle(ref p1, ref p2);
	}

	public static FP Area(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		return Area(ref a, ref b, ref c);
	}

	public static FP Area(ref TSVector2 a, ref TSVector2 b, ref TSVector2 c)
	{
		return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
	}

	public static bool IsCollinear(ref TSVector2 a, ref TSVector2 b, ref TSVector2 c, FP tolerance)
	{
		return FPInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);
	}

	public static void Cross(FP s, ref TSVector2 a, out TSVector2 b)
	{
		b = new TSVector2(-s * a.y, s * a.x);
	}

	public static bool FPEquals(FP value1, FP value2)
	{
		return FP.Abs(value1 - value2) <= Settings.Epsilon;
	}

	public static bool FPEquals(FP value1, FP value2, FP delta)
	{
		return FPInRange(value1, value2 - delta, value2 + delta);
	}

	public static bool FPInRange(FP value, FP min, FP max)
	{
		return value >= min && value <= max;
	}

	public static TSVector2 Mul(ref Rot rot, TSVector2 axis)
	{
		return Mul(rot, axis);
	}

	public static TSVector2 MulT(ref Rot rot, TSVector2 axis)
	{
		return MulT(rot, axis);
	}
}
