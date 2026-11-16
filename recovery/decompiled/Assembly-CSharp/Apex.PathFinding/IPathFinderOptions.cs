namespace Apex.PathFinding;

public interface IPathFinderOptions
{
	int pathingPriority { get; set; }

	int maxEscapeCellDistanceIfOriginBlocked { get; set; }

	bool usePathSmoothing { get; set; }

	bool optimizeUnobstructedPaths { get; set; }

	bool allowCornerCutting { get; set; }

	bool preventDiagonalMoves { get; set; }

	bool navigateToNearestIfBlocked { get; set; }
}
