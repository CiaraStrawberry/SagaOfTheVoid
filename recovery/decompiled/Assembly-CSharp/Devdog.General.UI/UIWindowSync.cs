using UnityEngine;

namespace Devdog.General.UI;

[RequireComponent(typeof(UIWindow))]
public class UIWindowSync : MonoBehaviour
{
	public UIWindow[] otherWindows = new UIWindow[0];

	public bool showWhenShown = true;

	public bool hideWhenHidden = true;

	public void Awake()
	{
		UIWindow component = GetComponent<UIWindow>();
		component.OnHide += () =>
		{
			UIWindow[] array = otherWindows;
			foreach (UIWindow uIWindow in array)
			{
				if (!(uIWindow == null) && hideWhenHidden)
				{
					uIWindow.Hide();
				}
			}
		};
		component.OnShow += () =>
		{
			UIWindow[] array = otherWindows;
			foreach (UIWindow uIWindow in array)
			{
				if (!(uIWindow == null) && showWhenShown)
				{
					uIWindow.Show();
				}
			}
		};
	}
}
