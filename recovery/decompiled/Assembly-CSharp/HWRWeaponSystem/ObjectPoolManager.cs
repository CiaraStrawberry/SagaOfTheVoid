using System.Collections.Generic;
using UnityEngine;

namespace HWRWeaponSystem;

public class ObjectPoolManager : MonoBehaviour
{
	private List<ObjectPool> usedObject;

	private void Start()
	{
		usedObject = new List<ObjectPool>();
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float lifeTime)
	{
		if (usedObject == null)
		{
			return null;
		}
		foreach (ObjectPool item in usedObject)
		{
			if (item != null && item.Prefab.gameObject == obj.gameObject && !item.Active)
			{
				item.OnSpawn(position, obj.transform.localScale, rotation, obj, lifeTime);
				return item.gameObject;
			}
		}
		GameObject gameObject = Object.Instantiate(obj.gameObject, position, obj.transform.rotation);
		ObjectPool component = gameObject.GetComponent<ObjectPool>();
		if (component != null)
		{
			component.OnSpawn(position, gameObject.transform.localScale, rotation, obj, lifeTime);
			usedObject.Add(component);
		}
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation)
	{
		if (usedObject == null)
		{
			return null;
		}
		foreach (ObjectPool item in usedObject)
		{
			if (item != null && item.Prefab.gameObject == obj.gameObject && !item.Active)
			{
				item.OnSpawn(position, obj.transform.localScale, rotation, obj, -1f);
				return item.gameObject;
			}
		}
		GameObject gameObject = Object.Instantiate(obj.gameObject, position, obj.transform.rotation);
		ObjectPool component = gameObject.GetComponent<ObjectPool>();
		if (component != null)
		{
			component.OnSpawn(position, gameObject.transform.localScale, rotation, obj, -1f);
			usedObject.Add(component);
		}
		return gameObject;
	}
}
