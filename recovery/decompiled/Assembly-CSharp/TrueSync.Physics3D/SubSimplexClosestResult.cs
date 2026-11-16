namespace TrueSync.Physics3D;

public class SubSimplexClosestResult
{
	private TSVector _closestPointOnSimplex;

	private UsageBitfield _usedVertices = new UsageBitfield();

	private FP[] _barycentricCoords = new FP[4];

	private bool _degenerate;

	public TSVector ClosestPointOnSimplex
	{
		get
		{
			return _closestPointOnSimplex;
		}
		set
		{
			_closestPointOnSimplex = value;
		}
	}

	public UsageBitfield UsedVertices
	{
		get
		{
			return _usedVertices;
		}
		set
		{
			_usedVertices = value;
		}
	}

	public FP[] BarycentricCoords
	{
		get
		{
			return _barycentricCoords;
		}
		set
		{
			_barycentricCoords = value;
		}
	}

	public bool Degenerate
	{
		get
		{
			return _degenerate;
		}
		set
		{
			_degenerate = value;
		}
	}

	public bool IsValid => _barycentricCoords[0] >= FP.Zero && _barycentricCoords[1] >= FP.Zero && _barycentricCoords[2] >= FP.Zero && _barycentricCoords[3] >= FP.Zero;

	public void Reset()
	{
		_degenerate = false;
		SetBarycentricCoordinates();
		_usedVertices.Reset();
	}

	public void SetBarycentricCoordinates()
	{
		SetBarycentricCoordinates(FP.Zero, FP.Zero, FP.Zero, FP.Zero);
	}

	public void SetBarycentricCoordinates(FP a, FP b, FP c, FP d)
	{
		_barycentricCoords[0] = a;
		_barycentricCoords[1] = b;
		_barycentricCoords[2] = c;
		_barycentricCoords[3] = d;
	}
}
