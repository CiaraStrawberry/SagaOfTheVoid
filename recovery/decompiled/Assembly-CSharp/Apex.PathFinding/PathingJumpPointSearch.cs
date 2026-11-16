using System;
using Apex.DataStructures;
using Apex.PathFinding.MoveCost;
using Apex.WorldGeometry;

namespace Apex.PathFinding;

public class PathingJumpPointSearch : PathingAStar
{
	private DynamicArray<IPathNode> _neighbours;

	public PathingJumpPointSearch(int heapInitialSize)
		: base(heapInitialSize)
	{
		_neighbours = new DynamicArray<IPathNode>(8);
	}

	public PathingJumpPointSearch(int heapInitialSize, IMoveCost moveCostProvider, ICellCostStrategy cellCostStrategy, ISmoothPaths pathSmoother, IRequestPreProcessor[] requestPreprocessors)
		: base(heapInitialSize, moveCostProvider, cellCostStrategy, pathSmoother, requestPreprocessors)
	{
		_neighbours = new DynamicArray<IPathNode>(8);
	}

	protected override void ValidateRequest(IPathRequest request)
	{
		base.ValidateRequest(request);
		if (request.pathFinderOptions.preventDiagonalMoves)
		{
			throw new NotSupportedException("Jump Point Search does not support path requests set to Prevent Diagonal Moves.");
		}
	}

	protected override void GetWalkableSuccessors(IPathNode current, DynamicArray<IPathNode> successorArray)
	{
		_neighbours.Clear();
		if (current.predecessor == null)
		{
			current.GetWalkableNeighbours(_neighbours, _unitProps, _cutCorners, preventDiagonalMoves: false);
		}
		else
		{
			if (current is IPortalNode)
			{
				current.GetWalkableNeighbours(successorArray, _unitProps, _cutCorners, preventDiagonalMoves: false);
				return;
			}
			VectorXZ directionTo = current.predecessor.GetDirectionTo(current);
			PruneNeighbours(current, directionTo);
		}
		int count = _neighbours.count;
		for (int i = 0; i < count; i++)
		{
			IPathNode pathNode = _neighbours[i];
			if (pathNode == null)
			{
				break;
			}
			VectorXZ directionTo2 = current.GetDirectionTo(pathNode);
			IPathNode pathNode2 = Jump(current, directionTo2);
			if (pathNode2 != null)
			{
				successorArray.Add(pathNode2);
			}
		}
	}

	private IPathNode Jump(IPathNode current, VectorXZ direction)
	{
		Cell neighbour = current.GetNeighbour(direction.x, direction.z);
		if (neighbour == null || !neighbour.IsWalkableFromWithClearance(current, _unitProps))
		{
			return null;
		}
		if (neighbour == goal)
		{
			return neighbour;
		}
		if (HasForcedNeighbour(neighbour, direction))
		{
			return neighbour;
		}
		bool num;
		if (direction.x != 0 && direction.z != 0)
		{
			if (Jump(neighbour, new VectorXZ(direction.x, 0)) != null || Jump(neighbour, new VectorXZ(0, direction.z)) != null)
			{
				return neighbour;
			}
			Cell neighbour2 = neighbour.GetNeighbour(direction.x, 0);
			Cell neighbour3 = neighbour.GetNeighbour(0, direction.z);
			if (_cutCorners)
			{
				if (neighbour2 != null && neighbour2.IsWalkableFromWithClearance(current, _unitProps))
				{
					goto IL_012f;
				}
				if (neighbour3 != null)
				{
					num = neighbour3.IsWalkableFromWithClearance(current, _unitProps);
					goto IL_0128;
				}
			}
			else if (neighbour2 != null && neighbour2.IsWalkableFromWithClearance(current, _unitProps) && neighbour3 != null)
			{
				num = neighbour3.IsWalkableFromWithClearance(current, _unitProps);
				goto IL_0128;
			}
			goto IL_0146;
		}
		return Jump(neighbour, direction);
		IL_0128:
		if (num)
		{
			goto IL_012f;
		}
		goto IL_0146;
		IL_0146:
		return null;
		IL_012f:
		return Jump(neighbour, direction);
	}

