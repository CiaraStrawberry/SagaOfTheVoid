using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.Highlighters;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractableObject")]
public class VRTK_InteractableObject : MonoBehaviour
{
	public enum AllowedController
	{
		Both,
		LeftOnly,
		RightOnly
	}

	public enum ValidDropTypes
	{
		NoDrop,
		DropAnywhere,
		DropValidSnapDropZone
	}

	[Tooltip("If this is checked then the interactable object script will be disabled when the object is not being interacted with. This will eliminate the potential number of calls the interactable objects make each frame.")]
	public bool disableWhenIdle = true;

	[Header("Touch Options", order = 1)]
	[Tooltip("The colour to highlight the object when it is touched. This colour will override any globally set colour (for instance on the `VRTK_InteractTouch` script).")]
	public Color touchHighlightColor = Color.clear;

	[Tooltip("Determines which controller can initiate a touch action.")]
	public AllowedController allowedTouchControllers;

	[Tooltip("An array of colliders on the object to ignore when being touched.")]
	public Collider[] ignoredColliders;

	[Header("Grab Options", order = 2)]
	[Tooltip("Determines if the object can be grabbed.")]
	public bool isGrabbable;

	[Tooltip("If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.")]
	public bool holdButtonToGrab = true;

	[Tooltip("If this is checked then the object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the object will be released when a teleport occurs.")]
	public bool stayGrabbedOnTeleport = true;

	[Tooltip("Determines in what situation the object can be dropped by the controller grab button.")]
	public ValidDropTypes validDrop = ValidDropTypes.DropAnywhere;

	[Tooltip("If this is set to `Undefined` then the global grab alias button will grab the object, setting it to any other button will ensure the override button is used to grab this specific interactable object.")]
	public VRTK_ControllerEvents.ButtonAlias grabOverrideButton;

	[Tooltip("Determines which controller can initiate a grab action.")]
	public AllowedController allowedGrabControllers;

	[Tooltip("This determines how the grabbed item will be attached to the controller when it is grabbed. If one isn't provided then the first Grab Attach script on the GameObject will be used, if one is not found and the object is grabbable then a Fixed Joint Grab Attach script will be created at runtime.")]
	public VRTK_BaseGrabAttach grabAttachMechanicScript;

	[Tooltip("The script to utilise when processing the secondary controller action on a secondary grab attempt. If one isn't provided then the first Secondary Controller Grab Action script on the GameObject will be used, if one is not found then no action will be taken on secondary grab.")]
	public VRTK_BaseGrabAction secondaryGrabActionScript;

	[Header("Use Options", order = 3)]
	[Tooltip("Determines if the object can be used.")]
	public bool isUsable;

	[Tooltip("If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.")]
	public bool holdButtonToUse = true;

	[Tooltip("If this is checked the object can be used only if it is currently being grabbed.")]
	public bool useOnlyIfGrabbed;

	[Tooltip("If this is checked then when a Base Pointer beam (projected from the controller) hits the interactable object, if the object has `Hold Button To Use` unchecked then whilst the pointer is over the object it will run it's `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the pointer is deactivated. The world pointer will not throw the `Destination Set` event if it is affecting an interactable object with this setting checked as this prevents unwanted teleporting from happening when using an object with a pointer.")]
	public bool pointerActivatesUseAction;

	[Tooltip("If this is set to `Undefined` then the global use alias button will use the object, setting it to any other button will ensure the override button is used to use this specific interactable object.")]
	public VRTK_ControllerEvents.ButtonAlias useOverrideButton;

	[Tooltip("Determines which controller can initiate a use action.")]
	public AllowedController allowedUseControllers;

	[HideInInspector]
	public int usingState;

	protected Rigidbody interactableRigidbody;

	protected List<GameObject> touchingObjects = new List<GameObject>();

	protected List<GameObject> grabbingObjects = new List<GameObject>();

	protected List<GameObject> hoveredSnapObjects = new List<GameObject>();

	protected GameObject usingObject;

	protected Transform trackPoint;

	protected bool customTrackPoint;

	protected Transform primaryControllerAttachPoint;

