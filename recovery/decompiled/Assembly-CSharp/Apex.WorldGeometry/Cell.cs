using System.Collections.Generic;
using Apex.Common;
using Apex.DataStructures;
using Apex.PathFinding;
using Apex.Units;
using Apex.Utilities;
using UnityEngine;

namespace Apex.WorldGeometry;

public abstract class Cell : IPathNode, IGridCell, IPositioned
{
	private CellMatrix _parent;

	private bool _permanentlyBlocked;

	private int _blockMask;

	private NeighbourPosition _neighbours;

	private List<IPathNode> _virtualNeighbours;

	private List<IDynamicObstacle> _dynamicObstacles;

	int IPathNode.g { get; set; }

	int IPathNode.h { get; set; }

	int IPathNode.f { get; set; }

	IPathNode IPathNode.predecessor { get; set; }

	bool IPathNode.isClosed { get; set; }

	bool IPathNode.hasVirtualNeighbour => _virtualNeighbours != null && _virtualNeighbours.Count > 0;

	public CellMatrix parent => _parent;

	public Vector3 position { get; private set; }

	public int matrixPosX { get; private set; }

	public int matrixPosZ { get; private set; }

	public int cost { get; set; }

	public NeighbourPosition neighbours => _neighbours;

	public bool isPermanentlyBlocked
	{
		get
		{
			return _permanentlyBlocked;
		}
		set
		{
			_permanentlyBlocked = value;
		}
	}

	public Cell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
	{
		Ensure.ArgumentNotNull(parent, "parent");
		this.position = position;
		this.matrixPosX = matrixPosX;
		this.matrixPosZ = matrixPosZ;
		_parent = parent;
		_permanentlyBlocked = blocked;
		_neighbours = NeighbourPosition.None;
		if (matrixPosX < parent.columns - 1)
		{
			_neighbours |= NeighbourPosition.Right;
			if (matrixPosZ < parent.rows - 1)
			{
				_neighbours |= NeighbourPosition.Top | NeighbourPosition.TopRight;
			}
			if (matrixPosZ > 0)
			{
				_neighbours |= NeighbourPosition.Bottom | NeighbourPosition.BottomRight;
			}
		}
		if (matrixPosX > 0)
		{
			_neighbours |= NeighbourPosition.Left;
			if (matrixPosZ < parent.rows - 1)
			{
				_neighbours |= NeighbourPosition.TopLeft | NeighbourPosition.Top;
			}
			if (matrixPosZ > 0)
			{
				_neighbours |= NeighbourPosition.BottomLeft | NeighbourPosition.Bottom;
			}
		}
	}

	public bool IsWalkable(AttributeMask mask)
	{
		if (_permanentlyBlocked)
		{
			return false;
		}
		if (mask == AttributeMask.None)
		{
			return _blockMask == AttributeMask.None;
		}
		return (_blockMask & (int)mask) != mask;
	}

	public virtual bool IsWalkableWithClearance(IUnitProperties unitProps)
	{
		return IsWalkable(unitProps.attributes);
	}

	public abstract bool IsWalkableFromAllDirections(IUnitProperties unitProps);

