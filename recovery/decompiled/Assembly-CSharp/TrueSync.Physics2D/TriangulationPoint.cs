using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class TriangulationPoint
{
	public FP X;

	public FP Y;

	public List<DTSweepConstraint> Edges { get; private set; }

	public FP Xf
	{
		get
		{
			return X;
		}
		set
		{
			X = value;
		}
	}

	public FP Yf
	{
		get
		{
			return Y;
		}
		set
		{
			Y = value;
		}
	}

	public bool HasEdges => Edges != null;

	public TriangulationPoint(FP x, FP y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return string.Concat("[", X, ",", Y, "]");
	}

	public void AddEdge(DTSweepConstraint e)
	{
		if (Edges == null)
		{
			Edges = new List<DTSweepConstraint>();
		}
		Edges.Add(e);
	}
}
