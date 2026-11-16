using System;
using Apex.PathFinding.MoveCost;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class PortalConnectorAction : IPortalAction
{
	public static readonly PortalConnectorAction instance = new PortalConnectorAction();

	private PortalConnectorAction()
	{
	}

	public void Execute(Transform unit, PortalCell from, IPositioned to, Action callWhenComplete)
	{
		callWhenComplete();
	}

	public int GetActionCost(IPositioned from, IPositioned to, IMoveCost costProvider)
	{
		return costProvider.GetHeuristic(from, to);
	}
}
