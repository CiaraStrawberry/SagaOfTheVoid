using System.Collections.Generic;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Behaviours;

public static class WanderMaster
{
	private class WanderClient
	{
		internal float radius = 10f;

		internal float minimumDistance = 4f;

		internal float lingerForSeconds;

		internal IUnitFacade unit;

		internal void MoveNext()
		{
			WanderMaster.MoveNext(this, false);
		}
	}

	private class Handler : IHandleMessage<UnitNavigationEventMessage>
	{
		public Handler()
		{
			GameServices.messageBus.Subscribe(this);
		}

		public void Stop()
		{
			GameServices.messageBus.Unsubscribe(this);
		}

		void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
		{
			if (!message.isHandled)
			{
				switch (message.eventCode)
				{
				case UnitNavigationEventMessage.Event.WaypointReached:
					message.isHandled = MoveNext(message.entity, append: true);
					break;
				case UnitNavigationEventMessage.Event.DestinationReached:
					message.isHandled = DelayedMove(message.entity);
					break;
				case UnitNavigationEventMessage.Event.StoppedNoRouteExists:
				case UnitNavigationEventMessage.Event.StoppedDestinationBlocked:
				case UnitNavigationEventMessage.Event.Stuck:
					message.isHandled = Start(message.entity);
					break;
				case UnitNavigationEventMessage.Event.StoppedRequestDecayed:
					break;
				}
			}
		}
	}

	public static int bailAfterFailedAttempts = 100;

	private static readonly Dictionary<GameObject, WanderClient> _clients = new Dictionary<GameObject, WanderClient>();

	private static Handler _handler;

	public static void Wander(this IUnitFacade unit, float radius, float minimumDistance, float lingerForSeconds)
	{
		if (_handler == null)
		{
			_handler = new Handler();
		}
		WanderClient wanderClient = new WanderClient();
		wanderClient.radius = radius;
		wanderClient.minimumDistance = minimumDistance;
		wanderClient.lingerForSeconds = lingerForSeconds;
		wanderClient.unit = unit;
		WanderClient wanderClient2 = wanderClient;
		_clients.Add(unit.gameObject, wanderClient2);
		Start(wanderClient2);
	}

	public static void StopWander(this IUnitFacade unit)
	{
		_clients.Remove(unit.gameObject);
		if (_clients.Count == 0 && _handler != null)
		{
			_handler.Stop();
			_handler = null;
		}
		if (unit.isAlive)
		{
			unit.Stop();
		}
	}

	private static bool DelayedMove(GameObject go)
	{
		if (!_clients.TryGetValue(go, out var value))
		{
			return false;
		}
		LoadBalancer.defaultBalancer.ExecuteOnce(value.MoveNext, value.lingerForSeconds);
		return true;
	}

	private static bool Start(GameObject go)
	{
		if (!_clients.TryGetValue(go, out var value))
		{
			return false;
		}
		Start(value);
		return true;
	}

	private static void Start(WanderClient client)
	{
		MoveNext(client.unit.gameObject, append: false);
		if (client.lingerForSeconds == 0f)
		{
			MoveNext(client.unit.gameObject, append: true);
		}
	}

	private static bool MoveNext(GameObject go, bool append)
	{
		if (!_clients.TryGetValue(go, out var value))
		{
			return false;
		}
		MoveNext(value, append);
		return true;
	}

	private static void MoveNext(WanderClient client, bool append)
	{
		if (!client.unit.isAlive)
		{
			client.unit.StopWander();
			return;
		}
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < bailAfterFailedAttempts; i++)
		{
			zero = client.unit.position + Random.insideUnitSphere.normalized.OnlyXZ() * Random.Range(client.minimumDistance, client.radius);
			IGrid grid = GridManager.instance.GetGrid(zero);
			if (grid != null)
			{
				Cell cell = grid.GetCell(zero, adjustToBounds: true);
				if (cell.IsWalkableWithClearance(client.unit))
				{
					client.unit.MoveTo(cell.position, append);
					return;
				}
			}
		}
		client.unit.StopWander();
	}
}
