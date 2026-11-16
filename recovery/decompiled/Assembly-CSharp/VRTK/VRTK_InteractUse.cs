using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractUse")]
public class VRTK_InteractUse : MonoBehaviour
{
	[Header("Use Settings")]
	[Tooltip("The button used to use/unuse a touched object.")]
	public VRTK_ControllerEvents.ButtonAlias useButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;

	[Header("Custom Settings")]
	[Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_ControllerEvents controllerEvents;

	[Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_InteractTouch interactTouch;

	[Tooltip("The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_InteractGrab interactGrab;

	protected VRTK_ControllerEvents.ButtonAlias subscribedUseButton;

	protected VRTK_ControllerEvents.ButtonAlias savedUseButton;

	protected bool usePressed;

	protected GameObject usingObject;

	protected VRTK_ControllerReference controllerReference => VRTK_ControllerReference.GetControllerReference((!(interactTouch != null)) ? null : interactTouch.gameObject);

	public event ControllerInteractionEventHandler UseButtonPressed;

	public event ControllerInteractionEventHandler UseButtonReleased;

	public event ObjectInteractEventHandler ControllerUseInteractableObject;

	public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

	public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
	{
		if (ControllerUseInteractableObject != null)
		{
			ControllerUseInteractableObject(this, e);
		}
	}

	public virtual void OnControllerUnuseInteractableObject(ObjectInteractEventArgs e)
	{
		if (ControllerUnuseInteractableObject != null)
		{
			ControllerUnuseInteractableObject(this, e);
		}
	}

	public virtual void OnUseButtonPressed(ControllerInteractionEventArgs e)
	{
		if (UseButtonPressed != null)
		{
			UseButtonPressed(this, e);
		}
	}

	public virtual void OnUseButtonReleased(ControllerInteractionEventArgs e)
	{
		if (UseButtonReleased != null)
		{
			UseButtonReleased(this, e);
		}
	}

	public virtual bool IsUseButtonPressed()
	{
		return usePressed;
	}

	public virtual GameObject GetUsingObject()
	{
		return usingObject;
	}

	public virtual void ForceStopUsing()
	{
		if (usingObject != null)
		{
			StopUsing();
		}
	}

	public virtual void ForceResetUsing()
	{
		if (usingObject != null)
		{
			UnuseInteractedObject(completeStop: false);
		}
	}

	public virtual void AttemptUse()
	{
		AttemptUseObject();
	}

	protected virtual void OnEnable()
	{
		controllerEvents = ((!(controllerEvents != null)) ? GetComponentInParent<VRTK_ControllerEvents>() : controllerEvents);
		interactTouch = ((!(interactTouch != null)) ? GetComponentInParent<VRTK_InteractTouch>() : interactTouch);
		interactGrab = ((!(interactGrab != null)) ? GetComponentInParent<VRTK_InteractGrab>() : interactGrab);
		if (interactTouch == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractUse", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
		}
		ManageUseListener(state: true);
		ManageInteractTouchListener(state: true);
	}

	protected virtual void OnDisable()
	{
		ForceResetUsing();
		ManageUseListener(state: false);
		ManageInteractTouchListener(state: false);
	}

	protected virtual void Update()
	{
		ManageUseListener(state: true);
	}

	protected virtual void ManageInteractTouchListener(bool state)
	{
		if (interactTouch != null && !state)
		{
			interactTouch.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
			interactTouch.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
		}
		if (interactTouch != null && state)
		{
			interactTouch.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
			interactTouch.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
		}
	}

	protected virtual void ControllerTouchInteractableObject(object sender, ObjectInteractEventArgs e)
	{
		if (e.target != null)
		{
			VRTK_InteractableObject component = e.target.GetComponent<VRTK_InteractableObject>();
			if (component != null && component.useOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
			{
				savedUseButton = subscribedUseButton;
				useButton = component.useOverrideButton;
			}
		}
	}

	protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
	{
		if (e.target != null)
		{
			VRTK_InteractableObject component = e.target.GetComponent<VRTK_InteractableObject>();
			if (component != null && !component.IsUsing() && savedUseButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
			{
				useButton = savedUseButton;
				savedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
			}
		}
	}

	protected virtual void ManageUseListener(bool state)
	{
		if (controllerEvents != null && subscribedUseButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || !useButton.Equals(subscribedUseButton)))
		{
			controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, startEvent: true, DoStartUseObject);
			controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, startEvent: false, DoStopUseObject);
			subscribedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		}
		if (controllerEvents != null && state && useButton != VRTK_ControllerEvents.ButtonAlias.Undefined && !useButton.Equals(subscribedUseButton))
		{
			controllerEvents.SubscribeToButtonAliasEvent(useButton, startEvent: true, DoStartUseObject);
			controllerEvents.SubscribeToButtonAliasEvent(useButton, startEvent: false, DoStopUseObject);
			subscribedUseButton = useButton;
		}
	}

	protected virtual bool IsObjectUsable(GameObject obj)
	{
		return interactTouch.IsObjectInteractable(obj) && obj.GetComponent<VRTK_InteractableObject>().isUsable;
	}

	protected virtual bool IsObjectHoldOnUse(GameObject obj)
	{
		if (obj != null)
		{
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			return component != null && component.holdButtonToUse;
		}
		return false;
	}

	protected virtual int GetObjectUsingState(GameObject obj)
	{
		if (obj != null)
		{
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			if (component != null)
			{
				return component.usingState;
			}
		}
		return 0;
	}

	protected virtual void SetObjectUsingState(GameObject obj, int value)
	{
		if (obj != null)
		{
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			if (component != null)
			{
				component.usingState = value;
			}
		}
	}

	protected virtual void AttemptHaptics()
	{
		if (usingObject != null)
		{
			VRTK_InteractHaptics componentInParent = usingObject.GetComponentInParent<VRTK_InteractHaptics>();
			if (componentInParent != null)
			{
				componentInParent.HapticsOnUse(controllerReference);
			}
		}
	}

	protected virtual void ToggleControllerVisibility(bool visible)
	{
		if (usingObject != null)
		{
			VRTK_InteractControllerAppearance[] componentsInParent = usingObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(includeInactive: true);
			if (componentsInParent.Length > 0)
			{
				componentsInParent[0].ToggleControllerOnUse(visible, controllerReference.model, usingObject);
			}
		}
	}

	protected virtual void UseInteractedObject(GameObject touchedObject)
	{
		if ((usingObject == null || usingObject != touchedObject) && IsObjectUsable(touchedObject))
		{
			usingObject = touchedObject;
			VRTK_InteractableObject component = usingObject.GetComponent<VRTK_InteractableObject>();
			if (!component.IsValidInteractableController(controllerReference.scriptAlias, component.allowedUseControllers))
			{
				usingObject = null;
				return;
			}
			component.StartUsing(controllerReference.scriptAlias);
			ToggleControllerVisibility(visible: false);
			AttemptHaptics();
			OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
		}
	}

	protected virtual void UnuseInteractedObject(bool completeStop)
	{
		if (usingObject != null)
		{
			VRTK_InteractableObject component = usingObject.GetComponent<VRTK_InteractableObject>();
			if (component != null && completeStop)
			{
				component.StopUsing(controllerReference.scriptAlias);
			}
			ToggleControllerVisibility(visible: true);
			OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
			usingObject = null;
		}
	}

	protected virtual GameObject GetFromGrab()
	{
		if (interactGrab != null)
		{
			return interactGrab.GetGrabbedObject();
		}
		return null;
	}

	protected virtual void StopUsing()
	{
		SetObjectUsingState(usingObject, 0);
		UnuseInteractedObject(completeStop: true);
	}

	protected virtual void AttemptUseObject()
	{
		GameObject gameObject = interactTouch.GetTouchedObject();
		if (gameObject == null)
		{
			gameObject = GetFromGrab();
		}
		if (!(gameObject != null) || !interactTouch.IsObjectInteractable(gameObject))
		{
			return;
		}
		VRTK_InteractableObject component = gameObject.GetComponent<VRTK_InteractableObject>();
		if (!component.useOnlyIfGrabbed || component.IsGrabbed())
		{
			UseInteractedObject(gameObject);
			if (usingObject != null && !IsObjectHoldOnUse(usingObject))
			{
				SetObjectUsingState(usingObject, GetObjectUsingState(usingObject) + 1);
			}
		}
	}

	protected virtual void DoStartUseObject(object sender, ControllerInteractionEventArgs e)
	{
		OnUseButtonPressed(controllerEvents.SetControllerEvent(ref usePressed, value: true));
		AttemptUseObject();
	}

	protected virtual void DoStopUseObject(object sender, ControllerInteractionEventArgs e)
	{
		if (IsObjectHoldOnUse(usingObject) || GetObjectUsingState(usingObject) >= 2)
		{
			StopUsing();
		}
		OnUseButtonReleased(controllerEvents.SetControllerEvent(ref usePressed));
	}
}
