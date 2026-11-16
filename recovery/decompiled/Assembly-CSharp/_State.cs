public abstract class _State : _iState
{
	public abstract void OnTransitionFrom();

	public abstract void OnActive();

	public abstract void OnDeactivate();

	public abstract void OnTransitionTo();
}
