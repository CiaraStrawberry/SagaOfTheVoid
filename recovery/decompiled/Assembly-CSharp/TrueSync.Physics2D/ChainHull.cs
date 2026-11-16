using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class ChainHull
{
	private class PointComparer : Comparer<TSVector2>
	{
		public override int Compare(TSVector2 a, TSVector2 b)
		{
			int num = a.x.CompareTo(b.x);
			return (num == 0) ? a.y.CompareTo(b.y) : num;
		}
	}

	private static PointComparer _pointComparer = new PointComparer();

	public static Vertices GetConvexHull(Vertices vertices)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		Vertices vertices2 = new Vertices(vertices);
		vertices2.Sort(_pointComparer);
		TSVector2[] array = new TSVector2[vertices2.Count];
		int num = -1;
		FP x = vertices2[0].x;
		int i;
		for (i = 1; i < vertices2.Count && !(vertices2[i].x != x); i++)
		{
		}
		int num2 = i - 1;
		Vertices vertices3;
		if (num2 == vertices2.Count - 1)
		{
			ref TSVector2 reference = ref array[++num];
			reference = vertices2[0];
			if (vertices2[num2].y != vertices2[0].y)
			{
				ref TSVector2 reference2 = ref array[++num];
				reference2 = vertices2[num2];
			}
			ref TSVector2 reference3 = ref array[++num];
			reference3 = vertices2[0];
			vertices3 = new Vertices(num + 1);
			for (int j = 0; j < num + 1; j++)
			{
				vertices3.Add(array[j]);
			}
			return vertices3;
		}
		num = -1;
		int num3 = vertices2.Count - 1;
		FP x2 = vertices2[vertices2.Count - 1].x;
		i = vertices2.Count - 2;
		while (i >= 0 && !(vertices2[i].x != x2))
		{
			i--;
		}
		int num4 = i + 1;
		ref TSVector2 reference4 = ref array[++num];
		reference4 = vertices2[0];
		i = num2;
		while (++i <= num4)
		{
			if (!(MathUtils.Area(vertices2[0], vertices2[num4], vertices2[i]) >= 0) || i >= num4)
			{
				while (num > 0 && !(MathUtils.Area(array[num - 1], array[num], vertices2[i]) > 0))
				{
					num--;
				}
				ref TSVector2 reference5 = ref array[++num];
				reference5 = vertices2[i];
			}
		}
		if (num3 != num4)
		{
			ref TSVector2 reference6 = ref array[++num];
			reference6 = vertices2[num3];
		}
		int num5 = num;
		i = num4;
		while (--i >= num2)
		{
			if (!(MathUtils.Area(vertices2[num3], vertices2[num2], vertices2[i]) >= 0) || i <= num2)
			{
				while (num > num5 && !(MathUtils.Area(array[num - 1], array[num], vertices2[i]) > 0))
				{
					num--;
				}
				ref TSVector2 reference7 = ref array[++num];
				reference7 = vertices2[i];
			}
		}
		if (num2 != 0)
		{
			ref TSVector2 reference8 = ref array[++num];
			reference8 = vertices2[0];
		}
		vertices3 = new Vertices(num + 1);
		for (int k = 0; k < num + 1; k++)
		{
			vertices3.Add(array[k]);
		}
		return vertices3;
	}
}
