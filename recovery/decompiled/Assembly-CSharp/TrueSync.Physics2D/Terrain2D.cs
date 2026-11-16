using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class Terrain2D
{
	public World World;

	public TSVector2 Center;

	public FP Width;

	public FP Height;

	public int PointsPerUnit;

	public int CellSize;

	public int SubCellSize;

	public int Iterations = 2;

	public TriangulationAlgorithm Decomposer;

	private sbyte[,] _terrainMap;

	private List<Body>[,] _bodyMap;

	private FP _localWidth;

	private FP _localHeight;

	private int _xnum;

	private int _ynum;

	private AABB _dirtyArea;

	private TSVector2 _topLeft;

	public Terrain2D(World world, AABB area)
	{
		World = world;
		Width = area.Width;
		Height = area.Height;
		Center = area.Center;
	}

	public Terrain2D(World world, TSVector2 position, FP width, FP height)
	{
		World = world;
		Width = width;
		Height = height;
		Center = position;
	}

	public void Initialize()
	{
		_topLeft = new TSVector2(Center.x - Width * 0.5f, Center.y - -Height * 0.5f);
		_localWidth = Width * PointsPerUnit;
		_localHeight = Height * PointsPerUnit;
		_terrainMap = new sbyte[(int)(long)_localWidth + 1, (int)(long)_localHeight + 1];
		for (int i = 0; i < _localWidth; i++)
		{
			for (int j = 0; j < _localHeight; j++)
			{
				_terrainMap[i, j] = 1;
			}
		}
		_xnum = (int)(long)(_localWidth / CellSize);
		_ynum = (int)(long)(_localHeight / CellSize);
		_bodyMap = new List<Body>[_xnum, _ynum];
		_dirtyArea = new AABB(new TSVector2(FP.MaxValue, FP.MaxValue), new TSVector2(FP.MinValue, FP.MinValue));
	}

	public void ApplyData(sbyte[,] data, TSVector2 offset = default(TSVector2))
	{
		for (int i = 0; i < data.GetUpperBound(0); i++)
		{
			for (int j = 0; j < data.GetUpperBound(1); j++)
			{
				if (i + offset.x >= 0 && i + offset.x < _localWidth && j + offset.y >= 0 && j + offset.y < _localHeight)
				{
					_terrainMap[(int)(long)(i + offset.x), (int)(long)(j + offset.y)] = data[i, j];
				}
			}
		}
		RemoveOldData(0, _xnum, 0, _ynum);
	}

	public void ModifyTerrain(TSVector2 location, sbyte value)
	{
		TSVector2 tSVector = location - _topLeft;
		tSVector.x = tSVector.x * _localWidth / Width;
		tSVector.y = tSVector.y * -_localHeight / Height;
		if (tSVector.x >= 0 && tSVector.x < _localWidth && tSVector.y >= 0 && tSVector.y < _localHeight)
		{
			_terrainMap[(int)(long)tSVector.x, (int)(long)tSVector.y] = value;
			if (tSVector.x < _dirtyArea.LowerBound.x)
			{
				_dirtyArea.LowerBound.x = tSVector.x;
			}
			if (tSVector.x > _dirtyArea.UpperBound.x)
			{
				_dirtyArea.UpperBound.x = tSVector.x;
			}
			if (tSVector.y < _dirtyArea.LowerBound.y)
			{
				_dirtyArea.LowerBound.y = tSVector.y;
			}
			if (tSVector.y > _dirtyArea.UpperBound.y)
			{
				_dirtyArea.UpperBound.y = tSVector.y;
			}
		}
	}

	public void RegenerateTerrain()
	{
		int num = (int)(long)(_dirtyArea.LowerBound.x / CellSize);
		if (num < 0)
		{
			num = 0;
		}
		int num2 = (int)(long)(_dirtyArea.UpperBound.x / CellSize) + 1;
		if (num2 > _xnum)
		{
			num2 = _xnum;
		}
		int num3 = (int)(long)(_dirtyArea.LowerBound.y / CellSize);
		if (num3 < 0)
		{
			num3 = 0;
		}
		int num4 = (int)(long)(_dirtyArea.UpperBound.y / CellSize) + 1;
		if (num4 > _ynum)
		{
			num4 = _ynum;
		}
		RemoveOldData(num, num2, num3, num4);
		_dirtyArea = new AABB(new TSVector2(FP.MaxValue, FP.MaxValue), new TSVector2(FP.MinValue, FP.MinValue));
	}

	private void RemoveOldData(int xStart, int xEnd, int yStart, int yEnd)
	{
		for (int i = xStart; i < xEnd; i++)
		{
			for (int j = yStart; j < yEnd; j++)
			{
				if (_bodyMap[i, j] != null)
				{
					for (int k = 0; k < _bodyMap[i, j].Count; k++)
					{
						World.RemoveBody(_bodyMap[i, j][k]);
					}
				}
				_bodyMap[i, j] = null;
				GenerateTerrain(i, j);
			}
		}
	}

	private void GenerateTerrain(int gx, int gy)
	{
		FP fP = gx * CellSize;
		FP fP2 = gy * CellSize;
		List<Vertices> list = MarchingSquares.DetectSquares(new AABB(new TSVector2(fP, fP2), new TSVector2(fP + CellSize, fP2 + CellSize)), SubCellSize, SubCellSize, _terrainMap, Iterations, combine: true);
		if (list.Count == 0)
		{
			return;
		}
		_bodyMap[gx, gy] = new List<Body>();
		TSVector2 value = new TSVector2(1f / (float)PointsPerUnit, 1f / (float)(-PointsPerUnit));
		foreach (Vertices item in list)
		{
			item.Scale(ref value);
			item.Translate(ref _topLeft);
			Vertices vertices = SimplifyTools.CollinearSimplify(item, FP.Zero);
			List<Vertices> list2 = Triangulate.ConvexPartition(vertices, Decomposer, discardAndFixInvalid: true, FP.EN3);
			foreach (Vertices item2 in list2)
			{
				if (item2.Count > 2)
				{
					_bodyMap[gx, gy].Add(BodyFactory.CreatePolygon(World, item2, 1, null));
				}
			}
		}
	}
}
