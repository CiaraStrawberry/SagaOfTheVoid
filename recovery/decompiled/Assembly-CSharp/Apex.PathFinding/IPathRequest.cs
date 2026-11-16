using Apex.Units;
using UnityEngine;

namespace Apex.PathFinding;

public interface IPathRequest
{
	Vector3 from { get; set; }

	Vector3 to { get; set; }

	Vector3[] via { get; set; }

	INeedPath requester { get; set; }

	IUnitProperties requesterProperties { get; set; }

	IPathFinderOptions pathFinderOptions { get; set; }

	RequestType type { get; set; }

	float timeStamp { get; set; }

	bool isValid { get; }

	bool hasDecayed { get; set; }

	object customData { get; set; }

	void Complete(PathResult result);
}
