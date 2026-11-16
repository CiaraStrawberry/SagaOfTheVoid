using UnityEngine;

namespace Devdog.General.UI;

[RequireComponent(typeof(UIWindow))]
public class UIWindowInputHandler : MonoBehaviour, IUIWindowInputHandler
{
	public KeyCode keyCode;

	protected UIWindow window;

	protected virtual void Awake()
	{
		window = GetComponent<UIWindow>();
	}

	protected virtual void Update()
	{
		if (Input.GetKeyDown(keyCode))
		{
			window.Toggle();
		}
	}
}
