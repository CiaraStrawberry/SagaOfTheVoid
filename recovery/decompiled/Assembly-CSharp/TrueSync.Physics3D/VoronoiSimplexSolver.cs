namespace TrueSync.Physics3D;

public class VoronoiSimplexSolver
{
	private const int VertexA = 0;

	private const int VertexB = 1;

	private const int VertexC = 2;

	private const int VertexD = 3;

	private const int VoronoiSimplexMaxVerts = 5;

	private const bool CatchDegenerateTetrahedron = true;

	private int _numVertices;

	private TSVector[] _simplexVectorW = new TSVector[5];

	private TSVector[] _simplexPointsP = new TSVector[5];

	private TSVector[] _simplexPointsQ = new TSVector[5];

	private TSVector _cachedPA;

	private TSVector _cachedPB;

	private TSVector _cachedV;

	private TSVector _lastW;

	private bool _cachedValidClosest;

	private SubSimplexClosestResult _cachedBC = new SubSimplexClosestResult();

	private SubSimplexClosestResult tempResult = new SubSimplexClosestResult();

	private bool _needsUpdate;

	public bool FullSimplex => _numVertices == 4;

	public int NumVertices => _numVertices;

	public FP MaxVertex
	{
		get
		{
			int numVertices = NumVertices;
			FP fP = FP.Zero;
			for (int i = 0; i < numVertices; i++)
			{
				FP sqrMagnitude = _simplexVectorW[i].sqrMagnitude;
				if (fP < sqrMagnitude)
				{
					fP = sqrMagnitude;
				}
			}
			return fP;
		}
	}

	public bool EmptySimplex => NumVertices == 0;

	public void Reset()
	{
		_cachedValidClosest = false;
		_numVertices = 0;
		_needsUpdate = true;
		_lastW = new TSVector(FP.MaxValue, FP.MaxValue, FP.MaxValue);
		_cachedBC.Reset();
	}

	public void AddVertex(TSVector w, TSVector p, TSVector q)
	{
		_lastW = w;
		_needsUpdate = true;
		_simplexVectorW[_numVertices] = w;
		_simplexPointsP[_numVertices] = p;
		_simplexPointsQ[_numVertices] = q;
		_numVertices++;
	}

	public bool Closest(out TSVector v)
	{
		bool result = UpdateClosestVectorAndPoints();
		v = _cachedV;
		return result;
	}

	public int GetSimplex(out TSVector[] pBuf, out TSVector[] qBuf, out TSVector[] yBuf)
	{
		int numVertices = NumVertices;
		pBuf = new TSVector[numVertices];
		qBuf = new TSVector[numVertices];
		yBuf = new TSVector[numVertices];
		for (int i = 0; i < numVertices; i++)
		{
			ref TSVector reference = ref yBuf[i];
			reference = _simplexVectorW[i];
			ref TSVector reference2 = ref pBuf[i];
			reference2 = _simplexPointsP[i];
			ref TSVector reference3 = ref qBuf[i];
			reference3 = _simplexPointsQ[i];
		}
		return numVertices;
	}

	public bool InSimplex(TSVector w)
	{
		if (w == _lastW)
		{
			return true;
		}
		int numVertices = NumVertices;
		for (int i = 0; i < numVertices; i++)
		{
			if (_simplexVectorW[i] == w)
			{
				return true;
			}
		}
		return false;
	}

	public void BackupClosest(out TSVector v)
	{
		v = _cachedV;
	}

	public void ComputePoints(out TSVector p1, out TSVector p2)
	{
		UpdateClosestVectorAndPoints();
		p1 = _cachedPA;
		p2 = _cachedPB;
	}

	public void RemoveVertex(int index)
	{
		_numVertices--;
		ref TSVector reference = ref _simplexVectorW[index];
		reference = _simplexVectorW[_numVertices];
		ref TSVector reference2 = ref _simplexPointsP[index];
		reference2 = _simplexPointsP[_numVertices];
		ref TSVector reference3 = ref _simplexPointsQ[index];
		reference3 = _simplexPointsQ[_numVertices];
	}

	public void ReduceVertices(UsageBitfield usedVerts)
	{
		if (NumVertices >= 4 && !usedVerts.UsedVertexD)
		{
			RemoveVertex(3);
		}
		if (NumVertices >= 3 && !usedVerts.UsedVertexC)
		{
			RemoveVertex(2);
		}
		if (NumVertices >= 2 && !usedVerts.UsedVertexB)
		{
			RemoveVertex(1);
		}
		if (NumVertices >= 1 && !usedVerts.UsedVertexA)
		{
			RemoveVertex(0);
		}
	}

