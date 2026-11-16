using UnityEngine;

namespace Devdog.General;

public static class ComponentExtensionMethod
{
	public static T GetOrAddComponent<T>(this Component comp) where T : Component
	{
		T component = comp.GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		return comp.gameObject.AddComponent<T>();
	}
}
