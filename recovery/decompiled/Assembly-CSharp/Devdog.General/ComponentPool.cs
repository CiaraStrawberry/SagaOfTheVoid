using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class ComponentPool<T> : PoolBase<T> where T : class, IPoolable
{
	private static List<IPoolable> _interfaceCache = new List<IPoolable>();

	public ComponentPool(T baseObject, int startSize = 32)
		: base(baseObject, startSize)
	{
	}

	public override T Instantiate()
	{
		T val = baseObject;
		GameObject gameObject = Object.Instantiate(val.gameObject);
		gameObject.transform.SetParent(rootObject);
		gameObject.gameObject.SetActive(value: false);
		T component = gameObject.GetComponent<T>();
		inactiveObjectsPool.Add(component);
		return component;
	}

	public override T Get(bool createWhenNoneLeft = true)
	{
		T val = (T)null;
		if (inactiveObjectsPool.Count == 0)
		{
			if (createWhenNoneLeft)
			{
				DevdogLogger.Log("New object created, considering increasing the pool size if this is logged often");
				val = Instantiate();
			}
		}
		else
		{
			val = inactiveObjectsPool[inactiveObjectsPool.Count - 1];
		}
		val.gameObject.SetActive(value: true);
		val.gameObject.transform.localScale = Vector3.one;
		val.gameObject.transform.localRotation = Quaternion.identity;
		activeObjectsList.Add(val);
		inactiveObjectsPool.RemoveAt(inactiveObjectsPool.Count - 1);
		return val;
	}

	public override void Destroy(T item)
	{
		item.gameObject.transform.SetParent(rootObject);
		item.gameObject.SetActive(value: false);
		item.gameObject.GetComponents(_interfaceCache);
		foreach (IPoolable item2 in _interfaceCache)
		{
			item2.ResetStateForPool();
		}
		inactiveObjectsPool.Add(item);
		activeObjectsList.Remove(item);
	}
}
