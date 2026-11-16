using System;

namespace Apex.PathFinding;

public interface IPathService : IDisposable
{
	void QueueRequest(IPathRequest request);

	void QueueRequest(IPathRequest request, int priority);
}
