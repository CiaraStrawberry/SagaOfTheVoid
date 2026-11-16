using System.Collections;
using Apex.Messages;
using Apex.Services;
using Apex.Steering.Props;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("Apex/Legacy/Navigation/Patrol (OLD)", 1020)]
[ApexComponent("Behaviours")]
public class PatrolBehaviour : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
{
	public PatrolRoute route;

	public float lingerAtNodesForSeconds;

	public bool reverseRoute;

	public bool randomize;

	private IMovable _mover;

	private int _currentPatrolPointIdx;

	private void Awake()
	{
		this.WarnIfMultipleInstances();
		_mover = this.As<IMovable>();
		if (_mover == null)
		{
			Debug.LogError("PatrolBehaviour requires a component that implements IMovable.");
			enabled = false;
		}
	}

	protected override void Start()
	{
		if (route == null || route.patrolPoints.Length < 2)
		{
			Debug.LogWarning("A patrol route with at least two points is required to patrol.");
			enabled = false;
		}
		else
		{
			_currentPatrolPointIdx = -1;
			base.Start();
		}
	}

	protected override void OnStartAndEnable()
	{
		GameServices.messageBus.Subscribe(this);
		MoveNext(append: false);
		if (lingerAtNodesForSeconds == 0f)
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
		if (!(message.entity != gameObject) && !message.isHandled)
		{
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
		}
	}

	private IEnumerator DelayedMove()
	{
		yield return new WaitForSeconds(lingerAtNodesForSeconds);
		MoveNext(append: false);
	}

	private void MoveNext(bool append)
	{
		PatrolPoint[] patrolPoints = route.patrolPoints;
		if (randomize)
		{
			int currentPatrolPointIdx = _currentPatrolPointIdx;
			while (currentPatrolPointIdx == _currentPatrolPointIdx)
			{
				_currentPatrolPointIdx = Random.Range(0, patrolPoints.Length - 1);
			}
		}
		else
		{
			_currentPatrolPointIdx = ++_currentPatrolPointIdx % patrolPoints.Length;
		}
		int num = _currentPatrolPointIdx;
		if (reverseRoute)
		{
			num = patrolPoints.Length - 1 - _currentPatrolPointIdx;
		}
		_mover.MoveTo(patrolPoints[num].position, append);
	}
}
