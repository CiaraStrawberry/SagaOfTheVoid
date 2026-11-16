using Apex.PathFinding;

namespace Apex.Steering.Components;

public interface IProcessPathResults
{
	int processingOrder { get; }

	bool HandleResult(PathResult result, SteerForPathComponent steerer);
}
