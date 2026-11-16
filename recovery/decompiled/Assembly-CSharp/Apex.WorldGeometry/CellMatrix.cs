using System;
using System.Collections;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Services;
using Apex.Steering;
using UnityEngine;

namespace Apex.WorldGeometry;

public class CellMatrix : Matrix<Cell>, IHeightMap, ISampleHeightsSimple
{
	internal class MatrixIncrementalInitializer
	{
		private CellMatrix _matrix;

		private IEnumerator _iter;

		internal bool isInitializing
		{
			get
			{
				bool flag = _iter.MoveNext();
				if (!flag)
				{
					matrix = _matrix;
				}
				return flag;
			}
		}

		internal CellMatrix matrix { get; private set; }

		internal MatrixIncrementalInitializer(ICellMatrixConfiguration cfg, CellMatrixData data)
		{
			_matrix = new CellMatrix(cfg);
			_iter = _matrix.Populate(data);
		}

		internal MatrixIncrementalInitializer(ICellMatrixConfiguration cfg)
		{
			_matrix = new CellMatrix(cfg);
			_iter = _matrix.Populate();
		}
	}

	private Vector3 _origin;

	private Vector3 _start;

	private Bounds _bounds;

	private float _cellSize;

	private float _obstacleSensitivityRange;

	private bool _generateHeightMap;

	private float _upperBoundary;

	private float _lowerBoundary;

	private ColliderDetectionMode _obstacleAndGroundDetection;

	private float _granularity;

	private int _heightMapSizeX;

	private int _heightMapSizeZ;

	private HeightLookupType _heightLookupType;

	private int _heightLookupMaxDepth;

	private IHeightLookup _heightLookup;

	private IList<PortalCell> _shortcutPortals;

	private IBlockDetector _obstacleAndGroundDetector;

	bool IHeightMap.isGridBound => true;

	public Bounds bounds => _bounds;

	public float granularity => _granularity;

	internal Vector3 origin => _origin;

	internal Vector3 start => _start;

	internal float upperBoundary => _upperBoundary;

	internal float lowerBoundary => _lowerBoundary;

	internal float cellSize => _cellSize;

	internal bool hasHeightMap => _heightLookup.hasHeights;

	internal VectorXZ heightMapSize => new VectorXZ(_heightMapSizeX, _heightMapSizeZ);

	internal int heightMapEntries => _heightLookup.heightCount;

	internal IList<PortalCell> shortcutPortals => _shortcutPortals;

	internal float obstacleSensitivityRange => _obstacleSensitivityRange;

	protected CellMatrix(ICellMatrixConfiguration cfg)
		: base(cfg.sizeX, cfg.sizeZ)
	{
		Initialize(cfg);
	}

	public static CellMatrix Create(ICellMatrixConfiguration cfg)
	{
		CellMatrix cellMatrix = new CellMatrix(cfg);
		IEnumerator enumerator = cellMatrix.Populate();
		while (enumerator.MoveNext())
		{
		}
		return cellMatrix;
	}

	public static CellMatrix Create(ICellMatrixConfiguration cfg, CellMatrixData data)
	{
		CellMatrix cellMatrix = new CellMatrix(cfg);
		IEnumerator enumerator = cellMatrix.Populate(data);
		while (enumerator.MoveNext())
		{
		}
		return cellMatrix;
	}

	internal static CellMatrix CreateForEditor(ICellMatrixConfiguration cfg, CellMatrixData data)
	{
		CellMatrix cellMatrix = new CellMatrix(cfg);
		if (data == null)
		{
			cellMatrix._heightLookup = cellMatrix.CreateHeightLookup(0);
		}
		else
		{
			cellMatrix.InitHeightMapForEditor(data);
		}
		return cellMatrix;
	}

	internal static MatrixIncrementalInitializer CreateIncrementally(ICellMatrixConfiguration cfg)
	{
		return new MatrixIncrementalInitializer(cfg);
	}

	internal static MatrixIncrementalInitializer CreateIncrementally(ICellMatrixConfiguration cfg, CellMatrixData data)
	{
		return new MatrixIncrementalInitializer(cfg, data);
	}

