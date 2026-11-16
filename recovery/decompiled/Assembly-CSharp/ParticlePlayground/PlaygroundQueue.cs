using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ParticlePlayground;

public class PlaygroundQueue<T> : IDisposable where T : class
{
	private readonly object _locker = new object();

	private List<Thread> _workers;

	private readonly Queue<T> _taskQueue = new Queue<T>();

	private readonly Action<T> _dequeueAction;

	private int _actionsPerThread = 1;

	public int actionsPerThread
	{
		get
		{
			return _actionsPerThread;
		}
		set
		{
			_actionsPerThread = Mathf.Max(1, value);
		}
	}

	public PlaygroundQueue(int workerCount, Action<T> dequeueAction)
	{
		_dequeueAction = dequeueAction;
		_workers = new List<Thread>(workerCount);
		for (int i = 0; i < workerCount; i++)
		{
			Thread thread = new Thread(Consume)
			{
				IsBackground = true,
				Name = $"PlaygroundQueue worker {i}"
			};
			_workers.Add(thread);
			thread.Start();
		}
	}

	public bool HasPool()
	{
		return _workers.Count > 0;
	}

	public bool HasTasks()
	{
		return _taskQueue.Count > 0;
	}

	public int ThreadPoolCount()
	{
		return _workers.Count;
	}

	public int TaskCount()
	{
		return _taskQueue.Count;
	}

	public void SetThreadPool(int amount)
	{
		if (amount == _workers.Count)
		{
			return;
		}
		amount = Mathf.Clamp(amount, 1, 128);
		if (amount > _workers.Count)
		{
			for (int i = _workers.Count; i < amount; i++)
			{
				Thread thread = new Thread(Consume);
				thread.IsBackground = true;
				thread.Name = $"PlaygroundQueue worker {i}";
				Thread thread2 = thread;
				_workers.Add(thread2);
				thread2.Start();
			}
			return;
		}
		Dispose();
		_workers = new List<Thread>(amount);
		for (int j = 0; j < amount; j++)
		{
			Thread thread = new Thread(Consume);
			thread.IsBackground = true;
			thread.Name = $"PlaygroundQueue worker {j}";
			Thread thread3 = thread;
			_workers.Add(thread3);
			thread3.Start();
		}
	}

	public void EnqueueTask(T task)
	{
		lock (_locker)
		{
			_taskQueue.Enqueue(task);
			Monitor.Pulse(_locker);
		}
	}

	private void Consume()
	{
		while (true)
		{
			for (int i = 0; i < _actionsPerThread; i++)
			{
				T val;
				lock (_locker)
				{
					while (_taskQueue.Count == 0)
					{
						Monitor.Wait(_locker);
					}
					val = _taskQueue.Dequeue();
				}
				if (val == null)
				{
					return;
				}
				_dequeueAction(val);
			}
		}
	}

	public void Dispose()
	{
		_workers.ForEach((Thread thread) =>
		{
			EnqueueTask((T)null);
		});
		_workers.ForEach((Thread thread) =>
		{
			thread.Join();
		});
	}
}
