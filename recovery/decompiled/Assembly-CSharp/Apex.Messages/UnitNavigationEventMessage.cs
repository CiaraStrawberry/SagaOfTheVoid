using UnityEngine;

namespace Apex.Messages;

public class UnitNavigationEventMessage
{
	public enum Event
	{
		None,
		WaypointReached,
		DestinationReached,
		StoppedNoRouteExists,
		StoppedDestinationBlocked,
		StoppedRequestDecayed,
		Stuck,
		NodeReached,
		StoppedUnitOutsideGrid,
		StoppedByRequest
	}

	public Event eventCode { get; set; }

	public Vector3 destination { get; set; }

	public Vector3[] pendingWaypoints { get; set; }

	public GameObject entity { get; private set; }

	public bool isHandled { get; set; }

	public UnitNavigationEventMessage(GameObject entity)
		: this(entity, Event.None)
	{
	}

	public UnitNavigationEventMessage(GameObject entity, Event e)
	{
		this.entity = entity;
		eventCode = e;
	}
}
