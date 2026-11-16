using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class SimplifyTools
{
	public static Vertices CollinearSimplify(Vertices vertices, FP collinearityTolerance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		Vertices vertices2 = new Vertices(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			TSVector2 a = vertices.PreviousVertex(i);
			TSVector2 b = vertices[i];
			TSVector2 c = vertices.NextVertex(i);
			if (!MathUtils.IsCollinear(ref a, ref b, ref c, collinearityTolerance))
			{
				vertices2.Add(b);
			}
		}
		return vertices2;
	}

	public static Vertices DouglasPeuckerSimplify(Vertices vertices, FP distanceTolerance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		bool[] array = new bool[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
		{
			array[i] = true;
		}
		SimplifySection(vertices, 0, vertices.Count - 1, array, distanceTolerance);
		Vertices vertices2 = new Vertices(vertices.Count);
		for (int j = 0; j < vertices.Count; j++)
		{
			if (array[j])
			{
				vertices2.Add(vertices[j]);
			}
		}
		return vertices2;
	}

	private static void SimplifySection(Vertices vertices, int i, int j, bool[] usePoint, FP distanceTolerance)
	{
		if (i + 1 == j)
		{
			return;
		}
		TSVector2 start = vertices[i];
		TSVector2 end = vertices[j];
		FP fP = -1.0;
		int num = i;
		for (int k = i + 1; k < j; k++)
		{
			TSVector2 point = vertices[k];
			FP fP2 = LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end);
			if (fP2 > fP)
			{
				fP = fP2;
				num = k;
			}
		}
		if (fP <= distanceTolerance)
		{
			for (int l = i + 1; l < j; l++)
			{
				usePoint[l] = false;
			}
		}
		else
		{
			SimplifySection(vertices, i, num, usePoint, distanceTolerance);
			SimplifySection(vertices, num, j, usePoint, distanceTolerance);
		}
	}

	public static Vertices MergeParallelEdges(Vertices vertices, FP tolerance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		bool[] array = new bool[vertices.Count];
		int num = vertices.Count;
		for (int i = 0; i < vertices.Count; i++)
		{
			int index = ((i != 0) ? (i - 1) : (vertices.Count - 1));
			int index2 = i;
			int index3 = ((i != vertices.Count - 1) ? (i + 1) : 0);
			FP fP = vertices[index2].x - vertices[index].x;
			FP fP2 = vertices[index2].y - vertices[index].y;
			FP fP3 = vertices[index3].y - vertices[index2].x;
			FP fP4 = vertices[index3].y - vertices[index2].y;
			FP fP5 = FP.Sqrt(fP * fP + fP2 * fP2);
			FP fP6 = FP.Sqrt(fP3 * fP3 + fP4 * fP4);
			if ((!(fP5 > 0f) || !(fP6 > 0f)) && num > 3)
			{
				array[i] = true;
				num--;
			}
			fP /= fP5;
			fP2 /= fP5;
			fP3 /= fP6;
			fP4 /= fP6;
			FP value = fP * fP4 - fP3 * fP2;
			FP fP7 = fP * fP3 + fP2 * fP4;
			if (FP.Abs(value) < tolerance && fP7 > 0 && num > 3)
			{
				array[i] = true;
				num--;
			}
			else
			{
				array[i] = false;
			}
		}
		if (num == vertices.Count || num == 0)
		{
			return vertices;
		}
		int num2 = 0;
		Vertices vertices2 = new Vertices(num);
		for (int j = 0; j < vertices.Count; j++)
		{
			if (!array[j] && num != 0 && num2 != num)
			{
				vertices2.Add(vertices[j]);
				num2++;
			}
		}
		return vertices2;
	}

	public static Vertices MergeIdenticalPoints(Vertices vertices)
	{
		HashSet<TSVector2> hashSet = new HashSet<TSVector2>();
		foreach (TSVector2 vertex in vertices)
		{
			hashSet.Add(vertex);
		}
		return new Vertices(hashSet);
	}

	public static Vertices ReduceByDistance(Vertices vertices, FP distance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		FP fP = distance * distance;
		Vertices vertices2 = new Vertices(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			TSVector2 tSVector = vertices[i];
			TSVector2 tSVector2 = vertices.NextVertex(i);
			if (!((tSVector2 - tSVector).LengthSquared() <= fP))
			{
				vertices2.Add(tSVector);
			}
		}
		return vertices2;
	}

	public static Vertices ReduceByNth(Vertices vertices, int nth)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		if (nth == 0)
		{
			return vertices;
		}
		Vertices vertices2 = new Vertices(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			if (i % nth != 0)
			{
				vertices2.Add(vertices[i]);
			}
		}
		return vertices2;
	}

	public static Vertices ReduceByArea(Vertices vertices, FP areaTolerance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		if (areaTolerance < 0)
		{
			throw new ArgumentOutOfRangeException("areaTolerance", "must be equal to or greater than zero.");
		}
		Vertices vertices2 = new Vertices(vertices.Count);
		TSVector2 a = vertices[vertices.Count - 2];
		TSVector2 b = vertices[vertices.Count - 1];
		areaTolerance *= (FP)2;
		int num = 0;
		while (num < vertices.Count)
		{
			TSVector2 b2 = ((num != vertices.Count - 1) ? vertices[num] : vertices2[0]);
			MathUtils.Cross(ref a, ref b, out var c);
			MathUtils.Cross(ref b, ref b2, out var c2);
			MathUtils.Cross(ref a, ref b2, out var c3);
			if (FP.Abs(c3 - (c + c2)) > areaTolerance)
			{
				vertices2.Add(b);
				a = b;
			}
			num++;
			b = b2;
		}
		return vertices2;
	}
}
