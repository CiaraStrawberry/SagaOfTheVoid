using System;

namespace TrueSync.Physics2D;

public static class SeparationFunction
{
	[ThreadStatic]
	private static TSVector2 _axis;

	[ThreadStatic]
	private static TSVector2 _localPoint;

	[ThreadStatic]
	private static DistanceProxy _proxyA;

	[ThreadStatic]
	private static DistanceProxy _proxyB;

	[ThreadStatic]
	private static Sweep _sweepA;

	[ThreadStatic]
	private static Sweep _sweepB;

	[ThreadStatic]
	private static SeparationFunctionType _type;

	public static void Set(ref SimplexCache cache, DistanceProxy proxyA, ref Sweep sweepA, DistanceProxy proxyB, ref Sweep sweepB, FP t1)
	{
		_localPoint = TSVector2.zero;
		_proxyA = proxyA;
		_proxyB = proxyB;
		int count = cache.Count;
		_sweepA = sweepA;
		_sweepB = sweepB;
		_sweepA.GetTransform(out var xfb, t1);
		_sweepB.GetTransform(out var xfb2, t1);
		if (count == 1)
		{
			_type = SeparationFunctionType.Points;
			TSVector2 v = _proxyA.Vertices[cache.IndexA[0]];
			TSVector2 v2 = _proxyB.Vertices[cache.IndexB[0]];
			TSVector2 tSVector = MathUtils.Mul(ref xfb, v);
			TSVector2 tSVector2 = MathUtils.Mul(ref xfb2, v2);
			_axis = tSVector2 - tSVector;
			_axis.Normalize();
		}
		else if (cache.IndexA[0] == cache.IndexA[1])
		{
			_type = SeparationFunctionType.FaceB;
			TSVector2 tSVector3 = proxyB.Vertices[cache.IndexB[0]];
			TSVector2 tSVector4 = proxyB.Vertices[cache.IndexB[1]];
			TSVector2 tSVector5 = tSVector4 - tSVector3;
			_axis = new TSVector2(tSVector5.y, -tSVector5.x);
			_axis.Normalize();
			TSVector2 value = MathUtils.Mul(ref xfb2.q, _axis);
			_localPoint = 0.5f * (tSVector3 + tSVector4);
			TSVector2 tSVector6 = MathUtils.Mul(ref xfb2, _localPoint);
			TSVector2 v3 = proxyA.Vertices[cache.IndexA[0]];
			TSVector2 tSVector7 = MathUtils.Mul(ref xfb, v3);
			FP fP = TSVector2.Dot(tSVector7 - tSVector6, value);
			if (fP < 0f)
			{
				_axis = -_axis;
			}
		}
		else
		{
			_type = SeparationFunctionType.FaceA;
			TSVector2 tSVector8 = _proxyA.Vertices[cache.IndexA[0]];
			TSVector2 tSVector9 = _proxyA.Vertices[cache.IndexA[1]];
			TSVector2 tSVector10 = tSVector9 - tSVector8;
			_axis = new TSVector2(tSVector10.y, -tSVector10.x);
			_axis.Normalize();
			TSVector2 value2 = MathUtils.Mul(ref xfb.q, _axis);
			_localPoint = 0.5f * (tSVector8 + tSVector9);
			TSVector2 tSVector11 = MathUtils.Mul(ref xfb, _localPoint);
			TSVector2 v4 = _proxyB.Vertices[cache.IndexB[0]];
			TSVector2 tSVector12 = MathUtils.Mul(ref xfb2, v4);
			FP fP2 = TSVector2.Dot(tSVector12 - tSVector11, value2);
			if (fP2 < 0f)
			{
				_axis = -_axis;
			}
		}
	}

	public static FP FindMinSeparation(out int indexA, out int indexB, FP t)
	{
		_sweepA.GetTransform(out var xfb, t);
		_sweepB.GetTransform(out var xfb2, t);
		switch (_type)
		{
		case SeparationFunctionType.Points:
		{
			TSVector2 direction3 = MathUtils.MulT(ref xfb.q, _axis);
			TSVector2 direction4 = MathUtils.MulT(ref xfb2.q, -_axis);
			indexA = _proxyA.GetSupport(direction3);
			indexB = _proxyB.GetSupport(direction4);
			TSVector2 v3 = _proxyA.Vertices[indexA];
			TSVector2 v4 = _proxyB.Vertices[indexB];
			TSVector2 tSVector7 = MathUtils.Mul(ref xfb, v3);
			TSVector2 tSVector8 = MathUtils.Mul(ref xfb2, v4);
			return TSVector2.Dot(tSVector8 - tSVector7, _axis);
		}
		case SeparationFunctionType.FaceA:
		{
			TSVector2 tSVector4 = MathUtils.Mul(ref xfb.q, _axis);
			TSVector2 tSVector5 = MathUtils.Mul(ref xfb, _localPoint);
			TSVector2 direction2 = MathUtils.MulT(ref xfb2.q, -tSVector4);
			indexA = -1;
			indexB = _proxyB.GetSupport(direction2);
			TSVector2 v2 = _proxyB.Vertices[indexB];
			TSVector2 tSVector6 = MathUtils.Mul(ref xfb2, v2);
			return TSVector2.Dot(tSVector6 - tSVector5, tSVector4);
		}
		case SeparationFunctionType.FaceB:
		{
			TSVector2 tSVector = MathUtils.Mul(ref xfb2.q, _axis);
			TSVector2 tSVector2 = MathUtils.Mul(ref xfb2, _localPoint);
			TSVector2 direction = MathUtils.MulT(ref xfb.q, -tSVector);
			indexB = -1;
			indexA = _proxyA.GetSupport(direction);
			TSVector2 v = _proxyA.Vertices[indexA];
			TSVector2 tSVector3 = MathUtils.Mul(ref xfb, v);
			return TSVector2.Dot(tSVector3 - tSVector2, tSVector);
		}
		default:
			indexA = -1;
			indexB = -1;
			return 0f;
		}
	}

	public static FP Evaluate(int indexA, int indexB, FP t)
	{
		_sweepA.GetTransform(out var xfb, t);
		_sweepB.GetTransform(out var xfb2, t);
		switch (_type)
		{
		case SeparationFunctionType.Points:
		{
			TSVector2 v3 = _proxyA.Vertices[indexA];
			TSVector2 v4 = _proxyB.Vertices[indexB];
			TSVector2 tSVector5 = MathUtils.Mul(ref xfb, v3);
			TSVector2 tSVector6 = MathUtils.Mul(ref xfb2, v4);
			return TSVector2.Dot(tSVector6 - tSVector5, _axis);
		}
		case SeparationFunctionType.FaceA:
		{
			TSVector2 value2 = MathUtils.Mul(ref xfb.q, _axis);
			TSVector2 tSVector3 = MathUtils.Mul(ref xfb, _localPoint);
			TSVector2 v2 = _proxyB.Vertices[indexB];
			TSVector2 tSVector4 = MathUtils.Mul(ref xfb2, v2);
			return TSVector2.Dot(tSVector4 - tSVector3, value2);
		}
		case SeparationFunctionType.FaceB:
		{
			TSVector2 value = MathUtils.Mul(ref xfb2.q, _axis);
			TSVector2 tSVector = MathUtils.Mul(ref xfb2, _localPoint);
			TSVector2 v = _proxyA.Vertices[indexA];
			TSVector2 tSVector2 = MathUtils.Mul(ref xfb, v);
			return TSVector2.Dot(tSVector2 - tSVector, value);
		}
		default:
			return 0f;
		}
	}
}
