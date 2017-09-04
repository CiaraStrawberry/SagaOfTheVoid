using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VivePaddleUIController : MonoBehaviour {

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
        } else if (go == null && _CurrentMenu != null) {
            _CurrentMenu.OnTransitionFrom();
            _CurrentMenu.OnDeactivate();
            DestroyImmediate(_CurrentMenu);
        }

        // Transition from previous if needed
        if (_CurrentMenu != null) {
            _CurrentMenu.OnTransitionFrom();
        }

        // Remove current menu, this will lose state
        if (_CurrentMenu != null) {
            _CurrentMenu.OnDeactivate();
            DestroyImmediate(_CurrentMenu);
        }

        // Set active
        go.OnActive();

        // Set current
        _CurrentMenu = go;

    }

    // Use this for initialization
    void Start () {
	
        if (DefaultMenu != null) {

            //_CurrentMenu = Instantiate(DefaultMenu, this.transform) as GameObject;
            _CurrentMenu = Instantiate<aMenuState>(DefaultMenu);

            // Determine if we need to resize
            _CurrentMenu.transform.parent = this.transform;

            // Transitioned to
            _CurrentMenu.OnTransitionTo();

            // Set active
            _CurrentMenu.OnActive();

        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
