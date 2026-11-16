using System;
using System.Collections;
using System.Collections.Generic;
using Apex.LoadBalancing;

namespace Apex.Services;

public class AdvancedMessageBus : IMessageBus
{
	private IDictionary<Type, IList<WeakReference>> _subscriptions;

	public AdvancedMessageBus()
	{
		_subscriptions = new Dictionary<Type, IList<WeakReference>>();
	}

	public void Subscribe<T>(IHandleMessage<T> subscriber)
	{
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out var value))
			{
				value = new List<WeakReference>();
				_subscriptions.Add(typeof(T), value);
			}
			lock (value)
			{
				value.Add(new WeakReference(subscriber));
			}
		}
	}

	public void Unsubscribe<T>(IHandleMessage<T> subscriber)
	{
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out var value))
			{
				return;
			}
			lock (value)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (object.ReferenceEquals(value[i].Target, subscriber))
					{
						value.RemoveAt(i);
						break;
					}
				}
				if (value.Count == 0)
				{
					_subscriptions.Remove(typeof(T));
				}
			}
		}
	}

	public void Post<T>(T message)
	{
		IList<WeakReference> value;
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out value))
			{
				return;
			}
		}
		lock (value)
		{
			for (int num = value.Count - 1; num >= 0; num--)
			{
				if (value[num].Target is IHandleMessage<T> handleMessage)
				{
					handleMessage.Handle(message);
				}
				else
				{
					value.RemoveAt(num);
				}
			}
		}
		if (value.Count != 0)
		{
			return;
		}
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out value))
			{
				return;
			}
			lock (value)
			{
				if (value.Count == 0)
				{
					_subscriptions.Remove(typeof(T));
				}
			}
		}
	}

	public int SubscribersFor<T>()
	{
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out var value))
			{
				return 0;
			}
			lock (value)
			{
				return value.Count;
			}
		}
	}

	public void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame)
	{
		PostBalanced(message, maxMillisecondUsedPerFrame, null);
	}

	public void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame, Action callback)
	{
		IList<WeakReference> subscribers;
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out subscribers))
			{
				callback?.Invoke();
				return;
			}
		}
		LongRunningAction item = new LongRunningAction(() => BalancedPoster(subscribers, message), maxMillisecondUsedPerFrame, callback);
		LoadBalancer.defaultBalancer.Add(item, 0f);
	}

	private IEnumerator BalancedPoster<T>(IList<WeakReference> subscribers, T message)
	{
		lock (subscribers)
		{
			for (int i = subscribers.Count - 1; i >= 0; i--)
			{
				if (subscribers[i].Target is IHandleMessage<T> subscriber)
				{
					subscriber.Handle(message);
				}
				else
				{
					subscribers.RemoveAt(i);
				}
				yield return null;
			}
		}
		if (subscribers.Count != 0)
		{
			yield break;
		}
		lock (_subscriptions)
		{
			if (!_subscriptions.TryGetValue(typeof(T), out subscribers))
			{
				yield break;
			}
			lock (subscribers)
			{
				if (subscribers.Count == 0)
				{
					_subscriptions.Remove(typeof(T));
				}
			}
		}
	}
}
