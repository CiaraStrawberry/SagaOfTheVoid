using Devdog.General.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class UseButtonOnKey : MonoBehaviour
{
	public KeyCode useKeyCode = KeyCode.Return;

	private UIWindow _window;

	protected virtual void Awake()
	{
		_window = GetComponent<UIWindow>();
	}

	protected void Update()
	{
		if (!isActiveAndEnabled || (_window != null && !_window.isVisible) || !Input.GetKeyDown(useKeyCode))
		{
			return;
		}
		GameObject currentlySelectedGameObject = UIUtility.currentlySelectedGameObject;
		if (currentlySelectedGameObject != null)
		{
			Button component = currentlySelectedGameObject.GetComponent<Button>();
			if (component != null)
			{
				component.OnPointerClick(new PointerEventData(EventSystem.current));
			}
		}
	}
}
