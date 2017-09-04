using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

    /// <summary>
    /// Current active menu
    /// </summary>
    private aMenuState _CurrentMenu;

    /// <summary>
    /// Default menu to transition to
    /// </summary>
    public aMenuState DefaultMenu;

    /// <summary>
    /// Menu for rotate down gesture
    /// </summary>
    public aMenuState OnRotateDownMenu;

    /// <summary>
    /// Menu for rotate up gesture
    /// </summary>
    public aMenuState OnRotateUpMenu;

    /// <summary>
    /// Cached menu items
    /// </summary>
    public List<aMenuState> CachedMenus;

    /// <summary>
    /// Transition to another menu
    /// </summary>
    /// <param name="go">Menu to transition to, or null to remove menu</param>
    public void TransitionTo(aMenuState go) {

        // Notify new menu that we are switching, if no new menu, deactive the other
        if (go != null) {
            go.OnTransitionTo();
        } else if (_CurrentMenu != null) {
            _CurrentMenu.OnTransitionFrom();
            _CurrentMenu.OnDeactivate();
            _CurrentMenu = null;
        }

        // Transition from previous if needed
        if (_CurrentMenu != null) {
            _CurrentMenu.OnTransitionFrom();
        }

        // Set active
        go.OnActive();

        // Set current
        _CurrentMenu = go;

    }

	// Use this for initialization
	void Start () {
	
        // Find all assets that implement the IMenu interface, register them, along with the class name


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
