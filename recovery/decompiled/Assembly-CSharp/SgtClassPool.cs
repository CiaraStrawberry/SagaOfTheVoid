using System;
using System.Collections.Generic;
using UnityEngine;

public static class SgtClassPool<T> where T : class
{
	private static List<T> pool;

	public static int Count => pool.Count;

	static SgtClassPool()
	{
		pool = new List<T>();
		if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
		{
			Debug.LogError("Attempting to use " + typeof(T).Name + " with SgtClassPool. Use SgtObjectPool instead.");
		}
	}

	public static T Add(T entry)
	{
		return Add(entry, null);
	}

	public static T Add(T element, Action<T> onAdd)
	{
		if (element != null)
		{
			onAdd?.Invoke(element);
			pool.Add(element);
		}
		return (T)null;
	}

	public static T Pop()
	{
		if (pool.Count > 0)
		{
			int index = pool.Count - 1;
			T result = pool[index];
			pool.RemoveAt(index);
			return result;
		}
		return (T)null;
	}
}
