using Photon;

public abstract class _PunBehaviourState : PunBehaviour, _iState
{
	public abstract void OnTransitionFrom();

	public abstract void OnActive();

	public abstract void OnDeactivate();

	public abstract void OnTransitionTo();
}