	public bool UpdateClosestVectorAndPoints()
	{
		if (_needsUpdate)
		{
			_cachedBC.Reset();
			_needsUpdate = false;
			switch (NumVertices)
			{
			case 0:
				_cachedValidClosest = false;
				break;
			case 1:
				_cachedPA = _simplexPointsP[0];
				_cachedPB = _simplexPointsQ[0];
				_cachedV = _cachedPA - _cachedPB;
				_cachedBC.Reset();
				_cachedBC.SetBarycentricCoordinates(1f, FP.Zero, FP.Zero, FP.Zero);
				_cachedValidClosest = _cachedBC.IsValid;
				break;
			case 2:
			{
				TSVector tSVector = _simplexVectorW[0];
				TSVector tSVector2 = _simplexVectorW[1];
				TSVector vector = tSVector * -1;
				TSVector tSVector3 = tSVector2 - tSVector;
				FP fP = TSVector.Dot(tSVector3, vector);
				if (fP > 0)
				{
					FP sqrMagnitude = tSVector3.sqrMagnitude;
					if (fP < sqrMagnitude)
					{
						fP /= sqrMagnitude;
						vector -= fP * tSVector3;
						_cachedBC.UsedVertices.UsedVertexA = true;
						_cachedBC.UsedVertices.UsedVertexB = true;
					}
					else
					{
						fP = 1;
						vector -= tSVector3;
						_cachedBC.UsedVertices.UsedVertexB = true;
					}
				}
				else
				{
					fP = 0;
					_cachedBC.UsedVertices.UsedVertexA = true;
				}
				_cachedBC.SetBarycentricCoordinates(1 - fP, fP, 0, 0);
				_cachedPA = _simplexPointsP[0] + fP * (_simplexPointsP[1] - _simplexPointsP[0]);
				_cachedPB = _simplexPointsQ[0] + fP * (_simplexPointsQ[1] - _simplexPointsQ[0]);
				_cachedV = _cachedPA - _cachedPB;
				ReduceVertices(_cachedBC.UsedVertices);
				_cachedValidClosest = _cachedBC.IsValid;
				break;
			}
			case 3:
			{
				TSVector p = default;
				TSVector a = _simplexVectorW[0];
				TSVector b = _simplexVectorW[1];
				TSVector c = _simplexVectorW[2];
				ClosestPtPointTriangle(p, a, b, c, ref _cachedBC);
				_cachedPA = _simplexPointsP[0] * _cachedBC.BarycentricCoords[0] + _simplexPointsP[1] * _cachedBC.BarycentricCoords[1] + _simplexPointsP[2] * _cachedBC.BarycentricCoords[2] + _simplexPointsP[3] * _cachedBC.BarycentricCoords[3];
				_cachedPB = _simplexPointsQ[0] * _cachedBC.BarycentricCoords[0] + _simplexPointsQ[1] * _cachedBC.BarycentricCoords[1] + _simplexPointsQ[2] * _cachedBC.BarycentricCoords[2] + _simplexPointsQ[3] * _cachedBC.BarycentricCoords[3];
				_cachedV = _cachedPA - _cachedPB;
				ReduceVertices(_cachedBC.UsedVertices);
				_cachedValidClosest = _cachedBC.IsValid;
				break;
			}
			case 4:
			{
				TSVector p = default;
				TSVector a = _simplexVectorW[0];
				TSVector b = _simplexVectorW[1];
				TSVector c = _simplexVectorW[2];
				TSVector d = _simplexVectorW[3];
				if (ClosestPtPointTetrahedron(p, a, b, c, d, ref _cachedBC))
				{
					_cachedPA = _simplexPointsP[0] * _cachedBC.BarycentricCoords[0] + _simplexPointsP[1] * _cachedBC.BarycentricCoords[1] + _simplexPointsP[2] * _cachedBC.BarycentricCoords[2] + _simplexPointsP[3] * _cachedBC.BarycentricCoords[3];
					_cachedPB = _simplexPointsQ[0] * _cachedBC.BarycentricCoords[0] + _simplexPointsQ[1] * _cachedBC.BarycentricCoords[1] + _simplexPointsQ[2] * _cachedBC.BarycentricCoords[2] + _simplexPointsQ[3] * _cachedBC.BarycentricCoords[3];
					_cachedV = _cachedPA - _cachedPB;
					ReduceVertices(_cachedBC.UsedVertices);
					_cachedValidClosest = _cachedBC.IsValid;
				}
				else if (_cachedBC.Degenerate)
				{
					_cachedValidClosest = false;
				}
				else
				{
					_cachedValidClosest = true;
					_cachedV.x = (_cachedV.y = (_cachedV.z = FP.Zero));
				}
				break;
			}
			default:
				_cachedValidClosest = false;
				break;
			}
		}
		return _cachedValidClosest;
	}

