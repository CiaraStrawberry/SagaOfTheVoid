using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class BayazitDecomposer
{
	public static List<Vertices> ConvexPartition(Vertices vertices)
	{
		vertices.ForceCounterClockWise();
		return TriangulatePolygon(vertices);
	}

	private static List<Vertices> TriangulatePolygon(Vertices vertices)
	{
		List<Vertices> list = new List<Vertices>();
		TSVector2 tSVector = default;
		TSVector2 tSVector2 = default;
		int num = 0;
		int i = 0;
		for (int j = 0; j < vertices.Count; j++)
		{
			if (!Reflex(j, vertices))
			{
				continue;
			}
			FP fP2;
			FP fP = (fP2 = FP.MaxValue);
			for (int k = 0; k < vertices.Count; k++)
			{
				TSVector2 tSVector3;
				if (Left(At(j - 1, vertices), At(j, vertices), At(k, vertices)) && RightOn(At(j - 1, vertices), At(j, vertices), At(k - 1, vertices)))
				{
					tSVector3 = LineTools.LineIntersect(At(j - 1, vertices), At(j, vertices), At(k, vertices), At(k - 1, vertices));
					if (Right(At(j + 1, vertices), At(j, vertices), tSVector3))
					{
						FP fP3 = SquareDist(At(j, vertices), tSVector3);
						if (fP3 < fP)
						{
							fP = fP3;
							tSVector = tSVector3;
							num = k;
						}
					}
				}
				if (!Left(At(j + 1, vertices), At(j, vertices), At(k + 1, vertices)) || !RightOn(At(j + 1, vertices), At(j, vertices), At(k, vertices)))
				{
					continue;
				}
				tSVector3 = LineTools.LineIntersect(At(j + 1, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices));
				if (Left(At(j - 1, vertices), At(j, vertices), tSVector3))
				{
					FP fP3 = SquareDist(At(j, vertices), tSVector3);
					if (fP3 < fP2)
					{
						fP2 = fP3;
						i = k;
						tSVector2 = tSVector3;
					}
				}
			}
			Vertices vertices2;
			Vertices vertices3;
			if (num == (i + 1) % vertices.Count)
			{
				TSVector2 item = (tSVector + tSVector2) / 2;
				vertices2 = Copy(j, i, vertices);
				vertices2.Add(item);
				vertices3 = Copy(num, j, vertices);
				vertices3.Add(item);
			}
			else
			{
				FP fP4 = 0;
				FP fP5 = num;
				for (; i < num; i += vertices.Count)
				{
				}
				for (int l = num; l <= i; l++)
				{
					if (!CanSee(j, l, vertices))
					{
						continue;
					}
					FP fP6 = 1 / (SquareDist(At(j, vertices), At(l, vertices)) + 1);
					if (Reflex(l, vertices))
					{
						if (RightOn(At(l - 1, vertices), At(l, vertices), At(j, vertices)) && LeftOn(At(l + 1, vertices), At(l, vertices), At(j, vertices)))
						{
							fP6 += (FP)3;
						}
						else
						{
							fP6 += (FP)2;
						}
					}
					else
					{
						fP6 += (FP)1;
					}
					if (fP6 > fP4)
					{
						fP5 = l;
						fP4 = fP6;
					}
				}
				vertices2 = Copy(j, (int)(long)fP5, vertices);
				vertices3 = Copy((int)(long)fP5, j, vertices);
			}
			list.AddRange(TriangulatePolygon(vertices2));
			list.AddRange(TriangulatePolygon(vertices3));
			return list;
		}
		if (vertices.Count > Settings.MaxPolygonVertices)
		{
			Vertices vertices2 = Copy(0, vertices.Count / 2, vertices);
			Vertices vertices3 = Copy(vertices.Count / 2, 0, vertices);
			list.AddRange(TriangulatePolygon(vertices2));
			list.AddRange(TriangulatePolygon(vertices3));
		}
		else
		{
			list.Add(vertices);
		}
		return list;
	}

	private static TSVector2 At(int i, Vertices vertices)
	{
		int count = vertices.Count;
		return vertices[(i >= 0) ? (i % count) : (count - 1 - (-i - 1) % count)];
	}

	private static Vertices Copy(int i, int j, Vertices vertices)
	{
		while (j < i)
		{
			j += vertices.Count;
		}
		Vertices vertices2 = new Vertices(j);
		while (i <= j)
		{
			vertices2.Add(At(i, vertices));
			i++;
		}
		return vertices2;
	}

	private static bool CanSee(int i, int j, Vertices vertices)
	{
		if (Reflex(i, vertices))
		{
			if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) && RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)))
			{
				return false;
			}
		}
		else if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) || LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)))
		{
			return false;
		}
		if (Reflex(j, vertices))
		{
			if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) && RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)))
			{
				return false;
			}
		}
		else if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) || LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)))
		{
			return false;
		}
		for (int k = 0; k < vertices.Count; k++)
		{
			if ((k + 1) % vertices.Count != i && k != i && (k + 1) % vertices.Count != j && k != j && LineTools.LineIntersect(At(i, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices), out var _))
			{
				return false;
			}
		}
		return true;
	}

	private static bool Reflex(int i, Vertices vertices)
	{
		return Right(i, vertices);
	}

	private static bool Right(int i, Vertices vertices)
	{
		return Right(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
	}

	private static bool Left(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		return MathUtils.Area(ref a, ref b, ref c) > 0;
	}

	private static bool LeftOn(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		return MathUtils.Area(ref a, ref b, ref c) >= 0;
	}

	private static bool Right(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		return MathUtils.Area(ref a, ref b, ref c) < 0;
	}

	private static bool RightOn(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		return MathUtils.Area(ref a, ref b, ref c) <= 0;
	}

	private static FP SquareDist(TSVector2 a, TSVector2 b)
	{
		FP fP = b.x - a.x;
		FP fP2 = b.y - a.y;
		return fP * fP + fP2 * fP2;
	}
}
