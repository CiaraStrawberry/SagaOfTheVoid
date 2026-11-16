using Apex.PathFinding;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Path Post Processing/Replan when Blocked", 1019)]
[ApexComponent("Behaviours")]
public class SteerForPathReplanWhenBlocked : SteerForPathResultProcessorComponent
{
	public int maxCellDistanceForNewDestination = 3;

	public override bool HandleResult(PathResult result, SteerForPathComponent steerer)
	{
		PathingStatus status = result.status;
		if (status == PathingStatus.DestinationBlocked)
		{
			IPathRequest originalRequest = result.originalRequest;
			IGrid grid = GridManager.instance.GetGrid(originalRequest.to);
			if (grid == null)
			{
				return false;
			}
			Cell nearestWalkableCell = grid.GetNearestWalkableCell(originalRequest.to, transform.position, requireWalkableFromPosition: false, maxCellDistanceForNewDestination, originalRequest.requesterProperties);
			if (nearestWalkableCell != null)
			{
				originalRequest.to = nearestWalkableCell.position;
				steerer.RequestPath(originalRequest);
				return true;
			}
		}
		return false;
	}
}
