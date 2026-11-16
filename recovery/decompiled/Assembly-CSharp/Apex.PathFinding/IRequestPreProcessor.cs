namespace Apex.PathFinding;

public interface IRequestPreProcessor
{
	int priority { get; }

	bool PreProcess(IPathRequest request);
}
