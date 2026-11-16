using System;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Devdog.SciFiDesign.UI;

[AddComponentMenu("Sci-Fi Design/UI Helpers/UI Element Key Actions")]
public class UIElementKeyActions : MonoBehaviour
{
	[Serializable]
	public class KeyAction : UnityEvent
	{
	}

	[Header("Action")]
	public KeyCode keyCode;

	public KeyAction keyActions = new KeyAction();

	[Header("Timers")]
	public float activationTime;

	public bool continous;

	[Header("Visuals")]
	public UIShowValue visualizer = new UIShowValue();

	private float _activeTime { get; set; }

	private bool _firedInActiveTime { get; set; }

	private UIWindow window { get; set; }

	protected virtual void Awake()
	{
		window = GetComponent<UIWindow>();
	}

	protected virtual void Update()
	{
		if (!gameObject.activeInHierarchy || (window != null && !window.isVisible))
		{
			return;
		}
		if (activationTime <= 0.01f)
		{
			if (continous)
			{
				if (Input.GetKey(keyCode))
				{
					Activate();
				}
			}
			else if (Input.GetKeyDown(keyCode))
			{
				Activate();
			}
			return;
		}
		if (Input.GetKey(keyCode))
		{
			_activeTime += Time.deltaTime;
			visualizer.Repaint(_activeTime, activationTime);
		}
		else
		{
			_activeTime = 0f;
			_firedInActiveTime = false;
			visualizer.Repaint(0f, 1f);
		}
		if (_activeTime < activationTime)
		{
			return;
		}
		if (continous)
		{
			if (Input.GetKey(keyCode))
			{
				keyActions.Invoke();
			}
		}
		else if (!_firedInActiveTime && Input.GetKey(keyCode))
		{
			keyActions.Invoke();
			_firedInActiveTime = true;
		}
	}

	protected virtual void Activate()
	{
		if (InputManager.CanReceiveUIInput(gameObject))
		{
			keyActions.Invoke();
			visualizer.Activate();
		}
	}
}
