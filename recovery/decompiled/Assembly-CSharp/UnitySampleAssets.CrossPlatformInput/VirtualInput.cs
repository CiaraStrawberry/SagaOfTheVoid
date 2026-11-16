using System.Collections.Generic;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput;

public abstract class VirtualInput
{
	protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> virtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

	protected Dictionary<string, CrossPlatformInputManager.VirtualButton> virtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

	protected List<string> alwaysUseVirtual = new List<string>();

	public Vector3 virtualMousePosition { get; private set; }

	public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
	{
		if (virtualAxes.ContainsKey(axis.name))
		{
			Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
			return;
		}
		virtualAxes.Add(axis.name, axis);
		if (!axis.matchWithInputManager)
		{
			alwaysUseVirtual.Add(axis.name);
		}
	}

	public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
	{
		if (virtualButtons.ContainsKey(button.name))
		{
			Debug.LogError("There is already a virtual button named " + button.name + " registered.");
			return;
		}
		virtualButtons.Add(button.name, button);
		if (!button.matchWithInputManager)
		{
			alwaysUseVirtual.Add(button.name);
		}
	}

	public void UnRegisterVirtualAxis(string name)
	{
		if (virtualAxes.ContainsKey(name))
		{
			virtualAxes.Remove(name);
		}
	}

	public void UnRegisterVirtualButton(string name)
	{
		if (virtualButtons.ContainsKey(name))
		{
			virtualButtons.Remove(name);
		}
	}

	public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
	{
		return (!virtualAxes.ContainsKey(name)) ? null : virtualAxes[name];
	}

	public void SetVirtualMousePositionX(float f)
	{
		virtualMousePosition = new Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
	}

	public void SetVirtualMousePositionY(float f)
	{
		virtualMousePosition = new Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
	}

	public void SetVirtualMousePositionZ(float f)
	{
		virtualMousePosition = new Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
	}

	public abstract float GetAxis(string name, bool raw);

	public abstract bool GetButton(string name);

	public abstract bool GetButtonDown(string name);

	public abstract bool GetButtonUp(string name);

	public abstract void SetButtonDown(string name);

	public abstract void SetButtonUp(string name);

	public abstract void SetAxisPositive(string name);

	public abstract void SetAxisNegative(string name);

	public abstract void SetAxisZero(string name);

	public abstract void SetAxis(string name, float value);

	public abstract Vector3 MousePosition();
}
