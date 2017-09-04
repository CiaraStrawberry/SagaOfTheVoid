using UnityEngine;
using System.Collections;

/// <summary>
/// State management attributes
/// </summary>
public abstract class _PunBehaviourState : Photon.PunBehaviour, _iState {

    /// <summary>
    /// Event that the menu will be transitioned from
    /// </summary>
    public abstract void OnTransitionFrom();

    /// <summary>
    /// Event when the menu is switched
    /// </summary>
    public abstract void OnActive();

    /// <summary>
    /// Event when the transition has completed, unload
    /// </summary>
    public abstract void OnDeactivate();

    /// <summary>
    /// Event when menu is being transitioned to, handle animations / loading / etc here
    /// </summary>
    public abstract void OnTransitionTo();

}
