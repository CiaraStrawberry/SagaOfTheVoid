using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
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
		else if (_CurrentMenu != null)
		{
			_CurrentMenu.OnTransitionFrom();
			_CurrentMenu.OnDeactivate();
			_CurrentMenu = null;
		}
		if (_CurrentMenu != null)
		{
			_CurrentMenu.OnTransitionFrom();
		}
		go.OnActive();
		_CurrentMenu = go;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
