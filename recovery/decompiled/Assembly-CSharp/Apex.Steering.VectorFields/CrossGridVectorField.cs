using System.Collections.Generic;
using Apex.DataStructures;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.VectorFields;

public class CrossGridVectorField : IVectorField, ILoadBalanced
{
	private float _obstacleStrengthFactor;

	private bool _allowCornerCutting;

	private bool _allowDiagonals;

	private bool _builtInContainment;

	private bool _announceAllNodes;

	private float _expectedGroupGrowthFactor;

	private float _updateInterval;

	private float _paddingIncrease;

	private float _maxExtraPadding;

	private float _boundsPadding;

	private float _boundsRecalculateThreshold;

	private DynamicArray<Cell> _tempWalkableNeighbours;

	private DynamicArray<Cell> _extraTempWalkableNeighbours;

	private SimpleQueue<Cell> _openSet;

	private Path _currentPath;

	private Vector3 _destination;

	private IList<IGrid> _grids;

	private IUnitProperties _unitProperties;

	private float _portalNewGroupThreshold;

	private int _currentPathStep;

	private float _currentLineSegment;

	private float _extraPadding;

	private RectangleXZ _groupBounds;

	private Vector3 _lastGroupCogCell;

	private int _lastAllocatedSize;

	private Dictionary<Vector3, PlaneVector> _fastMarchedCellsSet;

	private Dictionary<Vector3, VectorFieldCell> _cellDirsSet;

	public TransientGroup<IUnitFacade> group { get; private set; }

	public Path currentPath => _currentPath;

	public bool repeat => group.count > 0;

	public IPositioned destination
	{
		get
		{
			if (_currentPath == null || _currentPath.count == 0)
			{
				return null;
			}
			return _currentPath.Last();
		}
	}

	public IPositioned nextNodePosition
	{
		get
		{
			if (_currentPath == null || _currentPathStep >= _currentPath.count)
			{
				return null;
			}
			return _currentPath[_currentPathStep];
		}
	}

	public bool isOnFinalApproach => _currentPathStep + 1 >= _currentPath.count;

	public CrossGridVectorField(TransientGroup<IUnitFacade> group, Path path, VectorFieldOptions options)
	{
		Ensure.ArgumentNotNull(group, "group");
		this.group = group;
		_currentPath = path;
		IUnitFacade unitFacade = (IUnitFacade)(_unitProperties = group.modelUnit);
		IPathFinderOptions pathFinderOptions = unitFacade.pathFinderOptions;
		_allowCornerCutting = pathFinderOptions.allowCornerCutting;
		_allowDiagonals = !pathFinderOptions.preventDiagonalMoves;
		_announceAllNodes = unitFacade.pathNavigationOptions.announceAllNodes;
		_obstacleStrengthFactor = options.obstacleStrengthFactor;
		_builtInContainment = options.builtInContainment;
		_updateInterval = options.updateInterval;
		_paddingIncrease = options.paddingIncrease;
		_maxExtraPadding = options.maxExtraPadding;
		_boundsPadding = options.boundsPadding;
		_boundsRecalculateThreshold = options.boundsRecalculateThreshold;
		_expectedGroupGrowthFactor = 1f + options.expectedGroupGrowthFactor;
		_openSet = new SimpleQueue<Cell>(31);
		_tempWalkableNeighbours = new DynamicArray<Cell>(8);
		_extraTempWalkableNeighbours = new DynamicArray<Cell>(8);
		_groupBounds = new RectangleXZ(group.centerOfGravity, _boundsPadding, _boundsPadding);
		_grids = new List<IGrid>(4);
	}

	public void Initialize()
	{
		UpdateCrossGridVectorField(forceBoundsConstruction: true);
		NavLoadBalancer.steering.Add(this, _updateInterval, delayFirstUpdate: true);
	}

