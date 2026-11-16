using System;
using Apex.PathFinding.MoveCost;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public interface IPortalNode : IPathNode, IGridCell, IPositioned
{
	GridPortal portal { get; }

	IPortalNode partner { get; }

	int GetHeuristic(IPathNode node, IPathNode goal, IMoveCost moveCostProvider);

	int GetCost(IPositioned from, IPositioned to, IMoveCost costProvider);

	IGrid Execute(Transform unit, IPositioned to, Action callWhenComplete);
}