	protected Transform secondaryControllerAttachPoint;

	protected Transform previousParent;

	protected bool previousKinematicState;

	protected bool previousIsGrabbable;

	protected bool forcedDropped;

	protected bool forceDisabled;

	protected VRTK_BaseHighlighter objectHighlighter;

	protected bool autoHighlighter;

	protected bool hoveredOverSnapDropZone;

	protected bool snappedInSnapDropZone;

	protected VRTK_SnapDropZone storedSnapDropZone;

	protected Vector3 previousLocalScale = Vector3.zero;

	protected List<GameObject> currentIgnoredColliders = new List<GameObject>();

	protected bool startDisabled;

	public bool isKinematic
	{
		get
		{
			if ((bool)interactableRigidbody)
			{
				return interactableRigidbody.isKinematic;
			}
			return true;
		}
		set
		{
			if ((bool)interactableRigidbody)
			{
				interactableRigidbody.isKinematic = value;
			}
		}
	}

	public event InteractableObjectEventHandler InteractableObjectTouched;

	public event InteractableObjectEventHandler InteractableObjectUntouched;

	public event InteractableObjectEventHandler InteractableObjectGrabbed;

	public event InteractableObjectEventHandler InteractableObjectUngrabbed;

	public event InteractableObjectEventHandler InteractableObjectUsed;

	public event InteractableObjectEventHandler InteractableObjectUnused;

	public event InteractableObjectEventHandler InteractableObjectEnteredSnapDropZone;

	public event InteractableObjectEventHandler InteractableObjectExitedSnapDropZone;

	public event InteractableObjectEventHandler InteractableObjectSnappedToDropZone;

	public event InteractableObjectEventHandler InteractableObjectUnsnappedFromDropZone;

	public virtual void OnInteractableObjectTouched(InteractableObjectEventArgs e)
	{
		if (InteractableObjectTouched != null)
		{
			InteractableObjectTouched(this, e);
		}
	}

	public virtual void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
	{
		if (InteractableObjectUntouched != null)
		{
			InteractableObjectUntouched(this, e);
		}
	}

	public virtual void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
	{
		if (InteractableObjectGrabbed != null)
		{
			InteractableObjectGrabbed(this, e);
		}
	}

	public virtual void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
	{
		if (InteractableObjectUngrabbed != null)
		{
			InteractableObjectUngrabbed(this, e);
		}
	}

	public virtual void OnInteractableObjectUsed(InteractableObjectEventArgs e)
	{
		if (InteractableObjectUsed != null)
		{
			InteractableObjectUsed(this, e);
		}
	}

	public virtual void OnInteractableObjectUnused(InteractableObjectEventArgs e)
	{
		if (InteractableObjectUnused != null)
		{
			InteractableObjectUnused(this, e);
		}
	}

	public virtual void OnInteractableObjectEnteredSnapDropZone(InteractableObjectEventArgs e)
	{
		if (InteractableObjectEnteredSnapDropZone != null)
		{
			InteractableObjectEnteredSnapDropZone(this, e);
		}
	}

	public virtual void OnInteractableObjectExitedSnapDropZone(InteractableObjectEventArgs e)
	{
		if (InteractableObjectExitedSnapDropZone != null)
		{
			InteractableObjectExitedSnapDropZone(this, e);
		}
	}

	public virtual void OnInteractableObjectSnappedToDropZone(InteractableObjectEventArgs e)
	{
		if (InteractableObjectSnappedToDropZone != null)
		{
			InteractableObjectSnappedToDropZone(this, e);
		}
	}

	public virtual void OnInteractableObjectUnsnappedFromDropZone(InteractableObjectEventArgs e)
	{
		if (InteractableObjectUnsnappedFromDropZone != null)
		{
			InteractableObjectUnsnappedFromDropZone(this, e);
		}
	}

	public InteractableObjectEventArgs SetInteractableObjectEvent(GameObject interactingObject)
	{
		InteractableObjectEventArgs result = default;
		result.interactingObject = interactingObject;
		return result;
	}

