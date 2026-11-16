using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal static class CDTDecomposer
{
	public static List<Vertices> ConvexPartition(Vertices vertices)
	{
		Polygon polygon = new Polygon();
		foreach (TSVector2 vertex in vertices)
		{
			polygon.Points.Add(new TriangulationPoint(vertex.x, vertex.y));
		}
		if (vertices.Holes != null)
		{
			foreach (Vertices hole in vertices.Holes)
			{
				Polygon polygon2 = new Polygon();
				foreach (TSVector2 item in hole)
				{
					polygon2.Points.Add(new TriangulationPoint(item.x, item.y));
				}
				polygon.AddHole(polygon2);
			}
		}
		DTSweepContext dTSweepContext = new DTSweepContext();
		dTSweepContext.PrepareTriangulation(polygon);
		DTSweep.Triangulate(dTSweepContext);
		List<Vertices> list = new List<Vertices>();
		foreach (DelaunayTriangle triangle in polygon.Triangles)
		{
			Vertices vertices2 = new Vertices();
			foreach (TriangulationPoint point in triangle.Points)
			{
				vertices2.Add(new TSVector2(point.X, point.Y));
			}
			list.Add(vertices2);
		}
		return list;
	}
}
