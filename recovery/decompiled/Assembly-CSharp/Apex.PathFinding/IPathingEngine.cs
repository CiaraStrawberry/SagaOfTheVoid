using System.Collections;

namespace Apex.PathFinding;

public interface IPathingEngine
{
	void ProcessRequest(IPathRequest request);

	IEnumerator ProcessRequestCoroutine(IPathRequest request);
}
