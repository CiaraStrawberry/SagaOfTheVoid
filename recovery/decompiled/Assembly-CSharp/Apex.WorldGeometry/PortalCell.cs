using System;
using System.Linq;
using Apex.Common;
using Apex.DataStructures;
using Apex.PathFinding;
using Apex.PathFinding.MoveCost;
using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class PortalCell : IPortalNode, IPathNode, IGridCell, IPositioned
{
	private GridPortal _parentPortal;

	private IPortalAction _action;

	private PortalCell _partner;

	private IPathNode[] _neighbourNodes;

	private MatrixBounds _matrixBounds;

	private bool _isOneWay;

	int IPathNode.g { get; set; }

	int IPathNode.h { get; set; }

	int IPathNode.f { get; set; }

	IPathNode IPathNode.predecessor { get; set; }

	bool IPathNode.isClosed { get; set; }

	bool IPathNode.hasVirtualNeighbour => false;

	int IGridCell.matrixPosX => -1;

	int IGridCell.matrixPosZ => -1;

	NeighbourPosition IGridCell.neighbours => NeighbourPosition.None;

	internal IPathNode[] neighbourNodes => _neighbourNodes;

	public GridPortal portal => _parentPortal;

	public IPortalNode partner => _partner;

	public int cost { get; set; }

	public Vector3 position { get; private set; }

	public CellMatrix parent { get; private set; }

	public bool isPermanentlyBlocked
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	internal PortalCell(GridPortal parentPortal, bool oneWay, IPortalAction action)
	{
		_parentPortal = parentPortal;
		_isOneWay = oneWay;
		_action = action;
	}

	public int GetHeuristic(IPathNode node, IPathNode goal, IMoveCost moveCostProvider)
	{
		int num = _matrixBounds.AdjustColumnToBounds(node.matrixPosX);
		int num2 = _matrixBounds.AdjustRowToBounds(node.matrixPosZ);
		Cell cell = parent.rawMatrix[num, num2];
		num = _partner._matrixBounds.AdjustColumnToBounds(goal.matrixPosX);
		num2 = _partner._matrixBounds.AdjustRowToBounds(goal.matrixPosZ);
		Cell cell2 = _partner.parent.rawMatrix[num, num2];
		return moveCostProvider.GetHeuristic(node, cell) + _action.GetActionCost(cell, cell2, moveCostProvider) + moveCostProvider.GetHeuristic(cell2, goal);
	}

	public int GetCost(IPositioned from, IPositioned to, IMoveCost costProvider)
	{
		return _action.GetActionCost(from, to, costProvider);
	}

	public IGrid Execute(Transform unit, IPositioned to, Action callWhenComplete)
	{
		_action.Execute(unit, this, to, callWhenComplete);
		return _parentPortal.GetGridFor(this);
	}

	public bool IsWalkableWithClearance(IUnitProperties unitProps)
	{
		return _parentPortal.IsUsableBy(unitProps.attributes);
	}

	public bool IsWalkable(AttributeMask mask)
	{
		return _parentPortal.IsUsableBy(mask);
	}

	public bool Services(IGridCell cell)
	{
		return _matrixBounds.Contains(cell.matrixPosX, cell.matrixPosZ);
	}

	bool IGridCell.IsWalkableFromAllDirections(IUnitProperties unitProps)
	{
		return IsWalkableWithClearance(unitProps);
	}

	bool IGridCell.IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps)
	{
		return _parentPortal.IsUsableBy(unitProps.attributes);
	}

	bool IGridCell.IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps)
	{
		return _parentPortal.IsUsableBy(unitProps.attributes);
	}

	NeighbourPosition IGridCell.GetRelativePositionTo(IGridCell other)
	{
		return NeighbourPosition.None;
	}

	VectorXZ IGridCell.GetDirectionTo(IGridCell other)
	{
		return MatrixDirection.None;
	}

	Cell IGridCell.GetNeighbour(VectorXZ offset)
	{
		return null;
	}

	Cell IGridCell.GetNeighbour(int dx, int dz)
	{
		return null;
	}

	int IGridCell.GetNeighbours(Cell[] neighboursBuffer)
	{
		return 0;
	}

	void IPathNode.GetWalkableNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps, bool cornerCuttingAllowed, bool preventDiagonalMoves)
	{
		IPathNode[] array = _partner._neighbourNodes;
		int num = array.Length;
		if (_parentPortal.type == PortalType.Connector)
		{
			IPathNode predecessor = ((IPathNode)this).predecessor;
			int num2 = _matrixBounds.IndexOf(predecessor.matrixPosX, predecessor.matrixPosZ);
			if (num2 < 0)
			{
				return;
			}
			if (preventDiagonalMoves)
			{
				if (array[num2].IsWalkableWithClearance(unitProps))
				{
					neighbours.Add(array[num2]);
				}
				return;
			}
			int num3 = Math.Max(num2 - 1, 0);
			int num4 = Math.Min(num2 + 2, num);
			for (int i = num3; i < num4; i++)
			{
				if (array[i].IsWalkableWithClearance(unitProps))
				{
					neighbours.Add(array[i]);
				}
			}
			return;
		}
		for (int j = 0; j < num; j++)
		{
			if (array[j].IsWalkableWithClearance(unitProps))
			{
				neighbours.Add(array[j]);
			}
		}
	}

	bool IPathNode.TryGetWalkableNeighbour(int dx, int dz, IUnitProperties unitProps, DynamicArray<IPathNode> neighbours)
	{
		return false;
	}

	void IPathNode.GetVirtualNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps)
	{
	}

	void IPathNode.RegisterVirtualNeighbour(IPathNode neighbour)
	{
	}

	void IPathNode.UnregisterVirtualNeighbour(IPathNode neighbour)
	{
	}

	internal IGrid Initialize(PortalCell partner, Bounds portalBounds)
	{
		IGrid grid = GridManager.instance.GetGrid(portalBounds.center);
		if (grid == null)
		{
			return null;
		}
		_partner = partner;
		parent = grid.cellMatrix;
		position = portalBounds.center;
		_matrixBounds = grid.cellMatrix.GetMatrixBounds(portalBounds, 0f, adjustToBounds: true);
		_neighbourNodes = grid.cellMatrix.GetRange(_matrixBounds).ToArray();
		return grid;
	}

	internal void Activate()
	{
		if (!_isOneWay)
		{
			if (_parentPortal.type == PortalType.Shortcut)
			{
				parent.shortcutPortals.Add(this);
			}
			for (int i = 0; i < _neighbourNodes.Length; i++)
			{
				_neighbourNodes[i].RegisterVirtualNeighbour(this);
			}
		}
	}

	internal void Deactivate()
	{
		if (_parentPortal.type == PortalType.Shortcut)
		{
			parent.shortcutPortals.Remove(this);
		}
		for (int i = 0; i < _neighbourNodes.Length; i++)
		{
			_neighbourNodes[i].UnregisterVirtualNeighbour(this);
		}
	}
}
