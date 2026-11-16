namespace Apex.PathFinding;

public interface IPathNavigationOptions
{
	float nextNodeDistance { get; set; }

	float requestNextWaypointDistance { get; set; }

	bool announceAllNodes { get; set; }

	ReplanMode replanMode { get; set; }

	float replanInterval { get; set; }
}
