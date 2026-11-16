using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General.UI;

public static class UIWindowUtility
{
	public static UIWindow FindByName(string name)
	{
		return FindByName<UIWindow>(name);
	}

	public static T FindByName<T>(string name) where T : UIWindow
	{
		return Object.FindObjectsOfType<T>().FirstOrDefault((T o) => o.windowName == name);
	}

	public static UIWindow[] GetAllWindowsWithInput()
	{
		return GetAllWindowsWithInput<UIWindow>();
	}

	public static T[] GetAllWindowsWithInput<T>() where T : UIWindow
	{
		return (from o in Object.FindObjectsOfType<T>()
			where o.GetComponent<IUIWindowInputHandler>() != null
			select o).ToArray();
	}
}
