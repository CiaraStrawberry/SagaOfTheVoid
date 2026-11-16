using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal static class SeidelDecomposer
{
	public static List<Vertices> ConvexPartition(Vertices vertices, FP sheer)
	{
		List<Point> list = new List<Point>(vertices.Count);
		foreach (TSVector2 vertex in vertices)
		{
			list.Add(new Point(vertex.x, vertex.y));
		}
		Triangulator triangulator = new Triangulator(list, sheer);
		List<Vertices> list2 = new List<Vertices>();
		foreach (List<Point> triangle in triangulator.Triangles)
		{
			Vertices vertices2 = new Vertices(triangle.Count);
			foreach (Point item in triangle)
			{
				vertices2.Add(new TSVector2(item.X, item.Y));
			}
			list2.Add(vertices2);
		}
		return list2;
	}

	public static List<Vertices> ConvexPartitionTrapezoid(Vertices vertices, FP sheer)
	{
		List<Point> list = new List<Point>(vertices.Count);
		foreach (TSVector2 vertex in vertices)
		{
			list.Add(new Point(vertex.x, vertex.y));
		}
		Triangulator triangulator = new Triangulator(list, sheer);
		List<Vertices> list2 = new List<Vertices>();
		foreach (Trapezoid trapezoid in triangulator.Trapezoids)
		{
			Vertices vertices2 = new Vertices();
			List<Point> vertices3 = trapezoid.GetVertices();
			foreach (Point item in vertices3)
			{
				vertices2.Add(new TSVector2(item.X, item.Y));
			}
			list2.Add(vertices2);
		}
		return list2;
	}
}
