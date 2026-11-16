using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.General;

public abstract class TriggerInputHandlerBase : MonoBehaviour, ITriggerInputHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	protected TriggerBase trigger;

	public abstract TriggerActionInfo actionInfo { get; }

	protected virtual void Awake()
	{
		trigger = GetComponent<TriggerBase>();
	}

	protected virtual void Update()
	{
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		TriggerUtility.mouseOnTrigger = trigger;
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (TriggerUtility.mouseOnTrigger == trigger)
		{
			TriggerUtility.mouseOnTrigger = null;
		}
		ManagerBase<GeneralSettingsManager>.instance.settings.defaultCursor.Enable();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
	}

	public abstract bool AreKeysDown();

	public virtual void Use()
	{
		trigger.Toggle();
	}
}
