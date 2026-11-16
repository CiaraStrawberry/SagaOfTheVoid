using System;
using Apex.PathFinding.MoveCost;
using UnityEngine;

namespace Apex.WorldGeometry;

public interface IPortalAction
{
	void Execute(Transform unit, PortalCell from, IPositioned to, Action callWhenComplete);

	int GetActionCost(IPositioned from, IPositioned to, IMoveCost costProvider);
}
