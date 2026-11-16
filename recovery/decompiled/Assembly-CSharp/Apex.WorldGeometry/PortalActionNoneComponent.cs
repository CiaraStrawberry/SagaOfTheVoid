using System;
using Apex.PathFinding.MoveCost;
using UnityEngine;

namespace Apex.WorldGeometry;

[ApexComponent("Game World")]
[AddComponentMenu("Apex/Game World/Portals/Action None (deprecated)", 1019)]
[Obsolete("No longer used for connectors, use PortalConnectorAction instead if creating your own connectors.")]
public class PortalActionNoneComponent : MonoBehaviour, IPortalAction
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
