using System;

namespace Devdog.General;

public class TriggerManager : ManagerBase<TriggerManager>
{
	private static TriggerBase _currentActiveTrigger;

	public static TriggerBase currentActiveTrigger
	{
		get
		{
			return _currentActiveTrigger;
		}
		set
		{
			TriggerBase triggerBase = _currentActiveTrigger;
			_currentActiveTrigger = value;
			if (triggerBase != _currentActiveTrigger && OnCurrentTriggerChanged != null)
			{
				OnCurrentTriggerChanged(triggerBase, _currentActiveTrigger);
			}
		}
	}

	public static event Action<TriggerBase, TriggerBase> OnCurrentTriggerChanged;
}
