using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractGrab")]
public class VRTK_InteractGrab : MonoBehaviour
{
	[Header("Grab Settings")]
	[Tooltip("The button used to grab/release a touched object.")]
	public VRTK_ControllerEvents.ButtonAlias grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;

	[Tooltip("An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.")]
	public float grabPrecognition;

	[Tooltip("An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the play area to simulate being able to throw items further.")]
	public float throwMultiplier = 1f;

	[Tooltip("If this is checked and the controller is not touching an Interactable Object when the grab button is pressed then a rigid body is added to the controller to allow the controller to push other rigid body objects around.")]
	public bool createRigidBodyWhenNotTouching;

	[Header("Custom Settings")]
	[Tooltip("The rigidbody point on the controller model to snap the grabbed object to. If blank it will be set to the SDK default.")]
	public Rigidbody controllerAttachPoint;

	[Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_ControllerEvents controllerEvents;

	[Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_InteractTouch interactTouch;

	protected VRTK_ControllerEvents.ButtonAlias subscribedGrabButton;

	protected VRTK_ControllerEvents.ButtonAlias savedGrabButton;

	protected bool grabPressed;

	protected GameObject grabbedObject;

	protected bool influencingGrabbedObject;

	protected int grabEnabledState;

	protected float grabPrecognitionTimer;

	protected GameObject undroppableGrabbedObject;

	protected Rigidbody originalControllerAttachPoint;

	protected VRTK_ControllerReference controllerReference => VRTK_ControllerReference.GetControllerReference((!(interactTouch != null)) ? null : interactTouch.gameObject);

	public event ControllerInteractionEventHandler GrabButtonPressed;

	public event ControllerInteractionEventHandler GrabButtonReleased;

	public event ObjectInteractEventHandler ControllerGrabInteractableObject;

	public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

	public virtual void OnControllerGrabInteractableObject(ObjectInteractEventArgs e)
	{
		if (ControllerGrabInteractableObject != null)
		{
			ControllerGrabInteractableObject(this, e);
		}
	}

	public virtual void OnControllerUngrabInteractableObject(ObjectInteractEventArgs e)
	{
		if (ControllerUngrabInteractableObject != null)
		{
			ControllerUngrabInteractableObject(this, e);
		}
	}

	public virtual void OnGrabButtonPressed(ControllerInteractionEventArgs e)
	{
		if (GrabButtonPressed != null)
		{
			GrabButtonPressed(this, e);
		}
	}

	public virtual void OnGrabButtonReleased(ControllerInteractionEventArgs e)
	{
		if (GrabButtonReleased != null)
		{
			GrabButtonReleased(this, e);
		}
	}

	public virtual bool IsGrabButtonPressed()
	{
		return grabPressed;
	}

	public virtual void ForceRelease(bool applyGrabbingObjectVelocity = false)
	{
		InitUngrabbedObject(applyGrabbingObjectVelocity);
	}

	public virtual void AttemptGrab()
	{
		AttemptGrabObject();
	}

	public virtual GameObject GetGrabbedObject()
	{
		return grabbedObject;
	}

	protected virtual void Awake()
	{
		originalControllerAttachPoint = controllerAttachPoint;
		controllerEvents = ((!(controllerEvents != null)) ? GetComponentInParent<VRTK_ControllerEvents>() : controllerEvents);
		interactTouch = ((!(interactTouch != null)) ? GetComponentInParent<VRTK_InteractTouch>() : interactTouch);
		if (interactTouch == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
		}
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		RegrabUndroppableObject();
		ManageGrabListener(state: true);
		ManageInteractTouchListener(state: true);
		SetControllerAttachPoint();
	}

	protected virtual void OnDisable()
	{
		SetUndroppableObject();
		ForceRelease();
		ManageGrabListener(state: false);
		ManageInteractTouchListener(state: false);
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		ManageGrabListener(state: true);
		CheckControllerAttachPointSet();
		CreateNonTouchingRigidbody();
		CheckPrecognitionGrab();
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
			if (component != null && component.grabOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
			{
				savedGrabButton = subscribedGrabButton;
				grabButton = component.grabOverrideButton;
			}
		}
	}

	protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
	{
		if (e.target != null)
		{
			VRTK_InteractableObject component = e.target.GetComponent<VRTK_InteractableObject>();
			if (!component.IsGrabbed() && savedGrabButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
			{
				grabButton = savedGrabButton;
				savedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
			}
		}
	}

	protected virtual void ManageGrabListener(bool state)
	{
		if (controllerEvents != null && subscribedGrabButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || !grabButton.Equals(subscribedGrabButton)))
		{
			controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, startEvent: true, DoGrabObject);
			controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, startEvent: false, DoReleaseObject);
			subscribedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		}
		if (controllerEvents != null && state && grabButton != VRTK_ControllerEvents.ButtonAlias.Undefined && !grabButton.Equals(subscribedGrabButton))
		{
			controllerEvents.SubscribeToButtonAliasEvent(grabButton, startEvent: true, DoGrabObject);
			controllerEvents.SubscribeToButtonAliasEvent(grabButton, startEvent: false, DoReleaseObject);
			subscribedGrabButton = grabButton;
		}
	}

