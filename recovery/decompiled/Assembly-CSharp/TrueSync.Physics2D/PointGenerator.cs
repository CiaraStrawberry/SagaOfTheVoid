using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class PointGenerator
{
	private static readonly TSRandom RNG = TSRandom.New(0);

	public static List<TriangulationPoint> UniformDistribution(int n, FP scale)
	{
		List<TriangulationPoint> list = new List<TriangulationPoint>();
		for (int i = 0; i < n; i++)
		{
			list.Add(new TriangulationPoint(scale * (0.5 - RNG.NextFP()), scale * (0.5 - RNG.NextFP())));
		}
		return list;
	}

	public static List<TriangulationPoint> UniformGrid(int n, FP scale)
	{
		FP fP = 0;
		FP fP2 = scale / n;
		FP fP3 = 0.5 * scale;
		List<TriangulationPoint> list = new List<TriangulationPoint>();
		for (int i = 0; i < n + 1; i++)
		{
			fP = fP3 - i * fP2;
			for (int j = 0; j < n + 1; j++)
			{
				list.Add(new TriangulationPoint(fP, fP3 - j * fP2));
			}
		}
		return list;
	}
}