	private bool HasForcedNeighbour(IPathNode current, VectorXZ direction)
	{
		if (current.hasVirtualNeighbour)
		{
			return true;
		}
		bool flag = false;
		if (direction.x != 0)
		{
			if (direction.z != 0)
			{
				if (!_cutCorners)
				{
					return false;
				}
				Cell neighbour = current.GetNeighbour(-direction.x, 0);
				if (neighbour != null && !neighbour.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(-direction.x, direction.z)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
				Cell neighbour2 = current.GetNeighbour(0, -direction.z);
				if (neighbour2 != null && !neighbour2.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(direction.x, -direction.z)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
			}
			else if (_cutCorners)
			{
				Cell neighbour3 = current.GetNeighbour(0, 1);
				if (neighbour3 != null && !neighbour3.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(direction.x, 1)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
				Cell neighbour4 = current.GetNeighbour(0, -1);
				if (neighbour4 != null && !neighbour4.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(direction.x, -1)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
			}
			else
			{
				Cell neighbour5 = current.GetNeighbour(-direction.x, 1);
				if (neighbour5 != null && !neighbour5.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(0, 1)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
				Cell neighbour6 = current.GetNeighbour(-direction.x, -1);
				if (neighbour6 != null && !neighbour6.IsWalkableFromAllDirections(_unitProps))
				{
					flag |= current.GetNeighbour(0, -1)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
				}
			}
		}
		else if (_cutCorners)
		{
			Cell neighbour7 = current.GetNeighbour(-1, 0);
			if (neighbour7 != null && !neighbour7.IsWalkableFromAllDirections(_unitProps))
			{
				flag |= current.GetNeighbour(-1, direction.z)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
			}
			Cell neighbour8 = current.GetNeighbour(1, 0);
			if (neighbour8 != null && !neighbour8.IsWalkableFromAllDirections(_unitProps))
			{
				flag |= current.GetNeighbour(1, direction.z)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
			}
		}
		else
		{
			Cell neighbour9 = current.GetNeighbour(-1, -direction.z);
			if (neighbour9 != null && !neighbour9.IsWalkableFromAllDirections(_unitProps))
			{
				flag |= current.GetNeighbour(-1, 0)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
			}
			Cell neighbour10 = current.GetNeighbour(1, -direction.z);
			if (neighbour10 != null && !neighbour10.IsWalkableFromAllDirections(_unitProps))
			{
				flag |= current.GetNeighbour(1, 0)?.IsWalkableFromWithClearance(current, _unitProps) ?? false;
			}
		}
		return flag;
	}

	private void PruneNeighbours(IPathNode current, VectorXZ direction)
	{
		if (direction.x != 0)
		{
			if (direction.z != 0)
			{
				bool flag = current.TryGetWalkableNeighbour(0, direction.z, _unitProps, _neighbours);
				bool flag2 = current.TryGetWalkableNeighbour(direction.x, 0, _unitProps, _neighbours);
				if (_cutCorners)
				{
					if (flag || flag2)
					{
						current.TryGetWalkableNeighbour(direction.x, direction.z, _unitProps, _neighbours);
					}
					Cell neighbour = current.GetNeighbour(-direction.x, 0);
					if (neighbour != null && !neighbour.IsWalkableFromAllDirections(_unitProps))
					{
						current.TryGetWalkableNeighbour(-direction.x, direction.z, _unitProps, _neighbours);
					}
					Cell neighbour2 = current.GetNeighbour(0, -direction.z);
					if (neighbour2 != null && !neighbour2.IsWalkableFromAllDirections(_unitProps))
					{
						current.TryGetWalkableNeighbour(direction.x, -direction.z, _unitProps, _neighbours);
					}
				}
				else if (flag && flag2)
				{
					current.TryGetWalkableNeighbour(direction.x, direction.z, _unitProps, _neighbours);
				}
				return;
			}
			current.TryGetWalkableNeighbour(direction.x, 0, _unitProps, _neighbours);
			if (_cutCorners)
			{
				Cell neighbour3 = current.GetNeighbour(0, 1);
				if (neighbour3 != null && !neighbour3.IsWalkableFromAllDirections(_unitProps))
				{
					current.TryGetWalkableNeighbour(direction.x, 1, _unitProps, _neighbours);
				}
				Cell neighbour4 = current.GetNeighbour(0, -1);
				if (neighbour4 != null && !neighbour4.IsWalkableFromAllDirections(_unitProps))
				{
					current.TryGetWalkableNeighbour(direction.x, -1, _unitProps, _neighbours);
				}
				return;
			}
			Cell neighbour5 = current.GetNeighbour(-direction.x, 1);
			if (neighbour5 != null && !neighbour5.IsWalkableFromAllDirections(_unitProps))
			{
				current.TryGetWalkableNeighbour(0, 1, _unitProps, _neighbours);
				current.TryGetWalkableNeighbour(direction.x, 1, _unitProps, _neighbours);
			}
			Cell neighbour6 = current.GetNeighbour(-direction.x, -1);
			if (neighbour6 != null && !neighbour6.IsWalkableFromAllDirections(_unitProps))
			{
				current.TryGetWalkableNeighbour(0, -1, _unitProps, _neighbours);
				current.TryGetWalkableNeighbour(direction.x, -1, _unitProps, _neighbours);
			}
			return;
		}
		if (direction.z == 0)
		{
			current.GetWalkableNeighbours(_neighbours, _unitProps, _cutCorners, preventDiagonalMoves: false);
			return;
		}
		current.TryGetWalkableNeighbour(0, direction.z, _unitProps, _neighbours);
		if (_cutCorners)
		{
			Cell neighbour7 = current.GetNeighbour(-1, 0);
			if (neighbour7 != null && !neighbour7.IsWalkableFromAllDirections(_unitProps))
			{
				current.TryGetWalkableNeighbour(-1, direction.z, _unitProps, _neighbours);
			}
			Cell neighbour8 = current.GetNeighbour(1, 0);
			if (neighbour8 != null && !neighbour8.IsWalkableFromAllDirections(_unitProps))
			{
				current.TryGetWalkableNeighbour(1, direction.z, _unitProps, _neighbours);
			}
			return;
		}
		Cell neighbour9 = current.GetNeighbour(-1, -direction.z);
		if (neighbour9 != null && !neighbour9.IsWalkableFromAllDirections(_unitProps))
		{
			current.TryGetWalkableNeighbour(-1, 0, _unitProps, _neighbours);
			current.TryGetWalkableNeighbour(-1, direction.z, _unitProps, _neighbours);
		}
		Cell neighbour10 = current.GetNeighbour(1, -direction.z);
		if (neighbour10 != null && !neighbour10.IsWalkableFromAllDirections(_unitProps))
		{
			current.TryGetWalkableNeighbour(1, 0, _unitProps, _neighbours);
			current.TryGetWalkableNeighbour(1, direction.z, _unitProps, _neighbours);
		}
	}
}
