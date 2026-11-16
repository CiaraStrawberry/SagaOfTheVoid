using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtObjectPool : MonoBehaviour
{
	public static List<SgtObjectPool> AllObjectPools = new List<SgtObjectPool>();

	public string TypeName;

	public List<UnityEngine.Object> Elements = new List<UnityEngine.Object>();

	protected virtual void OnEnable()
	{
		AllObjectPools.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllObjectPools.Remove(this);
	}

	protected virtual void OnDestroy()
	{
		for (int num = Elements.Count - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(Elements[num]);
		}
	}
}
public static class SgtObjectPool<T> where T : UnityEngine.Object
{
	private static SgtObjectPool pool;

	static SgtObjectPool()
	{
		if (typeof(T).IsSubclassOf(typeof(Component)))
		{
			Debug.LogError("Attempting to use " + typeof(T).Name + " with SgtObjectPool. Use SgtComponentPool instead.");
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
			UpdateComponent(allowCreation: true);
			pool.Elements.Add(element);
		}
		return (T)null;
	}

	public static T Pop()
	{
		UpdateComponent(allowCreation: false);
		if (pool != null)
		{
			List<UnityEngine.Object> elements = pool.Elements;
			int count = elements.Count;
			if (count > 0)
			{
				int index = count - 1;
				T result = (T)elements[index];
				elements.RemoveAt(index);
				return result;
			}
		}
		return (T)null;
	}

	private static void UpdateComponent(bool allowCreation)
	{
		if (!(pool == null))
		{
			return;
		}
		string typeName = typeof(T).Name;
		pool = SgtObjectPool.AllObjectPools.Find((SgtObjectPool p) => p.TypeName == typeName);
		if (pool == null && allowCreation)
		{
			pool = new GameObject("SgtObjectPool<" + typeName + ">").AddComponent<SgtObjectPool>();
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(pool);
			}
		}
	}
}
