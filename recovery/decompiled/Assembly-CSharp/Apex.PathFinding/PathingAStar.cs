using System.Collections.Generic;
using Apex.Common;
using Apex.DataStructures;
using Apex.PathFinding.MoveCost;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;

namespace Apex.PathFinding;

public class PathingAStar : PathingEngineBase
{
	private BinaryHeap<IPathNode> _openSet;

	private DynamicArray<IPathNode> _successorArray;

	private IPathNode _closestNode;

	private bool _preventDiagonalMoves;

	protected List<IPathNode> _expandedSet;

	protected IPathNode _current;

	protected IUnitProperties _unitProps;

	protected AttributeMask _unitAttributes;

	protected bool _cutCorners;

	public PathingAStar(int heapInitialSize)
		: base(new DiagonalDistance(10), new DefaultCellCostStrategy(), new PathSmoother(), null)
	{
		_openSet = new BinaryHeap<IPathNode>(heapInitialSize, new PathNodeComparer());
		_expandedSet = new List<IPathNode>();
		_successorArray = new DynamicArray<IPathNode>(15);
	}

	public PathingAStar(int heapInitialSize, IMoveCost moveCostProvider, ICellCostStrategy cellCostStrategy, ISmoothPaths pathSmoother, IRequestPreProcessor[] requestPreprocessors)
		: base(moveCostProvider, cellCostStrategy, pathSmoother, requestPreprocessors)
	{
		_openSet = new BinaryHeap<IPathNode>(heapInitialSize, new PathNodeComparer());
		_expandedSet = new List<IPathNode>();
		_successorArray = new DynamicArray<IPathNode>(15);
	}

	protected virtual void GetWalkableSuccessors(IPathNode current, DynamicArray<IPathNode> successorArray)
	{
		current.GetWalkableNeighbours(successorArray, _unitProps, _cutCorners, _preventDiagonalMoves);
	}

	protected override void OnStart(IPathNode start, IPathNode actualStart)
	{
		_openSet.Clear();
		_expandedSet.Apply((IPathNode c) =>
		{
			c.g = 0;
		});
		_expandedSet.Clear();
		_closestNode = start;
		start.g = 1;
		start.h = costProvider.GetHeuristic(start, goal);
		start.predecessor = actualStart;
		_openSet.Add(start);
		_expandedSet.Add(start);
		if (actualStart != null)
		{
			actualStart.g = 1;
			actualStart.predecessor = null;
			_expandedSet.Add(actualStart);
		}
		_unitProps = currentRequest.requesterProperties;
		_unitAttributes = _unitProps.attributes;
		_preventDiagonalMoves = currentRequest.pathFinderOptions.preventDiagonalMoves;
		_cutCorners = currentRequest.pathFinderOptions.allowCornerCutting;
	}

	protected override PathingStatus ProcessNext()
	{
		if (!_openSet.hasNext)
		{
			if (currentRequest.pathFinderOptions.navigateToNearestIfBlocked)
			{
				UpdateGoal(_closestNode);
				return PathingStatus.Complete;
			}
			if (!goal.IsWalkableWithClearance(_unitProps))
			{
				return PathingStatus.DestinationBlocked;
			}
			return PathingStatus.NoRouteExists;
		}
		if (_openSet.count == _openSet.capacity)
		{
			UnityServices.debug.LogWarning("The pathing engine's heap capacity (" + _openSet.capacity + ") has been reached. If this happens often, it is recommended that you increase the initial heap size of the engine to avoid automatic resizing.\nDo this on the Path Service Component and increment it by 10 or so at a time to keep it as small as possible.");
			_openSet.Resize();
		}
		_current = _openSet.Remove();
		if (_current == goal)
		{
			return PathingStatus.Complete;
		}
		if (_closestNode.h > _current.h)
		{
			_closestNode = _current;
		}
		_current.isClosed = true;
		_successorArray.Clear();
		GetWalkableSuccessors(_current, _successorArray);
		_current.GetVirtualNeighbours(_successorArray, _unitProps);
		int count = _successorArray.count;
		for (int i = 0; i < count; i++)
		{
			IPathNode pathNode = _successorArray[i];
			if (pathNode == null)
			{
				break;
			}
			int num = _current.g + cellCostStrategy.GetCellCost(pathNode, _unitProps);
			if (_current is IPortalNode portalNode)
			{
				num += portalNode.GetCost(portalNode.predecessor, pathNode, costProvider);
			}
			else if (!(pathNode is IPortalNode))
			{
				num += costProvider.GetMoveCost(_current, pathNode);
			}
			if (pathNode.g > 0)
			{
				if (pathNode.g > num)
				{
					pathNode.g = num;
					pathNode.f = num + pathNode.h;
					pathNode.predecessor = _current;
					if (pathNode.isClosed)
					{
						pathNode.isClosed = false;
						_openSet.Add(pathNode);
					}
					else
					{
						_openSet.ReheapifyUpFrom(pathNode);
					}
				}
			}
			else
			{
				pathNode.g = num;
				pathNode.h = GetHeuristic(pathNode, _current);
				pathNode.f = num + pathNode.h;
				pathNode.predecessor = _current;
				pathNode.isClosed = false;
				_openSet.Add(pathNode);
				_expandedSet.Add(pathNode);
			}
		}
		return PathingStatus.Running;
	}

	private int GetHeuristic(IPathNode n, IPathNode current)
	{
		if (n is IPortalNode)
		{
			return current.h;
		}
		int num = costProvider.GetHeuristic(n, goal);
		IList<PortalCell> shortcutPortals = n.parent.shortcutPortals;
		for (int i = 0; i < shortcutPortals.Count; i++)
		{
			PortalCell portalCell = shortcutPortals[i];
			if (portalCell.IsWalkableWithClearance(_unitProps))
			{
				int heuristic = portalCell.GetHeuristic(n, goal, costProvider);
				if (heuristic < num)
				{
					num = heuristic;
				}
			}
		}
		return num;
	}
}
