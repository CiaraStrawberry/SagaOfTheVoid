namespace Apex.PathFinding;

public class PathFinderOptions : IPathFinderOptions
{
	public int pathingPriority { get; set; }

	public int maxEscapeCellDistanceIfOriginBlocked { get; set; }

	public bool usePathSmoothing { get; set; }

	public bool optimizeUnobstructedPaths { get; set; }

	public bool allowCornerCutting { get; set; }

	public bool preventDiagonalMoves { get; set; }

	public bool navigateToNearestIfBlocked { get; set; }

	public PathFinderOptions()
	{
		usePathSmoothing = true;
		optimizeUnobstructedPaths = true;
		maxEscapeCellDistanceIfOriginBlocked = 3;
	}
}
