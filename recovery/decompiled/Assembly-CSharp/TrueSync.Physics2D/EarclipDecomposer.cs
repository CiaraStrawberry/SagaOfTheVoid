using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal static class EarclipDecomposer
{
	private class Triangle : Vertices
	{
		public Triangle(FP x1, FP y1, FP x2, FP y2, FP x3, FP y3)
		{
			FP fP = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
			if (fP > 0)
			{
				Add(new TSVector2(x1, y1));
				Add(new TSVector2(x2, y2));
				Add(new TSVector2(x3, y3));
			}
			else
			{
				Add(new TSVector2(x1, y1));
				Add(new TSVector2(x3, y3));
				Add(new TSVector2(x2, y2));
			}
		}

		public bool IsInside(FP x, FP y)
		{
			TSVector2 tSVector = base[0];
			TSVector2 tSVector2 = base[1];
			TSVector2 tSVector3 = base[2];
			if (x < tSVector.x && x < tSVector2.x && x < tSVector3.x)
			{
				return false;
			}
			if (x > tSVector.x && x > tSVector2.x && x > tSVector3.x)
			{
				return false;
			}
			if (y < tSVector.y && y < tSVector2.y && y < tSVector3.y)
			{
				return false;
			}
			if (y > tSVector.y && y > tSVector2.y && y > tSVector3.y)
			{
				return false;
			}
			FP fP = x - tSVector.x;
			FP fP2 = y - tSVector.y;
			FP fP3 = tSVector2.x - tSVector.x;
			FP fP4 = tSVector2.y - tSVector.y;
			FP fP5 = tSVector3.x - tSVector.x;
			FP fP6 = tSVector3.y - tSVector.y;
			FP fP7 = fP5 * fP5 + fP6 * fP6;
			FP fP8 = fP5 * fP3 + fP6 * fP4;
			FP fP9 = fP5 * fP + fP6 * fP2;
			FP fP10 = fP3 * fP3 + fP4 * fP4;
			FP fP11 = fP3 * fP + fP4 * fP2;
			FP fP12 = 1f / (fP7 * fP10 - fP8 * fP8);
			FP fP13 = (fP10 * fP9 - fP8 * fP11) * fP12;
			FP fP14 = (fP7 * fP11 - fP8 * fP9) * fP12;
			return fP13 > 0 && fP14 > 0 && fP13 + fP14 < 1;
		}
	}

	public static List<Vertices> ConvexPartition(Vertices vertices, FP tolerance)
	{
		return TriangulatePolygon(vertices, tolerance);
	}

	private static List<Vertices> TriangulatePolygon(Vertices vertices, FP tolerance)
	{
		if (vertices.Count < 3)
		{
			return new List<Vertices>();
		}
		List<Vertices> list = new List<Vertices>();
		Vertices pin = new Vertices(vertices);
		if (ResolvePinchPoint(pin, out var poutA, out var poutB, tolerance))
		{
			List<Vertices> list2 = TriangulatePolygon(poutA, tolerance);
			List<Vertices> list3 = TriangulatePolygon(poutB, tolerance);
			if (list2.Count == -1 || list3.Count == -1)
			{
				throw new Exception("Can't triangulate your polygon.");
			}
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(new Vertices(list2[i]));
			}
			for (int j = 0; j < list3.Count; j++)
			{
				list.Add(new Vertices(list3[j]));
			}
			return list;
		}
		Vertices[] array = new Vertices[vertices.Count - 2];
		int num = 0;
		FP[] array2 = new FP[vertices.Count];
		FP[] array3 = new FP[vertices.Count];
		for (int k = 0; k < vertices.Count; k++)
		{
			ref FP reference = ref array2[k];
			reference = vertices[k].x;
			ref FP reference2 = ref array3[k];
			reference2 = vertices[k].y;
		}
		int num2 = vertices.Count;
		while (num2 > 3)
		{
			int num3 = -1;
			FP fP = -10f;
			for (int l = 0; l < num2; l++)
			{
				if (IsEar(l, array2, array3, num2))
				{
					int num4 = Remainder(l - 1, num2);
					int num5 = Remainder(l + 1, num2);
					TSVector2 a = new TSVector2(array2[num5] - array2[l], array3[num5] - array3[l]);
					TSVector2 b = new TSVector2(array2[l] - array2[num4], array3[l] - array3[num4]);
					TSVector2 b2 = new TSVector2(array2[num4] - array2[num5], array3[num4] - array3[num5]);
					a.Normalize();
					b.Normalize();
					b2.Normalize();
					MathUtils.Cross(ref a, ref b, out var c);
					c = FP.Abs(c);
					MathUtils.Cross(ref b, ref b2, out var c2);
					c2 = FP.Abs(c2);
					MathUtils.Cross(ref b2, ref a, out var c3);
					c3 = FP.Abs(c3);
					FP fP2 = TSMath.Min(c, TSMath.Min(c2, c3));
					if (fP2 > fP)
					{
						num3 = l;
						fP = fP2;
					}
				}
			}
			if (num3 == -1)
			{
				for (int m = 0; m < num; m++)
				{
					list.Add(array[m]);
				}
				return list;
			}
			num2--;
			FP[] array4 = new FP[num2];
			FP[] array5 = new FP[num2];
			int num6 = 0;
			for (int n = 0; n < num2; n++)
			{
				if (num6 == num3)
				{
					num6++;
				}
				ref FP reference3 = ref array4[n];
				reference3 = array2[num6];
				ref FP reference4 = ref array5[n];
				reference4 = array3[num6];
				num6++;
			}
			int num7 = ((num3 != 0) ? (num3 - 1) : num2);
			int num8 = ((num3 != num2) ? (num3 + 1) : 0);
			Triangle triangle = new Triangle(array2[num3], array3[num3], array2[num8], array3[num8], array2[num7], array3[num7]);
			array[num] = triangle;
			num++;
			array2 = array4;
			array3 = array5;
		}
		Triangle triangle2 = new Triangle(array2[1], array3[1], array2[2], array3[2], array2[0], array3[0]);
		array[num] = triangle2;
		num++;
		for (int num9 = 0; num9 < num; num9++)
		{
			list.Add(new Vertices(array[num9]));
		}
		return list;
	}

	private static bool ResolvePinchPoint(Vertices pin, out Vertices poutA, out Vertices poutB, FP tolerance)
	{
		poutA = new Vertices();
		poutB = new Vertices();
		if (pin.Count < 3)
		{
			return false;
		}
		bool flag = false;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < pin.Count; i++)
		{
			for (int j = i + 1; j < pin.Count; j++)
			{
				if (FP.Abs(pin[i].x - pin[j].x) < tolerance && FP.Abs(pin[i].y - pin[j].y) < tolerance && j != i + 1)
				{
					num = i;
					num2 = j;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			int num3 = num2 - num;
			if (num3 == pin.Count)
			{
				return false;
			}
			for (int k = 0; k < num3; k++)
			{
				int index = Remainder(num + k, pin.Count);
				poutA.Add(pin[index]);
			}
			int num4 = pin.Count - num3;
			for (int l = 0; l < num4; l++)
			{
				int index2 = Remainder(num2 + l, pin.Count);
				poutB.Add(pin[index2]);
			}
		}
		return flag;
	}

	private static int Remainder(int x, int modulus)
	{
		int i;
		for (i = x % modulus; i < 0; i += modulus)
		{
		}
		return i;
	}

	private static bool IsEar(int i, FP[] xv, FP[] yv, int xvLength)
	{
		if (i >= xvLength || i < 0 || xvLength < 3)
		{
			return false;
		}
		int num = i + 1;
		int num2 = i - 1;
		FP fP;
		FP fP2;
		FP fP3;
		FP fP4;
		if (i == 0)
		{
			fP = xv[0] - xv[xvLength - 1];
			fP2 = yv[0] - yv[xvLength - 1];
			fP3 = xv[1] - xv[0];
			fP4 = yv[1] - yv[0];
			num2 = xvLength - 1;
		}
		else if (i == xvLength - 1)
		{
			fP = xv[i] - xv[i - 1];
			fP2 = yv[i] - yv[i - 1];
			fP3 = xv[0] - xv[i];
			fP4 = yv[0] - yv[i];
			num = 0;
		}
		else
		{
			fP = xv[i] - xv[i - 1];
			fP2 = yv[i] - yv[i - 1];
			fP3 = xv[i + 1] - xv[i];
			fP4 = yv[i + 1] - yv[i];
		}
		FP fP5 = fP * fP4 - fP3 * fP2;
		if (fP5 > 0)
		{
			return false;
		}
		Triangle triangle = new Triangle(xv[i], yv[i], xv[num], yv[num], xv[num2], yv[num2]);
		for (int j = 0; j < xvLength; j++)
		{
			if (j != i && j != num2 && j != num && triangle.IsInside(xv[j], yv[j]))
			{
				return false;
			}
		}
		return true;
	}
}
