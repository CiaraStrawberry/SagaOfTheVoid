namespace TrueSync.Physics2D;

public static class GiftWrap
{
	public static Vertices GetConvexHull(Vertices vertices)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		int num = 0;
		FP fP = vertices[0].x;
		for (int i = 1; i < vertices.Count; i++)
		{
			FP x = vertices[i].x;
			if (x > fP || (x == fP && vertices[i].y < vertices[num].y))
			{
				num = i;
				fP = x;
			}
		}
		int[] array = new int[vertices.Count];
		int num2 = 0;
		int num3 = num;
		int num4;
		do
		{
			array[num2] = num3;
			num4 = 0;
			for (int j = 1; j < vertices.Count; j++)
			{
				if (num4 == num3)
				{
					num4 = j;
					continue;
				}
				TSVector2 a = vertices[num4] - vertices[array[num2]];
				TSVector2 b = vertices[j] - vertices[array[num2]];
				FP fP2 = MathUtils.Cross(ref a, ref b);
				if (fP2 < 0f)
				{
					num4 = j;
				}
				if (fP2 == 0f && b.LengthSquared() > a.LengthSquared())
				{
					num4 = j;
				}
			}
			num2++;
			num3 = num4;
		}
		while (num4 != num);
		Vertices vertices2 = new Vertices(num2);
		for (int k = 0; k < num2; k++)
		{
			vertices2.Add(vertices[array[k]]);
		}
		return vertices2;
	}
}
