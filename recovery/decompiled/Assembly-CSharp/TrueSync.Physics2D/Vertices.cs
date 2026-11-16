using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TrueSync.Physics2D;

[DebuggerDisplay("Count = {Count} Vertices = {ToString()}")]
public class Vertices : List<TSVector2>
{
	private struct Vertex(TSVector2 position, short index)
	{
		public readonly TSVector2 Position = position;

		public readonly short Index = index;

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Vertex))
			{
				return false;
			}
			return Equals((Vertex)obj);
		}

		public bool Equals(Vertex obj)
		{
			return obj.Position.Equals(Position) && obj.Index == Index;
		}

		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Index;
		}

		public override string ToString()
		{
			return $"{Position} ({Index})";
		}
	}

	private struct LineSegment(Vertex a, Vertex b)
	{
		public Vertex A = a;

		public Vertex B = b;

		public FP? IntersectsWithRay(TSVector2 origin, TSVector2 direction)
		{
			TSVector2 position = A.Position;
			FP val = position.x - origin.x;
			TSVector2 position2 = B.Position;
			FP fP = TSMath.Max(val, position2.x - origin.x) * 2f;
			TSVector2? tSVector = FindIntersection(b: new LineSegment(new Vertex(origin, 0), new Vertex(origin + direction * fP, 0)), a: this);
			FP? result = null;
			if (tSVector.HasValue)
			{
				result = TSVector2.Distance(origin, tSVector.Value);
			}
			return result;
		}

		public static TSVector2? FindIntersection(LineSegment a, LineSegment b)
		{
			TSVector2 position = a.A.Position;
			FP x = position.x;
			TSVector2 position2 = a.A.Position;
			FP y = position2.y;
			TSVector2 position3 = a.B.Position;
			FP x2 = position3.x;
			TSVector2 position4 = a.B.Position;
			FP y2 = position4.y;
			TSVector2 position5 = b.A.Position;
			FP x3 = position5.x;
			TSVector2 position6 = b.A.Position;
			FP y3 = position6.y;
			TSVector2 position7 = b.B.Position;
			FP x4 = position7.x;
			TSVector2 position8 = b.B.Position;
			FP y4 = position8.y;
			FP fP = (y4 - y3) * (x2 - x) - (x4 - x3) * (y2 - y);
			FP fP2 = (x4 - x3) * (y - y3) - (y4 - y3) * (x - x3);
			FP fP3 = (x2 - x) * (y - y3) - (y2 - y) * (x - x3);
			FP fP4 = fP2 / fP;
			FP fP5 = fP3 / fP;
			if (TSMath.Clamp(fP4, 0f, 1f) != fP4 || TSMath.Clamp(fP5, 0f, 1f) != fP5)
			{
				return null;
			}
			return a.A.Position + (a.B.Position - a.A.Position) * fP4;
		}
	}

	private struct Triangle(Vertex a, Vertex b, Vertex c)
	{
		public readonly Vertex A = a;

		public readonly Vertex B = b;

		public readonly Vertex C = c;

		public bool ContainsPoint(Vertex point)
		{
			if (point.Equals(A) || point.Equals(B) || point.Equals(C))
			{
				return true;
			}
			bool flag = false;
			if (checkPointToSegment(C, A, point))
			{
				flag = !flag;
			}
			if (checkPointToSegment(A, B, point))
			{
				flag = !flag;
			}
			if (checkPointToSegment(B, C, point))
			{
				flag = !flag;
			}
			return flag;
		}

		public static bool ContainsPoint(Vertex a, Vertex b, Vertex c, Vertex point)
		{
			return new Triangle(a, b, c).ContainsPoint(point);
		}

		private static bool checkPointToSegment(Vertex sA, Vertex sB, Vertex point)
		{
			TSVector2 position = sA.Position;
			FP y = position.y;
			TSVector2 position2 = point.Position;
			if (y < position2.y)
			{
				TSVector2 position3 = sB.Position;
				FP y2 = position3.y;
				TSVector2 position4 = point.Position;
				if (y2 >= position4.y)
				{
					goto IL_00a4;
				}
			}
			TSVector2 position5 = sB.Position;
			FP y3 = position5.y;
			TSVector2 position6 = point.Position;
			if (y3 < position6.y)
			{
				TSVector2 position7 = sA.Position;
				FP y4 = position7.y;
				TSVector2 position8 = point.Position;
				if (y4 >= position8.y)
				{
					goto IL_00a4;
				}
			}
			goto IL_0152;
			IL_00a4:
			TSVector2 position9 = sA.Position;
			FP x = position9.x;
			TSVector2 position10 = point.Position;
			FP y5 = position10.y;
			TSVector2 position11 = sA.Position;
			FP fP = y5 - position11.y;
			TSVector2 position12 = sB.Position;
			FP y6 = position12.y;
			TSVector2 position13 = sA.Position;
			FP fP2 = fP / (y6 - position13.y);
			TSVector2 position14 = sB.Position;
			FP x2 = position14.x;
			TSVector2 position15 = sA.Position;
			FP fP3 = x + fP2 * (x2 - position15.x);
			TSVector2 position16 = point.Position;
			if (fP3 < position16.x)
			{
				return true;
			}
			goto IL_0152;
			IL_0152:
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Triangle))
			{
				return false;
			}
			return Equals((Triangle)obj);
		}

		public bool Equals(Triangle obj)
		{
			return obj.A.Equals(A) && obj.B.Equals(B) && obj.C.Equals(C);
		}

		public override int GetHashCode()
		{
			int hashCode = A.GetHashCode();
			hashCode = (hashCode * 397) ^ B.GetHashCode();
			return (hashCode * 397) ^ C.GetHashCode();
		}
	}

	private class CyclicalList<T> : List<T>
	{
		public new T this[int index]
		{
			get
			{
				while (index < 0)
				{
					index = Count + index;
				}
				if (index >= Count)
				{
					index %= Count;
				}
				return base[index];
			}
		}
	}

	private class IndexableCyclicalLinkedList<T> : LinkedList<T>
	{
		public LinkedListNode<T> this[int index]
		{
			get
			{
				while (index < 0)
				{
					index = Count + index;
				}
				if (index >= Count)
				{
					index %= Count;
				}
				LinkedListNode<T> next = First;
				for (int i = 0; i < index; i++)
				{
					next = next.Next;
				}
				return next;
			}
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].Value.Equals(item))
				{
					return i;
				}
			}
			return -1;
		}
	}

	public enum WindingOrder
	{
		Clockwise,
		CounterClockwise
	}

	private static readonly IndexableCyclicalLinkedList<Vertex> polygonVertices = new IndexableCyclicalLinkedList<Vertex>();

	private static readonly IndexableCyclicalLinkedList<Vertex> earVertices = new IndexableCyclicalLinkedList<Vertex>();

	private static readonly CyclicalList<Vertex> convexVertices = new CyclicalList<Vertex>();

	private static readonly CyclicalList<Vertex> reflexVertices = new CyclicalList<Vertex>();

	internal bool AttachedToBody { get; set; }

	public List<Vertices> Holes { get; set; }

	public Vertices()
	{
	}

	public Vertices(int capacity)
		: base(capacity)
	{
	}

	public Vertices(IEnumerable<TSVector2> vertices)
	{
		AddRange(vertices);
	}

	public int NextIndex(int index)
	{
		return (index + 1 <= Count - 1) ? (index + 1) : 0;
	}

	public TSVector2 NextVertex(int index)
	{
		return base[NextIndex(index)];
	}

	public int PreviousIndex(int index)
	{
		return (index - 1 >= 0) ? (index - 1) : (Count - 1);
	}

	public TSVector2 PreviousVertex(int index)
	{
		return base[PreviousIndex(index)];
	}

	public FP GetSignedArea()
	{
		if (Count < 3)
		{
			return 0;
		}
		FP fP = 0;
		for (int i = 0; i < Count; i++)
		{
			int index = (i + 1) % Count;
			TSVector2 tSVector = base[i];
			TSVector2 tSVector2 = base[index];
			fP += tSVector.x * tSVector2.y;
			fP -= tSVector.y * tSVector2.x;
		}
		return fP / 2f;
	}

	public FP GetArea()
	{
		FP signedArea = GetSignedArea();
		return (!(signedArea < 0)) ? signedArea : (-signedArea);
	}

	public TSVector2 GetCentroid()
	{
		if (Count < 3)
		{
			return new TSVector2(FP.NaN, FP.NaN);
		}
		TSVector2 zero = TSVector2.zero;
		FP fP = 0f;
		FP fP2 = 1f / 3f;
		for (int i = 0; i < Count; i++)
		{
			TSVector2 tSVector = base[i];
			TSVector2 tSVector2 = ((i + 1 >= Count) ? base[0] : base[i + 1]);
			FP fP3 = 0.5f * (tSVector.x * tSVector2.y - tSVector.y * tSVector2.x);
			fP += fP3;
			zero += fP3 * fP2 * (tSVector + tSVector2);
		}
		return zero * (1f / fP);
	}

	public AABB GetAABB()
	{
		TSVector2 lowerBound = new TSVector2(FP.MaxValue, FP.MaxValue);
		TSVector2 upperBound = new TSVector2(FP.MinValue, FP.MinValue);
		for (int i = 0; i < Count; i++)
		{
			if (base[i].x < lowerBound.x)
			{
				lowerBound.x = base[i].x;
			}
			if (base[i].x > upperBound.x)
			{
				upperBound.x = base[i].x;
			}
			if (base[i].y < lowerBound.y)
			{
				lowerBound.y = base[i].y;
			}
			if (base[i].y > upperBound.y)
			{
				upperBound.y = base[i].y;
			}
		}
		AABB result = default;
		result.LowerBound = lowerBound;
		result.UpperBound = upperBound;
		return result;
	}

	public void Translate(TSVector2 value)
	{
		Translate(ref value);
	}

	public void Translate(ref TSVector2 value)
	{
		for (int i = 0; i < Count; i++)
		{
			base[i] = TSVector2.Add(base[i], value);
		}
		if (Holes == null || Holes.Count <= 0)
		{
			return;
		}
		foreach (Vertices hole in Holes)
		{
			hole.Translate(ref value);
		}
	}

	public void Scale(TSVector2 value)
	{
		Scale(ref value);
	}

	public void Scale(ref TSVector2 value)
	{
		for (int i = 0; i < Count; i++)
		{
			base[i] = TSVector2.Multiply(base[i], value);
		}
		if (Holes == null || Holes.Count <= 0)
		{
			return;
		}
		foreach (Vertices hole in Holes)
		{
			hole.Scale(ref value);
		}
	}

	public void Rotate(FP value)
	{
		FP fP = FP.Cos(value);
		FP fP2 = FP.Sin(value);
		for (int i = 0; i < Count; i++)
		{
			TSVector2 tSVector = base[i];
			base[i] = new TSVector2(tSVector.x * fP + tSVector.y * -fP2, tSVector.x * fP2 + tSVector.y * fP);
		}
		if (Holes == null || Holes.Count <= 0)
		{
			return;
		}
		foreach (Vertices hole in Holes)
		{
			hole.Rotate(value);
		}
	}

	public bool IsConvex()
	{
		if (Count < 3)
		{
			return false;
		}
		if (Count == 3)
		{
			return true;
		}
		for (int i = 0; i < Count; i++)
		{
			int num = ((i + 1 < Count) ? (i + 1) : 0);
			TSVector2 tSVector = base[num] - base[i];
			for (int j = 0; j < Count; j++)
			{
				if (j != i && j != num)
				{
					TSVector2 tSVector2 = base[j] - base[i];
					FP fP = tSVector.x * tSVector2.y - tSVector.y * tSVector2.x;
					if (fP <= 0f)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool IsCounterClockWise()
	{
		if (Count < 3)
		{
			return false;
		}
		return GetSignedArea() > 0f;
	}

	public void ForceCounterClockWise()
	{
		if (Count >= 3 && !IsCounterClockWise())
		{
			Reverse();
		}
	}

	public bool IsSimple()
	{
		if (Count < 3)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			TSVector2 a = base[i];
			TSVector2 a2 = NextVertex(i);
			for (int j = i + 1; j < Count; j++)
			{
				TSVector2 b = base[j];
				TSVector2 b2 = NextVertex(j);
				if (LineTools.LineIntersect2(ref a, ref a2, ref b, ref b2, out var _))
				{
					return false;
				}
			}
		}
		return true;
	}

	public PolygonError CheckPolygon()
	{
		if (Count < 3 || Count > Settings.MaxPolygonVertices)
		{
			return PolygonError.InvalidAmountOfVertices;
		}
		if (!IsSimple())
		{
			return PolygonError.NotSimple;
		}
		if (GetArea() <= Settings.Epsilon)
		{
			return PolygonError.AreaTooSmall;
		}
		if (!IsConvex())
		{
			return PolygonError.NotConvex;
		}
		for (int i = 0; i < Count; i++)
		{
			int index = ((i + 1 < Count) ? (i + 1) : 0);
			if ((base[index] - base[i]).LengthSquared() <= Settings.EpsilonSqr)
			{
				return PolygonError.SideTooSmall;
			}
		}
		if (!IsCounterClockWise())
		{
			return PolygonError.NotCounterClockWise;
		}
		return PolygonError.NoError;
	}

	public void ProjectToAxis(ref TSVector2 axis, out FP min, out FP max)
	{
		max = (min = TSVector2.Dot(axis, base[0]));
		for (int i = 0; i < Count; i++)
		{
			FP fP = TSVector2.Dot(base[i], axis);
			if (fP < min)
			{
				min = fP;
			}
			else if (fP > max)
			{
				max = fP;
			}
		}
	}

	public int PointInPolygon(ref TSVector2 point)
	{
		int num = 0;
		for (int i = 0; i < Count; i++)
		{
			TSVector2 a = base[i];
			TSVector2 b = base[NextIndex(i)];
			TSVector2 value = b - a;
			FP fP = MathUtils.Area(ref a, ref b, ref point);
			if (fP == 0f && TSVector2.Dot(point - a, value) >= 0f && TSVector2.Dot(point - b, value) <= 0f)
			{
				return 0;
			}
			if (a.y <= point.y)
			{
				if (b.y > point.y && fP > 0f)
				{
					num++;
				}
			}
			else if (b.y <= point.y && fP < 0f)
			{
				num--;
			}
		}
		return (num != 0) ? 1 : (-1);
	}

	public bool PointInPolygonAngle(ref TSVector2 point)
	{
		FP value = 0;
		for (int i = 0; i < Count; i++)
		{
			TSVector2 p = base[i] - point;
			TSVector2 p2 = base[NextIndex(i)] - point;
			value += MathUtils.VectorAngle(ref p, ref p2);
		}
		if (FP.Abs(value) < FP.Pi)
		{
			return false;
		}
		return true;
	}

	public static void Triangulate(TSVector2[] inputVertices, WindingOrder desiredWindingOrder, out TSVector2[] outputVertices, out short[] indices)
	{
		List<Triangle> list = new List<Triangle>();
		if (DetermineWindingOrder(inputVertices) == WindingOrder.Clockwise)
		{
			outputVertices = ReverseWindingOrder(inputVertices);
		}
		else
		{
			outputVertices = (TSVector2[])inputVertices.Clone();
		}
		polygonVertices.Clear();
		earVertices.Clear();
		convexVertices.Clear();
		reflexVertices.Clear();
		for (int i = 0; i < outputVertices.Length; i++)
		{
			polygonVertices.AddLast(new Vertex(outputVertices[i], (short)i));
		}
		FindConvexAndReflexVertices();
		FindEarVertices();
		while (polygonVertices.Count > 3 && earVertices.Count > 0)
		{
			ClipNextEar(list);
		}
		if (polygonVertices.Count == 3)
		{
			list.Add(new Triangle(polygonVertices[0].Value, polygonVertices[1].Value, polygonVertices[2].Value));
		}
		indices = new short[list.Count * 3];
		if (desiredWindingOrder == WindingOrder.CounterClockwise)
		{
			for (int j = 0; j < list.Count; j++)
			{
				short[] obj = indices;
				int num = j * 3;
				Vertex a = list[j].A;
				obj[num] = a.Index;
				short[] obj2 = indices;
				int num2 = j * 3 + 1;
				Vertex b = list[j].B;
				obj2[num2] = b.Index;
				short[] obj3 = indices;
				int num3 = j * 3 + 2;
				Vertex c = list[j].C;
				obj3[num3] = c.Index;
			}
		}
		else
		{
			for (int k = 0; k < list.Count; k++)
			{
				short[] obj4 = indices;
				int num4 = k * 3;
				Vertex c2 = list[k].C;
				obj4[num4] = c2.Index;
				short[] obj5 = indices;
				int num5 = k * 3 + 1;
				Vertex b2 = list[k].B;
				obj5[num5] = b2.Index;
				short[] obj6 = indices;
				int num6 = k * 3 + 2;
				Vertex a2 = list[k].A;
				obj6[num6] = a2.Index;
			}
		}
	}

	public static TSVector2[] CutHoleInShape(TSVector2[] shapeVerts, TSVector2[] holeVerts)
	{
		Log("\nCutting hole into shape...");
		shapeVerts = EnsureWindingOrder(shapeVerts, WindingOrder.CounterClockwise);
		holeVerts = EnsureWindingOrder(holeVerts, WindingOrder.Clockwise);
		polygonVertices.Clear();
		earVertices.Clear();
		convexVertices.Clear();
		reflexVertices.Clear();
		for (int i = 0; i < shapeVerts.Length; i++)
		{
			polygonVertices.AddLast(new Vertex(shapeVerts[i], (short)i));
		}
		CyclicalList<Vertex> cyclicalList = new CyclicalList<Vertex>();
		for (int j = 0; j < holeVerts.Length; j++)
		{
			cyclicalList.Add(new Vertex(holeVerts[j], (short)(j + polygonVertices.Count)));
		}
		FindConvexAndReflexVertices();
		FindEarVertices();
		Vertex vertex = cyclicalList[0];
		foreach (Vertex item in cyclicalList)
		{
			TSVector2 position = item.Position;
			FP x = position.x;
			TSVector2 position2 = vertex.Position;
			if (x > position2.x)
			{
				vertex = item;
			}
		}
		List<LineSegment> list = new List<LineSegment>();
		for (int k = 0; k < polygonVertices.Count; k++)
		{
			Vertex value = polygonVertices[k].Value;
			Vertex value2 = polygonVertices[k + 1].Value;
			TSVector2 position3 = value.Position;
			FP x2 = position3.x;
			TSVector2 position4 = vertex.Position;
			if (!(x2 > position4.x))
			{
				TSVector2 position5 = value2.Position;
				FP x3 = position5.x;
				TSVector2 position6 = vertex.Position;
				if (!(x3 > position6.x))
				{
					continue;
				}
			}
			TSVector2 position7 = value.Position;
			FP y = position7.y;
			TSVector2 position8 = vertex.Position;
			if (y >= position8.y)
			{
				TSVector2 position9 = value2.Position;
				FP y2 = position9.y;
				TSVector2 position10 = vertex.Position;
				if (y2 <= position10.y)
				{
					goto IL_0261;
				}
			}
			TSVector2 position11 = value.Position;
			FP y3 = position11.y;
			TSVector2 position12 = vertex.Position;
			if (!(y3 <= position12.y))
			{
				continue;
			}
			TSVector2 position13 = value2.Position;
			FP y4 = position13.y;
			TSVector2 position14 = vertex.Position;
			if (!(y4 >= position14.y))
			{
				continue;
			}
			goto IL_0261;
			IL_0261:
			list.Add(new LineSegment(value, value2));
		}
		FP? fP = null;
		LineSegment lineSegment = default;
		foreach (LineSegment item2 in list)
		{
			FP? fP2 = item2.IntersectsWithRay(vertex.Position, TSVector2.right);
			if (fP2.HasValue && (!fP.HasValue || fP.Value > fP2.Value))
			{
				fP = fP2;
				lineSegment = item2;
			}
		}
		if (!fP.HasValue)
		{
			return shapeVerts;
		}
		TSVector2 position15 = vertex.Position + TSVector2.right * fP.Value;
		TSVector2 position16 = lineSegment.A.Position;
		FP x4 = position16.x;
		TSVector2 position17 = lineSegment.B.Position;
		Vertex vertex2 = ((!(x4 > position17.x)) ? lineSegment.B : lineSegment.A);
		Triangle triangle = new Triangle(vertex, new Vertex(position15, 1), vertex2);
		List<Vertex> list2 = new List<Vertex>();
		foreach (Vertex reflexVertex in reflexVertices)
		{
			if (triangle.ContainsPoint(reflexVertex))
			{
				list2.Add(reflexVertex);
			}
		}
		if (list2.Count > 0)
		{
			FP fP3 = -1f;
			foreach (Vertex item3 in list2)
			{
				TSVector2 value3 = TSVector2.Normalize(item3.Position - vertex.Position);
				FP fP4 = TSVector2.Dot(TSVector2.right, value3);
				if (fP4 > fP3)
				{
					fP3 = fP4;
					vertex2 = item3;
				}
			}
		}
		int num = cyclicalList.IndexOf(vertex);
		int index = polygonVertices.IndexOf(vertex2);
		Log("Inserting hole at injection point {0} starting at hole vertex {1}.", vertex2, vertex);
		for (int l = num; l <= num + cyclicalList.Count; l++)
		{
			Log("Inserting vertex {0} after vertex {1}.", cyclicalList[l], polygonVertices[index].Value);
			polygonVertices.AddAfter(polygonVertices[index++], cyclicalList[l]);
		}
		polygonVertices.AddAfter(polygonVertices[index], vertex2);
		TSVector2[] array = new TSVector2[polygonVertices.Count];
		for (int m = 0; m < polygonVertices.Count; m++)
		{
			ref TSVector2 reference = ref array[m];
			reference = polygonVertices[m].Value.Position;
		}
		return array;
	}

	public static TSVector2[] EnsureWindingOrder(TSVector2[] vertices, WindingOrder windingOrder)
	{
		if (DetermineWindingOrder(vertices) != windingOrder)
		{
			return ReverseWindingOrder(vertices);
		}
		return vertices;
	}

	public static TSVector2[] ReverseWindingOrder(TSVector2[] vertices)
	{
		TSVector2[] array = new TSVector2[vertices.Length];
		ref TSVector2 reference = ref array[0];
		reference = vertices[0];
		for (int i = 1; i < array.Length; i++)
		{
			ref TSVector2 reference2 = ref array[i];
			reference2 = vertices[vertices.Length - i];
		}
		return array;
	}

	public static WindingOrder DetermineWindingOrder(TSVector2[] vertices)
	{
		int num = 0;
		int num2 = 0;
		TSVector2 tSVector = vertices[0];
		for (int i = 1; i < vertices.Length; i++)
		{
			TSVector2 tSVector2 = vertices[i];
			TSVector2 tSVector3 = vertices[(i + 1) % vertices.Length];
			TSVector2 tSVector4 = tSVector - tSVector2;
			TSVector2 tSVector5 = tSVector3 - tSVector2;
			if (tSVector4.x * tSVector5.y - tSVector4.y * tSVector5.x >= 0)
			{
				num++;
			}
			else
			{
				num2++;
			}
			tSVector = tSVector2;
		}
		return (num <= num2) ? WindingOrder.CounterClockwise : WindingOrder.Clockwise;
	}

	private static void ClipNextEar(ICollection<Triangle> triangles)
	{
		Vertex value = earVertices[0].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(value) - 1].Value;
		Vertex value3 = polygonVertices[polygonVertices.IndexOf(value) + 1].Value;
		triangles.Add(new Triangle(value, value3, value2));
		earVertices.RemoveAt(0);
		polygonVertices.RemoveAt(polygonVertices.IndexOf(value));
		ValidateAdjacentVertex(value2);
		ValidateAdjacentVertex(value3);
	}

	private static void ValidateAdjacentVertex(Vertex vertex)
	{
		if (reflexVertices.Contains(vertex) && IsConvex(vertex))
		{
			reflexVertices.Remove(vertex);
			convexVertices.Add(vertex);
		}
		if (convexVertices.Contains(vertex))
		{
			bool flag = earVertices.Contains(vertex);
			bool flag2 = IsEar(vertex);
			if (flag && !flag2)
			{
				earVertices.Remove(vertex);
			}
			else if (!flag && flag2)
			{
				earVertices.AddFirst(vertex);
			}
		}
	}

	private static void FindConvexAndReflexVertices()
	{
		for (int i = 0; i < polygonVertices.Count; i++)
		{
			Vertex value = polygonVertices[i].Value;
			if (IsConvex(value))
			{
				convexVertices.Add(value);
			}
			else
			{
				reflexVertices.Add(value);
			}
		}
	}

	private static void FindEarVertices()
	{
		for (int i = 0; i < convexVertices.Count; i++)
		{
			Vertex vertex = convexVertices[i];
			if (IsEar(vertex))
			{
				earVertices.AddLast(vertex);
			}
		}
	}

	private static bool IsEar(Vertex c)
	{
		Vertex value = polygonVertices[polygonVertices.IndexOf(c) - 1].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(c) + 1].Value;
		foreach (Vertex reflexVertex in reflexVertices)
		{
			if (reflexVertex.Equals(value) || reflexVertex.Equals(c) || reflexVertex.Equals(value2) || !Triangle.ContainsPoint(value, c, value2, reflexVertex))
			{
				continue;
			}
			return false;
		}
		return true;
	}

	private static bool IsConvex(Vertex c)
	{
		Vertex value = polygonVertices[polygonVertices.IndexOf(c) - 1].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(c) + 1].Value;
		TSVector2 value3 = TSVector2.Normalize(c.Position - value.Position);
		TSVector2 tSVector = TSVector2.Normalize(value2.Position - c.Position);
		TSVector2 value4 = new TSVector2(-tSVector.y, tSVector.x);
		return TSVector2.Dot(value3, value4) <= 0f;
	}

	private static bool IsReflex(Vertex c)
	{
		return !IsConvex(c);
	}

	private static void Log(string format, params object[] parameters)
	{
		Console.WriteLine(format, parameters);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Count; i++)
		{
			stringBuilder.Append(base[i].ToString());
			if (i < Count - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}
}
