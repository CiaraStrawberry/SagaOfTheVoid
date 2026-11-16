using Apex.Messages;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("")]
[ApexComponent("Behaviours")]
public class ReplanWhenBlockedBehaviour : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
{
	public int scanRadius = 2;

	private IUnitFacade _unit;

	private void Awake()
	{
		this.WarnIfMultipleInstances();
		_unit = this.GetUnitFacade();
		if (_unit == null)
		{
			Debug.LogError("ReplanWhenBlockedBehaviour requires a component that implements IMovable.");
			enabled = false;
		}
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
		if (message.eventCode != UnitNavigationEventMessage.Event.StoppedDestinationBlocked)
		{
			return;
		}
		IGrid grid = GridManager.instance.GetGrid(message.destination);
		if (grid != null)
		{
			Cell nearestWalkableCell = grid.GetNearestWalkableCell(message.destination, transform.position, requireWalkableFromPosition: false, scanRadius, _unit);
			if (nearestWalkableCell != null)
			{
				message.isHandled = true;
				_unit.MoveTo(nearestWalkableCell.position, append: false);
			}
		}
	}

	private bool Walkable(Cell c)
	{
		return c.IsWalkableWithClearance(_unit);
	}

	private bool NotWalkable(Cell c)
	{
		return !c.IsWalkableWithClearance(_unit);
	}
}