	public bool ClosestPtPointTriangle(TSVector p, TSVector a, TSVector b, TSVector c, ref SubSimplexClosestResult result)
	{
		result.UsedVertices.Reset();
		TSVector tSVector = b - a;
		TSVector tSVector2 = c - a;
		TSVector vector = p - a;
		FP fP = TSVector.Dot(tSVector, vector);
		FP fP2 = TSVector.Dot(tSVector2, vector);
		if (fP <= FP.Zero && fP2 <= FP.Zero)
		{
			result.ClosestPointOnSimplex = a;
			result.UsedVertices.UsedVertexA = true;
			result.SetBarycentricCoordinates(1, 0, 0, 0);
			return true;
		}
		TSVector vector2 = p - b;
		FP fP3 = TSVector.Dot(tSVector, vector2);
		FP fP4 = TSVector.Dot(tSVector2, vector2);
		if (fP3 >= FP.Zero && fP4 <= fP3)
		{
			result.ClosestPointOnSimplex = b;
			result.UsedVertices.UsedVertexB = true;
			result.SetBarycentricCoordinates(0, 1, 0, 0);
			return true;
		}
		FP fP5 = fP * fP4 - fP3 * fP2;
		FP fP6;
		if (fP5 <= FP.Zero && fP >= FP.Zero && fP3 <= FP.Zero)
		{
			fP6 = fP / (fP - fP3);
			result.ClosestPointOnSimplex = a + fP6 * tSVector;
			result.UsedVertices.UsedVertexA = true;
			result.UsedVertices.UsedVertexB = true;
			result.SetBarycentricCoordinates(1 - fP6, fP6, 0, 0);
			return true;
		}
		TSVector vector3 = p - c;
		FP fP7 = TSVector.Dot(tSVector, vector3);
		FP fP8 = TSVector.Dot(tSVector2, vector3);
		if (fP8 >= FP.Zero && fP7 <= fP8)
		{
			result.ClosestPointOnSimplex = c;
			result.UsedVertices.UsedVertexC = true;
			result.SetBarycentricCoordinates(0, 0, 1, 0);
			return true;
		}
		FP fP9 = fP7 * fP2 - fP * fP8;
		FP fP10;
		if (fP9 <= FP.Zero && fP2 >= FP.Zero && fP8 <= FP.Zero)
		{
			fP10 = fP2 / (fP2 - fP8);
			result.ClosestPointOnSimplex = a + fP10 * tSVector2;
			result.UsedVertices.UsedVertexA = true;
			result.UsedVertices.UsedVertexC = true;
			result.SetBarycentricCoordinates(1 - fP10, 0, fP10, 0);
			return true;
		}
		FP fP11 = fP3 * fP8 - fP7 * fP4;
		if (fP11 <= FP.Zero && fP4 - fP3 >= FP.Zero && fP7 - fP8 >= FP.Zero)
		{
			fP10 = (fP4 - fP3) / (fP4 - fP3 + (fP7 - fP8));
			result.ClosestPointOnSimplex = b + fP10 * (c - b);
			result.UsedVertices.UsedVertexB = true;
			result.UsedVertices.UsedVertexC = true;
			result.SetBarycentricCoordinates(0, 1 - fP10, fP10, 0);
			return true;
		}
		FP fP12 = FP.One / (fP11 + fP9 + fP5);
		fP6 = fP9 * fP12;
		fP10 = fP5 * fP12;
		result.ClosestPointOnSimplex = a + tSVector * fP6 + tSVector2 * fP10;
		result.UsedVertices.UsedVertexA = true;
		result.UsedVertices.UsedVertexB = true;
		result.UsedVertices.UsedVertexC = true;
		result.SetBarycentricCoordinates(1 - fP6 - fP10, fP6, fP10, 0);
		return true;
	}

