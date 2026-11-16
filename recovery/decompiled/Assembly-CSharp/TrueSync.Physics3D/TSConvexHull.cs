using System.Collections.Generic;

namespace TrueSync.Physics3D;

public static class TSConvexHull
{
	public enum Approximation
	{
		Level1 = 6,
		Level2 = 7,
		Level3 = 8,
		Level4 = 9,
		Level5 = 10,
		Level6 = 11,
		Level7 = 12,
		Level8 = 13,
		Level9 = 15,
		Level10 = 20,
		Level15 = 25,
		Level20 = 30
	}

	public static int[] Build(List<TSVector> pointCloud, Approximation factor)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < (int)factor; i++)
		{
			FP x = TSMath.Pi / (int)(factor - 1) * i;
			FP fP = FP.Sin(x);
			FP y = FP.Cos(x);
			for (int j = 0; j < (int)factor; j++)
			{
				FP x2 = 2 * FP.One * TSMath.Pi / (int)factor * j - TSMath.Pi;
				FP fP2 = FP.Sin(x2);
				FP fP3 = FP.Cos(x2);
				TSVector dir = new TSVector(fP * fP3, y, fP * fP2);
				int item = FindExtremePoint(pointCloud, ref dir);
				list.Add(item);
			}
		}
		list.Sort();
		for (int k = 1; k < list.Count; k++)
		{
			if (list[k - 1] == list[k])
			{
				list.RemoveAt(k - 1);
				k--;
			}
		}
		return list.ToArray();
	}

	private static int FindExtremePoint(List<TSVector> points, ref TSVector dir)
	{
		int result = 0;
		FP fP = FP.MinValue;
		for (int i = 1; i < points.Count; i++)
		{
			TSVector vector = points[i];
			FP fP2 = TSVector.Dot(ref vector, ref dir);
			if (fP2 > fP)
			{
				fP = fP2;
				result = i;
			}
		}
		return result;
	}
}
