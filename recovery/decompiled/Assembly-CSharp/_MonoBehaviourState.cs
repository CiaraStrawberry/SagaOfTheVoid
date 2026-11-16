using UnityEngine;

public abstract class _MonoBehaviourState : MonoBehaviour, _iState
{
	public abstract void OnTransitionFrom();

	public abstract void OnActive();

	public abstract void OnDeactivate();

	public abstract void OnTransitionTo();
}