	public bool Contains(Vector3 pos)
	{
		return _bounds.Contains(pos);
	}

	float ISampleHeightsSimple.SampleHeight(Vector3 position, CellMatrix matrix)
	{
		float f = (position.x - _start.x) / _granularity;
		float f2 = (position.z - _start.z) / _granularity;
		int x = Mathf.RoundToInt(f);
		int z = Mathf.RoundToInt(f2);
		if (_heightLookup.TryGetHeight(x, z, out var height))
		{
			return height;
		}
		return origin.y;
	}

	float IHeightMap.SampleHeight(Vector3 position)
	{
		if (!_bounds.Contains(position))
		{
			return -10000f;
		}
		float f = (position.x - _start.x) / _granularity;
		float f2 = (position.z - _start.z) / _granularity;
		int x = Mathf.RoundToInt(f);
		int z = Mathf.RoundToInt(f2);
		if (_heightLookup.TryGetHeight(x, z, out var height))
		{
			return height;
		}
		return origin.y;
	}

	public void RenderHeightOverlay(Color drawColor)
	{
		if (_heightLookup != null)
		{
			_heightLookup.Render(_start, _granularity, drawColor);
		}
	}

	bool IHeightMap.TrySampleHeight(Vector3 position, out float height)
	{
		if (!_bounds.Contains(position))
		{
			height = -10000f;
			return false;
		}
		float f = (position.x - _start.x) / _granularity;
		float f2 = (position.z - _start.z) / _granularity;
		int x = Mathf.RoundToInt(f);
		int z = Mathf.RoundToInt(f2);
		if (_heightLookup.TryGetHeight(x, z, out height))
		{
			return true;
		}
		height = origin.y;
		return true;
	}

	internal float SampleHeight(int x, int z)
	{
		if (_heightLookup.TryGetHeight(x, z, out var height))
		{
			return height;
		}
		return origin.y;
	}

	internal Cell GetCell(Vector3 position, bool adjustToBounds)
	{
		float f = (position.x - _start.x) / _cellSize;
		float f2 = (position.z - _start.z) / _cellSize;
		int num = Mathf.FloorToInt(f);
		int num2 = Mathf.FloorToInt(f2);
		if (!adjustToBounds && !InBounds(num, num2) && _bounds.Contains(position))
		{
			adjustToBounds = true;
		}
		if (adjustToBounds)
		{
			num = AdjustColumnToBounds(num);
			num2 = AdjustRowToBounds(num2);
			return rawMatrix[num, num2];
		}
		return base[num, num2];
	}

	internal MatrixBounds GetMatrixBounds(Bounds b, float minOverlapToInclude, bool adjustToBounds)
	{
		return GetMatrixBounds(b.center, b.extents.x, b.extents.z, minOverlapToInclude, adjustToBounds);
	}

	internal MatrixBounds GetMatrixBounds(Vector3 position, float radiusX, float radiusZ, float minOverlapToInclude, bool adjustToBounds)
	{
		float num = 0f;
		float num2 = 0f;
		if (minOverlapToInclude != 0f)
		{
			radiusX -= minOverlapToInclude;
			radiusZ -= minOverlapToInclude;
			num2 = 1E-05f;
		}
		else
		{
			num = 1E-05f;
		}
		if (adjustToBounds)
		{
			return new MatrixBounds
			{
				minColumn = AdjustColumnToBounds(Mathf.FloorToInt((position.x - _start.x - radiusX - num2) / _cellSize)),
				minRow = AdjustRowToBounds(Mathf.FloorToInt((position.z - _start.z - radiusZ - num2) / _cellSize)),
				maxColumn = AdjustColumnToBounds(Mathf.FloorToInt((position.x - _start.x + radiusX - num) / _cellSize)),
				maxRow = AdjustRowToBounds(Mathf.FloorToInt((position.z - _start.z + radiusZ - num) / _cellSize))
			};
		}
		return new MatrixBounds
		{
			minColumn = Mathf.FloorToInt((position.x - _start.x - radiusX - num2) / _cellSize),
			minRow = Mathf.FloorToInt((position.z - _start.z - radiusZ - num2) / _cellSize),
			maxColumn = Mathf.FloorToInt((position.x - _start.x + radiusX - num) / _cellSize),
			maxRow = Mathf.FloorToInt((position.z - _start.z + radiusZ - num) / _cellSize)
		};
	}

