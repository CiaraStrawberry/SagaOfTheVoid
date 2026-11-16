namespace TrueSync.Physics2D;

internal class DTSweepContext : TriangulationContext
{
	public class DTSweepBasin
	{
		public AdvancingFrontNode bottomNode;

		public bool leftHighest;

		public AdvancingFrontNode leftNode;

		public AdvancingFrontNode rightNode;

		public FP width;
	}

	public class DTSweepEdgeEvent
	{
		public DTSweepConstraint ConstrainedEdge;

		public bool Right;
	}

	private static readonly FP ALPHA = 0.3f;

	public DTSweepBasin Basin = new DTSweepBasin();

	public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

	private DTSweepPointComparator _comparator = new DTSweepPointComparator();

	public AdvancingFront aFront;

	public TriangulationPoint Head { get; set; }

	public TriangulationPoint Tail { get; set; }

	public DTSweepContext()
	{
		Clear();
	}

	public void RemoveFromList(DelaunayTriangle triangle)
	{
		Triangles.Remove(triangle);
	}

	public void MeshClean(DelaunayTriangle triangle)
	{
		MeshCleanReq(triangle);
	}

	private void MeshCleanReq(DelaunayTriangle triangle)
	{
		if (triangle == null || triangle.IsInterior)
		{
			return;
		}
		triangle.IsInterior = true;
		Triangulatable.AddTriangle(triangle);
		for (int i = 0; i < 3; i++)
		{
			if (!triangle.EdgeIsConstrained[i])
			{
				MeshCleanReq(triangle.Neighbors[i]);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		Triangles.Clear();
	}

	public void AddNode(AdvancingFrontNode node)
	{
		aFront.AddNode(node);
	}

	public void RemoveNode(AdvancingFrontNode node)
	{
		aFront.RemoveNode(node);
	}

	public AdvancingFrontNode LocateNode(TriangulationPoint point)
	{
		return aFront.LocateNode(point);
	}

	public void CreateAdvancingFront()
	{
		DelaunayTriangle delaunayTriangle = new DelaunayTriangle(Points[0], Tail, Head);
		Triangles.Add(delaunayTriangle);
		AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(delaunayTriangle.Points[1]);
		advancingFrontNode.Triangle = delaunayTriangle;
		AdvancingFrontNode advancingFrontNode2 = new AdvancingFrontNode(delaunayTriangle.Points[0]);
		advancingFrontNode2.Triangle = delaunayTriangle;
		AdvancingFrontNode tail = new AdvancingFrontNode(delaunayTriangle.Points[2]);
		aFront = new AdvancingFront(advancingFrontNode, tail);
		aFront.AddNode(advancingFrontNode2);
		aFront.Head.Next = advancingFrontNode2;
		advancingFrontNode2.Next = aFront.Tail;
		advancingFrontNode2.Prev = aFront.Head;
		aFront.Tail.Prev = advancingFrontNode2;
	}

	public void MapTriangleToNodes(DelaunayTriangle t)
	{
		for (int i = 0; i < 3; i++)
		{
			if (t.Neighbors[i] == null)
			{
				AdvancingFrontNode advancingFrontNode = aFront.LocatePoint(t.PointCW(t.Points[i]));
				if (advancingFrontNode != null)
				{
					advancingFrontNode.Triangle = t;
				}
			}
		}
	}

	public override void PrepareTriangulation(Triangulatable t)
	{
		base.PrepareTriangulation(t);
		FP x;
		FP fP = (x = Points[0].X);
		FP y;
		FP fP2 = (y = Points[0].Y);
		foreach (TriangulationPoint point in Points)
		{
			if (point.X > fP)
			{
				fP = point.X;
			}
			if (point.X < x)
			{
				x = point.X;
			}
			if (point.Y > fP2)
			{
				fP2 = point.Y;
			}
			if (point.Y < y)
			{
				y = point.Y;
			}
		}
		FP fP3 = ALPHA * (fP - x);
		FP fP4 = ALPHA * (fP2 - y);
		TriangulationPoint head = new TriangulationPoint(fP + fP3, y - fP4);
		TriangulationPoint tail = new TriangulationPoint(x - fP3, y - fP4);
		Head = head;
		Tail = tail;
		Points.Sort(_comparator);
	}

	public void FinalizeTriangulation()
	{
		Triangulatable.AddTriangles(Triangles);
		Triangles.Clear();
	}

	public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
	{
		return new DTSweepConstraint(a, b);
	}
}
