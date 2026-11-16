namespace Apex.PathFinding;

public interface ISmoothPaths
{
	Path Smooth(IPathNode goal, int maxPathLength, IPathRequest request, ICellCostStrategy costStrategy);
}
