using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class YuPengClipper
{
	private sealed class Edge
	{
		public TSVector2 EdgeStart { get; private set; }

		public TSVector2 EdgeEnd { get; private set; }

		public Edge(TSVector2 edgeStart, TSVector2 edgeEnd)
		{
			EdgeStart = edgeStart;
			EdgeEnd = edgeEnd;
		}

		public TSVector2 GetCenter()
		{
			return (EdgeStart + EdgeEnd) / 2f;
		}

		public static Edge operator -(Edge e)
		{
			return new Edge(e.EdgeEnd, e.EdgeStart);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return Equals(obj as Edge);
		}

		public bool Equals(Edge e)
		{
			if (e == null)
			{
				return false;
			}
			return VectorEqual(EdgeStart, e.EdgeStart) && VectorEqual(EdgeEnd, e.EdgeEnd);
		}

		public override int GetHashCode()
		{
			return EdgeStart.GetHashCode() ^ EdgeEnd.GetHashCode();
		}
	}

	private static readonly FP ClipperEpsilonSquared = 1.1920929E-07f;

	public static List<Vertices> Union(Vertices polygon1, Vertices polygon2, out PolyClipError error)
	{
		return Execute(polygon1, polygon2, PolyClipType.Union, out error);
	}

	public static List<Vertices> Difference(Vertices polygon1, Vertices polygon2, out PolyClipError error)
	{
		return Execute(polygon1, polygon2, PolyClipType.Difference, out error);
	}

	public static List<Vertices> Intersect(Vertices polygon1, Vertices polygon2, out PolyClipError error)
	{
		return Execute(polygon1, polygon2, PolyClipType.Intersect, out error);
	}

	private static List<Vertices> Execute(Vertices subject, Vertices clip, PolyClipType clipType, out PolyClipError error)
	{
		CalculateIntersections(subject, clip, out var slicedPoly, out var slicedPoly2);
		TSVector2 value = subject.GetAABB().LowerBound;
		TSVector2 value2 = clip.GetAABB().LowerBound;
		TSVector2.Min(ref value, ref value2, out var result);
		result = TSVector2.one - result;
		if (result != TSVector2.zero)
		{
			slicedPoly.Translate(ref result);
			slicedPoly2.Translate(ref result);
		}
		slicedPoly.ForceCounterClockWise();
		slicedPoly2.ForceCounterClockWise();
		CalculateSimplicalChain(slicedPoly, out var coeff, out var simplicies);
		CalculateSimplicalChain(slicedPoly2, out var coeff2, out var simplicies2);
		CalculateResultChain(coeff, simplicies, coeff2, simplicies2, clipType, out var resultSimplices);
		error = BuildPolygonsFromChain(resultSimplices, out var result2);
		result *= (FP)(-1f);
		for (int i = 0; i < result2.Count; i++)
		{
			result2[i].Translate(ref result);
			SimplifyTools.CollinearSimplify(result2[i], FP.Zero);
		}
		return result2;
	}

	private static void CalculateIntersections(Vertices polygon1, Vertices polygon2, out Vertices slicedPoly1, out Vertices slicedPoly2)
	{
		slicedPoly1 = new Vertices(polygon1);
		slicedPoly2 = new Vertices(polygon2);
		for (int i = 0; i < polygon1.Count; i++)
		{
			TSVector2 tSVector = polygon1[i];
			TSVector2 tSVector2 = polygon1[polygon1.NextIndex(i)];
			for (int j = 0; j < polygon2.Count; j++)
			{
				TSVector2 tSVector3 = polygon2[j];
				TSVector2 tSVector4 = polygon2[polygon2.NextIndex(j)];
				if (!LineTools.LineIntersect(tSVector, tSVector2, tSVector3, tSVector4, out var intersectionPoint))
				{
					continue;
				}
				FP alpha = GetAlpha(tSVector, tSVector2, intersectionPoint);
				if (alpha > 0f && alpha < 1f)
				{
					int k;
					for (k = slicedPoly1.IndexOf(tSVector) + 1; k < slicedPoly1.Count && GetAlpha(tSVector, tSVector2, slicedPoly1[k]) <= alpha; k++)
					{
					}
					slicedPoly1.Insert(k, intersectionPoint);
				}
				alpha = GetAlpha(tSVector3, tSVector4, intersectionPoint);
				if (alpha > 0f && alpha < 1f)
				{
					int l;
					for (l = slicedPoly2.IndexOf(tSVector3) + 1; l < slicedPoly2.Count && GetAlpha(tSVector3, tSVector4, slicedPoly2[l]) <= alpha; l++)
					{
					}
					slicedPoly2.Insert(l, intersectionPoint);
				}
			}
		}
		for (int m = 0; m < slicedPoly1.Count; m++)
		{
			int index = slicedPoly1.NextIndex(m);
			if ((slicedPoly1[index] - slicedPoly1[m]).LengthSquared() <= ClipperEpsilonSquared)
			{
				slicedPoly1.RemoveAt(m);
				m--;
			}
		}
		for (int n = 0; n < slicedPoly2.Count; n++)
		{
			int index2 = slicedPoly2.NextIndex(n);
			if ((slicedPoly2[index2] - slicedPoly2[n]).LengthSquared() <= ClipperEpsilonSquared)
			{
				slicedPoly2.RemoveAt(n);
				n--;
			}
		}
	}

	private static void CalculateSimplicalChain(Vertices poly, out List<FP> coeff, out List<Edge> simplicies)
	{
		simplicies = new List<Edge>();
		coeff = new List<FP>();
		for (int i = 0; i < poly.Count; i++)
		{
			simplicies.Add(new Edge(poly[i], poly[poly.NextIndex(i)]));
			coeff.Add(CalculateSimplexCoefficient(TSVector2.zero, poly[i], poly[poly.NextIndex(i)]));
		}
	}

	private static void CalculateResultChain(List<FP> poly1Coeff, List<Edge> poly1Simplicies, List<FP> poly2Coeff, List<Edge> poly2Simplicies, PolyClipType clipType, out List<Edge> resultSimplices)
	{
		resultSimplices = new List<Edge>();
		for (int i = 0; i < poly1Simplicies.Count; i++)
		{
			FP fP = 0;
			if (poly2Simplicies.Contains(poly1Simplicies[i]))
			{
				fP = 1f;
			}
			else if (poly2Simplicies.Contains(-poly1Simplicies[i]) && clipType == PolyClipType.Union)
			{
				fP = 1f;
			}
			else
			{
				for (int j = 0; j < poly2Simplicies.Count; j++)
				{
					if (!poly2Simplicies.Contains(-poly1Simplicies[i]))
					{
						fP += CalculateBeta(poly1Simplicies[i].GetCenter(), poly2Simplicies[j], poly2Coeff[j]);
					}
				}
			}
			if (clipType == PolyClipType.Intersect)
			{
				if (fP == 1f)
				{
					resultSimplices.Add(poly1Simplicies[i]);
				}
			}
			else if (fP == 0f)
			{
				resultSimplices.Add(poly1Simplicies[i]);
			}
		}
		for (int k = 0; k < poly2Simplicies.Count; k++)
		{
			FP fP2 = 0f;
			if (resultSimplices.Contains(poly2Simplicies[k]) || resultSimplices.Contains(-poly2Simplicies[k]))
			{
				continue;
			}
			if (poly1Simplicies.Contains(-poly2Simplicies[k]) && clipType == PolyClipType.Union)
			{
				fP2 = 1f;
				continue;
			}
			fP2 = 0f;
			for (int l = 0; l < poly1Simplicies.Count; l++)
			{
				if (!poly1Simplicies.Contains(poly2Simplicies[k]) && !poly1Simplicies.Contains(-poly2Simplicies[k]))
				{
					fP2 += CalculateBeta(poly2Simplicies[k].GetCenter(), poly1Simplicies[l], poly1Coeff[l]);
				}
			}
			if (clipType == PolyClipType.Intersect || clipType == PolyClipType.Difference)
			{
				if (fP2 == 1f)
				{
					resultSimplices.Add(-poly2Simplicies[k]);
				}
			}
			else if (fP2 == 0f)
			{
				resultSimplices.Add(poly2Simplicies[k]);
			}
		}
	}

	private static PolyClipError BuildPolygonsFromChain(List<Edge> simplicies, out List<Vertices> result)
	{
		result = new List<Vertices>();
		PolyClipError result2 = PolyClipError.None;
		while (simplicies.Count > 0)
		{
			Vertices vertices = new Vertices();
			vertices.Add(simplicies[0].EdgeStart);
			vertices.Add(simplicies[0].EdgeEnd);
			simplicies.RemoveAt(0);
			bool flag = false;
			int num = 0;
			int count = simplicies.Count;
			while (!flag && simplicies.Count > 0)
			{
				if (VectorEqual(vertices[vertices.Count - 1], simplicies[num].EdgeStart))
				{
					if (VectorEqual(simplicies[num].EdgeEnd, vertices[0]))
					{
						flag = true;
					}
					else
					{
						vertices.Add(simplicies[num].EdgeEnd);
					}
					simplicies.RemoveAt(num);
					num--;
				}
				else if (VectorEqual(vertices[vertices.Count - 1], simplicies[num].EdgeEnd))
				{
					if (VectorEqual(simplicies[num].EdgeStart, vertices[0]))
					{
						flag = true;
					}
					else
					{
						vertices.Add(simplicies[num].EdgeStart);
					}
					simplicies.RemoveAt(num);
					num--;
				}
				if (!flag && ++num == simplicies.Count)
				{
					if (count == simplicies.Count)
					{
						result = new List<Vertices>();
						return PolyClipError.BrokenResult;
					}
					num = 0;
					count = simplicies.Count;
				}
			}
			if (vertices.Count < 3)
			{
				result2 = PolyClipError.DegeneratedOutput;
			}
			result.Add(vertices);
		}
		return result2;
	}

	private static FP CalculateBeta(TSVector2 point, Edge e, FP coefficient)
	{
		FP result = 0f;
		if (PointInSimplex(point, e))
		{
			result = coefficient;
		}
		if (PointOnLineSegment(TSVector2.zero, e.EdgeStart, point) || PointOnLineSegment(TSVector2.zero, e.EdgeEnd, point))
		{
			result = 0.5f * coefficient;
		}
		return result;
	}

	private static FP GetAlpha(TSVector2 start, TSVector2 end, TSVector2 point)
	{
		return (point - start).LengthSquared() / (end - start).LengthSquared();
	}

	private static FP CalculateSimplexCoefficient(TSVector2 a, TSVector2 b, TSVector2 c)
	{
		FP fP = MathUtils.Area(ref a, ref b, ref c);
		if (fP < 0f)
		{
			return -1f;
		}
		if (fP > 0f)
		{
			return 1f;
		}
		return 0f;
	}

	private static bool PointInSimplex(TSVector2 point, Edge edge)
	{
		Vertices vertices = new Vertices();
		vertices.Add(TSVector2.zero);
		vertices.Add(edge.EdgeStart);
		vertices.Add(edge.EdgeEnd);
		return vertices.PointInPolygon(ref point) == 1;
	}

	private static bool PointOnLineSegment(TSVector2 start, TSVector2 end, TSVector2 point)
	{
		TSVector2 value = end - start;
		return MathUtils.Area(ref start, ref end, ref point) == 0f && TSVector2.Dot(point - start, value) >= 0f && TSVector2.Dot(point - end, value) <= 0f;
	}

	private static bool VectorEqual(TSVector2 vec1, TSVector2 vec2)
	{
		return (vec2 - vec1).LengthSquared() <= ClipperEpsilonSquared;
	}
}
