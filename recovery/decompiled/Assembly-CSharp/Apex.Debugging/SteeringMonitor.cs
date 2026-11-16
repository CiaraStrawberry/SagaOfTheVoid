using Apex.Messages;
using Apex.Services;
using UnityEngine;

namespace Apex.Debugging;

[AddComponentMenu("Apex/Game World/Debugging/Steering Monitor", 1016)]
[ApexComponent("Debugging")]
public class SteeringMonitor : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
{
	public GameObject unitFilter;

	public UnitNavigationEventMessage.Event eventFilter;

	protected override void OnStartAndEnable()
	{
		GameServices.messageBus.Subscribe(this);
	}

	private void OnDisable()
	{
		GameServices.messageBus.Unsubscribe(this);
	}

	void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
	{
		if ((!(unitFilter != null) || unitFilter.Equals(message.entity)) && (eventFilter == UnitNavigationEventMessage.Event.None || message.eventCode == eventFilter))
		{
			switch (message.eventCode)
			{
			case UnitNavigationEventMessage.Event.WaypointReached:
			case UnitNavigationEventMessage.Event.DestinationReached:
			case UnitNavigationEventMessage.Event.NodeReached:
				Debug.Log($"Unit '{message.entity.name}' ({message.entity.transform.position}) reports: {message.eventCode} at position: {message.destination}.");
				break;
			case UnitNavigationEventMessage.Event.StoppedNoRouteExists:
			case UnitNavigationEventMessage.Event.StoppedDestinationBlocked:
			case UnitNavigationEventMessage.Event.StoppedRequestDecayed:
			case UnitNavigationEventMessage.Event.Stuck:
				Debug.Log(string.Format("Unit '{0}' ({1}) reports: {2} moving towards: {3}.{4}", message.entity.name, message.entity.transform.position, message.eventCode, message.destination, (!message.isHandled) ? string.Empty : " (Handled)"));
				break;
			}
		}
	}
}
