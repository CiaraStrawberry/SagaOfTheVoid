using UnityEngine;
using System.Collections;

public interface _iState {

    /// <summary>
    /// Event that the menu will be transitioned from
    /// </summary>
    void OnTransitionFrom();

    /// <summary>
    /// Event when the menu is switched
    /// </summary>
    void OnActive();

    /// <summary>
    /// Event when the transition has completed, unload
    /// </summary>
    void OnDeactivate();

    /// <summary>
    /// Event when menu is being transitioned to, handle animations / loading / etc here
    /// </summary>
    void OnTransitionTo();

}
