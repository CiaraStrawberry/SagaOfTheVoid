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

public class ProgressiveVectorField : IVectorField, ILoadBalanced
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

	private IGrid _grid;

	private IUnitProperties _unitProperties;

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

	public ProgressiveVectorField(TransientGroup<IUnitFacade> group, Path path, VectorFieldOptions options)
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
	}

	public void Initialize()
	{
		UpdateProgressiveVectorField(forceBoundsConstruction: true);
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
		float num5 = _boundsPadding + _extraPadding;
		num -= num5;
		num2 -= num5;
		num3 += num5;
		num4 += num5;
		float y = zero.y;
		Vector3 position2 = new Vector3(num, y, num2);
		Cell cell = _grid.GetCell(position2, adjustToBounds: true);
		num = cell.position.x;
		num2 = cell.position.z;
		Vector3 position3 = new Vector3(num3, y, num4);
		Cell cell2 = _grid.GetCell(position3, adjustToBounds: true);
		num3 = cell2.position.x;
		num4 = cell2.position.z;
		_groupBounds = new RectangleXZ(num, num2, num3 - num, num4 - num2);
		return true;
	}

	private void FindTemporaryDestination()
	{
		float cellSize = _grid.cellSize;
		_destination = _currentPath[_currentPathStep].position;
		while (_groupBounds.Contains(_destination) && !isOnFinalApproach)
		{
			IPositioned positioned = _currentPath[_currentPathStep + 1];
			if (positioned is IPortalNode)
			{
				break;
			}
			_destination = positioned.position;
			_currentPathStep++;
			_currentLineSegment = 0f;
			if (_announceAllNodes && group is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup)
			{
				defaultSteeringTransientUnitGroup.AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, _destination, null);
			}
		}
		while (_currentPathStep != 0)
		{
			Vector3 position = _currentPath[_currentPathStep - 1].position;
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

	private void AllocateFields()
	{
		int num = Mathf.CeilToInt(_groupBounds.size.x * _groupBounds.size.z * _expectedGroupGrowthFactor);
		if (_fastMarchedCellsSet == null || _cellDirsSet == null || _lastAllocatedSize < num)
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

	private void UpdateProgressiveVectorField(bool forceBoundsConstruction)
	{
		_lastGroupCogCell = Vector3.zero;
		if (group == null || group.count == 0 || _currentPath == null || _currentPath.count == 0)
		{
			return;
		}
		Vector3 centerOfGravity = group.centerOfGravity;
		_grid = GridManager.instance.GetGrid(centerOfGravity);
		if (_grid == null)
		{
			return;
		}
		Cell nearestWalkableCell = _grid.GetNearestWalkableCell(centerOfGravity, centerOfGravity, requireWalkableFromPosition: false, Mathf.CeilToInt((_boundsPadding + _extraPadding) / 2f), _unitProperties);
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
				UpdateProgressiveVectorField(forceBoundsConstruction: true);
			}
			return true;
		}
		if (_extraPadding >= _paddingIncrease)
		{
			_extraPadding -= _paddingIncrease;
		}
		return false;
	}

	private void StartFastMarchingMethod()
	{
		Cell cell = _grid.GetCell(_destination, adjustToBounds: true);
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
			Cell cell = _grid.GetCell(key);
			int num = 1;
			PlaneVector value = item.Value;
			if (GetCellNeighboursAndUnwalkability(cell, _tempWalkableNeighbours, !_allowDiagonals))
			{
				int count = _tempWalkableNeighbours.count;
				for (int i = 0; i < count; i++)
				{
					Cell cell2 = _tempWalkableNeighbours[i];
					Vector3 position = cell2.position;
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

	private bool GetCellNeighboursAndUnwalkability(Cell cell, DynamicArray<Cell> walkableCells, bool preventDiagonals)
	{
		if (walkableCells.count != 0)
		{
			walkableCells.Clear();
		}
		int matrixPosX = cell.matrixPosX;
		int matrixPosZ = cell.matrixPosZ;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if ((i == 0 && j == 0) || (i != 0 && j != 0 && preventDiagonals))
				{
					continue;
				}
				Cell cell2 = _grid.cellMatrix[matrixPosX + i, matrixPosZ + j];
				if (cell2 != null)
				{
					if (!cell2.IsWalkableWithClearance(_unitProperties) || !cell.IsWalkableFromWithClearance(cell2, _unitProperties))
					{
						return false;
					}
					walkableCells.Add(cell2);
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
		int matrixPosX = cell.matrixPosX;
		int matrixPosZ = cell.matrixPosZ;
		Cell cell2 = _grid.cellMatrix[matrixPosX - 1, matrixPosZ];
		Cell cell3 = _grid.cellMatrix[matrixPosX, matrixPosZ - 1];
		Cell cell4 = _grid.cellMatrix[matrixPosX + 1, matrixPosZ];
		Cell cell5 = _grid.cellMatrix[matrixPosX, matrixPosZ + 1];
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (cell2 != null)
		{
			flag = _fastMarchedCellsSet.ContainsKey(cell2.position);
			if (!flag && cell2.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell2, _unitProperties))
			{
				walkableCells.Add(cell2);
				flag = true;
			}
		}
		if (cell3 != null)
		{
			flag4 = _fastMarchedCellsSet.ContainsKey(cell3.position);
			if (!flag4 && cell3.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell3, _unitProperties))
			{
				walkableCells.Add(cell3);
				flag4 = true;
			}
		}
		if (cell4 != null)
		{
			flag2 = _fastMarchedCellsSet.ContainsKey(cell4.position);
			if (!flag2 && cell4.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell4, _unitProperties))
			{
				walkableCells.Add(cell4);
				flag2 = true;
			}
		}
		if (cell5 != null)
		{
			flag3 = _fastMarchedCellsSet.ContainsKey(cell5.position);
			if (!flag3 && cell5.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell5, _unitProperties))
			{
				walkableCells.Add(cell5);
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
			Cell cell6 = _grid.cellMatrix[matrixPosX - 1, matrixPosZ - 1];
			if (cell6 != null && !_fastMarchedCellsSet.ContainsKey(cell6.position) && cell6.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell6, _unitProperties))
			{
				walkableCells.Add(cell6);
			}
		}
		if (flag8)
		{
			Cell cell7 = _grid.cellMatrix[matrixPosX - 1, matrixPosZ + 1];
			if (cell7 != null && !_fastMarchedCellsSet.ContainsKey(cell7.position) && cell7.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell7, _unitProperties))
			{
				walkableCells.Add(cell7);
			}
		}
		if (flag6)
		{
			Cell cell8 = _grid.cellMatrix[matrixPosX + 1, matrixPosZ - 1];
			if (cell8 != null && !_fastMarchedCellsSet.ContainsKey(cell8.position) && cell8.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell8, _unitProperties))
			{
				walkableCells.Add(cell8);
			}
		}
		if (flag5)
		{
			Cell cell9 = _grid.cellMatrix[matrixPosX + 1, matrixPosZ + 1];
			if (cell9 != null && !_fastMarchedCellsSet.ContainsKey(cell9.position) && cell9.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell9, _unitProperties))
			{
				walkableCells.Add(cell9);
			}
		}
	}

	private PlaneVector GetFastMarchedCellAtPos(Vector3 pos)
	{
		_fastMarchedCellsSet.TryGetValue(pos, out var value);
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
		UpdateProgressiveVectorField(forceBoundsConstruction: false);
		return null;
	}

	public void DebugRender()
	{
		if (_grid == null)
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
			Gizmos.DrawWireCube(_lastGroupCogCell, new Vector3(_grid.cellSize, 1f, _grid.cellSize));
		}
	}
}