	public abstract bool IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps);

	public abstract bool IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps);

	public NeighbourPosition GetRelativePositionTo(IGridCell other)
	{
		int num = Mathf.Clamp(matrixPosX - other.matrixPosX, -1, 1);
		int num2 = Mathf.Clamp(matrixPosZ - other.matrixPosZ, -1, 1);
		return (NeighbourPosition)(1 << num + 3 * num2 + 4);
	}

	public VectorXZ GetDirectionTo(IGridCell other)
	{
		int num = Mathf.Clamp(other.matrixPosX - matrixPosX, -1, 1) + 1;
		int num2 = Mathf.Clamp(other.matrixPosZ - matrixPosZ, -1, 1) + 1;
		return MatrixDirection.Directions[num2 * 3 + num];
	}

	public Cell GetNeighbour(VectorXZ offset)
	{
		return GetNeighbour(offset.x, offset.z);
	}

	public Cell GetNeighbour(int dx, int dz)
	{
		int column = matrixPosX + dx;
		int row = matrixPosZ + dz;
		return _parent[column, row];
	}

	public int GetNeighbours(Cell[] neighboursBuffer)
	{
		int num = matrixPosX;
		int num2 = matrixPosZ;
		int result = 0;
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (j != 0 || i != 0)
				{
					Cell cell = _parent[num + i, num2 + j];
					if (cell != null)
					{
						neighboursBuffer[result++] = cell;
					}
				}
			}
		}
		return result;
	}

	public override string ToString()
	{
		return "Cell: " + position.ToString();
	}

	bool IPathNode.TryGetWalkableNeighbour(int dx, int dz, IUnitProperties unitProps, DynamicArray<IPathNode> neighbours)
	{
		int column = matrixPosX + dx;
		int row = matrixPosZ + dz;
		Cell cell = _parent[column, row];
		if (cell == null)
		{
			return false;
		}
		if (cell.IsWalkableFromWithClearance(this, unitProps))
		{
			neighbours.Add(cell);
			return true;
		}
		return false;
	}

	void IPathNode.GetWalkableNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps, bool cornerCuttingAllowed, bool preventDiagonalMoves)
	{
		bool flag = ((IPathNode)this).TryGetWalkableNeighbour(0, 1, unitProps, neighbours);
		bool flag2 = ((IPathNode)this).TryGetWalkableNeighbour(0, -1, unitProps, neighbours);
		bool flag3 = ((IPathNode)this).TryGetWalkableNeighbour(1, 0, unitProps, neighbours);
		bool flag4 = ((IPathNode)this).TryGetWalkableNeighbour(-1, 0, unitProps, neighbours);
		if (!preventDiagonalMoves)
		{
			bool flag5;
			bool flag6;
			bool flag7;
			bool flag8;
			if (cornerCuttingAllowed)
			{
				flag5 = flag || flag3;
				flag6 = flag2 || flag3;
				flag7 = flag2 || flag4;
				flag8 = flag || flag4;
			}
			else
			{
				flag5 = flag && flag3;
				flag6 = flag2 && flag3;
				flag7 = flag2 && flag4;
				flag8 = flag && flag4;
			}
			if (flag5)
			{
				((IPathNode)this).TryGetWalkableNeighbour(1, 1, unitProps, neighbours);
			}
			if (flag6)
			{
				((IPathNode)this).TryGetWalkableNeighbour(1, -1, unitProps, neighbours);
			}
			if (flag7)
			{
				((IPathNode)this).TryGetWalkableNeighbour(-1, -1, unitProps, neighbours);
			}
			if (flag8)
			{
				((IPathNode)this).TryGetWalkableNeighbour(-1, 1, unitProps, neighbours);
			}
		}
	}

	void IPathNode.GetVirtualNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps)
	{
		if (_virtualNeighbours == null)
		{
			return;
		}
		for (int i = 0; i < _virtualNeighbours.Count; i++)
		{
			IPathNode pathNode = _virtualNeighbours[i];
			if (pathNode.IsWalkableWithClearance(unitProps))
			{
				neighbours.Add(pathNode);
			}
		}
	}

	void IPathNode.RegisterVirtualNeighbour(IPathNode neighbour)
	{
		if (_virtualNeighbours == null)
		{
			_virtualNeighbours = new List<IPathNode>(1);
		}
		_virtualNeighbours.Add(neighbour);
	}

	void IPathNode.UnregisterVirtualNeighbour(IPathNode neighbour)
	{
		if (_virtualNeighbours != null)
		{
			_virtualNeighbours.Remove(neighbour);
		}
	}

	internal virtual void EnsureDataForEditor()
	{
	}

	internal void UpdateState(float newHeight, bool isBlocked)
	{
		Vector3 vector = position;
		position = new Vector3(vector.x, newHeight, vector.z);
		_permanentlyBlocked = isBlocked;
	}

	internal bool AddDynamicObstacle(IDynamicObstacle dynamicObstacle)
	{
		if (_permanentlyBlocked)
		{
			return false;
		}
		if (_dynamicObstacles == null)
		{
			_dynamicObstacles = new List<IDynamicObstacle>(2);
		}
		if (_dynamicObstacles.AddUnique(dynamicObstacle))
		{
			return UpdateExclusionMask();
		}
		return false;
	}

	internal bool RemoveDynamicObstacle(IDynamicObstacle dynamicObstacle)
	{
		if (_dynamicObstacles != null && _dynamicObstacles.Remove(dynamicObstacle))
		{
			return UpdateExclusionMask();
		}
		return false;
	}

	private bool UpdateExclusionMask()
	{
		int blockMask = _blockMask;
		int num = 0;
		for (int i = 0; i < _dynamicObstacles.Count; i++)
		{
			num |= ~(int)_dynamicObstacles[i].exceptionsMask;
		}
		_blockMask = num;
		return blockMask != _blockMask;
	}
}
