using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VivePaddleUIController : MonoBehaviour
{
	private aMenuState _CurrentMenu;

	public aMenuState DefaultMenu;

	public aMenuState OnRotateDownMenu;

	public aMenuState OnRotateUpMenu;

	public List<aMenuState> CachedMenus;

	public void TransitionTo(aMenuState go)
	{
		if (go != null)
		{
			go.OnTransitionTo();
		}
		else if (go == null && _CurrentMenu != null)
		{
			_CurrentMenu.OnTransitionFrom();
			_CurrentMenu.OnDeactivate();
			Object.DestroyImmediate(_CurrentMenu);
		}
		if (_CurrentMenu != null)
		{
			_CurrentMenu.OnTransitionFrom();
		}
		if (_CurrentMenu != null)
		{
			_CurrentMenu.OnDeactivate();
			Object.DestroyImmediate(_CurrentMenu);
		}
		go.OnActive();
		_CurrentMenu = go;
	}

	private void Start()
	{
		if (DefaultMenu != null)
		{
			_CurrentMenu = Object.Instantiate(DefaultMenu);
			_CurrentMenu.transform.parent = transform;
			_CurrentMenu.OnTransitionTo();
			_CurrentMenu.OnActive();
		}
	}

	private void Update()
	{
	}
}
