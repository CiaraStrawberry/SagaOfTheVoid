using System;

namespace TrueSync.Physics2D;

public static class Distance
{
	[ThreadStatic]
	public static int GJKCalls;

	[ThreadStatic]
	public static int GJKIters;

	[ThreadStatic]
	public static int GJKMaxIters;

	public static void ComputeDistance(out DistanceOutput output, out SimplexCache cache, DistanceInput input)
	{
		cache = default;
		Simplex simplex = default;
		simplex.ReadCache(ref cache, input.ProxyA, ref input.TransformA, input.ProxyB, ref input.TransformB);
		FixedArray3<int> fixedArray = default;
		FixedArray3<int> fixedArray2 = default;
		int num = 0;
		while (num < 20)
		{
			int count = simplex.Count;
			for (int i = 0; i < count; i++)
			{
				fixedArray[i] = simplex.V[i].IndexA;
				fixedArray2[i] = simplex.V[i].IndexB;
			}
			switch (simplex.Count)
			{
			case 2:
				simplex.Solve2();
				break;
			case 3:
				simplex.Solve3();
				break;
			}
			if (simplex.Count == 3)
			{
				break;
			}
			TSVector2 searchDirection = simplex.GetSearchDirection();
			if (searchDirection.LengthSquared() < Settings.EpsilonSqr)
			{
				break;
			}
			SimplexVertex value = simplex.V[simplex.Count];
			value.IndexA = input.ProxyA.GetSupport(MathUtils.MulT(input.TransformA.q, -searchDirection));
			value.WA = MathUtils.Mul(ref input.TransformA, input.ProxyA.Vertices[value.IndexA]);
			value.IndexB = input.ProxyB.GetSupport(MathUtils.MulT(input.TransformB.q, searchDirection));
			value.WB = MathUtils.Mul(ref input.TransformB, input.ProxyB.Vertices[value.IndexB]);
			value.W = value.WB - value.WA;
			simplex.V[simplex.Count] = value;
			num++;
			bool flag = false;
			for (int j = 0; j < count; j++)
			{
				if (value.IndexA == fixedArray[j] && value.IndexB == fixedArray2[j])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			simplex.Count++;
		}
		simplex.GetWitnessPoints(out output.PointA, out output.PointB);
		output.Distance = (output.PointA - output.PointB).magnitude;
		output.Iterations = num;
		simplex.WriteCache(ref cache);
		if (input.UseRadii)
		{
			FP radius = input.ProxyA.Radius;
			FP radius2 = input.ProxyB.Radius;
			if (output.Distance > radius + radius2 && output.Distance > Settings.Epsilon)
			{
				output.Distance -= radius + radius2;
				TSVector2 tSVector = output.PointB - output.PointA;
				tSVector.Normalize();
				output.PointA += radius * tSVector;
				output.PointB -= radius2 * tSVector;
			}
			else
			{
				output.PointB = (output.PointA = 0.5f * (output.PointA + output.PointB));
				output.Distance = 0f;
			}
		}
	}
}
