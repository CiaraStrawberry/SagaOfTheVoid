using Apex.Units;
using UnityEngine;

namespace Apex.PathFinding;

public abstract class PathRequestBase
{
	public Vector3 from { get; set; }

	public Vector3 to { get; set; }

	public Vector3[] via { get; set; }

	public virtual INeedPath requester { get; set; }

	public IUnitProperties requesterProperties { get; set; }

	public RequestType type { get; set; }

	public IPathFinderOptions pathFinderOptions { get; set; }

	public float timeStamp { get; set; }

	public object customData { get; set; }
}
