using UnityEngine;

namespace Apex.PathFinding;

public class PathResult
{
	public class InnerResultData
	{
		public PathingStatus status;

		public Vector3[] pendingWaypoints;

		public bool hasPendingWaypoints => pendingWaypoints != null && pendingWaypoints.Length > 0;
	}

	private static readonly Path _pathEmpty = new Path();

	private PathingStatus _status;

	public PathingStatus status
	{
		get
		{
			return _status;
		}
		set
		{
			_status = value;
		}
	}

	public string errorInfo { get; set; }

	public Path path { get; set; }

	public int pathCost { get; set; }

	public IPathRequest originalRequest { get; protected set; }

	public InnerResultData innerResult { get; private set; }

	public PathResult(PathingStatus status, Path path, int pathCost, IPathRequest originalRequest)
	{
		this.status = status;
		this.path = path ?? _pathEmpty;
		this.pathCost = pathCost;
		this.originalRequest = originalRequest;
	}

	protected PathResult()
	{
	}

	internal void RegisterPartialResult(PathingStatus status, Vector3[] pendingWaypoints)
	{
		this.status = PathingStatus.CompletePartial;
		innerResult = new InnerResultData
		{
			status = status,
			pendingWaypoints = pendingWaypoints
		};
	}
}