	protected virtual void RegrabUndroppableObject()
	{
		if (undroppableGrabbedObject != null && !undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
		{
			undroppableGrabbedObject.SetActive(value: true);
			interactTouch.ForceTouch(undroppableGrabbedObject);
			AttemptGrab();
		}
		else
		{
			undroppableGrabbedObject = null;
		}
	}

	protected virtual void SetUndroppableObject()
	{
		if (undroppableGrabbedObject != null)
		{
			if (undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable())
			{
				undroppableGrabbedObject = null;
			}
			else
			{
				undroppableGrabbedObject.SetActive(value: false);
			}
		}
	}

	protected virtual void SetControllerAttachPoint()
	{
		if (!(controllerReference.model != null) || !(originalControllerAttachPoint == null))
		{
			return;
		}
		Transform transform = controllerReference.model.transform.Find(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, VRTK_DeviceFinder.GetControllerHand(interactTouch.gameObject)));
		if (transform != null)
		{
			controllerAttachPoint = transform.GetComponent<Rigidbody>();
			if (controllerAttachPoint == null)
			{
				Rigidbody rigidbody = transform.gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				controllerAttachPoint = rigidbody;
			}
		}
	}

	protected virtual bool IsObjectGrabbable(GameObject obj)
	{
		VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
		return interactTouch.IsObjectInteractable(obj) && component != null && (component.isGrabbable || component.PerformSecondaryAction());
	}

	protected virtual bool IsObjectHoldOnGrab(GameObject obj)
	{
		if (obj != null)
		{
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			return (bool)component && component.holdButtonToGrab;
		}
		return false;
	}

	protected virtual void ChooseGrabSequence(VRTK_InteractableObject grabbedObjectScript)
	{
		if (!grabbedObjectScript.IsGrabbed() || grabbedObjectScript.IsSwappable())
		{
			InitPrimaryGrab(grabbedObjectScript);
		}
		else
		{
			InitSecondaryGrab(grabbedObjectScript);
		}
	}

	protected virtual void ToggleControllerVisibility(bool visible)
	{
		if (grabbedObject != null)
		{
			VRTK_InteractControllerAppearance[] componentsInParent = grabbedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(includeInactive: true);
			if (componentsInParent.Length > 0)
			{
				componentsInParent[0].ToggleControllerOnGrab(visible, controllerReference.model, grabbedObject);
			}
		}
		else if (visible)
		{
			VRTK_ObjectAppearance.SetRendererVisible(controllerReference.model, grabbedObject);
		}
	}

	protected virtual void InitGrabbedObject()
	{
		grabbedObject = interactTouch.GetTouchedObject();
		if (grabbedObject != null)
		{
			VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
			ChooseGrabSequence(component);
			ToggleControllerVisibility(visible: false);
			OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
		}
	}

	protected virtual void InitPrimaryGrab(VRTK_InteractableObject currentGrabbedObject)
	{
		GameObject scriptAlias = controllerReference.scriptAlias;
		if (!currentGrabbedObject.IsValidInteractableController(scriptAlias, currentGrabbedObject.allowedGrabControllers))
		{
			grabbedObject = null;
			if (currentGrabbedObject.IsGrabbed(scriptAlias))
			{
				interactTouch.ForceStopTouching();
			}
		}
		else
		{
			influencingGrabbedObject = false;
			currentGrabbedObject.SaveCurrentState();
			currentGrabbedObject.Grabbed(scriptAlias);
			currentGrabbedObject.ZeroVelocity();
			currentGrabbedObject.ToggleHighlight(toggle: false);
			currentGrabbedObject.isKinematic = false;
		}
	}

	protected virtual void InitSecondaryGrab(VRTK_InteractableObject currentGrabbedObject)
	{
		GameObject scriptAlias = controllerReference.scriptAlias;
		if (!currentGrabbedObject.IsValidInteractableController(scriptAlias, currentGrabbedObject.allowedGrabControllers))
		{
			grabbedObject = null;
			influencingGrabbedObject = false;
			currentGrabbedObject.Ungrabbed(scriptAlias);
		}
		else
		{
			influencingGrabbedObject = true;
			currentGrabbedObject.Grabbed(scriptAlias);
		}
	}

