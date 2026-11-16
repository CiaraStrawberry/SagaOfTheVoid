using Apex.Messages;
using Apex.Services;
using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("Apex/Unit/Navigation/Replan When Stuck", 1030)]
[ApexComponent("Behaviours")]
public class ReplanWhenStuckBehaviour : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
{
	private int _currentRetries;

	public int maxRetries = 3;

	private void Awake()
	{
		this.WarnIfMultipleInstances();
	}

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
		if (message.entity != gameObject)
		{
			return;
		}
		if (message.eventCode == UnitNavigationEventMessage.Event.Stuck)
		{
			if (_currentRetries++ > maxRetries)
			{
				return;
			}
			IUnitFacade unitFacade = this.GetUnitFacade();
			if (unitFacade == null)
			{
				return;
			}
			unitFacade.MoveTo(message.destination, append: false);
			if (message.pendingWaypoints != null)
			{
				Vector3[] pendingWaypoints = message.pendingWaypoints;
				int num = pendingWaypoints.Length;
				for (int i = 0; i < num; i++)
				{
					unitFacade.MoveTo(pendingWaypoints[i], append: true);
				}
			}
		}
		else
		{
			_currentRetries = 0;
		}
	}
}
