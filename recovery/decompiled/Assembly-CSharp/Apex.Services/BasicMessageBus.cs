using System;
using System.Collections;
using System.Collections.Generic;
using Apex.LoadBalancing;

namespace Apex.Services;

public class BasicMessageBus : IMessageBus
{
	private IDictionary<Type, IList<object>> _subscriptions;

	public BasicMessageBus()
	{
		_subscriptions = new Dictionary<Type, IList<object>>();
	}

	public void Subscribe<T>(IHandleMessage<T> subscriber)
	{
		if (!_subscriptions.TryGetValue(typeof(T), out var value))
		{
			value = new List<object>();
			_subscriptions.Add(typeof(T), value);
		}
		value.Add(subscriber);
	}

	public void Unsubscribe<T>(IHandleMessage<T> subscriber)
	{
		if (_subscriptions.TryGetValue(typeof(T), out var value))
		{
			value.Remove(subscriber);
			if (value.Count == 0)
			{
				_subscriptions.Remove(typeof(T));
			}
		}
	}

	public void Post<T>(T message)
	{
		if (_subscriptions.TryGetValue(typeof(T), out var value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				IHandleMessage<T> handleMessage = value[i] as IHandleMessage<T>;
				handleMessage.Handle(message);
			}
		}
	}

	public void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame)
	{
		PostBalanced(message, maxMillisecondUsedPerFrame, null);
	}

	public void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame, Action callback)
	{
		if (!_subscriptions.TryGetValue(typeof(T), out var subscribers))
		{
			callback?.Invoke();
			return;
		}
		LongRunningAction item = new LongRunningAction(() => BalancedPoster(subscribers, message), maxMillisecondUsedPerFrame, callback);
		LoadBalancer.defaultBalancer.Add(item, 0f);
	}

	private IEnumerator BalancedPoster<T>(IList<object> subscribers, T message)
	{
		for (int i = 0; i < subscribers.Count; i++)
		{
			IHandleMessage<T> subscriber = subscribers[i] as IHandleMessage<T>;
			subscriber.Handle(message);
			yield return null;
		}
	}
}