	protected virtual void CheckInfluencingObjectOnRelease()
	{
		if (!influencingGrabbedObject)
		{
			interactTouch.ForceStopTouching();
			ToggleControllerVisibility(visible: true);
		}
		influencingGrabbedObject = false;
	}

	protected virtual void InitUngrabbedObject(bool applyGrabbingObjectVelocity)
	{
		if (grabbedObject != null)
		{
			GameObject scriptAlias = controllerReference.scriptAlias;
			VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
			if (!influencingGrabbedObject)
			{
				component.grabAttachMechanicScript.StopGrab(applyGrabbingObjectVelocity);
			}
			component.Ungrabbed(scriptAlias);
			component.ToggleHighlight(toggle: false);
			ToggleControllerVisibility(visible: true);
			OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
		}
		CheckInfluencingObjectOnRelease();
		grabEnabledState = 0;
		grabbedObject = null;
	}

	protected virtual GameObject GetGrabbableObject()
	{
		GameObject touchedObject = interactTouch.GetTouchedObject();
		if (touchedObject != null && interactTouch.IsObjectInteractable(touchedObject))
		{
			return touchedObject;
		}
		return grabbedObject;
	}

	protected virtual void IncrementGrabState()
	{
		if (!IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
		{
			grabEnabledState++;
		}
	}

	protected virtual GameObject GetUndroppableObject()
	{
		if (grabbedObject != null)
		{
			VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
			return (!component || component.IsDroppable()) ? null : grabbedObject;
		}
		return null;
	}

	protected virtual void AttemptHaptics(bool initialGrabAttempt)
	{
		if (grabbedObject != null && initialGrabAttempt)
		{
			VRTK_InteractHaptics componentInParent = grabbedObject.GetComponentInParent<VRTK_InteractHaptics>();
			if (componentInParent != null)
			{
				componentInParent.HapticsOnGrab(controllerReference);
			}
		}
	}

	protected virtual void AttemptGrabObject()
	{
		GameObject grabbableObject = GetGrabbableObject();
		if (grabbableObject != null)
		{
			PerformGrabAttempt(grabbableObject);
		}
		else
		{
			grabPrecognitionTimer = Time.time + grabPrecognition;
		}
	}

	protected virtual void PerformGrabAttempt(GameObject objectToGrab)
	{
		IncrementGrabState();
		bool initialGrabAttempt = IsValidGrabAttempt(objectToGrab);
		undroppableGrabbedObject = GetUndroppableObject();
		AttemptHaptics(initialGrabAttempt);
	}

	protected virtual bool IsValidGrabAttempt(GameObject objectToGrab)
	{
		GameObject scriptAlias = controllerReference.scriptAlias;
		bool result = false;
		VRTK_InteractableObject component = objectToGrab.GetComponent<VRTK_InteractableObject>();
		if (grabbedObject == null && interactTouch != null && IsObjectGrabbable(interactTouch.GetTouchedObject()) && (bool)component && component.grabAttachMechanicScript.ValidGrab(controllerAttachPoint))
		{
			InitGrabbedObject();
			if (!influencingGrabbedObject)
			{
				result = component.grabAttachMechanicScript.StartGrab(scriptAlias, grabbedObject, controllerAttachPoint);
			}
		}
		return result;
	}

	protected virtual bool CanRelease()
	{
		return grabbedObject != null && grabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable();
	}

	protected virtual void AttemptReleaseObject()
	{
		if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
		{
			InitUngrabbedObject(applyGrabbingObjectVelocity: true);
		}
	}

	protected virtual void DoGrabObject(object sender, ControllerInteractionEventArgs e)
	{
		OnGrabButtonPressed(controllerEvents.SetControllerEvent(ref grabPressed, value: true));
		AttemptGrabObject();
	}

	protected virtual void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
	{
		AttemptReleaseObject();
		OnGrabButtonReleased(controllerEvents.SetControllerEvent(ref grabPressed));
	}

	protected virtual void CheckControllerAttachPointSet()
	{
		if (controllerAttachPoint == null)
		{
			SetControllerAttachPoint();
		}
	}

	protected virtual void CreateNonTouchingRigidbody()
	{
		if (createRigidBodyWhenNotTouching && grabbedObject == null && !interactTouch.IsRigidBodyForcedActive() && interactTouch.IsRigidBodyActive() != grabPressed)
		{
			interactTouch.ToggleControllerRigidBody(grabPressed);
		}
	}

	protected virtual void CheckPrecognitionGrab()
	{
		if (grabPrecognitionTimer >= Time.time && GetGrabbableObject() != null)
		{
			AttemptGrabObject();
			if (GetGrabbedObject() != null)
			{
				grabPrecognitionTimer = 0f;
			}
		}
	}
}
