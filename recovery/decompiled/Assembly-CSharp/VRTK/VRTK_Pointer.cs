using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Pointers/VRTK_Pointer")]
public class VRTK_Pointer : VRTK_DestinationMarker
{
	[Header("Pointer Activation Settings")]
	[Tooltip("The specific renderer to use when the pointer is activated. The renderer also determines how the pointer reaches it's destination (e.g. straight line, bezier curve).")]
	public VRTK_BasePointerRenderer pointerRenderer;

	[Tooltip("The button used to activate/deactivate the pointer.")]
	public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

	[Tooltip("If this is checked then the Activation Button needs to be continuously held down to keep the pointer active. If this is unchecked then the Activation Button works as a toggle, the first press/release enables the pointer and the second press/release disables the pointer.")]
	public bool holdButtonToActivate = true;

	[Tooltip("If this is checked then the pointer will be toggled on when the script is enabled.")]
	public bool activateOnEnable;

	[Tooltip("The time in seconds to delay the pointer being able to be active again.")]
	public float activationDelay;

	[Header("Pointer Selection Settings")]
	[Tooltip("The button used to execute the select action at the pointer's target position.")]
	public VRTK_ControllerEvents.ButtonAlias selectionButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

	[Tooltip("If this is checked then the pointer selection action is executed when the Selection Button is pressed down. If this is unchecked then the selection action is executed when the Selection Button is released.")]
	public bool selectOnPress;

	[Tooltip("The time in seconds to delay the pointer being able to execute the select action again.")]
	public float selectionDelay;

	[Tooltip("The amount of time the pointer can be over the same collider before it automatically attempts to select it. 0f means no selection attempt will be made.")]
	public float selectAfterHoverDuration;

	[Header("Pointer Interaction Settings")]
	[Tooltip("If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.")]
	public bool interactWithObjects;

	[Tooltip("If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.")]
	public bool grabToPointerTip;