	private bool ConstructGroupBounds(bool forceConstruction = false)
	{
		int count = group.count;
		if (count == 0)
		{
			return false;
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = float.MinValue;
		float num4 = float.MinValue;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			IUnitFacade unitFacade = group[i];
			if (unitFacade != null)
			{
				Vector3 position = unitFacade.position;
				if (position.x < num)
				{
					num = position.x;
				}
				if (position.z < num2)
				{
					num2 = position.z;
				}
				if (position.x > num3)
				{
					num3 = position.x;
				}
				if (position.z > num4)
				{
					num4 = position.z;
				}
				zero += position;
			}
		}
		zero /= (float)count;
		if (!forceConstruction && (zero - _groupBounds.center).sqrMagnitude < _boundsRecalculateThreshold * _boundsRecalculateThreshold)
		{
			return false;
		}
		IGrid grid = GridManager.instance.GetGrid(zero);
		if (grid == null)
		{
			return false;
		}
		if (_portalNewGroupThreshold <= 0f)
		{
			_portalNewGroupThreshold = grid.cellSize * 1.414214f + 0.1f;
		}
		_grids.Clear();
		AddGrid(grid);
		float num5 = _boundsPadding + _extraPadding;
		num -= num5;
		num2 -= num5;
		num3 += num5;
		num4 += num5;
		float y = zero.y;
		Vector3 vector = new Vector3(num, y, num2);
		IGrid grid2 = GridManager.instance.GetGrid(vector);
		if (grid2 != null)
		{
			AddGrid(grid2);
			Cell cell = grid2.GetCell(vector, adjustToBounds: true);
			num = cell.position.x;
			num2 = cell.position.z;
		}
		Vector3 vector2 = new Vector3(num3, y, num4);
		IGrid grid3 = GridManager.instance.GetGrid(vector2);
		if (grid3 != null)
		{
			AddGrid(grid3);
			Cell cell2 = grid3.GetCell(vector2, adjustToBounds: true);
			num3 = cell2.position.x;
			num4 = cell2.position.z;
		}
		IGrid grid4 = GridManager.instance.GetGrid(new Vector3(num, y, num4));
		AddGrid(grid4);
		IGrid grid5 = GridManager.instance.GetGrid(new Vector3(num3, y, num2));
		AddGrid(grid5);
		_groupBounds = new RectangleXZ(num, num2, num3 - num, num4 - num2);
		return true;
	}

