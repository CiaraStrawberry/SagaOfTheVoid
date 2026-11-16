using System;
using UnityEngine;

namespace Devdog.General.UI;

[Serializable]
public class UIWindowField : UIWindowField<UIWindow>
{
}
[Serializable]
public class UIWindowField<T> where T : UIWindow
{
	public bool useDynamicSearch;

	public string name;

	[SerializeField]
	private T _window;

	public T window
	{
		get
		{
			if (_window == null && useDynamicSearch)
			{
				_window = UIWindowUtility.FindByName<T>(name);
			}
			return _window;
		}
		set
		{
			_window = value;
		}
	}
}
