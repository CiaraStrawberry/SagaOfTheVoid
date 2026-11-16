using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class GameObjectPool : PoolBase<GameObject>
{
	private static List<IPoolable> _interfaceCache = new List<IPoolable>();

	public GameObjectPool(GameObject prefab, int startSize = 32)
		: base(prefab, startSize)
	{
	}

	public override GameObject Instantiate()
	{
		GameObject gameObject = Object.Instantiate(baseObject);
		gameObject.transform.SetParent(rootObject);
		gameObject.gameObject.SetActive(value: false);
		inactiveObjectsPool.Add(gameObject);
		return gameObject;
	}

	public override GameObject Get(bool createWhenNoneLeft = true)
	{
		GameObject gameObject = null;
		if (inactiveObjectsPool.Count == 0)
		{
			if (createWhenNoneLeft)
			{
				DevdogLogger.Log("New object created, considering increasing the pool size if this is logged often");
				gameObject = Instantiate();
			}
		}
		else
		{
			gameObject = inactiveObjectsPool[inactiveObjectsPool.Count - 1];
		}
		gameObject.gameObject.SetActive(value: true);
		gameObject.gameObject.transform.localScale = Vector3.one;
		gameObject.gameObject.transform.localRotation = Quaternion.identity;
		activeObjectsList.Add(gameObject);
		inactiveObjectsPool.RemoveAt(inactiveObjectsPool.Count - 1);
		return gameObject;
	}

	public override void Destroy(GameObject obj)
	{
		obj.transform.SetParent(rootObject);
		obj.SetActive(value: false);
		obj.GetComponents(_interfaceCache);
		foreach (IPoolable item in _interfaceCache)
		{
			item.ResetStateForPool();
		}
		inactiveObjectsPool.Add(obj);
		activeObjectsList.Remove(obj);
	}
}
