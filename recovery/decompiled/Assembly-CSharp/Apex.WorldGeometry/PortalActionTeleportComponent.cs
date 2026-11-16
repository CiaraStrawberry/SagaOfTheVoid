using System;
using Apex.PathFinding.MoveCost;
using Apex.Services;
using Apex.Steering;
using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Portals/Action Teleport", 1018)]
[ApexComponent("Game World")]
public class PortalActionTeleportComponent : MonoBehaviour, IPortalAction
{
	public void Execute(Transform unit, PortalCell from, IPositioned to, Action callWhenComplete)
	{
		ISampleHeights heightSampler = GameServices.heightStrategy.heightSampler;
		if (!heightSampler.TrySampleHeight(unit.position, out var height))
		{
			height = to.position.y;
		}
		float num = to.position.y - height;
		unit.position = new Vector3(to.position.x, unit.position.y + num, to.position.z);
		callWhenComplete();
	}

	public int GetActionCost(IPositioned from, IPositioned to, IMoveCost costProvider)
	{
		return 0;
	}
}
