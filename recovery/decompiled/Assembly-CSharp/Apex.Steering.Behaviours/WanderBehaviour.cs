using System.Collections;
using Apex.Messages;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("Apex/Legacy/Navigation/Wander (OLD)", 1023)]
[ApexComponent("Behaviours")]
public class WanderBehaviour : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
{
	public float radius = 10f;

	public float minimumDistance = 4f;

	public float lingerForSeconds;

	public int bailAfterFailedAttempts = 100;

	private IUnitFacade _unit;

	private Vector3 _startPos;

	private void Awake()
	{
		this.WarnIfMultipleInstances();
		_unit = this.GetUnitFacade();
		if (_unit == null)
		{
			Debug.LogError("WanderBehaviour requires a component that implements IMovable.");
			enabled = false;
		}
	}

	protected override void OnStartAndEnable()
	{
		GameServices.messageBus.Subscribe(this);
		_startPos = _unit.position;
		MoveNext(append: false);
		if (lingerForSeconds == 0f)
		{
			MoveNext(append: true);
		}
	}

	private void OnDisable()
	{
		GameServices.messageBus.Unsubscribe(this);
	}

	void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
	{
		if (message.entity != gameObject || message.isHandled)
		{
			return;
		}
		if (message.eventCode == UnitNavigationEventMessage.Event.WaypointReached)
		{
			message.isHandled = true;
			MoveNext(append: true);
		}
		else if (message.eventCode == UnitNavigationEventMessage.Event.DestinationReached)
		{
			message.isHandled = true;
			StartCoroutine(DelayedMove());
		}
		else if (message.eventCode == UnitNavigationEventMessage.Event.StoppedNoRouteExists)
		{
			message.isHandled = true;
			MoveNext(append: false);
			if (lingerForSeconds == 0f)
			{
				MoveNext(append: true);
			}
		}
	}

	private IEnumerator DelayedMove()
	{
		yield return new WaitForSeconds(lingerForSeconds);
		MoveNext(append: false);
	}

	private void MoveNext(bool append)
	{
		Vector3 vector = Vector3.zero;
		bool flag = false;
		int num = 0;
		while (!flag && num < bailAfterFailedAttempts)
		{
			vector = _startPos + Random.insideUnitSphere.OnlyXZ() * Random.Range(1f, radius);
			Vector3 vector2 = _unit.position.DirToXZ(vector);
			if (vector2.sqrMagnitude < minimumDistance * minimumDistance)
			{
				vector = _unit.position + vector2.normalized * minimumDistance;
			}
			IGrid grid = GridManager.instance.GetGrid(vector);
			if (grid != null)
			{
				Cell cell = grid.GetCell(vector, adjustToBounds: true);
				flag = cell.IsWalkableWithClearance(_unit);
			}
			else
			{
				flag = true;
			}
			num++;
		}
		_unit.MoveTo(vector, append);
	}
}
