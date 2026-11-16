namespace Apex.PathFinding;

public enum PathingStatus
{
	Running,
	Failed,
	Decayed,
	StartOutsideGrid,
	EndOutsideGrid,
	NoRouteExists,
	DestinationBlocked,
	Complete,
	CompletePartial
}
