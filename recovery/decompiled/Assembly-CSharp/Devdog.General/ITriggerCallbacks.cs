namespace Devdog.General;

public interface ITriggerCallbacks
{
	bool OnTriggerUsed(Player player);

	bool OnTriggerUnUsed(Player player);
}
