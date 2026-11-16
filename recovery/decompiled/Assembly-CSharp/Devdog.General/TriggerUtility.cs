namespace Devdog.General;

public static class TriggerUtility
{
	public static TriggerBase mouseOnTrigger { get; set; }

	public static bool isMouseOnTrigger => mouseOnTrigger != null;
}
