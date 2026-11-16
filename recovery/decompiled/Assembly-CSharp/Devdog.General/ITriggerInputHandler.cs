namespace Devdog.General;

public interface ITriggerInputHandler
{
	TriggerActionInfo actionInfo { get; }

	bool AreKeysDown();

	void Use();
}
