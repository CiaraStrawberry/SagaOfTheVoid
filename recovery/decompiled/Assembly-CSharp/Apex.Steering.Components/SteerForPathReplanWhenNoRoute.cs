using Apex.LoadBalancing;
using Apex.PathFinding;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Path Post Processing/Replan when no Route", 1020)]
[ApexComponent("Behaviours")]
public class SteerForPathReplanWhenNoRoute : SteerForPathResultProcessorComponent
{
	public float retryDelay = 0.1f;

	public int maxRetries = 3;

	private int _retries;

	public override bool HandleResult(PathResult result, SteerForPathComponent steerer)
	{
		PathingStatus status = result.status;
		if (status == PathingStatus.NoRouteExists)
		{
			IPathRequest originalRequest = result.originalRequest;
			if (_retries < maxRetries)
			{
				_retries++;
				IssueRequest(originalRequest, steerer);
				return true;
			}
		}
		_retries = 0;
		return false;
	}

	private void IssueRequest(IPathRequest request, SteerForPathComponent steerer)
	{
		OneTimeAction item = new OneTimeAction((float ignored) =>
		{
			steerer.RequestPath(request);
		});
		LoadBalancer.defaultBalancer.Add(item, retryDelay, delayFirstUpdate: true);
	}
}