	private void FindTemporaryDestination()
	{
		bool flag = false;
		float cellSize = _grids[0].cellSize;
		float num = _portalNewGroupThreshold * _portalNewGroupThreshold;
		_destination = _currentPath[_currentPathStep].position;
		while (_groupBounds.Contains(_destination) && !isOnFinalApproach)
		{
			IPositioned positioned = _currentPath[_currentPathStep + 1];
			if (positioned is IPortalNode portalNode)
			{
				IPositioned positioned2 = _currentPath[_currentPathStep + 2];
				float sqrMagnitude = (portalNode.position - portalNode.partner.position).sqrMagnitude;
				if (sqrMagnitude > num)
				{
					flag = true;
					break;
				}
				_currentPathStep++;
				positioned = positioned2;
			}
			if (_announceAllNodes && group is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup)
			{
				defaultSteeringTransientUnitGroup.AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, _destination, null);
			}
			_currentPathStep++;
			_destination = positioned.position;
			_currentLineSegment = 0f;
		}
		if (!flag)
		{
			while (_currentPathStep != 0)
			{
				int num2 = _currentPathStep - 1;
				if (_currentPath[num2] is IPortalNode)
				{
					num2--;
				}
				Vector3 position = _currentPath[num2].position;
				Vector3 position2 = _currentPath[_currentPathStep].position;
				Vector3 vector = position2 - position;
				if (vector.sqrMagnitude == 0f)
				{
					break;
				}
				Vector3 vector2 = position + vector.normalized * _currentLineSegment;
				if ((vector2 - position).sqrMagnitude > vector.sqrMagnitude || !_groupBounds.Contains(vector2))
				{
					break;
				}
				_destination = vector2;
				_currentLineSegment += cellSize;
			}
		}
		AddGrid(GridManager.instance.GetGrid(_destination));
	}

	private void AllocateFields()
	{
		int num = Mathf.CeilToInt(_groupBounds.size.x * _groupBounds.size.z * _expectedGroupGrowthFactor);
		if (_fastMarchedCellsSet == null || _cellDirsSet == null || num > _lastAllocatedSize)
		{
			_lastAllocatedSize = num;
			float equalityThreshold = 9.9999994E-11f;
			_fastMarchedCellsSet = new Dictionary<Vector3, PlaneVector>(num, new Vector3EqualityComparer(equalityThreshold));
			_cellDirsSet = new Dictionary<Vector3, VectorFieldCell>(num, new Vector3EqualityComparer(equalityThreshold));
		}
		else
		{
			_fastMarchedCellsSet.Clear();
			_cellDirsSet.Clear();
		}
	}

	private void UpdateCrossGridVectorField(bool forceBoundsConstruction)
	{
		_lastGroupCogCell = Vector3.zero;
		if (group == null || group.count == 0 || _currentPath == null || _currentPath.count == 0)
		{
			return;
		}
		Vector3 centerOfGravity = group.centerOfGravity;
		IGrid grid = GridManager.instance.GetGrid(centerOfGravity);
		if (grid == null)
		{
			return;
		}
		Cell nearestWalkableCell = grid.GetNearestWalkableCell(centerOfGravity, centerOfGravity, requireWalkableFromPosition: false, Mathf.CeilToInt((_boundsPadding + _extraPadding) / 2f), _unitProperties);
		if (nearestWalkableCell == null)
		{
			return;
		}
		_lastGroupCogCell = nearestWalkableCell.position;
		if (ConstructGroupBounds(forceBoundsConstruction))
		{
			FindTemporaryDestination();
			AllocateFields();
			StartFastMarchingMethod();
			if (!GrowOrShrinkBounds())
			{
				SmoothFields();
				HandlePortals();
			}
		}
	}

	private bool GrowOrShrinkBounds()
	{
		if (_lastGroupCogCell.sqrMagnitude == 0f)
		{
			return false;
		}
		if (GetFastMarchedCellAtPos(_lastGroupCogCell).sqrMagnitude == 0f)
		{
			if (_extraPadding + _paddingIncrease < _maxExtraPadding)
			{
				_extraPadding += _paddingIncrease;
				UpdateCrossGridVectorField(forceBoundsConstruction: true);
			}
			return true;
		}
		if (_extraPadding >= _paddingIncrease)
		{
			_extraPadding -= _paddingIncrease;
		}
		return false;
	}

	private Cell EnsureDestinationCell()
	{
		Cell cell = GetCellAtPos(_destination);
		if (cell == null)
		{
			IGrid gridFromPosition = GetGridFromPosition(_destination);
			if (gridFromPosition != null)
			{
				cell = gridFromPosition.GetNearestWalkableCell(_destination, _destination, requireWalkableFromPosition: false, Mathf.CeilToInt(_boundsPadding + _extraPadding), _unitProperties);
			}
		}
		return cell;
	}

	private void StartFastMarchingMethod()
	{
		Cell cell = EnsureDestinationCell();
		if (cell != null)
		{
			_destination = cell.position;
			_openSet.Enqueue(cell);
			while (_openSet.count > 0)
			{
				FastMarchingMethod(_openSet.Dequeue());
			}
		}
	}

	private void FastMarchingMethod(Cell currentCell)
	{
		Vector3 position = currentCell.position;
		GetWalkableCellNeighbours(currentCell, _tempWalkableNeighbours, !_allowDiagonals);
		int count = _tempWalkableNeighbours.count;
		for (int i = 0; i < count; i++)
		{
			Cell cell = _tempWalkableNeighbours[i];
			Vector3 position2 = cell.position;
			float num = ((!GetCellNeighboursAndUnwalkability(cell, _extraTempWalkableNeighbours, !_allowDiagonals)) ? _obstacleStrengthFactor : 1f);
			_fastMarchedCellsSet[position2] = position2.DirToXZ(position) * num;
			if (_groupBounds.Contains(position2))
			{
				_openSet.Enqueue(cell);
			}
		}
	}

	private void SmoothFields()
	{
		foreach (KeyValuePair<Vector3, PlaneVector> item in _fastMarchedCellsSet)
		{
			Vector3 key = item.Key;
			Cell cellAtPos = GetCellAtPos(key);
			int num = 1;
			PlaneVector value = item.Value;
			if (GetCellNeighboursAndUnwalkability(cellAtPos, _tempWalkableNeighbours, !_allowDiagonals))
			{
				int count = _tempWalkableNeighbours.count;
				for (int i = 0; i < count; i++)
				{
					Cell cell = _tempWalkableNeighbours[i];
					Vector3 position = cell.position;
					PlaneVector fastMarchedCellAtPos = GetFastMarchedCellAtPos(position);
					if (fastMarchedCellAtPos.sqrMagnitude != 0f)
					{
						num++;
						value += fastMarchedCellAtPos;
					}
				}
				value /= (float)num;
			}
			_cellDirsSet[key] = new VectorFieldCell(value);
		}
	}

	private void HandlePortals()
	{
		int num = _currentPath.count - 1;
		for (int i = 1; i < num; i++)
		{
			if (!(_currentPath[i] is PortalCell portalCell))
			{
				continue;
			}
			int num2 = portalCell.neighbourNodes.Length;
			for (int j = 0; j < num2; j++)
			{
				IPathNode pathNode = portalCell.neighbourNodes[j];
				Vector3 position = pathNode.position;
				if (_groupBounds.Contains(position) && _cellDirsSet.TryGetValue(position, out var value))
				{
					_cellDirsSet[position] = new VectorFieldCell(value.direction, i);
				}
			}
		}
	}

	private Cell GetCellNeighbour(Cell cell, int dx, int dz)
	{
		int num = cell.matrixPosX + dx;
		int num2 = cell.matrixPosZ + dz;
		CellMatrix parent = cell.parent;
		if (num >= 0 && num < parent.columns && num2 >= 0 && num2 < parent.rows)
		{
			return parent[num, num2];
		}
		float cellSize = parent.cellSize;
		Vector3 position = cell.position + new Vector3((float)dx * cellSize, 0f, (float)dz * cellSize);
		return GetCellAtPos(position);
	}

	private bool GetCellNeighboursAndUnwalkability(Cell cell, DynamicArray<Cell> walkableCells, bool preventDiagonals)
	{
		if (walkableCells.count != 0)
		{
			walkableCells.Clear();
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if ((i == 0 && j == 0) || (i != 0 && j != 0 && preventDiagonals))
				{
					continue;
				}
				Cell cellNeighbour = GetCellNeighbour(cell, i, j);
				if (cellNeighbour != null)
				{
					if (!cellNeighbour.IsWalkableWithClearance(_unitProperties) || !cell.IsWalkableFromWithClearance(cellNeighbour, _unitProperties))
					{
						return false;
					}
					walkableCells.Add(cellNeighbour);
				}
				else if (_builtInContainment)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void GetWalkableCellNeighbours(Cell cell, DynamicArray<Cell> walkableCells, bool preventDiagonals)
	{
		if (walkableCells.count != 0)
		{
			walkableCells.Clear();
		}
		Cell cellNeighbour = GetCellNeighbour(cell, -1, 0);
		Cell cellNeighbour2 = GetCellNeighbour(cell, 0, -1);
		Cell cellNeighbour3 = GetCellNeighbour(cell, 1, 0);
		Cell cellNeighbour4 = GetCellNeighbour(cell, 0, 1);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (cellNeighbour != null)
		{
			flag = _fastMarchedCellsSet.ContainsKey(cellNeighbour.position);
			if (!flag && cellNeighbour.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour, _unitProperties))
			{
				walkableCells.Add(cellNeighbour);
				flag = true;
			}
		}
		if (cellNeighbour2 != null)
		{
			flag4 = _fastMarchedCellsSet.ContainsKey(cellNeighbour2.position);
			if (!flag4 && cellNeighbour2.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour2, _unitProperties))
			{
				walkableCells.Add(cellNeighbour2);
				flag4 = true;
			}
		}
		if (cellNeighbour3 != null)
		{
			flag2 = _fastMarchedCellsSet.ContainsKey(cellNeighbour3.position);
			if (!flag2 && cellNeighbour3.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour3, _unitProperties))
			{
				walkableCells.Add(cellNeighbour3);
				flag2 = true;
			}
		}
		if (cellNeighbour4 != null)
		{
			flag3 = _fastMarchedCellsSet.ContainsKey(cellNeighbour4.position);
			if (!flag3 && cellNeighbour4.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour4, _unitProperties))
			{
				walkableCells.Add(cellNeighbour4);
				flag3 = true;
			}
		}
		if (preventDiagonals)
		{
			return;
		}
		bool flag5;
		bool flag6;
		bool flag7;
		bool flag8;
		if (_allowCornerCutting)
		{
			flag5 = flag3 || flag2;
			flag6 = flag4 || flag2;
			flag7 = flag4 || flag;
			flag8 = flag3 || flag;
		}
		else
		{
			flag5 = flag3 && flag2;
			flag6 = flag4 && flag2;
			flag7 = flag4 && flag;
			flag8 = flag3 && flag;
		}
		if (flag7)
		{
			Cell cellNeighbour5 = GetCellNeighbour(cell, -1, -1);
			if (cellNeighbour5 != null && !_fastMarchedCellsSet.ContainsKey(cellNeighbour5.position) && cellNeighbour5.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour5, _unitProperties))
			{
				walkableCells.Add(cellNeighbour5);
			}
		}
		if (flag8)
		{
			Cell cellNeighbour6 = GetCellNeighbour(cell, -1, 1);
			if (cellNeighbour6 != null && !_fastMarchedCellsSet.ContainsKey(cellNeighbour6.position) && cellNeighbour6.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour6, _unitProperties))
			{
				walkableCells.Add(cellNeighbour6);
			}
		}
		if (flag6)
		{
			Cell cellNeighbour7 = GetCellNeighbour(cell, 1, -1);
			if (cellNeighbour7 != null && !_fastMarchedCellsSet.ContainsKey(cellNeighbour7.position) && cellNeighbour7.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour7, _unitProperties))
			{
				walkableCells.Add(cellNeighbour7);
			}
		}
		if (flag5)
		{
			Cell cellNeighbour8 = GetCellNeighbour(cell, 1, 1);
			if (cellNeighbour8 != null && !_fastMarchedCellsSet.ContainsKey(cellNeighbour8.position) && cellNeighbour8.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cellNeighbour8, _unitProperties))
			{
				walkableCells.Add(cellNeighbour8);
			}
		}
	}

	private PlaneVector GetFastMarchedCellAtPos(Vector3 position)
	{
		if (_fastMarchedCellsSet == null)
		{
			return default;
		}
		_fastMarchedCellsSet.TryGetValue(position, out var value);
		return value;
	}

	public VectorFieldCell GetFieldCellAtPos(IGridCell cell)
	{
		Ensure.ArgumentNotNull(cell, "cell");
		if (_cellDirsSet == null)
		{
			return default;
		}
		_cellDirsSet.TryGetValue(cell.position, out var value);
		return value;
	}

	private Cell GetCellAtPos(Vector3 position)
	{
		return GetGridFromPosition(position)?.GetCell(position);
	}

	private void AddGrid(IGrid grid)
	{
		if (grid != null && !_grids.Contains(grid))
		{
			_grids.Add(grid);
		}
	}

	private IGrid GetGridFromPosition(Vector3 position)
	{
		int count = _grids.Count;
		for (int i = 0; i < count; i++)
		{
			if (_grids[i].Contains(position))
			{
				return _grids[i];
			}
		}
		return null;
	}

	public void SetNewPath(Path path)
	{
		if (path == null || path.count == 0)
		{
			Debug.LogWarning("Cannot SetNewPath on VectorField, since Path is null or path count is 0");
			return;
		}
		_currentPath = path;
		_currentPathStep = 0;
		_currentLineSegment = 0f;
	}

	public float? ExecuteUpdate(float deltaTime, float nextInterval)
	{
		UpdateCrossGridVectorField(forceBoundsConstruction: false);
		return null;
	}

	public void DebugRender()
	{
		if (_grids == null || _grids.Count == 0)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0.5f, 0f);
		foreach (KeyValuePair<Vector3, VectorFieldCell> item in _cellDirsSet)
		{
			Vector3 vector2 = item.Key + vector;
			VectorFieldCell value = item.Value;
			if (value.pathPortalIndex == -1)
			{
				Gizmos.color = Color.white;
			}
			else
			{
				Gizmos.color = Color.blue;
			}
			Gizmos.DrawSphere(vector2, 0.25f);
			Vector3 vector3 = value.direction.normalized;
			Gizmos.DrawLine(vector2, vector2 + vector3);
		}
		if (_destination.sqrMagnitude != 0f)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(_destination, 0.25f);
		}
		if (_groupBounds.center.sqrMagnitude != 0f)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(_groupBounds.center, _groupBounds.size);
		}
		if (_lastGroupCogCell.sqrMagnitude != 0f)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(_lastGroupCogCell, 1f);
		}
	}
}
