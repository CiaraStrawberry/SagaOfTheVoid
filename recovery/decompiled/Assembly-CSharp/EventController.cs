using System;
using System.Collections.Generic;
using UnityEngine;

public class EventController
{
	public struct EventData
	{
		public GameObject TriggeringObject;

		public EventTypes EventType;

		public object[] EventParameters;

		internal EventData(GameObject triggeringObject, EventTypes eventType, params object[] eventParameters)
		{
			TriggeringObject = triggeringObject;
			EventType = eventType;
			EventParameters = eventParameters;
		}
	}

	public enum EventTypes
	{
		ObjectSpawned,
		ObjectWarpedIn,
		ObjectDestroyed,
		AllWidgetsHidden,
		AllGesturesDisabled,
		GestureEnabled,
		AllGesturesEnabled,
		WidgetDisplayed,
		ObjectInProximity,
		ObjectSelected,
		SoundPlayed,
		LookAtAnimated,
		MapChanged
	}

	private object _LockObject = new object();

	private Dictionary<EventTypes, List<Action<EventData>>> _Listeners;

	private static EventController _controller;

	public EventController GetInstance
	{
		get
		{
			lock (_LockObject)
			{
				if (_controller == null)
				{
					_controller = new EventController();
				}
			}
			return _controller;
		}
	}

	private EventController()
	{
		_Listeners = new Dictionary<EventTypes, List<Action<EventData>>>();
	}

	public void AttachListener(EventTypes eventType, Action<EventData> callback)
	{
		lock (_LockObject)
		{
			if (!_Listeners.ContainsKey(eventType))
			{
				_Listeners.Add(eventType, new List<Action<EventData>>());
			}
			_Listeners[eventType].Add(callback);
		}
	}

	public void DetatchListender(EventTypes eventType, Action<EventData> callback)
	{
		lock (_LockObject)
		{
			if (_Listeners.ContainsKey(eventType))
			{
				_Listeners[eventType].Remove(callback);
			}
		}
	}

	public void TriggerEvent(EventTypes eventType, EventData data)
	{
		lock (_LockObject)
		{
			if (!_Listeners.ContainsKey(eventType))
			{
				return;
			}
			foreach (Action<EventData> item in _Listeners[eventType])
			{
				item.DynamicInvoke(data);
			}
		}
	}
}
