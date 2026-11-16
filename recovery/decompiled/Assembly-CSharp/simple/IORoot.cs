using UnityEngine;

namespace simple;

public class IORoot : IORootBase
{
	public static IOComponent findIO(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return null;
		}
		Object[] array = Object.FindObjectsOfType(typeof(IORoot));
		for (int i = 0; i < array.Length; i++)
		{
			IORoot iORoot = (IORoot)array[i];
			IOComponent[] componentsInChildren = iORoot.gameObject.GetComponentsInChildren<IOComponent>();
			if (componentsInChildren == null || componentsInChildren.Length <= 0)
			{
				continue;
			}
			IOComponent[] array2 = componentsInChildren;
			foreach (IOComponent iOComponent in array2)
			{
				if (iOComponent.isEqualFile(fileName))
				{
					return iOComponent;
				}
			}
		}
		Debug.LogWarning("Not found file name : " + fileName);
		return null;
	}

	public static bool saveComponent(MonoBehaviour mono)
	{
		return saveComponent(mono.gameObject);
	}

	public static bool saveComponent(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		Object[] array = Object.FindObjectsOfType(typeof(IORoot));
		for (int i = 0; i < array.Length; i++)
		{
			IORoot iORoot = (IORoot)array[i];
			if (iORoot.saveProperty(go))
			{
				return true;
			}
		}
		Debug.LogWarning("Not found attach component : Check IORoot inspector!");
		return false;
	}
}