	public virtual bool IsTouched()
	{
		return touchingObjects.Count > 0;
	}

	public virtual bool IsGrabbed(GameObject grabbedBy = null)
	{
		if (grabbingObjects.Count > 0 && grabbedBy != null)
		{
			return grabbingObjects.Contains(grabbedBy);
		}
		return grabbingObjects.Count > 0;
	}

	public virtual bool IsUsing(GameObject usedBy = null)
	{
		if ((bool)usingObject && usedBy != null)
		{
			return usingObject == usedBy;
		}
		return usingObject != null;
	}

	public virtual void StartTouching(GameObject currentTouchingObject)
	{
		IgnoreColliders(currentTouchingObject);
		if (!touchingObjects.Contains(currentTouchingObject))
		{
			ToggleEnableState(state: true);
			touchingObjects.Add(currentTouchingObject);
			OnInteractableObjectTouched(SetInteractableObjectEvent(currentTouchingObject));
		}
	}

	public virtual void StopTouching(GameObject previousTouchingObject)
	{
		if (touchingObjects.Contains(previousTouchingObject))
		{
			ResetUseState(previousTouchingObject);
			OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingObject));
			touchingObjects.Remove(previousTouchingObject);
		}
	}

	public virtual void Grabbed(GameObject currentGrabbingObject)
	{
		ToggleEnableState(state: true);
		if (!IsGrabbed() || IsSwappable())
		{
			PrimaryControllerGrab(currentGrabbingObject);
		}
		else
		{
			SecondaryControllerGrab(currentGrabbingObject);
		}
		OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingObject));
	}

	public virtual void Ungrabbed(GameObject previousGrabbingObject)
	{
		GameObject secondaryGrabbingObject = GetSecondaryGrabbingObject();
		if (!secondaryGrabbingObject || secondaryGrabbingObject != previousGrabbingObject)
		{
			SecondaryControllerUngrab(secondaryGrabbingObject);
			PrimaryControllerUngrab(previousGrabbingObject, secondaryGrabbingObject);
		}
		else
		{
			SecondaryControllerUngrab(previousGrabbingObject);
		}
		OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingObject));
	}

	public virtual void StartUsing(GameObject currentUsingObject)
	{
		ToggleEnableState(state: true);
		if (IsUsing() && !IsUsing(currentUsingObject))
		{
			ResetUsingObject();
		}
		OnInteractableObjectUsed(SetInteractableObjectEvent(currentUsingObject));
		usingObject = currentUsingObject;
	}

	public virtual void StopUsing(GameObject previousUsingObject)
	{
		OnInteractableObjectUnused(SetInteractableObjectEvent(previousUsingObject));
		ResetUsingObject();
		usingState = 0;
		usingObject = null;
	}

	public virtual void ToggleHighlight(bool toggle)
	{
		InitialiseHighlighter();
		if (touchHighlightColor != Color.clear && (bool)objectHighlighter)
		{
			if (toggle && !IsGrabbed())
			{
				objectHighlighter.Highlight(touchHighlightColor);
			}
			else
			{
				objectHighlighter.Unhighlight();
			}
		}
	}

	public virtual void ResetHighlighter()
	{
		if ((bool)objectHighlighter)
		{
			objectHighlighter.ResetHighlighter();
		}
	}

	public virtual void PauseCollisions(float delay)
	{
		if (delay > 0f)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				rigidbody.detectCollisions = false;
			}
			Invoke("UnpauseCollisions", delay);
		}
	}

	public virtual void ZeroVelocity()
	{
		if ((bool)interactableRigidbody)
		{
			interactableRigidbody.velocity = Vector3.zero;
			interactableRigidbody.angularVelocity = Vector3.zero;
		}
	}

	public virtual void SaveCurrentState()
	{
		if (!IsGrabbed() && !snappedInSnapDropZone)
		{
			previousParent = transform.parent;
			if (!IsSwappable())
			{
				previousIsGrabbable = isGrabbable;
			}
			if ((bool)interactableRigidbody)
			{
				previousKinematicState = interactableRigidbody.isKinematic;
			}
		}
	}

	public virtual List<GameObject> GetTouchingObjects()
	{
		return touchingObjects;
	}

	public virtual GameObject GetGrabbingObject()
	{
		return (!IsGrabbed()) ? null : grabbingObjects[0];
	}

	public virtual GameObject GetSecondaryGrabbingObject()
	{
		return (grabbingObjects.Count <= 1) ? null : grabbingObjects[1];
	}

	public virtual GameObject GetUsingObject()
	{
		return usingObject;
	}

	public virtual bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)
	{
		if (controllerCheck == AllowedController.Both)
		{
			return true;
		}
		SDK_BaseController.ControllerHand controllerHandType = VRTK_DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("Only", string.Empty));
		return VRTK_DeviceFinder.IsControllerOfHand(actualController, controllerHandType);
	}

	public virtual void ForceStopInteracting()
	{
		if (gameObject.activeInHierarchy)
		{
			forceDisabled = false;
			StartCoroutine(ForceStopInteractingAtEndOfFrame());
		}
		if (!gameObject.activeInHierarchy && forceDisabled)
		{
			ForceStopAllInteractions();
			forceDisabled = false;
		}
	}

	public virtual void ForceStopSecondaryGrabInteraction()
	{
		GameObject secondaryGrabbingObject = GetSecondaryGrabbingObject();
		if (secondaryGrabbingObject != null)
		{
			secondaryGrabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
		}
	}

	public virtual void RegisterTeleporters()
	{
		StartCoroutine(RegisterTeleportersAtEndOfFrame());
	}

	public virtual void UnregisterTeleporters()
	{
		foreach (VRTK_BasicTeleport registeredTeleporter in VRTK_ObjectCache.registeredTeleporters)
		{
			registeredTeleporter.Teleporting -= OnTeleporting;
			registeredTeleporter.Teleported -= OnTeleported;
		}
	}

	public virtual void StoreLocalScale()
	{
		previousLocalScale = transform.localScale;
	}

	public virtual void ToggleSnapDropZone(VRTK_SnapDropZone snapDropZone, bool state)
	{
		snappedInSnapDropZone = state;
		if (state)
		{
			storedSnapDropZone = snapDropZone;
			OnInteractableObjectSnappedToDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
		}
		else
		{
			interactableRigidbody.WakeUp();
			ResetDropSnapType();
			OnInteractableObjectUnsnappedFromDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
		}
	}

	public virtual bool IsInSnapDropZone()
	{
		return snappedInSnapDropZone;
	}

	public virtual void SetSnapDropZoneHover(VRTK_SnapDropZone snapDropZone, bool state)
	{
		if (state)
		{
			if (!hoveredSnapObjects.Contains(snapDropZone.gameObject))
			{
				hoveredSnapObjects.Add(snapDropZone.gameObject);
				OnInteractableObjectEnteredSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
			}
		}
		else if (hoveredSnapObjects.Contains(snapDropZone.gameObject))
		{
			hoveredSnapObjects.Remove(snapDropZone.gameObject);
			OnInteractableObjectExitedSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
		}
		hoveredOverSnapDropZone = hoveredSnapObjects.Count > 0;
	}

	public virtual VRTK_SnapDropZone GetStoredSnapDropZone()
	{
		return storedSnapDropZone;
	}

	public virtual bool IsDroppable()
	{
		return validDrop switch
		{
			ValidDropTypes.NoDrop => false, 
			ValidDropTypes.DropAnywhere => true, 
			ValidDropTypes.DropValidSnapDropZone => hoveredOverSnapDropZone, 
			_ => false, 
		};
	}

	public virtual bool IsSwappable()
	{
		return secondaryGrabActionScript != null && secondaryGrabActionScript.IsSwappable();
	}

	public virtual bool PerformSecondaryAction()
	{
		return GetGrabbingObject() != null && GetSecondaryGrabbingObject() == null && secondaryGrabActionScript != null && secondaryGrabActionScript.IsActionable();
	}

	public virtual void ResetIgnoredColliders()
	{
		for (int i = 0; i < currentIgnoredColliders.Count; i++)
		{
			if (!(currentIgnoredColliders[i] != null))
			{
				continue;
			}
			Collider[] componentsInChildren = currentIgnoredColliders[i].GetComponentsInChildren<Collider>();
			if (ignoredColliders == null)
			{
				continue;
			}
			for (int j = 0; j < ignoredColliders.Length; j++)
			{
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					Physics.IgnoreCollision(componentsInChildren[k], ignoredColliders[j], ignore: false);
				}
			}
		}
		currentIgnoredColliders.Clear();
	}

	protected virtual void Awake()
	{
		interactableRigidbody = GetComponent<Rigidbody>();
		if (interactableRigidbody != null)
		{
			interactableRigidbody.maxAngularVelocity = float.MaxValue;
		}
		if (disableWhenIdle && enabled)
		{
			startDisabled = true;
			enabled = false;
		}
	}

	protected virtual void OnEnable()
	{
		InitialiseHighlighter();
		RegisterTeleporters();
		forceDisabled = false;
		if (forcedDropped)
		{
			LoadPreviousState();
		}
		forcedDropped = false;
		startDisabled = false;
	}

	protected virtual void OnDisable()
	{
		UnregisterTeleporters();
		if (autoHighlighter)
		{
			Object.Destroy(objectHighlighter);
			objectHighlighter = null;
		}
		if (!startDisabled)
		{
			forceDisabled = true;
			ForceStopInteracting();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (trackPoint != null && grabAttachMechanicScript != null)
		{
			grabAttachMechanicScript.ProcessFixedUpdate();
		}
		if (secondaryGrabActionScript != null)
		{
			secondaryGrabActionScript.ProcessFixedUpdate();
		}
	}

	protected virtual void Update()
	{
		AttemptSetGrabMechanic();
		AttemptSetSecondaryGrabAction();
		if (trackPoint != null && grabAttachMechanicScript != null)
		{
			grabAttachMechanicScript.ProcessUpdate();
		}
		if (secondaryGrabActionScript != null)
		{
			secondaryGrabActionScript.ProcessUpdate();
		}
	}

	protected virtual void LateUpdate()
	{
		if (disableWhenIdle && !IsTouched() && !IsGrabbed() && !IsUsing())
		{
			ToggleEnableState(state: false);
		}
	}

	protected virtual void LoadPreviousState()
	{
		if (gameObject.activeInHierarchy)
		{
			transform.SetParent(previousParent);
			forcedDropped = false;
		}
		if (interactableRigidbody != null)
		{
			interactableRigidbody.isKinematic = previousKinematicState;
		}
		if (!IsSwappable())
		{
			isGrabbable = previousIsGrabbable;
		}
	}

	protected virtual void InitialiseHighlighter()
	{
		if (touchHighlightColor != Color.clear && objectHighlighter == null)
		{
			autoHighlighter = false;
			objectHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
			if (objectHighlighter == null)
			{
				autoHighlighter = true;
				objectHighlighter = gameObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
			}
			objectHighlighter.Initialise(touchHighlightColor);
		}
	}

	protected virtual void IgnoreColliders(GameObject touchingObject)
	{
		if (ignoredColliders == null || currentIgnoredColliders.Contains(touchingObject))
		{
			return;
		}
		bool flag = false;
		Collider[] componentsInChildren = touchingObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < ignoredColliders.Length; i++)
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[j], ignoredColliders[i]);
				flag = true;
			}
		}
		if (flag)
		{
			currentIgnoredColliders.Add(touchingObject);
		}
	}

	protected virtual void ToggleEnableState(bool state)
	{
		if (disableWhenIdle)
		{
			enabled = state;
		}
	}

	protected virtual void AttemptSetGrabMechanic()
	{
		if (isGrabbable && grabAttachMechanicScript == null)
		{
			VRTK_BaseGrabAttach vRTK_BaseGrabAttach = GetComponent<VRTK_BaseGrabAttach>();
			if (!vRTK_BaseGrabAttach)
			{
				vRTK_BaseGrabAttach = gameObject.AddComponent<VRTK_FixedJointGrabAttach>();
			}
			grabAttachMechanicScript = vRTK_BaseGrabAttach;
		}
	}

	protected virtual void AttemptSetSecondaryGrabAction()
	{
		if (isGrabbable && secondaryGrabActionScript == null)
		{
			secondaryGrabActionScript = GetComponent<VRTK_BaseGrabAction>();
		}
	}

	protected virtual void ForceReleaseGrab()
	{
		GameObject grabbingObject = GetGrabbingObject();
		if (grabbingObject != null)
		{
			grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
		}
	}

	protected virtual void PrimaryControllerGrab(GameObject currentGrabbingObject)
	{
		if (snappedInSnapDropZone)
		{
			ToggleSnapDropZone(storedSnapDropZone, state: false);
		}
		ForceReleaseGrab();
		RemoveTrackPoint();
		grabbingObjects.Add(currentGrabbingObject);
		SetTrackPoint(currentGrabbingObject);
		if (!IsSwappable())
		{
			previousIsGrabbable = isGrabbable;
			isGrabbable = false;
		}
	}

	protected virtual void SecondaryControllerGrab(GameObject currentGrabbingObject)
	{
		if (!grabbingObjects.Contains(currentGrabbingObject))
		{
			grabbingObjects.Add(currentGrabbingObject);
			secondaryControllerAttachPoint = CreateAttachPoint(currentGrabbingObject.name, "Secondary", currentGrabbingObject.transform);
			if (secondaryGrabActionScript != null)
			{
				secondaryGrabActionScript.Initialise(this, GetGrabbingObject().GetComponent<VRTK_InteractGrab>(), GetSecondaryGrabbingObject().GetComponent<VRTK_InteractGrab>(), primaryControllerAttachPoint, secondaryControllerAttachPoint);
			}
		}
	}

	protected virtual void PrimaryControllerUngrab(GameObject previousGrabbingObject, GameObject previousSecondaryGrabbingObject)
	{
		UnpauseCollisions();
		RemoveTrackPoint();
		ResetUseState(previousGrabbingObject);
		grabbingObjects.Clear();
		if (secondaryGrabActionScript != null && previousSecondaryGrabbingObject != null)
		{
			secondaryGrabActionScript.OnDropAction();
			previousSecondaryGrabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
		}
		LoadPreviousState();
	}

	protected virtual void SecondaryControllerUngrab(GameObject previousGrabbingObject)
	{
		if (grabbingObjects.Contains(previousGrabbingObject))
		{
			grabbingObjects.Remove(previousGrabbingObject);
			Object.Destroy(secondaryControllerAttachPoint.gameObject);
			secondaryControllerAttachPoint = null;
			if (secondaryGrabActionScript != null)
			{
				secondaryGrabActionScript.ResetAction();
			}
		}
	}

	protected virtual void UnpauseCollisions()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].detectCollisions = true;
		}
	}

	protected virtual void SetTrackPoint(GameObject currentGrabbingObject)
	{
		AddTrackPoint(currentGrabbingObject);
		primaryControllerAttachPoint = CreateAttachPoint(GetGrabbingObject().name, "Original", trackPoint);
		if (grabAttachMechanicScript != null)
		{
			grabAttachMechanicScript.SetTrackPoint(trackPoint);
			grabAttachMechanicScript.SetInitialAttachPoint(primaryControllerAttachPoint);
		}
	}

	protected virtual Transform CreateAttachPoint(string namePrefix, string nameSuffix, Transform origin)
	{
		Transform transform = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, namePrefix, nameSuffix, "Controller", "AttachPoint")).transform;
		transform.parent = base.transform;
		transform.position = origin.position;
		transform.rotation = origin.rotation;
		return transform;
	}

	protected virtual void AddTrackPoint(GameObject currentGrabbingObject)
	{
		VRTK_InteractGrab component = currentGrabbingObject.GetComponent<VRTK_InteractGrab>();
		Transform controllerPoint = ((!component || !component.controllerAttachPoint) ? currentGrabbingObject.transform : component.controllerAttachPoint.transform);
		if (grabAttachMechanicScript != null)
		{
			trackPoint = grabAttachMechanicScript.CreateTrackPoint(controllerPoint, gameObject, currentGrabbingObject, ref customTrackPoint);
		}
	}

	protected virtual void RemoveTrackPoint()
	{
		if (customTrackPoint && trackPoint != null)
		{
			Object.Destroy(trackPoint.gameObject);
		}
		else
		{
			trackPoint = null;
		}
		if (primaryControllerAttachPoint != null)
		{
			Object.Destroy(primaryControllerAttachPoint.gameObject);
		}
	}

	protected virtual void OnTeleporting(object sender, DestinationMarkerEventArgs e)
	{
		if (!stayGrabbedOnTeleport)
		{
			ZeroVelocity();
			ForceStopAllInteractions();
		}
	}

	protected virtual void OnTeleported(object sender, DestinationMarkerEventArgs e)
	{
		if (grabAttachMechanicScript != null && grabAttachMechanicScript.IsTracked() && stayGrabbedOnTeleport && trackPoint != null)
		{
			GameObject actualController = VRTK_DeviceFinder.GetActualController(GetGrabbingObject());
			transform.position = ((!actualController) ? transform.position : actualController.transform.position);
		}
	}

	protected virtual IEnumerator RegisterTeleportersAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		foreach (VRTK_BasicTeleport registeredTeleporter in VRTK_ObjectCache.registeredTeleporters)
		{
			registeredTeleporter.Teleporting += OnTeleporting;
			registeredTeleporter.Teleported += OnTeleported;
		}
	}

	protected virtual void ResetUseState(GameObject checkObject)
	{
		if (checkObject != null)
		{
			VRTK_InteractUse component = checkObject.GetComponent<VRTK_InteractUse>();
			if (component != null && holdButtonToUse)
			{
				component.ForceStopUsing();
			}
		}
	}

	protected virtual IEnumerator ForceStopInteractingAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		ForceStopAllInteractions();
	}

	protected virtual void ForceStopAllInteractions()
	{
		if (touchingObjects != null)
		{
			StopTouchingInteractions();
			StopGrabbingInteractions();
			StopUsingInteractions();
		}
	}

	protected virtual void StopTouchingInteractions()
	{
		for (int i = 0; i < touchingObjects.Count; i++)
		{
			GameObject gameObject = touchingObjects[i];
			if (gameObject.activeInHierarchy || forceDisabled)
			{
				gameObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
			}
		}
	}

	protected virtual void StopGrabbingInteractions()
	{
		GameObject grabbingObject = GetGrabbingObject();
		if (grabbingObject != null && (grabbingObject.activeInHierarchy || forceDisabled))
		{
			grabbingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
			grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
			forcedDropped = true;
		}
	}

	protected virtual void StopUsingInteractions()
	{
		if (usingObject != null && (usingObject.activeInHierarchy || forceDisabled))
		{
			usingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
			usingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
		}
	}

	protected virtual void ResetDropSnapType()
	{
		switch (storedSnapDropZone.snapType)
		{
		case VRTK_SnapDropZone.SnapTypes.UseKinematic:
		case VRTK_SnapDropZone.SnapTypes.UseParenting:
			LoadPreviousState();
			break;
		case VRTK_SnapDropZone.SnapTypes.UseJoint:
		{
			Joint component = storedSnapDropZone.GetComponent<Joint>();
			if ((bool)component)
			{
				component.connectedBody = null;
			}
			break;
		}
		}
		if (!previousLocalScale.Equals(Vector3.zero))
		{
			transform.localScale = previousLocalScale;
		}
		storedSnapDropZone.OnObjectUnsnappedFromDropZone(storedSnapDropZone.SetSnapDropZoneEvent(gameObject));
		storedSnapDropZone = null;
	}

	protected virtual void ResetUsingObject()
	{
		if (usingObject != null)
		{
			VRTK_InteractUse component = usingObject.GetComponent<VRTK_InteractUse>();
			if (component != null)
			{
				component.ForceResetUsing();
			}
		}
	}
}
