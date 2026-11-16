using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class MarchingSquares
{
	internal class CxFastList<T>
	{
		private CxFastListNode<T> _head;

		private int _count;

		public CxFastListNode<T> Begin()
		{
			return _head;
		}

		public CxFastListNode<T> End()
		{
			return null;
		}

		public T Front()
		{
			return _head.Elem();
		}

		public CxFastListNode<T> Add(T value)
		{
			CxFastListNode<T> cxFastListNode = new CxFastListNode<T>(value);
			if (_head == null)
			{
				cxFastListNode._next = null;
				_head = cxFastListNode;
				_count++;
				return cxFastListNode;
			}
			cxFastListNode._next = _head;
			_head = cxFastListNode;
			_count++;
			return cxFastListNode;
		}

		public bool Remove(T value)
		{
			CxFastListNode<T> cxFastListNode = _head;
			CxFastListNode<T> cxFastListNode2 = _head;
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			if (cxFastListNode != null && value != null)
			{
				do
				{
					if (equalityComparer.Equals(cxFastListNode._elt, value))
					{
						if (cxFastListNode == _head)
						{
							_head = cxFastListNode._next;
							_count--;
							return true;
						}
						cxFastListNode2._next = cxFastListNode._next;
						_count--;
						return true;
					}
					cxFastListNode2 = cxFastListNode;
					cxFastListNode = cxFastListNode._next;
				}
				while (cxFastListNode != null);
			}
			return false;
		}

		public CxFastListNode<T> Pop()
		{
			return Erase(null, _head);
		}

		public CxFastListNode<T> Insert(CxFastListNode<T> node, T value)
		{
			if (node == null)
			{
				return Add(value);
			}
			CxFastListNode<T> cxFastListNode = new CxFastListNode<T>(value);
			CxFastListNode<T> next = node._next;
			cxFastListNode._next = next;
			node._next = cxFastListNode;
			_count++;
			return cxFastListNode;
		}

		public CxFastListNode<T> Erase(CxFastListNode<T> prev, CxFastListNode<T> node)
		{
			CxFastListNode<T> next = node._next;
			if (prev != null)
			{
				prev._next = next;
			}
			else
			{
				if (_head == null)
				{
					return null;
				}
				_head = _head._next;
			}
			_count--;
			return next;
		}

		public bool Empty()
		{
			if (_head == null)
			{
				return true;
			}
			return false;
		}

		public int Size()
		{
			CxFastListNode<T> cxFastListNode = Begin();
			int num = 0;
			do
			{
				num++;
			}
			while (cxFastListNode.Next() != null);
			return num;
		}

		public void Clear()
		{
			CxFastListNode<T> cxFastListNode = _head;
			while (cxFastListNode != null)
			{
				CxFastListNode<T> cxFastListNode2 = cxFastListNode;
				cxFastListNode = cxFastListNode._next;
				cxFastListNode2._next = null;
			}
			_head = null;
			_count = 0;
		}

		public bool Has(T value)
		{
			return Find(value) != null;
		}

		public CxFastListNode<T> Find(T value)
		{
			CxFastListNode<T> cxFastListNode = _head;
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			if (cxFastListNode != null)
			{
				if (value != null)
				{
					do
					{
						if (equalityComparer.Equals(cxFastListNode._elt, value))
						{
							return cxFastListNode;
						}
						cxFastListNode = cxFastListNode._next;
					}
					while (cxFastListNode != _head);
				}
				else
				{
					do
					{
						if (cxFastListNode._elt == null)
						{
							return cxFastListNode;
						}
						cxFastListNode = cxFastListNode._next;
					}
					while (cxFastListNode != _head);
				}
			}
			return null;
		}

		public List<T> GetListOfElements()
		{
			List<T> list = new List<T>();
			CxFastListNode<T> cxFastListNode = Begin();
			if (cxFastListNode != null)
			{
				do
				{
					list.Add(cxFastListNode._elt);
					cxFastListNode = cxFastListNode._next;
				}
				while (cxFastListNode != null);
			}
			return list;
		}
	}

	internal class CxFastListNode<T>
	{
		internal T _elt;

		internal CxFastListNode<T> _next;

		public CxFastListNode(T obj)
		{
			_elt = obj;
		}

		public T Elem()
		{
			return _elt;
		}

		public CxFastListNode<T> Next()
		{
			return _next;
		}
	}

	internal class GeomPoly
	{
		public int Length;

		public CxFastList<TSVector2> Points;

		public GeomPoly()
		{
			Points = new CxFastList<TSVector2>();
			Length = 0;
		}
	}

	private class GeomPolyVal
	{
		public int Key;

		public GeomPoly GeomP;

		public GeomPolyVal(GeomPoly geomP, int K)
		{
			GeomP = geomP;
			Key = K;
		}
	}

	private static int[] _lookMarch = new int[16]
	{
		0, 224, 56, 216, 14, 238, 54, 214, 131, 99,
		187, 91, 141, 109, 181, 85
	};

	public static List<Vertices> DetectSquares(AABB domain, FP cellWidth, FP cellHeight, sbyte[,] f, int lerpCount, bool combine)
	{
		CxFastList<GeomPoly> cxFastList = new CxFastList<GeomPoly>();
		List<Vertices> list = new List<Vertices>();
		int num = (int)(long)(domain.Extents.x * 2 / cellWidth);
		bool flag = num == domain.Extents.x * 2 / cellWidth;
		int num2 = (int)(long)(domain.Extents.y * 2 / cellHeight);
		bool flag2 = num2 == domain.Extents.y * 2 / cellHeight;
		if (!flag)
		{
			num++;
		}
		if (!flag2)
		{
			num2++;
		}
		sbyte[,] array = new sbyte[num + 1, num2 + 1];
		GeomPolyVal[,] array2 = new GeomPolyVal[num + 1, num2 + 1];
		for (int i = 0; i < num + 1; i++)
		{
			int num3 = (int)((i != num) ? ((long)(i * cellWidth + domain.LowerBound.x)) : ((long)domain.UpperBound.x));
			for (int j = 0; j < num2 + 1; j++)
			{
				int num4 = (int)((j != num2) ? ((long)(j * cellHeight + domain.LowerBound.y)) : ((long)domain.UpperBound.y));
				array[i, j] = f[num3, num4];
			}
		}
		for (int k = 0; k < num2; k++)
		{
			FP fP = k * cellHeight + domain.LowerBound.y;
			FP y = ((k != num2 - 1) ? (fP + cellHeight) : domain.UpperBound.y);
			GeomPoly polya = null;
			for (int l = 0; l < num; l++)
			{
				FP fP2 = l * cellWidth + domain.LowerBound.x;
				FP x = ((l != num - 1) ? (fP2 + cellWidth) : domain.UpperBound.x);
				GeomPoly poly = new GeomPoly();
				int num5 = MarchSquare(f, array, ref poly, l, k, fP2, fP, x, y, lerpCount);
				if (poly.Length != 0)
				{
					if (combine && polya != null && (num5 & 9) != 0)
					{
						combLeft(ref polya, ref poly);
						poly = polya;
					}
					else
					{
						cxFastList.Add(poly);
					}
					array2[l, k] = new GeomPolyVal(poly, num5);
				}
				else
				{
					poly = null;
				}
				polya = poly;
			}
		}
		List<GeomPoly> listOfElements;
		if (!combine)
		{
			listOfElements = cxFastList.GetListOfElements();
			{
				foreach (GeomPoly item in listOfElements)
				{
					list.Add(new Vertices(item.Points.GetListOfElements()));
				}
				return list;
			}
		}
		for (int m = 1; m < num2; m++)
		{
			int num6 = 0;
			while (num6 < num)
			{
				GeomPolyVal geomPolyVal = array2[num6, m];
				if (geomPolyVal == null)
				{
					num6++;
					continue;
				}
				if ((geomPolyVal.Key & 0xC) == 0)
				{
					num6++;
					continue;
				}
				GeomPolyVal geomPolyVal2 = array2[num6, m - 1];
				if (geomPolyVal2 == null)
				{
					num6++;
					continue;
				}
				if ((geomPolyVal2.Key & 3) == 0)
				{
					num6++;
					continue;
				}
				FP fP3 = num6 * cellWidth + domain.LowerBound.x;
				FP fP4 = m * cellHeight + domain.LowerBound.y;
				CxFastList<TSVector2> points = geomPolyVal.GeomP.Points;
				CxFastList<TSVector2> points2 = geomPolyVal2.GeomP.Points;
				if (geomPolyVal2.GeomP == geomPolyVal.GeomP)
				{
					num6++;
					continue;
				}
				CxFastListNode<TSVector2> cxFastListNode = points.Begin();
				while (Square(cxFastListNode.Elem().y - fP4) > Settings.Epsilon || cxFastListNode.Elem().x < fP3)
				{
					cxFastListNode = cxFastListNode.Next();
				}
				TSVector2 b = cxFastListNode.Next().Elem();
				if (Square(b.y - fP4) > Settings.Epsilon)
				{
					num6++;
					continue;
				}
				bool flag3 = true;
				CxFastListNode<TSVector2> cxFastListNode2;
				for (cxFastListNode2 = points2.Begin(); cxFastListNode2 != points2.End(); cxFastListNode2 = cxFastListNode2.Next())
				{
					if (VecDsq(cxFastListNode2.Elem(), b) < Settings.Epsilon)
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					num6++;
					continue;
				}
				CxFastListNode<TSVector2> cxFastListNode3 = cxFastListNode.Next().Next();
				if (cxFastListNode3 == points.End())
				{
					cxFastListNode3 = points.Begin();
				}
				while (cxFastListNode3 != cxFastListNode)
				{
					cxFastListNode2 = points2.Insert(cxFastListNode2, cxFastListNode3.Elem());
					cxFastListNode3 = cxFastListNode3.Next();
					if (cxFastListNode3 == points.End())
					{
						cxFastListNode3 = points.Begin();
					}
					geomPolyVal2.GeomP.Length++;
				}
				fP3 = num6 + 1;
				while (fP3 < num)
				{
					GeomPolyVal geomPolyVal3 = array2[(int)(long)fP3, m];
					if (geomPolyVal3 == null || geomPolyVal3.GeomP != geomPolyVal.GeomP)
					{
						fP3 += (FP)1;
						continue;
					}
					geomPolyVal3.GeomP = geomPolyVal2.GeomP;
					fP3 += (FP)1;
				}
				fP3 = num6 - 1;
				while (fP3 >= 0)
				{
					GeomPolyVal geomPolyVal4 = array2[(int)(long)fP3, m];
					if (geomPolyVal4 == null || geomPolyVal4.GeomP != geomPolyVal.GeomP)
					{
						fP3 -= (FP)1;
						continue;
					}
					geomPolyVal4.GeomP = geomPolyVal2.GeomP;
					fP3 -= (FP)1;
				}
				cxFastList.Remove(geomPolyVal.GeomP);
				geomPolyVal.GeomP = geomPolyVal2.GeomP;
				num6 = (int)(long)((cxFastListNode.Next().Elem().x - domain.LowerBound.x) / cellWidth) + 1;
			}
		}
		listOfElements = cxFastList.GetListOfElements();
		foreach (GeomPoly item2 in listOfElements)
		{
			list.Add(new Vertices(item2.Points.GetListOfElements()));
		}
		return list;
	}

	private static FP Lerp(FP x0, FP x1, FP v0, FP v1)
	{
		FP fP = v0 - v1;
		FP fP2 = ((!(fP * fP < Settings.Epsilon)) ? (v0 / fP) : ((FP)0.5f));
		return x0 + fP2 * (x1 - x0);
	}

	private static FP Xlerp(FP x0, FP x1, FP y, FP v0, FP v1, sbyte[,] f, int c)
	{
		FP fP = Lerp(x0, x1, v0, v1);
		if (c == 0)
		{
			return fP;
		}
		sbyte b = f[(int)(long)fP, (int)(long)y];
		if (v0 * b < 0)
		{
			return Xlerp(x0, fP, y, v0, b, f, c - 1);
		}
		return Xlerp(fP, x1, y, b, v1, f, c - 1);
	}

	private static FP Ylerp(FP y0, FP y1, FP x, FP v0, FP v1, sbyte[,] f, int c)
	{
		FP fP = Lerp(y0, y1, v0, v1);
		if (c == 0)
		{
			return fP;
		}
		sbyte b = f[(int)(long)x, (int)(long)fP];
		if (v0 * b < 0)
		{
			return Ylerp(y0, fP, x, v0, b, f, c - 1);
		}
		return Ylerp(fP, y1, x, b, v1, f, c - 1);
	}

	private static FP Square(FP x)
	{
		return x * x;
	}

	private static FP VecDsq(TSVector2 a, TSVector2 b)
	{
		TSVector2 tSVector = a - b;
		return tSVector.x * tSVector.x + tSVector.y * tSVector.y;
	}

	private static FP VecCross(TSVector2 a, TSVector2 b)
	{
		return a.x * b.y - a.y * b.x;
	}

	private static int MarchSquare(sbyte[,] f, sbyte[,] fs, ref GeomPoly poly, int ax, int ay, FP x0, FP y0, FP x1, FP y1, int bin)
	{
		int num = 0;
		sbyte b = fs[ax, ay];
		if (b < 0)
		{
			num |= 8;
		}
		sbyte b2 = fs[ax + 1, ay];
		if (b2 < 0)
		{
			num |= 4;
		}
		sbyte b3 = fs[ax + 1, ay + 1];
		if (b3 < 0)
		{
			num |= 2;
		}
		sbyte b4 = fs[ax, ay + 1];
		if (b4 < 0)
		{
			num |= 1;
		}
		int num2 = _lookMarch[num];
		if (num2 != 0)
		{
			CxFastListNode<TSVector2> node = null;
			for (int i = 0; i < 8; i++)
			{
				if ((num2 & (1 << i)) != 0)
				{
					if (i == 7 && (num2 & 1) == 0)
					{
						CxFastList<TSVector2> points = poly.Points;
						TSVector2 value = new TSVector2(x0, Ylerp(y0, y1, x0, b, b4, f, bin));
						points.Add(value);
					}
					else
					{
						TSVector2 value = i switch
						{
							0 => new TSVector2(x0, y0), 
							2 => new TSVector2(x1, y0), 
							4 => new TSVector2(x1, y1), 
							6 => new TSVector2(x0, y1), 
							1 => new TSVector2(Xlerp(x0, x1, y0, b, b2, f, bin), y0), 
							5 => new TSVector2(Xlerp(x0, x1, y1, b4, b3, f, bin), y1), 
							3 => new TSVector2(x1, Ylerp(y0, y1, x1, b2, b3, f, bin)), 
							_ => new TSVector2(x0, Ylerp(y0, y1, x0, b, b4, f, bin)), 
						};
						node = poly.Points.Insert(node, value);
					}
					poly.Length++;
				}
			}
		}
		return num;
	}

	private static void combLeft(ref GeomPoly polya, ref GeomPoly polyb)
	{
		CxFastList<TSVector2> points = polya.Points;
		CxFastList<TSVector2> points2 = polyb.Points;
		CxFastListNode<TSVector2> cxFastListNode = points.Begin();
		CxFastListNode<TSVector2> cxFastListNode2 = points2.Begin();
		TSVector2 b = cxFastListNode2.Elem();
		CxFastListNode<TSVector2> cxFastListNode3 = null;
		while (cxFastListNode != points.End())
		{
			TSVector2 tSVector = cxFastListNode.Elem();
			if (VecDsq(tSVector, b) < Settings.Epsilon)
			{
				if (cxFastListNode3 != null)
				{
					TSVector2 tSVector2 = cxFastListNode3.Elem();
					b = cxFastListNode2.Next().Elem();
					TSVector2 a = tSVector - tSVector2;
					TSVector2 b2 = b - tSVector;
					FP fP = VecCross(a, b2);
					if (fP * fP < Settings.Epsilon)
					{
						points.Erase(cxFastListNode3, cxFastListNode);
						polya.Length--;
						cxFastListNode = cxFastListNode3;
					}
				}
				bool flag = true;
				CxFastListNode<TSVector2> cxFastListNode4 = null;
				while (!points2.Empty())
				{
					TSVector2 value = points2.Front();
					points2.Pop();
					if (!flag && !points2.Empty())
					{
						cxFastListNode = points.Insert(cxFastListNode, value);
						polya.Length++;
						cxFastListNode4 = cxFastListNode;
					}
					flag = false;
				}
				cxFastListNode = cxFastListNode.Next();
				TSVector2 tSVector3 = cxFastListNode.Elem();
				cxFastListNode = cxFastListNode.Next();
				if (cxFastListNode == points.End())
				{
					cxFastListNode = points.Begin();
				}
				TSVector2 tSVector4 = cxFastListNode.Elem();
				TSVector2 tSVector5 = cxFastListNode4.Elem();
				TSVector2 a2 = tSVector3 - tSVector5;
				TSVector2 b3 = tSVector4 - tSVector3;
				FP fP2 = VecCross(a2, b3);
				if (fP2 * fP2 < Settings.Epsilon)
				{
					points.Erase(cxFastListNode4, cxFastListNode4.Next());
					polya.Length--;
				}
				break;
			}
			cxFastListNode3 = cxFastListNode;
			cxFastListNode = cxFastListNode.Next();
		}
	}
}
