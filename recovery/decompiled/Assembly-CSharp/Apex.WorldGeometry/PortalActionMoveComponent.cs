using System;
using Apex.PathFinding.MoveCost;
using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Portals/Action Move", 1017)]
[ApexComponent("Game World")]
public class PortalActionMoveComponent : MonoBehaviour, IPortalAction
{
	public void Execute(Transform unit, PortalCell from, IPositioned to, Action callWhenComplete)
	{
		callWhenComplete();
	}

	public int GetActionCost(IPositioned from, IPositioned to, IMoveCost costProvider)
	{
		return costProvider.GetHeuristic(from, to);
	}
}
