namespace Apex.PathFinding;

public class PathNavigationOptions : IPathNavigationOptions
{
	public float nextNodeDistance { get; set; }

	public float requestNextWaypointDistance { get; set; }

	public bool announceAllNodes { get; set; }

	public ReplanMode replanMode { get; set; }

	public float replanInterval { get; set; }

	public PathNavigationOptions()
	{
		nextNodeDistance = 1f;
		requestNextWaypointDistance = 2f;
		replanMode = ReplanMode.Dynamic;
		replanInterval = 0.5f;
	}
}
