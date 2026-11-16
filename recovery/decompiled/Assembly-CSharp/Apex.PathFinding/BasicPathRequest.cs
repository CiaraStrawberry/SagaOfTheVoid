namespace Apex.PathFinding;

public class BasicPathRequest : PathRequestBase, IPathRequest
{
	public bool isValid => requester != null && requesterProperties != null && pathFinderOptions != null;

	public virtual bool hasDecayed { get; set; }

	void IPathRequest.Complete(PathResult result)
	{
		requester.ConsumePathResult(result);
	}
}
