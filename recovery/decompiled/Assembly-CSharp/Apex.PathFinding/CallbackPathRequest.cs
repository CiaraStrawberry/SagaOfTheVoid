using System;
using Apex.LoadBalancing;

namespace Apex.PathFinding;

public class CallbackPathRequest : PathRequestBase, IPathRequest, INeedPath
{
	private Action<PathResult> _callback;

	public override INeedPath requester
	{
		get
		{
			return this;
		}
		set
		{
			throw new InvalidOperationException("This type of request cannot change the requester.");
		}
	}

	public bool isValid => requesterProperties != null && pathFinderOptions != null;

	public bool hasDecayed { get; set; }

	public CallbackPathRequest(Action<PathResult> callback)
	{
		_callback = callback;
	}

	void IPathRequest.Complete(PathResult result)
	{
		requester.ConsumePathResult(result);
	}

	void INeedPath.ConsumePathResult(PathResult result)
	{
		LoadBalancer.marshaller.ExecuteOnMainThread(() =>
		{
			_callback(result);
		});
	}
}
