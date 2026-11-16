using System.Collections.Generic;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.Services;
using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Behaviours;

public static class PatrolMaster
{
	private class PatrolClient
	{
		private int _currentIndex;

		private int _nextIndex;

		private bool _stopped;

		internal Vector3[] route;

		internal IUnitFacade unit;

		internal bool randomize;

		internal bool reverseRoute;

		internal float lingerForSeconds;

		internal void Start()
		{
			_nextIndex = ((!reverseRoute) ? (-1) : route.Length);
			IncrementNext();
			_currentIndex = _nextIndex;
			IncrementNext();
			Resume();
		}

		internal void Stop()
		{
			unit.Stop();
			_stopped = true;
		}

		internal void Resume()
		{
			_stopped = false;
			unit.MoveTo(route[_currentIndex], append: false);
			if (lingerForSeconds == 0f)
			{
				unit.MoveTo(route[_nextIndex], append: true);
			}
		}

		internal void Continue()
		{
			if (_stopped)
			{
				return;
			}
			if (!unit.isAlive)
			{
				unit.StopPatrol();
				return;
			}
			_currentIndex = _nextIndex;
			IncrementNext();
			if (lingerForSeconds == 0f)
			{
				unit.MoveTo(route[_nextIndex], append: true);
			}
			else
			{
				unit.MoveTo(route[_currentIndex], append: false);
			}
		}

		private void IncrementNext()
		{
			if (randomize)
			{
				int nextIndex = _nextIndex;
				while (nextIndex == _nextIndex)
				{
					_nextIndex = Random.Range(0, route.Length);
				}
			}
			else if (reverseRoute)
			{
				_nextIndex = (--_nextIndex + route.Length) % route.Length;
			}
			else
			{
				_nextIndex = ++_nextIndex % route.Length;
			}
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
				case UnitNavigationEventMessage.Event.DestinationReached:
					message.isHandled = Continue(message.entity);
					break;
				case UnitNavigationEventMessage.Event.StoppedNoRouteExists:
				case UnitNavigationEventMessage.Event.StoppedDestinationBlocked:
				case UnitNavigationEventMessage.Event.Stuck:
					message.isHandled = PatrolMaster.Stop(message.entity);
					break;
				case UnitNavigationEventMessage.Event.StoppedRequestDecayed:
					break;
				}
			}
		}
	}

	private static readonly Dictionary<GameObject, PatrolClient> _clients = new Dictionary<GameObject, PatrolClient>();

	private static readonly Dictionary<GameObject, PatrolClient> _pausedClients = new Dictionary<GameObject, PatrolClient>();

	private static Handler _handler;

	public static void Patrol(this IUnitFacade unit, Vector3[] patrolPoints, bool randomize = false, bool reverse = false, float lingerForSeconds = 0f)
	{
		if (_handler == null)
		{
			_handler = new Handler();
		}
		PatrolClient patrolClient = new PatrolClient();
		patrolClient.route = patrolPoints;
		patrolClient.randomize = randomize;
		patrolClient.reverseRoute = reverse;
		patrolClient.lingerForSeconds = lingerForSeconds;
		patrolClient.unit = unit;
		PatrolClient patrolClient2 = patrolClient;
		_clients[unit.gameObject] = patrolClient2;
		patrolClient2.Start();
	}

	public static void StopPatrol(this IUnitFacade unit)
	{
		if (_clients.TryGetValue(unit.gameObject, out var value) || _pausedClients.TryGetValue(unit.gameObject, out value))
		{
			_clients.Remove(unit.gameObject);
			_pausedClients.Remove(unit.gameObject);
			if (_clients.Count == 0)
			{
				_handler.Stop();
				_handler = null;
			}
			if (unit.isAlive)
			{
				value.Stop();
			}
		}
	}

	public static void PausePatrol(this IUnitFacade unit)
	{
		if (_clients.TryGetValue(unit.gameObject, out var value))
		{
			_clients.Remove(unit.gameObject);
			if (unit.isAlive)
			{
				_pausedClients[unit.gameObject] = value;
				value.Stop();
			}
		}
	}

	public static bool ResumePatrol(this IUnitFacade unit)
	{
		if (!_pausedClients.TryGetValue(unit.gameObject, out var value))
		{
			return false;
		}
		_pausedClients.Remove(unit.gameObject);
		if (unit.isAlive)
		{
			_clients[unit.gameObject] = value;
			value.Resume();
			return true;
		}
		return false;
	}

	public static bool IsOnPatrol(this IUnitFacade unit)
	{
		return _clients.ContainsKey(unit.gameObject) || _pausedClients.ContainsKey(unit.gameObject);
	}

	private static bool Continue(GameObject go)
	{
		if (!_clients.TryGetValue(go, out var value))
		{
			return false;
		}
		if (value.lingerForSeconds > 0f)
		{
			LoadBalancer.defaultBalancer.ExecuteOnce(value.Continue, value.lingerForSeconds);
		}
		else
		{
			value.Continue();
		}
		return true;
	}

	private static bool Stop(GameObject go)
	{
		IUnitFacade unitFacade = go.GetUnitFacade(createIfMissing: false);
		if (unitFacade == null)
		{
			return false;
		}
		if (!unitFacade.IsOnPatrol())
		{
			return false;
		}
		unitFacade.StopPatrol();
		return true;
	}
}
