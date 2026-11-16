using UnityEngine;

namespace VRTK.UnityEventHelper;

public abstract class VRTK_UnityEvents<T> : MonoBehaviour where T : Component
{
	private T component;

	protected abstract void AddListeners(T component);

	protected abstract void RemoveListeners(T component);

	protected virtual void OnEnable()
	{
		component = GetComponent<T>();
		if (component != null)
		{
			AddListeners(component);
			return;
		}
		string arg = GetType().Name;
		string arg2 = component.GetType().Name;
		VRTK_Logger.Error($"The {arg} script requires to be attached to a GameObject that contains a {arg2} script.");
	}

	protected virtual void OnDisable()
	{
		if (component != null)
		{
			RemoveListeners(component);
		}
	}
}
