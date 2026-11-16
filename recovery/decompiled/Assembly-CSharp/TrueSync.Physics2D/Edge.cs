using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class Edge
{
	public Trapezoid Above;

	public FP B;

	public Trapezoid Below;

	public HashSet<Point> MPoints;

	public Point P;

	public Point Q;

	public FP Slope;

	public Edge(Point p, Point q)
	{
		P = p;
		Q = q;
		if (q.X - p.X != 0)
		{
			Slope = (q.Y - p.Y) / (q.X - p.X);
		}
		else
		{
			Slope = 0;
		}
		B = p.Y - p.X * Slope;
		Above = null;
		Below = null;
		MPoints = new HashSet<Point>();
		MPoints.Add(p);
		MPoints.Add(q);
	}

	public bool IsAbove(Point point)
	{
		return P.Orient2D(Q, point) < 0;
	}

	public bool IsBelow(Point point)
	{
		return P.Orient2D(Q, point) > 0;
	}

	public void AddMpoint(Point point)
	{
		foreach (Point mPoint in MPoints)
		{
			if (!mPoint.Neq(point))
			{
				return;
			}
		}
		MPoints.Add(point);
	}
}