	public int PointOutsideOfPlane(TSVector p, TSVector a, TSVector b, TSVector c, TSVector d)
	{
		TSVector vector = TSVector.Cross(b - a, c - a);
		FP fP = TSVector.Dot(p - a, vector);
		FP fP2 = TSVector.Dot(d - a, vector);
		if (fP2 * fP2 < FP.EN8)
		{
			return -1;
		}
		return (fP * fP2 < FP.Zero) ? 1 : 0;
	}

	public bool ClosestPtPointTetrahedron(TSVector p, TSVector a, TSVector b, TSVector c, TSVector d, ref SubSimplexClosestResult finalResult)
	{
		tempResult.Reset();
		finalResult.ClosestPointOnSimplex = p;
		finalResult.UsedVertices.Reset();
		finalResult.UsedVertices.UsedVertexA = true;
		finalResult.UsedVertices.UsedVertexB = true;
		finalResult.UsedVertices.UsedVertexC = true;
		finalResult.UsedVertices.UsedVertexD = true;
		int num = PointOutsideOfPlane(p, a, b, c, d);
		int num2 = PointOutsideOfPlane(p, a, c, d, b);
		int num3 = PointOutsideOfPlane(p, a, d, b, c);
		int num4 = PointOutsideOfPlane(p, b, d, c, a);
		if (num < 0 || num2 < 0 || num3 < 0 || num4 < 0)
		{
			finalResult.Degenerate = true;
			return false;
		}
		if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
		{
			return false;
		}
		FP fP = FP.MaxValue;
		if (num != 0)
		{
			ClosestPtPointTriangle(p, a, b, c, ref tempResult);
			TSVector closestPointOnSimplex = tempResult.ClosestPointOnSimplex;
			FP sqrMagnitude = (closestPointOnSimplex - p).sqrMagnitude;
			if (sqrMagnitude < fP)
			{
				fP = sqrMagnitude;
				finalResult.ClosestPointOnSimplex = closestPointOnSimplex;
				finalResult.UsedVertices.Reset();
				finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
				finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexB;
				finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexC;
				finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[0], tempResult.BarycentricCoords[1], tempResult.BarycentricCoords[2], 0);
			}
		}
		if (num2 != 0)
		{
			ClosestPtPointTriangle(p, a, c, d, ref tempResult);
			TSVector closestPointOnSimplex2 = tempResult.ClosestPointOnSimplex;
			FP sqrMagnitude2 = (closestPointOnSimplex2 - p).sqrMagnitude;
			if (sqrMagnitude2 < fP)
			{
				fP = sqrMagnitude2;
				finalResult.ClosestPointOnSimplex = closestPointOnSimplex2;
				finalResult.UsedVertices.Reset();
				finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
				finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexB;
				finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexC;
				finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[0], 0, tempResult.BarycentricCoords[1], tempResult.BarycentricCoords[2]);
			}
		}
		if (num3 != 0)
		{
			ClosestPtPointTriangle(p, a, d, b, ref tempResult);
			TSVector closestPointOnSimplex3 = tempResult.ClosestPointOnSimplex;
			FP sqrMagnitude3 = (closestPointOnSimplex3 - p).sqrMagnitude;
			if (sqrMagnitude3 < fP)
			{
				fP = sqrMagnitude3;
				finalResult.ClosestPointOnSimplex = closestPointOnSimplex3;
				finalResult.UsedVertices.Reset();
				finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
				finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexB;
				finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexC;
				finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[0], tempResult.BarycentricCoords[2], 0, tempResult.BarycentricCoords[1]);
			}
		}
		if (num4 != 0)
		{
			ClosestPtPointTriangle(p, b, d, c, ref tempResult);
			TSVector closestPointOnSimplex4 = tempResult.ClosestPointOnSimplex;
			FP sqrMagnitude4 = (closestPointOnSimplex4 - p).sqrMagnitude;
			if (sqrMagnitude4 < fP)
			{
				fP = sqrMagnitude4;
				finalResult.ClosestPointOnSimplex = closestPointOnSimplex4;
				finalResult.UsedVertices.Reset();
				finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexA;
				finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexB;
				finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexC;
				finalResult.SetBarycentricCoordinates(0, tempResult.BarycentricCoords[0], tempResult.BarycentricCoords[2], tempResult.BarycentricCoords[1]);
			}
		}
		if (finalResult.UsedVertices.UsedVertexA && finalResult.UsedVertices.UsedVertexB && finalResult.UsedVertices.UsedVertexC && finalResult.UsedVertices.UsedVertexD)
		{
			return true;
		}
		return true;
	}
}