	internal IEnumerator Update(Bounds extent)
	{
		if (_generateHeightMap)
		{
			Vector3 min = extent.min;
			Vector3 max = extent.max;
			MatrixBounds updateBounds = new MatrixBounds(Mathf.Clamp(Mathf.FloorToInt((min.x - _start.x) / _granularity), 0, _heightMapSizeX - 1), Mathf.Clamp(Mathf.FloorToInt((min.z - _start.z) / _granularity), 0, _heightMapSizeZ - 1), Mathf.Clamp(Mathf.CeilToInt((max.x - _start.x) / _granularity), 0, _heightMapSizeX - 1), Mathf.Clamp(Mathf.CeilToInt((max.z - _start.z) / _granularity), 0, _heightMapSizeZ - 1));
			IHeightLookup heightUpdater = _heightLookup.PrepareForUpdate(updateBounds, out updateBounds);
			bool detailedHeightMap = GameServices.heightStrategy.heightMapDetail == HeightMapDetailLevel.High;
			float plotRange = _lowerBoundary + _upperBoundary;
			Vector3 down = Vector3.down;
			Vector3 samplePos = default;
			samplePos.y = _start.y + _upperBoundary;
			for (int x = updateBounds.minColumn; x <= updateBounds.maxColumn; x++)
			{
				samplePos.x = ((!detailedHeightMap) ? (_start.x + (float)x * _granularity) : ((float)Math.Round(_start.x + (float)x * _granularity, 4)));
				for (int z = updateBounds.minRow; z <= updateBounds.maxRow; z++)
				{
					samplePos.z = ((!detailedHeightMap) ? (_start.z + (float)z * _granularity) : ((float)Math.Round(_start.z + (float)z * _granularity, 4)));
					if (UnityServices.physics.Raycast(samplePos, down, out var hit, plotRange, Layers.terrain))
					{
						heightUpdater.Add(x, z, hit.point.y);
					}
					else
					{
						heightUpdater.Add(x, z, -10000f);
					}
					yield return null;
				}
			}
			_heightLookup.FinishUpdate(heightUpdater);
		}
		ISampleHeightsSimple heightSampler = GetHeightSampler();
		MatrixBounds bounds = GetMatrixBounds(extent.center, extent.extents.x, extent.extents.z, 0f, adjustToBounds: true);
		float blockThreshold = Mathf.Max(_obstacleSensitivityRange, 0.01f);
		float maxHeight = _origin.y + _upperBoundary;
		float minHeight = _origin.y - _lowerBoundary;
		for (int i = bounds.minColumn; i <= bounds.maxColumn; i++)
		{
			for (int j = bounds.minRow; j <= bounds.maxRow; j++)
			{
				Cell cell = rawMatrix[i, j];
				Vector3 cellPos = cell.position;
				cellPos.y = Mathf.Clamp(heightSampler.SampleHeight(cellPos, this), minHeight, maxHeight);
				cell.UpdateState(isBlocked: IsBlocked(cellPos, blockThreshold), newHeight: cellPos.y);
				yield return null;
			}
		}
		CellConstructionStrategy cellConstruct = GameServices.cellConstruction;
		if (cellConstruct.calculateHeights)
		{
			IEnumerator iter = cellConstruct.heightSettingsProvider.AssignHeightSettings(this, bounds);
			while (iter.MoveNext())
			{
				yield return null;
			}
		}
		if (cellConstruct.clearanceProvider != null)
		{
			IEnumerator iter2 = cellConstruct.clearanceProvider.Reset(this);
			while (iter2.MoveNext())
			{
				yield return null;
			}
			iter2 = cellConstruct.clearanceProvider.SetClearance(this);
			while (iter2.MoveNext())
			{
				yield return null;
			}
		}
	}