	[Header("Pointer Customisation Settings")]
	[Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_ControllerEvents controller;

	[Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
	public Transform customOrigin;

	[Tooltip("A custom VRTK_PointerDirectionIndicator to use to determine the rotation given to the destination set event.")]
	public VRTK_PointerDirectionIndicator directionIndicator;

	protected VRTK_ControllerEvents.ButtonAlias subscribedActivationButton;

	protected VRTK_ControllerEvents.ButtonAlias subscribedSelectionButton;

	protected bool currentSelectOnPress;

	protected float activateDelayTimer;

	protected float selectDelayTimer;

	protected float hoverDurationTimer;

	protected int currentActivationState;

	protected bool willDeactivate;

	protected bool wasActivated;

	protected VRTK_ControllerReference controllerReference;

	protected VRTK_InteractableObject pointerInteractableObject;

	protected Collider currentCollider;

	protected bool canClickOnHover;

	protected bool activationButtonPressed;

	protected bool selectionButtonPressed;

	protected bool attemptControllerSetup;

	protected Transform originalCustomOrigin;

	public event ControllerInteractionEventHandler ActivationButtonPressed;

	public event ControllerInteractionEventHandler ActivationButtonReleased;

	public event ControllerInteractionEventHandler SelectionButtonPressed;

	public event ControllerInteractionEventHandler SelectionButtonReleased;

	public virtual void OnActivationButtonPressed(ControllerInteractionEventArgs e)
	{
		if (ActivationButtonPressed != null)
		{
			ActivationButtonPressed(this, e);
		}
	}

	public virtual void OnActivationButtonReleased(ControllerInteractionEventArgs e)
	{
		if (ActivationButtonReleased != null)
		{
			ActivationButtonReleased(this, e);
		}
	}

	public virtual void OnSelectionButtonPressed(ControllerInteractionEventArgs e)
	{
		if (SelectionButtonPressed != null)
		{
			SelectionButtonPressed(this, e);
		}
	}

	public virtual void OnSelectionButtonReleased(ControllerInteractionEventArgs e)
	{
		if (SelectionButtonReleased != null)
		{
			SelectionButtonReleased(this, e);
		}
	}

	public virtual bool IsActivationButtonPressed()
	{
		return activationButtonPressed;
	}

	public virtual bool IsSelectionButtonPressed()
	{
		return selectionButtonPressed;
	}

	public virtual void PointerEnter(RaycastHit givenHit)
	{
		if (enabled && givenHit.transform != null && (!ControllerRequired() || VRTK_ControllerReference.IsValid(controllerReference)))
		{
			SetHoverSelectionTimer(givenHit.collider);
			DestinationMarkerEventArgs e = SetDestinationMarkerEvent(givenHit.distance, givenHit.transform, givenHit, givenHit.point, controllerReference, forceDestinationPosition: false, GetCursorRotation());
			if (pointerRenderer != null && givenHit.collider != pointerRenderer.GetDestinationHit().collider)
			{
				OnDestinationMarkerEnter(e);
			}
			else
			{
				OnDestinationMarkerHover(e);
			}
			StartUseAction(givenHit.transform);
		}
	}

	public virtual void PointerExit(RaycastHit givenHit)
	{
		ResetHoverSelectionTimer(givenHit.collider);
		if (givenHit.transform != null && (!ControllerRequired() || VRTK_ControllerReference.IsValid(controllerReference)))
		{
			OnDestinationMarkerExit(SetDestinationMarkerEvent(givenHit.distance, givenHit.transform, givenHit, givenHit.point, controllerReference, forceDestinationPosition: false, GetCursorRotation()));
			StopUseAction();
		}
	}

	public virtual bool CanActivate()
	{
		return Time.time >= activateDelayTimer;
	}

	public virtual bool CanSelect()
	{
		return Time.time >= selectDelayTimer;
	}

	public virtual bool IsPointerActive()
	{
		return currentActivationState != 0;
	}

	public virtual void ResetActivationTimer(bool forceZero = false)
	{
		activateDelayTimer = ((!forceZero) ? (Time.time + activationDelay) : 0f);
	}

	public virtual void ResetSelectionTimer(bool forceZero = false)
	{
		selectDelayTimer = ((!forceZero) ? (Time.time + selectionDelay) : 0f);
	}

	public virtual void Toggle(bool state)
	{
		if (CanActivate() && !NoPointerRenderer() && !CanActivateOnToggleButton(state))
		{
			ManageActivationState(willDeactivate || state);
			pointerRenderer.Toggle(IsPointerActive(), state);
			willDeactivate = false;
			if (!state)
			{
				StopUseAction();
			}
		}
	}

	protected virtual void Awake()
	{
		originalCustomOrigin = customOrigin;
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
		SetDefaultValues();
		if (NoPointerRenderer())
		{
			VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_PARAMETER, "VRTK_Pointer", "VRTK_BasePointerRenderer", "Pointer Renderer"));
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UnsubscribeActivationButton();
		UnsubscribeSelectionButton();
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		AttemptControllerSetup();
		CheckButtonSubscriptions();
		HandleEnabledPointer();
		UpdateDirectionIndicator();
	}

	protected virtual void SetDefaultValues()
	{
		customOrigin = ((!(originalCustomOrigin == null)) ? originalCustomOrigin : VRTK_SDK_Bridge.GenerateControllerPointerOrigin(gameObject));
		SetupRenderer();
		activateDelayTimer = 0f;
		selectDelayTimer = 0f;
		hoverDurationTimer = 0f;
		currentActivationState = 0;
		wasActivated = false;
		willDeactivate = false;
		canClickOnHover = false;
		attemptControllerSetup = true;
	}

	protected virtual void AttemptControllerSetup()
	{
		if (attemptControllerSetup && FindController())
		{
			attemptControllerSetup = false;
			SetupController();
			SetupRenderer();
			if (activateOnEnable)
			{
				Toggle(state: true);
			}
		}
	}

	protected virtual void HandleEnabledPointer()
	{
		if (EnabledPointerRenderer())
		{
			pointerRenderer.InitalizePointer(this, invalidListPolicy, navMeshCheckDistance, headsetPositionCompensation);
			pointerRenderer.UpdateRenderer();
			if (!IsPointerActive())
			{
				bool state = pointerRenderer.IsVisible();
				pointerRenderer.ToggleInteraction(state);
			}
			CheckHoverSelect();
		}
	}

	protected virtual void UpdateDirectionIndicator()
	{
		if (directionIndicator != null && pointerRenderer != null)
		{
			RaycastHit destinationHit = pointerRenderer.GetDestinationHit();
			directionIndicator.SetPosition(IsPointerActive() && destinationHit.collider != null, destinationHit.point);
		}
	}

	protected virtual Quaternion? GetCursorRotation()
	{
		if (directionIndicator != null)
		{
			return directionIndicator.GetRotation();
		}
		return null;
	}

	protected virtual bool EnabledPointerRenderer()
	{
		return pointerRenderer != null && pointerRenderer.enabled;
	}

	protected virtual bool NoPointerRenderer()
	{
		return pointerRenderer == null || !pointerRenderer.enabled;
	}

	protected virtual bool CanActivateOnToggleButton(bool state)
	{
		bool flag = state && !holdButtonToActivate && IsPointerActive();
		if (flag)
		{
			willDeactivate = true;
		}
		return flag;
	}

	protected virtual bool ControllerRequired()
	{
		return activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined || selectionButton != VRTK_ControllerEvents.ButtonAlias.Undefined;
	}

	protected virtual bool FindController()
	{
		if (controller == null)
		{
			controller = GetComponentInParent<VRTK_ControllerEvents>();
		}
		if (controller == null && ControllerRequired())
		{
			VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_Pointer", "VRTK_ControllerEvents", "the Controller Alias", ". To omit this warning, set the `Activation Button` and `Selection Button` to `Undefined`"));
			return false;
		}
		if (directionIndicator != null)
		{
			directionIndicator.Initialize(controller);
		}
		return true;
	}

	protected virtual void SetupController()
	{
		if (controller != null)
		{
			CheckButtonMappingConflict();
			SubscribeSelectionButton();
			SubscribeActivationButton();
		}
	}

	protected virtual void SetupRenderer()
	{
		if (EnabledPointerRenderer())
		{
			pointerRenderer.InitalizePointer(this, invalidListPolicy, navMeshCheckDistance, headsetPositionCompensation);
		}
	}

	protected virtual bool ButtonMappingIsUndefined(VRTK_ControllerEvents.ButtonAlias givenButton, VRTK_ControllerEvents.ButtonAlias givenSubscribedButton)
	{
		return givenSubscribedButton != VRTK_ControllerEvents.ButtonAlias.Undefined && givenButton == VRTK_ControllerEvents.ButtonAlias.Undefined;
	}

	protected virtual void CheckButtonMappingConflict()
	{
		if (activationButton == selectionButton)
		{
			if (selectOnPress && holdButtonToActivate)
			{
				VRTK_Logger.Warn("`Hold Button To Activate` and `Select On Press` cannot both be checked when using the same button for Activation and Selection. Fixing by setting `Select On Press` to `false`.");
			}
			if (!selectOnPress && !holdButtonToActivate)
			{
				VRTK_Logger.Warn("`Hold Button To Activate` and `Select On Press` cannot both be unchecked when using the same button for Activation and Selection. Fixing by setting `Select On Press` to `true`.");
			}
			selectOnPress = !holdButtonToActivate;
		}
	}

	protected virtual void CheckButtonSubscriptions()
	{
		CheckButtonMappingConflict();
		if (ButtonMappingIsUndefined(selectionButton, subscribedSelectionButton) || selectOnPress != currentSelectOnPress)
		{
			UnsubscribeSelectionButton();
		}
		if (selectionButton != subscribedSelectionButton)
		{
			SubscribeSelectionButton();
			UnsubscribeActivationButton();
		}
		if (ButtonMappingIsUndefined(activationButton, subscribedActivationButton))
		{
			UnsubscribeActivationButton();
		}
		if (activationButton != subscribedActivationButton)
		{
			SubscribeActivationButton();
		}
	}

	protected virtual void SubscribeActivationButton()
	{
		if (subscribedActivationButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
		{
			UnsubscribeActivationButton();
		}
		if (controller != null)
		{
			controller.SubscribeToButtonAliasEvent(activationButton, startEvent: true, DoActivationButtonPressed);
			controller.SubscribeToButtonAliasEvent(activationButton, startEvent: false, DoActivationButtonReleased);
			subscribedActivationButton = activationButton;
		}
	}

	protected virtual void UnsubscribeActivationButton()
	{
		if (controller != null && subscribedActivationButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
		{
			controller.UnsubscribeToButtonAliasEvent(subscribedActivationButton, startEvent: true, DoActivationButtonPressed);
			controller.UnsubscribeToButtonAliasEvent(subscribedActivationButton, startEvent: false, DoActivationButtonReleased);
			subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		}
	}

	protected virtual void DoActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
	{
		OnActivationButtonPressed(controller.SetControllerEvent(ref activationButtonPressed, value: true));
		if (EnabledPointerRenderer())
		{
			controllerReference = e.controllerReference;
			Toggle(state: true);
		}
	}

	protected virtual void DoActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
	{
		if (EnabledPointerRenderer())
		{
			controllerReference = e.controllerReference;
			if (IsPointerActive())
			{
				Toggle(state: false);
			}
		}
		OnActivationButtonReleased(controller.SetControllerEvent(ref activationButtonPressed));
	}

	protected virtual void SubscribeSelectionButton()
	{
		if (subscribedSelectionButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
		{
			UnsubscribeSelectionButton();
		}
		if (controller != null)
		{
			controller.SubscribeToButtonAliasEvent(selectionButton, startEvent: true, DoSelectionButtonPressed);
			controller.SubscribeToButtonAliasEvent(selectionButton, startEvent: false, DoSelectionButtonReleased);
			controller.SubscribeToButtonAliasEvent(selectionButton, selectOnPress, SelectionButtonAction);
			subscribedSelectionButton = selectionButton;
			currentSelectOnPress = selectOnPress;
		}
	}

	protected virtual void UnsubscribeSelectionButton()
	{
		if (controller != null && subscribedSelectionButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
		{
			controller.UnsubscribeToButtonAliasEvent(selectionButton, startEvent: true, DoSelectionButtonPressed);
			controller.UnsubscribeToButtonAliasEvent(selectionButton, startEvent: false, DoSelectionButtonReleased);
			controller.UnsubscribeToButtonAliasEvent(subscribedSelectionButton, currentSelectOnPress, SelectionButtonAction);
			subscribedSelectionButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
		}
	}

	protected virtual void DoSelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
	{
		OnSelectionButtonPressed(controller.SetControllerEvent(ref selectionButtonPressed, value: true));
	}

	protected virtual void DoSelectionButtonReleased(object sender, ControllerInteractionEventArgs e)
	{
		OnSelectionButtonReleased(controller.SetControllerEvent(ref selectionButtonPressed));
	}

	protected virtual void SelectionButtonAction(object sender, ControllerInteractionEventArgs e)
	{
		controllerReference = e.controllerReference;
		ExecuteSelectionButtonAction();
	}

	protected virtual void ExecuteSelectionButtonAction()
	{
		if (EnabledPointerRenderer() && CanSelect() && (IsPointerActive() || wasActivated))
		{
			wasActivated = false;
			RaycastHit destinationHit = pointerRenderer.GetDestinationHit();
			AttemptUseOnSet(destinationHit.transform);
			if ((bool)destinationHit.transform && IsPointerActive() && pointerRenderer.ValidPlayArea() && !PointerActivatesUseAction(pointerInteractableObject) && pointerRenderer.IsValidCollision())
			{
				ResetHoverSelectionTimer(destinationHit.collider);
				ResetSelectionTimer();
				OnDestinationMarkerSet(SetDestinationMarkerEvent(destinationHit.distance, destinationHit.transform, destinationHit, destinationHit.point, controllerReference, forceDestinationPosition: false, GetCursorRotation()));
			}
		}
	}

	protected virtual bool CanResetActivationState(bool givenState)
	{
		return (!givenState && holdButtonToActivate) || (givenState && !holdButtonToActivate && currentActivationState >= 2);
	}

	protected virtual void ManageActivationState(bool state)
	{
		if (state)
		{
			currentActivationState++;
		}
		wasActivated = currentActivationState == 2;
		if (CanResetActivationState(state))
		{
			currentActivationState = 0;
		}
	}

	protected virtual bool PointerActivatesUseAction(VRTK_InteractableObject givenInteractableObject)
	{
		return givenInteractableObject != null && givenInteractableObject.pointerActivatesUseAction && givenInteractableObject.IsValidInteractableController(controller.gameObject, givenInteractableObject.allowedUseControllers);
	}

	protected virtual void StartUseAction(Transform target)
	{
		pointerInteractableObject = target.GetComponent<VRTK_InteractableObject>();
		bool flag = (bool)pointerInteractableObject && pointerInteractableObject.useOnlyIfGrabbed && !pointerInteractableObject.IsGrabbed();
		if (PointerActivatesUseAction(pointerInteractableObject) && pointerInteractableObject.holdButtonToUse && !flag && pointerInteractableObject.usingState == 0)
		{
			pointerInteractableObject.StartUsing(controller.gameObject);
			pointerInteractableObject.usingState++;
		}
	}

	protected virtual void StopUseAction()
	{
		if (PointerActivatesUseAction(pointerInteractableObject) && pointerInteractableObject.holdButtonToUse && pointerInteractableObject.IsUsing())
		{
			pointerInteractableObject.StopUsing(controller.gameObject);
			pointerInteractableObject.usingState = 0;
		}
	}

	protected virtual void AttemptUseOnSet(Transform target)
	{
		if (pointerInteractableObject != null && target != null && PointerActivatesUseAction(pointerInteractableObject))
		{
			if (pointerInteractableObject.IsUsing())
			{
				pointerInteractableObject.StopUsing(controller.gameObject);
				pointerInteractableObject.usingState = 0;
			}
			else if (!pointerInteractableObject.holdButtonToUse)
			{
				pointerInteractableObject.StartUsing(controller.gameObject);
				pointerInteractableObject.usingState++;
			}
		}
	}

	protected virtual void SetHoverSelectionTimer(Collider collider)
	{
		if (collider != currentCollider)
		{
			hoverDurationTimer = 0f;
		}
		if (selectAfterHoverDuration > 0f && hoverDurationTimer <= 0f)
		{
			canClickOnHover = true;
			hoverDurationTimer = selectAfterHoverDuration;
		}
		currentCollider = collider;
	}

	protected virtual void ResetHoverSelectionTimer(Collider collider)
	{
		canClickOnHover = false;
		hoverDurationTimer = ((!(collider == currentCollider)) ? hoverDurationTimer : 0f);
	}

	protected virtual void CheckHoverSelect()
	{
		if (hoverDurationTimer > 0f)
		{
			hoverDurationTimer -= Time.deltaTime;
		}
		if (canClickOnHover && hoverDurationTimer <= 0f)
		{
			canClickOnHover = false;
			ExecuteSelectionButtonAction();
		}
	}
}
