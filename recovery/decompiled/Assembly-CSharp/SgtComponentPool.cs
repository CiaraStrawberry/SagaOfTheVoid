using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtComponentPool : MonoBehaviour
{
	public static List<SgtComponentPool> AllComponentPools = new List<SgtComponentPool>();

	[Tooltip("The name of the type this pool manages")]
	public string TypeName;

	[Tooltip("The pooled elements in this pool")]
	public List<Component> Elements = new List<Component>();

	protected virtual void OnEnable()
	{
		AllComponentPools.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllComponentPools.Remove(this);
	}

	protected virtual void OnDestroy()
	{
		for (int num = Elements.Count - 1; num >= 0; num--)
		{
			Component component = Elements[num];
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component.gameObject);
			}
		}
	}
}
public static class SgtComponentPool<T> where T : Component
{
	private static SgtComponentPool pool;

	public static int Count
	{
		get
		{
			UpdateComponent(allowCreation: false);
			if (pool != null)
			{
				return pool.Elements.Count;
			}
			return 0;
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
			element.transform.SetParent(pool.transform, worldPositionStays: false);
			element.gameObject.SetActive(value: false);
			pool.Elements.Add(element);
		}
		return (T)null;
	}

	public static void Cache()
	{
		UpdateComponent(allowCreation: true);
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(pool.transform, worldPositionStays: false);
		gameObject.SetActive(value: false);
		T item = gameObject.AddComponent<T>();
		pool.Elements.Add(item);
	}

	public static T Pop(Transform parent, string name, int layer)
	{
		T val = Pop();
		if (val != null)
		{
			val.name = name;
			val.transform.SetParent(parent, worldPositionStays: false);
			val.gameObject.layer = layer;
		}
		else
		{
			GameObject gameObject = new GameObject(name);
			gameObject.layer = layer;
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			val = gameObject.AddComponent<T>();
		}
		return val;
	}

	public static T Pop()
	{
		return Pop(null);
	}

	public static T Pop(Predicate<T> match)
	{
		UpdateComponent(allowCreation: false);
		if (pool != null)
		{
			List<Component> elements = pool.Elements;
			int num = elements.Count - 1;
			if (match != null)
			{
				for (int num2 = num; num2 >= 0; num2--)
				{
					Component component = elements[num2];
					if (match((T)component))
					{
						num = num2;
						break;
					}
				}
			}
			if (num >= 0)
			{
				T val = (T)elements[num];
				elements.RemoveAt(num);
				if (val != null)
				{
					val.gameObject.SetActive(value: true);
				}
				return val;
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
		pool = SgtComponentPool.AllComponentPools.Find((SgtComponentPool p) => p.TypeName == typeName);
		if (pool == null && allowCreation)
		{
			pool = new GameObject("SgtComponentPool<" + typeName + ">").AddComponent<SgtComponentPool>();
			pool.TypeName = typeName;
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(pool);
			}
		}
	}
}