	internal void UpdateForEditor(Bounds extent, CellMatrixData data)
	{
		MatrixBounds matrixBounds = GetMatrixBounds(extent.center, extent.extents.x + _cellSize * 2f, extent.extents.z + _cellSize * 2f, 0f, adjustToBounds: true);
		ICellFactory cellFactory = GameServices.cellConstruction.cellFactory;
		if (data == null)
		{
			float blockThreshold = Mathf.Max(_obstacleSensitivityRange, 0.01f);
			for (int i = matrixBounds.minColumn; i <= matrixBounds.maxColumn; i++)
			{
				for (int j = matrixBounds.minRow; j <= matrixBounds.maxRow; j++)
				{
					Cell cell = rawMatrix[i, j];
					if (cell == null)
					{
						Vector3 position = new Vector3(_start.x + (float)i * _cellSize + _cellSize / 2f, _origin.y, _start.z + (float)j * _cellSize + _cellSize / 2f);
						bool blocked = IsBlocked(position, blockThreshold);
						rawMatrix[i, j] = cellFactory.Create(this, position, i, j, blocked);
					}
					else
					{
						Vector3 position2 = cell.position;
						bool isBlocked = IsBlocked(position2, blockThreshold);
						cell.UpdateState(position2.y, isBlocked);
					}
				}
			}
			return;
		}
		CellMatrixData.DataAccessor accessor = data.GetAccessor();
		ISampleHeightsSimple heightSampler = GetHeightSampler();
		float max = _origin.y + _upperBoundary;
		float min = _origin.y - _lowerBoundary;
		for (int k = matrixBounds.minColumn; k <= matrixBounds.maxColumn; k++)
		{
			for (int l = matrixBounds.minRow; l <= matrixBounds.maxRow; l++)
			{
				Cell cell2 = rawMatrix[k, l];
				if (cell2 == null)
				{
					int num = l * columns + k;
					Vector3 position3 = new Vector3(_start.x + (float)k * _cellSize + _cellSize / 2f, _origin.y, _start.z + (float)l * _cellSize + _cellSize / 2f);
					position3.y = Mathf.Clamp(heightSampler.SampleHeight(position3, this), min, max);
					bool blocked2 = accessor.IsPermaBlocked(num);
					cell2 = cellFactory.Create(this, position3, k, l, blocked2);
					accessor.InjectData(cell2, num);
					rawMatrix[k, l] = cell2;
					cell2.EnsureDataForEditor();
				}
			}
		}
	}

	private void InitHeightMapForEditor(CellMatrixData data)
	{
		CellMatrixData.DataAccessor accessor = data.GetAccessor();
		_heightLookup = CreateHeightLookup(accessor.heightEntries);
		for (int i = 0; i < _heightMapSizeX; i++)
		{
			for (int j = 0; j < _heightMapSizeZ; j++)
			{
				int idx = i * _heightMapSizeZ + j;
				float height = accessor.GetHeight(idx);
				_heightLookup.Add(i, j, height);
			}
		}
		_heightLookup.Cleanup();
	}

	private void Initialize(ICellMatrixConfiguration cfg)
	{
		_cellSize = cfg.cellSize;
		_origin = cfg.origin;
		_start = cfg.origin;
		_start.x -= (float)columns * 0.5f * _cellSize;
		_start.z -= (float)rows * 0.5f * _cellSize;
		_obstacleSensitivityRange = cfg.obstacleSensitivityRange;
		_generateHeightMap = cfg.generateHeightmap && GameServices.heightStrategy.heightMode == HeightSamplingMode.HeightMap;
		_upperBoundary = cfg.upperBoundary;
		_lowerBoundary = cfg.lowerBoundary;
		_obstacleAndGroundDetection = cfg.obstacleAndGroundDetection;
		_obstacleAndGroundDetector = cfg.obstacleAndGroundDetector;
		_granularity = GameServices.heightStrategy.sampleGranularity;
		if (_generateHeightMap)
		{
			_heightMapSizeX = Mathf.RoundToInt((float)columns * cellSize / _granularity) + 1;
			_heightMapSizeZ = Mathf.RoundToInt((float)rows * cellSize / _granularity) + 1;
			_heightLookupType = cfg.heightLookupType;
			_heightLookupMaxDepth = cfg.heightLookupMaxDepth;
		}
		_bounds = cfg.bounds;
		_shortcutPortals = new List<PortalCell>();
	}

