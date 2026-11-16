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

public class FunnelVectorField : IVectorField, ILoadBalanced
{
	private float _funnelWidth;

	private float _obstacleStrengthFactor;

	private bool _allowCornerCutting;

	private bool _allowDiagonals;

	private bool _builtInContainment;

	private bool _announceAllNodes;

	private float _updateInterval;

	private bool _onFinalApproach;

	private Path _currentPath;

	private Vector3 _destination;

	private IGrid _grid;

	private IUnitProperties _unitProperties;

	private float _funnelWidthSqr;

	private Dictionary<Vector3, VectorFieldCell> _cellDirsSet;

	private Dictionary<Vector3, PlaneVector> _fastMarchedCellsSet;

	private DynamicArray<Cell> _tempWalkableNeighbours;

	private DynamicArray<Cell> _extraTempWalkableNeighbours;

	private SimpleQueue<Cell> _openSet;

	private int _currentPathStep;

	public bool repeat => group.count > 0;

	public TransientGroup<IUnitFacade> group { get; private set; }

	public Path currentPath => _currentPath;

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
			if (_currentPath == null || _currentPath.count == 0)
			{
				return null;
			}
			return _currentPath.Peek();
		}
	}

	public bool isOnFinalApproach => _onFinalApproach;

	public FunnelVectorField(TransientGroup<IUnitFacade> group, Path path, VectorFieldOptions options)
	{
		Ensure.ArgumentNotNull(group, "group");
		this.group = group;
		_currentPath = path;
		IUnitFacade unitFacade = (IUnitFacade)(_unitProperties = group.modelUnit);
		IPathFinderOptions pathFinderOptions = unitFacade.pathFinderOptions;
		_funnelWidth = options.funnelWidth;
		_obstacleStrengthFactor = options.obstacleStrengthFactor;
		_allowCornerCutting = pathFinderOptions.allowCornerCutting;
		_allowDiagonals = !pathFinderOptions.preventDiagonalMoves;
		_announceAllNodes = unitFacade.pathNavigationOptions.announceAllNodes;
		_builtInContainment = options.builtInContainment;
		_updateInterval = options.updateInterval;
		_openSet = new SimpleQueue<Cell>(31);
		_tempWalkableNeighbours = new DynamicArray<Cell>(8);
		_extraTempWalkableNeighbours = new DynamicArray<Cell>(8);
		_grid = GridManager.instance.GetGrid(group.modelUnit.position);
		if (_grid != null)
		{
			int capacity = Mathf.CeilToInt((float)(_grid.sizeX * _grid.sizeZ) / 2f);
			float equalityThreshold = 9.9999994E-11f;
			_cellDirsSet = new Dictionary<Vector3, VectorFieldCell>(capacity, new Vector3EqualityComparer(equalityThreshold));
			_fastMarchedCellsSet = new Dictionary<Vector3, PlaneVector>(capacity, new Vector3EqualityComparer(equalityThreshold));
		}
		_funnelWidthSqr = _funnelWidth * _funnelWidth;
	}

	public void Initialize()
	{
		GenerateFunnelVectorField();
		NavLoadBalancer.steering.Add(this, _updateInterval, delayFirstUpdate: true);
	}

	private void ResetField()
	{
		_fastMarchedCellsSet.Clear();
		_cellDirsSet.Clear();
	}

	private void GenerateFunnelVectorField()
	{
		if (_grid != null && _currentPath != null && _currentPath.count != 0)
		{
			StartFastMarchingMethod();
			SmoothFields();
			HandlePortals();
		}
	}

	private void StartFastMarchingMethod()
	{
		_destination = _currentPath.Last().position;
		_currentPathStep = _currentPath.count - 1;
		int num = _currentPath.count - 1;
		for (int i = 1; i < num; i++)
		{
			IPositioned positioned = _currentPath[i];
			if (positioned is IPortalNode)
			{
				_destination = _currentPath[i - 1].position;
				_currentPathStep = i;
				break;
			}
		}
		Cell cell = _grid.GetCell(_destination, adjustToBounds: true);
		_openSet.Enqueue(cell);
		while (_openSet.count > 0)
		{
			FunnelMethod(_openSet.Dequeue(), cell);
		}
	}

	private void FunnelMethod(Cell currentCell, Cell destinationCell)
	{
		Vector3 position = currentCell.position;
		Vector3 position2 = _currentPath[_currentPathStep].position;
		Vector3 vector = ((_currentPathStep <= 0) ? group.centerOfGravity : _currentPath[_currentPathStep - 1].position);
		GetWalkableCellNeighbours(currentCell, _tempWalkableNeighbours, !_allowDiagonals);
		int count = _tempWalkableNeighbours.count;
		for (int i = 0; i < count; i++)
		{
			Cell cell = _tempWalkableNeighbours[i];
			Vector3 position3 = cell.position;
			Vector3 zero = Vector3.zero;
			if (GetCellNeighboursAndUnwalkability(cell, _extraTempWalkableNeighbours, !_allowDiagonals) && (position3 - destinationCell.position).sqrMagnitude > _funnelWidthSqr)
			{
				Vector3 normalized = position3.DirToXZ(position).normalized;
				Vector3 normalized2 = position3.DirToXZ(position2).normalized;
				Vector3 normalized3 = vector.DirToXZ(position2).normalized;
				zero = (normalized + normalized2 + normalized3) / 3f;
			}
			else
			{
				zero = position3.DirToXZ(position).normalized * _obstacleStrengthFactor;
			}
			_fastMarchedCellsSet[position3] = new PlaneVector(zero);
			Vector3 closestPositionOnLine = GetClosestPositionOnLine(vector, position2, position3);
			if (!((closestPositionOnLine - position3).sqrMagnitude > _funnelWidthSqr))
			{
				_openSet.Enqueue(cell);
			}
		}
		if (_currentPathStep > 0 && (vector - position).sqrMagnitude < _grid.cellSize * _grid.cellSize && !(_currentPath[_currentPathStep - 1] is IPortalNode))
		{
			_currentPathStep--;
		}
	}

	private void SmoothFields()
	{
		foreach (KeyValuePair<Vector3, PlaneVector> item in _fastMarchedCellsSet)
		{
			Vector3 key = item.Key;
			Cell cell = _grid.GetCell(key);
			int num = 1;
			Vector3 vector = item.Value;
			if (GetCellNeighboursAndUnwalkability(cell, _tempWalkableNeighbours, !_allowDiagonals))
			{
				int count = _tempWalkableNeighbours.count;
				for (int i = 0; i < count; i++)
				{
					Cell cell2 = _tempWalkableNeighbours[i];
					Vector3 position = cell2.position;
					Vector3 vector2 = GetFastMarchedCellAtPos(position);
					if (vector2.sqrMagnitude != 0f)
					{
						num++;
						vector += vector2;
					}
				}
				vector /= (float)num;
			}
			_cellDirsSet[key] = new VectorFieldCell(vector);
		}
	}

	private void HandlePortals()
	{
		int num = _currentPath.count - 1;
		for (int i = 1; i < num; i++)
		{
			if (!(_currentPath[i] is PortalCell portalCell) || !_grid.Contains(portalCell.position))
			{
				continue;
			}
			int num2 = portalCell.neighbourNodes.Length;
			for (int j = 0; j < num2; j++)
			{
				IPathNode pathNode = portalCell.neighbourNodes[j];
				Vector3 position = pathNode.position;
				if (_cellDirsSet.TryGetValue(position, out var value))
				{
					_cellDirsSet[position] = new VectorFieldCell(value.direction, i);
				}
			}
		}
	}

	private Vector3 GetClosestPositionOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 position)
	{
		Vector3 vector = lineEnd - lineStart;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude == 0f)
		{
			return lineEnd;
		}
		float num = Vector3.Dot(position - lineStart, vector);
		float num2 = num / sqrMagnitude;
		if (num2 < 0f)
		{
			return lineStart;
		}
		if (num2 > 1f)
		{
			return lineEnd;
		}
		return lineStart + vector * num2;
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
		if (_fastMarchedCellsSet == null)
		{
			return default;
		}
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
		_grid = GridManager.instance.GetGrid(group.modelUnit.position);
		if (_grid != null)
		{
			ResetField();
			_currentPath = path;
			_currentPathStep = 0;
			_onFinalApproach = false;
			GenerateFunnelVectorField();
		}
	}

	private void PopPathNode()
	{
		IPositioned positioned = _currentPath.Pop();
		HandlePortals();
		if (_announceAllNodes && group is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup)
		{
			defaultSteeringTransientUnitGroup.AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, positioned.position, null);
		}
	}

	public float? ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (group == null || group.count == 0)
		{
			return null;
		}
		if (!_onFinalApproach && _currentPath != null && _currentPath.count > 0)
		{
			IUnitFacade modelUnit = group.modelUnit;
			if (modelUnit == null || modelUnit.transform == null)
			{
				return null;
			}
			Vector3 centerOfGravity = group.centerOfGravity;
			float sqrMagnitude = (centerOfGravity - _destination).sqrMagnitude;
			float sqrMagnitude2 = (modelUnit.position - _destination).sqrMagnitude;
			float num = modelUnit.radius * 2f;
			float num2 = num * num;
			if (sqrMagnitude <= num2 || sqrMagnitude2 <= num2)
			{
				_onFinalApproach = true;
				while (_currentPath.count > 1)
				{
					_currentPath.Pop();
				}
				return null;
			}
			int num3 = _currentPath.count - 1;
			for (int i = 0; i < num3 && !(_currentPath[i] is IPortalNode) && !(_currentPath[i + 1] is IPortalNode); i++)
			{
				Vector3 position = _currentPath[i].position;
				float sqrMagnitude3 = (centerOfGravity - position).sqrMagnitude;
				float sqrMagnitude4 = (modelUnit.position - position).sqrMagnitude;
				if (sqrMagnitude3 <= num2 || sqrMagnitude3 > sqrMagnitude || sqrMagnitude4 <= num2)
				{
					PopPathNode();
					break;
				}
			}
			_onFinalApproach = _currentPath.count <= 1;
		}
		return null;
	}

	public void DebugRender()
	{
		if (_grid == null)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0.01f, 0f);
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
	}
}
