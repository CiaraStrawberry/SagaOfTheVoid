using Apex.DataStructures;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.VectorFields;

public class FullGridVectorField : IVectorField, ILoadBalanced
{
	private float _obstacleStrengthFactor;

	private bool _allowCornerCutting;

	private bool _allowDiagonals;

	private bool _announceAllNodes;

	private bool _builtInContainment;

	private float _updateInterval;

	private bool _onFinalApproach;

	private IUnitProperties _unitProperties;

	private Path _currentPath;

	private Vector3 _destination;

	private IGrid _grid;

	private PlaneVector[,] _fastMarchedCells;

	private VectorFieldCell[,] _cellDirs;

	private DynamicArray<Cell> _tempWalkableNeighbours;

	private DynamicArray<Cell> _extraTempWalkableNeighbours;

	private SimpleQueue<Cell> _openSet;

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

	public bool repeat => group.count > 0;

	public FullGridVectorField(TransientGroup<IUnitFacade> group, Path path, VectorFieldOptions options)
	{
		Ensure.ArgumentNotNull(group, "group");
		this.group = group;
		_currentPath = path;
		IUnitFacade unitFacade = (IUnitFacade)(_unitProperties = group.modelUnit);
		IPathFinderOptions pathFinderOptions = unitFacade.pathFinderOptions;
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
			_fastMarchedCells = new PlaneVector[_grid.sizeX, _grid.sizeZ];
			_cellDirs = new VectorFieldCell[_grid.sizeX, _grid.sizeZ];
		}
	}

	public void Initialize()
	{
		GenerateVectorField();
		NavLoadBalancer.steering.Add(this, _updateInterval, delayFirstUpdate: true);
	}

	private void ResetField()
	{
		PlaneVector zero = PlaneVector.zero;
		int sizeX = _grid.sizeX;
		int sizeZ = _grid.sizeZ;
		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeZ; j++)
			{
				_fastMarchedCells[i, j] = zero;
			}
		}
	}

	private void GenerateVectorField()
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
		int num = _currentPath.count - 1;
		for (int i = 1; i < num; i++)
		{
			IPositioned positioned = _currentPath[i];
			if (positioned is IPortalNode)
			{
				_destination = _currentPath[i - 1].position;
				break;
			}
		}
		Cell cell = _grid.GetCell(_destination, adjustToBounds: true);
		_openSet.Enqueue(cell);
		while (_openSet.count > 0)
		{
			FastMarchingMethod(_openSet.Dequeue());
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
			int matrixPosX = cell.matrixPosX;
			int matrixPosZ = cell.matrixPosZ;
			if (_fastMarchedCells[matrixPosX, matrixPosZ].sqrMagnitude == 0f)
			{
				float num = ((!GetCellNeighboursAndUnwalkability(cell, _extraTempWalkableNeighbours, !_allowDiagonals)) ? _obstacleStrengthFactor : 1f);
				Vector3 position2 = cell.position;
				ref PlaneVector reference = ref _fastMarchedCells[matrixPosX, matrixPosZ];
				reference = position2.DirToXZ(position) * num;
				_openSet.Enqueue(cell);
			}
		}
	}

	private void SmoothFields()
	{
		PlaneVector zero = PlaneVector.zero;
		int sizeX = _grid.sizeX;
		int sizeZ = _grid.sizeZ;
		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeZ; j++)
			{
				PlaneVector planeVector = _fastMarchedCells[i, j];
				if (planeVector.sqrMagnitude == 0f)
				{
					_cellDirs[i, j].direction = zero;
					_cellDirs[i, j].pathPortalIndex = -1;
					continue;
				}
				int num = 1;
				PlaneVector direction = planeVector;
				Cell cell = _grid.cellMatrix[i, j];
				if (GetCellNeighboursAndUnwalkability(cell, _tempWalkableNeighbours, !_allowDiagonals))
				{
					int count = _tempWalkableNeighbours.count;
					for (int k = 0; k < count; k++)
					{
						Cell cell2 = _tempWalkableNeighbours[k];
						PlaneVector planeVector2 = _fastMarchedCells[cell2.matrixPosX, cell2.matrixPosZ];
						if (planeVector2.sqrMagnitude != 0f)
						{
							num++;
							direction += planeVector2;
						}
					}
					direction /= (float)num;
				}
				_cellDirs[i, j].direction = direction;
				_cellDirs[i, j].pathPortalIndex = -1;
			}
		}
	}

	private void HandlePortals()
	{
		int num = _currentPath.count - 1;
		for (int i = 1; i < num; i++)
		{
			if (_currentPath[i] is PortalCell portalCell && _grid.Contains(portalCell.position))
			{
				int num2 = portalCell.neighbourNodes.Length;
				for (int j = 0; j < num2; j++)
				{
					IPathNode pathNode = portalCell.neighbourNodes[j];
					int matrixPosX = pathNode.matrixPosX;
					int matrixPosZ = pathNode.matrixPosZ;
					_cellDirs[matrixPosX, matrixPosZ].pathPortalIndex = i;
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
			flag = _fastMarchedCells[matrixPosX - 1, matrixPosZ].sqrMagnitude != 0f;
			if (!flag && cell2.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell2, _unitProperties))
			{
				walkableCells.Add(cell2);
				flag = true;
			}
		}
		if (cell3 != null)
		{
			flag4 = _fastMarchedCells[matrixPosX, matrixPosZ - 1].sqrMagnitude != 0f;
			if (!flag4 && cell3.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell3, _unitProperties))
			{
				walkableCells.Add(cell3);
				flag4 = true;
			}
		}
		if (cell4 != null)
		{
			flag2 = _fastMarchedCells[matrixPosX + 1, matrixPosZ].sqrMagnitude != 0f;
			if (!flag2 && cell4.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell4, _unitProperties))
			{
				walkableCells.Add(cell4);
				flag2 = true;
			}
		}
		if (cell5 != null)
		{
			flag3 = _fastMarchedCells[matrixPosX, matrixPosZ + 1].sqrMagnitude != 0f;
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
			if (cell6 != null && _fastMarchedCells[matrixPosX - 1, matrixPosZ - 1].sqrMagnitude == 0f && cell6.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell6, _unitProperties))
			{
				walkableCells.Add(cell6);
			}
		}
		if (flag8)
		{
			Cell cell7 = _grid.cellMatrix[matrixPosX - 1, matrixPosZ + 1];
			if (cell7 != null && _fastMarchedCells[matrixPosX - 1, matrixPosZ + 1].sqrMagnitude == 0f && cell7.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell7, _unitProperties))
			{
				walkableCells.Add(cell7);
			}
		}
		if (flag6)
		{
			Cell cell8 = _grid.cellMatrix[matrixPosX + 1, matrixPosZ - 1];
			if (cell8 != null && _fastMarchedCells[matrixPosX + 1, matrixPosZ - 1].sqrMagnitude == 0f && cell8.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell8, _unitProperties))
			{
				walkableCells.Add(cell8);
			}
		}
		if (flag5)
		{
			Cell cell9 = _grid.cellMatrix[matrixPosX + 1, matrixPosZ + 1];
			if (cell9 != null && _fastMarchedCells[matrixPosX + 1, matrixPosZ + 1].sqrMagnitude == 0f && cell9.IsWalkableWithClearance(_unitProperties) && cell.IsWalkableFromWithClearance(cell9, _unitProperties))
			{
				walkableCells.Add(cell9);
			}
		}
	}

	public VectorFieldCell GetFieldCellAtPos(IGridCell cell)
	{
		Ensure.ArgumentNotNull(cell, "cell");
		return _cellDirs[cell.matrixPosX, cell.matrixPosZ];
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
			_onFinalApproach = false;
			GenerateVectorField();
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
			Vector3 centerOfGravity = group.centerOfGravity;
			float sqrMagnitude = (centerOfGravity - _destination).sqrMagnitude;
			float sqrMagnitude2 = (modelUnit.position - _destination).sqrMagnitude;
			float num = modelUnit.radius * 2f;
			float num2 = num * num;
			if (sqrMagnitude <= num2 || sqrMagnitude2 <= num2)
			{
				_onFinalApproach = true;
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

	private void PopPathNode()
	{
		IPositioned positioned = _currentPath.Pop();
		HandlePortals();
		if (_announceAllNodes && group is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup)
		{
			defaultSteeringTransientUnitGroup.AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, positioned.position, null);
		}
	}

	public void DebugRender()
	{
		if (_grid == null)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0.5f, 0f);
		int sizeX = _grid.sizeX;
		int sizeZ = _grid.sizeZ;
		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeZ; j++)
			{
				Vector3 vector2 = _grid.cellMatrix[i, j].position + vector;
				VectorFieldCell vectorFieldCell = _cellDirs[i, j];
				if (vectorFieldCell.pathPortalIndex == -1)
				{
					Gizmos.color = Color.white;
				}
				else
				{
					Gizmos.color = Color.blue;
				}
				Gizmos.DrawSphere(vector2, 0.25f);
				Vector3 vector3 = vectorFieldCell.direction.normalized;
				Gizmos.DrawLine(vector2, vector2 + vector3);
			}
		}
		if (_destination.sqrMagnitude != 0f)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(_destination, 0.25f);
		}
	}
}