	private IEnumerator Populate()
	{
		_heightLookup = CreateHeightLookup(0);
		if (_generateHeightMap)
		{
			bool detailedHeightMap = GameServices.heightStrategy.heightMapDetail == HeightMapDetailLevel.High;
			float plotRange = _lowerBoundary + _upperBoundary;
			Vector3 down = Vector3.down;
			Vector3 samplePos = default;
			samplePos.y = _start.y + _upperBoundary;
			for (int x = 0; x < _heightMapSizeX; x++)
			{
				samplePos.x = ((!detailedHeightMap) ? (_start.x + (float)x * _granularity) : ((float)Math.Round(_start.x + (float)x * _granularity, 4)));
				for (int z = 0; z < _heightMapSizeZ; z++)
				{
					samplePos.z = ((!detailedHeightMap) ? (_start.z + (float)z * _granularity) : ((float)Math.Round(_start.z + (float)z * _granularity, 4)));
					if (UnityServices.physics.Raycast(samplePos, down, out var hit, plotRange, Layers.terrain))
					{
						_heightLookup.Add(x, z, hit.point.y);
					}
					else
					{
						_heightLookup.Add(x, z, -10000f);
					}
					yield return null;
				}
			}
			_heightLookup.Cleanup();
		}
		ISampleHeightsSimple heightSampler = GetHeightSampler();
		CellConstructionStrategy cellConstruct = GameServices.cellConstruction;
		ICellFactory cellFactory = cellConstruct.cellFactory;
		bool calculateHeights = cellConstruct.calculateHeights;
		float maxHeight = _origin.y + _upperBoundary;
		float minHeight = _origin.y - _lowerBoundary;
		float blockThreshold = Mathf.Max(_obstacleSensitivityRange, 0.01f);
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				Vector3 position = new Vector3(_start.x + (float)i * _cellSize + _cellSize / 2f, _origin.y, _start.z + (float)j * _cellSize + _cellSize / 2f);
				if (calculateHeights)
				{
					position.y = Mathf.Clamp(heightSampler.SampleHeight(position, this), minHeight, maxHeight);
				}
				bool blocked = IsBlocked(position, blockThreshold);
				rawMatrix[i, j] = cellFactory.Create(this, position, i, j, blocked);
				yield return null;
			}
		}
		if (calculateHeights)
		{
			MatrixBounds entireMatrix = new MatrixBounds(0, 0, columns - 1, rows - 1);
			IEnumerator iter = cellConstruct.heightSettingsProvider.AssignHeightSettings(this, entireMatrix);
			while (iter.MoveNext())
			{
				yield return null;
			}
		}
		if (cellConstruct.clearanceProvider != null)
		{
			IEnumerator iter2 = cellConstruct.clearanceProvider.SetClearance(this);
			while (iter2.MoveNext())
			{
				yield return null;
			}
		}
	}

	private IEnumerator Populate(CellMatrixData data)
	{
		CellMatrixData.DataAccessor accessor = data.GetAccessor();
		_heightLookup = CreateHeightLookup(accessor.heightEntries);
		for (int x = 0; x < _heightMapSizeX; x++)
		{
			for (int z = 0; z < _heightMapSizeZ; z++)
			{
				int idx = x * _heightMapSizeZ + z;
				float height = accessor.GetHeight(idx);
				_heightLookup.Add(x, z, height);
				yield return null;
			}
		}
		_heightLookup.Cleanup();
		ISampleHeightsSimple heightSampler = GetHeightSampler();
		ICellFactory cellFactory = GameServices.cellConstruction.cellFactory;
		float maxHeight = _origin.y + _upperBoundary;
		float minHeight = _origin.y - _lowerBoundary;
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				int idx2 = j * columns + i;
				Vector3 position = new Vector3(_start.x + (float)i * _cellSize + _cellSize / 2f, _origin.y, _start.z + (float)j * _cellSize + _cellSize / 2f);
				position.y = Mathf.Clamp(heightSampler.SampleHeight(position, this), minHeight, maxHeight);
				Cell cell = cellFactory.Create(blocked: accessor.IsPermaBlocked(idx2), parent: this, position: position, matrixPosX: i, matrixPosZ: j);
				accessor.InjectData(cell, idx2);
				rawMatrix[i, j] = cell;
				yield return null;
			}
		}
		CellConstructionStrategy cellConstruct = GameServices.cellConstruction;
		if (cellConstruct.clearanceProvider != null)
		{
			IEnumerator iter = cellConstruct.clearanceProvider.SetClearance(this);
			while (iter.MoveNext())
			{
				yield return null;
			}
		}
	}

	private ISampleHeightsSimple GetHeightSampler()
	{
		if (GameServices.heightStrategy.heightMode == HeightSamplingMode.HeightMap)
		{
			return this;
		}
		return GameServices.heightStrategy.heightSampler;
	}

	private IHeightLookup CreateHeightLookup(int heightEntries)
	{
		if (_heightLookupType == HeightLookupType.Dictionary)
		{
			return new HeightDictionary(_heightMapSizeX, _heightMapSizeZ, heightEntries, _origin.y);
		}
		return new HeightQuadTree(_heightMapSizeX, _heightMapSizeZ, _heightLookupMaxDepth, _origin.y);
	}

	private bool IsBlocked(Vector3 position, float blockThreshold)
	{
		position.y = _origin.y;
		switch (_obstacleAndGroundDetection)
		{
		case ColliderDetectionMode.Mixed:
		{
			Vector3 vector3 = position + Vector3.up * (_upperBoundary - blockThreshold);
			Vector3 end2 = position + Vector3.down * (_lowerBoundary - blockThreshold);
			if (UnityServices.physics.CheckCapsule(vector3, end2, blockThreshold, Layers.blocks))
			{
				return true;
			}
			float distance2 = _upperBoundary + _lowerBoundary;
			vector3 = position + Vector3.up * (_upperBoundary + 0.01f);
			Ray ray2 = new Ray(vector3, Vector3.down);
			return !UnityServices.physics.SphereCast(ray2, 0.01f, distance2, Layers.terrain);
		}
		case ColliderDetectionMode.CheckCapsule:
		{
			Vector3 vector2 = position + Vector3.up * (_upperBoundary - blockThreshold);
			Vector3 end = position + Vector3.down * (_lowerBoundary - blockThreshold);
			if (UnityServices.physics.CheckCapsule(vector2, end, blockThreshold, Layers.blocks))
			{
				return true;
			}
			vector2 = position + Vector3.up * (_upperBoundary - 0.01f);
			end = position + Vector3.down * (_lowerBoundary - 0.01f);
			return !UnityServices.physics.CheckCapsule(vector2, end, 0.01f, Layers.terrain);
		}
		case ColliderDetectionMode.Custom:
			return _obstacleAndGroundDetector.IsBlocked(this, position, blockThreshold);
		default:
		{
			float distance = _upperBoundary + _lowerBoundary;
			Vector3 vector = position + Vector3.up * (_upperBoundary + blockThreshold);
			Ray ray = new Ray(vector, Vector3.down);
			if (UnityServices.physics.SphereCast(ray, blockThreshold, distance, Layers.blocks))
			{
				return true;
			}
			vector = position + Vector3.up * (_upperBoundary + 0.01f);
			ray = new Ray(vector, Vector3.down);
			return !UnityServices.physics.SphereCast(ray, 0.01f, distance, Layers.terrain);
		}
		}
	}
}
