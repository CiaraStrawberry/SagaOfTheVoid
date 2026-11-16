using System;
using UnityEngine;
using UnityEngine.AI;

namespace VRTK;

[Obsolete("`VRTK_BasePointer` has been replaced with `VRTK_Pointer`. This script will be removed in a future version of VRTK.")]
public abstract class VRTK_BasePointer : VRTK_DestinationMarker
{
	public enum pointerVisibilityStates
	{
		On_When_Active,
		Always_On,
		Always_Off
	}

	[Serializable]
	public sealed class PointerOriginSmoothingSettings
	{
		[Tooltip("Whether or not to smooth the position of the pointer origin when positioning the pointer tip.")]
		public bool smoothsPosition;

		[Tooltip("The maximum allowed distance between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
		public float maxAllowedPerFrameDistanceDifference = 0.003f;

		[Tooltip("Whether or not to smooth the rotation of the pointer origin when positioning the pointer tip.")]
		public bool smoothsRotation;

		[Tooltip("The maximum allowed angle between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
		public float maxAllowedPerFrameAngleDifference = 1.5f;
	}

	[Header("Base Pointer Settings", order = 2)]
	[Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
	public VRTK_ControllerEvents controller;

	[Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
	public Transform pointerOriginTransform;

	[Tooltip("Specifies the smoothing to be applied to the pointer origin when positioning the pointer tip.")]
	public PointerOriginSmoothingSettings pointerOriginSmoothingSettings = new PointerOriginSmoothingSettings();

	[Tooltip("The material to use on the rendered version of the pointer. If no material is selected then the default `WorldPointer` material will be used.")]
	public Material pointerMaterial;

	[Tooltip("The colour of the beam when it is colliding with a valid target. It can be set to a different colour for each controller.")]
	public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);

	[Tooltip("The colour of the beam when it is not hitting a valid target. It can be set to a different colour for each controller.")]
	public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);

	[Tooltip("If this is checked then the pointer beam will be activated on first press of the pointer alias button and will stay active until the pointer alias button is pressed again. The destination set event is emitted when the beam is deactivated on the second button press.")]
	public bool holdButtonToActivate = true;

	[Tooltip("If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.")]
	public bool interactWithObjects;

	[Tooltip("If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.")]
	public bool grabToPointerTip;

	[Tooltip("The time in seconds to delay the pointer beam being able to be active again. Useful for preventing constant teleportation.")]
	public float activateDelay;

	[Tooltip("Determines when the pointer beam should be displayed.")]
	public pointerVisibilityStates pointerVisibility;

	[Tooltip("The layers to ignore when raycasting.")]
	public LayerMask layersToIgnore = 4;

	protected Vector3 destinationPosition;

	protected float pointerContactDistance;

	protected Transform pointerContactTarget;

	protected RaycastHit pointerContactRaycastHit = default;

	protected VRTK_ControllerReference controllerReference;

	protected VRTK_PlayAreaCursor playAreaCursor;

	protected Color currentPointerColor;

	protected GameObject objectInteractor;

	protected GameObject objectInteractorAttachPoint;

	private bool isActive;

	private bool destinationSetActive;

	private float activateDelayTimer;

	private int beamEnabledState;

	private VRTK_InteractableObject interactableObject;

	private Rigidbody savedAttachPoint;

	private bool attachedToInteractorAttachPoint;

	private float savedBeamLength;

	private VRTK_InteractGrab controllerGrabScript;

	private GameObject pointerOriginTransformFollowGameObject;

	private VRTK_TransformFollow pointerOriginTransformFollow;

	private Transform originalPointerOriginTransform;

	public virtual bool IsActive()
	{
		return isActive;
	}

	public virtual bool CanActivate()
	{
		return Time.time >= activateDelayTimer;
	}

	public virtual void ToggleBeam(bool state)
	{
		VRTK_ControllerReference givenControllerReference = ((!(controller != null)) ? null : VRTK_ControllerReference.GetControllerReference(controller.gameObject));
		if (state)
		{
			TurnOnBeam(givenControllerReference);
		}
		else
		{
			TurnOffBeam(givenControllerReference);
		}
	}

	protected virtual void Awake()
	{
		VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
		originalPointerOriginTransform = pointerOriginTransform;
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		pointerOriginTransform = ((!(originalPointerOriginTransform == null)) ? originalPointerOriginTransform : VRTK_SDK_Bridge.GenerateControllerPointerOrigin(gameObject));
		AttemptSetController();
		CreatePointerOriginTransformFollow();
		Material source = Resources.Load("WorldPointer") as Material;
		if (pointerMaterial != null)
		{
			source = pointerMaterial;
		}
		pointerMaterial = new Material(source);
		pointerMaterial.color = pointerMissColor;
		playAreaCursor = GetComponent<VRTK_PlayAreaCursor>();
	}

	protected virtual void Start()
	{
		SetupController();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		DisableBeam();
		UnityEngine.Object.Destroy(objectInteractor);
		destinationSetActive = false;
		pointerContactDistance = 0f;
		pointerContactTarget = null;
		destinationPosition = Vector3.zero;
		AliasRegistration(state: false);
		controllerGrabScript = null;
		UnityEngine.Object.Destroy(pointerOriginTransformFollowGameObject);
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate()
	{
		if (interactWithObjects && (bool)objectInteractor && objectInteractor.activeInHierarchy)
		{
			UpdateObjectInteractor();
		}
		if (pointerOriginTransformFollow.isActiveAndEnabled)
		{
			UpdatePointerOriginTransformFollow();
		}
	}

	protected virtual void AliasRegistration(bool state)
	{
		if ((bool)controller)
		{
			if (state)
			{
				controller.AliasPointerOn += EnablePointerBeam;
				controller.AliasPointerOff += DisablePointerBeam;
				controller.AliasPointerSet += SetPointerDestination;
			}
			else
			{
				controller.AliasPointerOn -= EnablePointerBeam;
				controller.AliasPointerOff -= DisablePointerBeam;
				controller.AliasPointerSet -= SetPointerDestination;
			}
		}
	}

	protected virtual void OnValidate()
	{
		pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference);
		pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference);
	}

	protected Transform GetOrigin(bool smoothed = true)
	{
		Transform result;
		if (smoothed && isActive)
		{
			result = pointerOriginTransformFollow.gameObjectToChange.transform;
		}
		else
		{
			result = ((!(pointerOriginTransform == null)) ? pointerOriginTransform : transform);
		}
		return result;
	}

	protected virtual void UpdateObjectInteractor()
	{
		objectInteractor.transform.position = destinationPosition;
	}

	protected virtual void UpdatePointerOriginTransformFollow()
	{
		pointerOriginTransformFollow.gameObjectToFollow = ((!(pointerOriginTransform == null)) ? pointerOriginTransform : transform).gameObject;
		pointerOriginTransformFollow.smoothsPosition = pointerOriginSmoothingSettings.smoothsPosition;
		pointerOriginTransformFollow.maxAllowedPerFrameDistanceDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference;
		pointerOriginTransformFollow.smoothsRotation = pointerOriginSmoothingSettings.smoothsRotation;
		pointerOriginTransformFollow.maxAllowedPerFrameAngleDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference;
	}

	protected virtual void InitPointer()
	{
	}

	protected virtual void UpdateDependencies(Vector3 location)
	{
		if ((bool)playAreaCursor)
		{
			playAreaCursor.SetPlayAreaCursorTransform(location);
		}
	}

	protected virtual void EnablePointerBeam(object sender, ControllerInteractionEventArgs e)
	{
		TurnOnBeam(e.controllerReference);
	}

	protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
	{
		TurnOffBeam(e.controllerReference);
	}

	protected virtual void SetPointerDestination(object sender, ControllerInteractionEventArgs e)
	{
		PointerSet();
	}

	protected virtual void PointerIn()
	{
		if (enabled && (bool)pointerContactTarget)
		{
			OnDestinationMarkerEnter(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerReference));
			StartUseAction(pointerContactTarget);
		}
	}

	protected virtual void PointerOut()
	{
		if (enabled && (bool)pointerContactTarget)
		{
			OnDestinationMarkerExit(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerReference));
			StopUseAction();
		}
	}

	protected virtual void PointerSet()
	{
		if (!enabled || !destinationSetActive || !pointerContactTarget || !CanActivate() || InvalidConstantBeam())
		{
			return;
		}
		activateDelayTimer = Time.time + activateDelay;
		VRTK_InteractableObject component = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
		if (PointerActivatesUseAction(component))
		{
			if (component.IsUsing())
			{
				component.StopUsing(controller.gameObject);
				interactableObject.usingState = 0;
			}
			else if (!component.holdButtonToUse)
			{
				component.StartUsing(controller.gameObject);
				interactableObject.usingState++;
			}
		}
		if ((!playAreaCursor || !playAreaCursor.HasCollided()) && !PointerActivatesUseAction(interactableObject))
		{
			OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerReference));
		}
		if (!isActive)
		{
			destinationSetActive = false;
		}
	}

	protected virtual void TogglePointer(bool state)
	{
		ToggleObjectInteraction(state);
		if ((bool)playAreaCursor)
		{
			playAreaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
			playAreaCursor.ToggleState(state);
		}
		if (!state && PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && interactableObject.IsUsing())
		{
			interactableObject.StopUsing(controller.gameObject);
		}
	}

	protected virtual void ToggleObjectInteraction(bool state)
	{
		if (!interactWithObjects)
		{
			return;
		}
		if (state && grabToPointerTip && (bool)controllerGrabScript)
		{
			savedAttachPoint = controllerGrabScript.controllerAttachPoint;
			controllerGrabScript.controllerAttachPoint = objectInteractorAttachPoint.GetComponent<Rigidbody>();
			attachedToInteractorAttachPoint = true;
		}
		if (!state && grabToPointerTip && (bool)controllerGrabScript)
		{
			if (attachedToInteractorAttachPoint)
			{
				controllerGrabScript.ForceRelease(applyGrabbingObjectVelocity: true);
			}
			controllerGrabScript.controllerAttachPoint = savedAttachPoint;
			savedAttachPoint = null;
			attachedToInteractorAttachPoint = false;
			savedBeamLength = 0f;
		}
		if ((bool)objectInteractor)
		{
			objectInteractor.SetActive(state);
		}
	}

	protected virtual void ChangeMaterialColor(GameObject obj, Color color)
	{
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if ((bool)renderer.material)
			{
				renderer.material.EnableKeyword("_EMISSION");
				if (renderer.material.HasProperty("_Color"))
				{
					renderer.material.color = color;
				}
				if (renderer.material.HasProperty("_EmissionColor"))
				{
					renderer.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, 50f));
				}
			}
		}
	}

	protected virtual void SetPointerMaterial(Color color)
	{
		if ((bool)playAreaCursor)
		{
			playAreaCursor.SetMaterialColor(color, validity: true);
		}
	}

	protected void UpdatePointerMaterial(Color color)
	{
		if (((bool)playAreaCursor && playAreaCursor.HasCollided()) || !ValidDestination(pointerContactTarget, destinationPosition))
		{
			color = pointerMissColor;
		}
		currentPointerColor = color;
		SetPointerMaterial(color);
	}

	protected virtual bool ValidDestination(Transform target, Vector3 destinationPosition)
	{
		bool flag = false;
		if ((bool)target)
		{
			flag = NavMesh.SamplePosition(destinationPosition, out var _, navMeshCheckDistance, -1);
		}
		if (navMeshCheckDistance == 0f)
		{
			flag = true;
		}
		return flag && (bool)target && !VRTK_PolicyList.Check(target.gameObject, invalidListPolicy);
	}

	protected virtual void CreateObjectInteractor()
	{
		objectInteractor = new GameObject($"[{base.gameObject.name}]BasePointer_ObjectInteractor_Holder");
		objectInteractor.transform.SetParent(controller.transform);
		objectInteractor.transform.localPosition = Vector3.zero;
		objectInteractor.layer = LayerMask.NameToLayer("Ignore Raycast");
		VRTK_PlayerObject.SetPlayerObject(objectInteractor, VRTK_PlayerObject.ObjectTypes.Pointer);
		GameObject gameObject = new GameObject($"[{base.gameObject.name}]BasePointer_ObjectInteractor_Collider");
		gameObject.transform.SetParent(objectInteractor.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
		if (grabToPointerTip)
		{
			objectInteractorAttachPoint = new GameObject($"[{base.gameObject.name}]BasePointer_ObjectInteractor_AttachPoint");
			objectInteractorAttachPoint.transform.SetParent(objectInteractor.transform);
			objectInteractorAttachPoint.transform.localPosition = Vector3.zero;
			objectInteractorAttachPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
			Rigidbody rigidbody = objectInteractorAttachPoint.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.freezeRotation = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			VRTK_PlayerObject.SetPlayerObject(objectInteractorAttachPoint, VRTK_PlayerObject.ObjectTypes.Pointer);
		}
		float num = 0.025f;
		objectInteractor.transform.localScale = new Vector3(num, num, num);
		objectInteractor.SetActive(value: false);
	}

	protected virtual void CreatePointerOriginTransformFollow()
	{
		pointerOriginTransformFollowGameObject = new GameObject($"[{gameObject.name}]BasePointer_Origin_Smoothed");
		pointerOriginTransformFollowGameObject.SetActive(value: false);
		pointerOriginTransformFollow = pointerOriginTransformFollowGameObject.AddComponent<VRTK_TransformFollow>();
		pointerOriginTransformFollow.followsScale = false;
		UpdatePointerOriginTransformFollow();
	}

	protected virtual float OverrideBeamLength(float currentLength)
	{
		if (!controllerGrabScript || !controllerGrabScript.GetGrabbedObject())
		{
			savedBeamLength = 0f;
		}
		if (interactWithObjects && grabToPointerTip && attachedToInteractorAttachPoint && (bool)controllerGrabScript && (bool)controllerGrabScript.GetGrabbedObject())
		{
			savedBeamLength = ((savedBeamLength != 0f) ? savedBeamLength : currentLength);
			return savedBeamLength;
		}
		return currentLength;
	}

	private void SetupController()
	{
		if (controller == null)
		{
			controller = GetComponent<VRTK_ControllerEvents>();
			AttemptSetController();
		}
		if (controller == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_BasePointer", "VRTK_ControllerEvents", "controller", "the same"));
		}
	}

	private void AttemptSetController()
	{
		if ((bool)controller)
		{
			AliasRegistration(state: true);
			controllerGrabScript = controller.GetComponent<VRTK_InteractGrab>();
			if (interactWithObjects)
			{
				CreateObjectInteractor();
			}
		}
	}

	private bool InvalidConstantBeam()
	{
		bool flag = controller.pointerToggleButton == controller.pointerSetButton;
		return !holdButtonToActivate && ((flag && beamEnabledState != 0) || (!flag && !isActive));
	}

	private bool PointerActivatesUseAction(VRTK_InteractableObject io)
	{
		return (bool)io && io.pointerActivatesUseAction && io.IsValidInteractableController(gameObject, io.allowedUseControllers);
	}

	private void StartUseAction(Transform target)
	{
		interactableObject = target.GetComponent<VRTK_InteractableObject>();
		bool flag = (bool)interactableObject && interactableObject.useOnlyIfGrabbed && !interactableObject.IsGrabbed();
		if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && !flag && interactableObject.usingState == 0)
		{
			interactableObject.StartUsing(controller.gameObject);
			interactableObject.usingState++;
		}
	}

	private void StopUseAction()
	{
		if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse)
		{
			interactableObject.StopUsing(controller.gameObject);
			interactableObject.usingState = 0;
		}
	}

	private void TurnOnBeam(VRTK_ControllerReference givenControllerReference)
	{
		beamEnabledState++;
		if (enabled && !isActive && CanActivate())
		{
			if ((bool)playAreaCursor)
			{
				playAreaCursor.SetPlayAreaCursorCollision(state: false);
			}
			controllerReference = givenControllerReference;
			TogglePointer(state: true);
			isActive = true;
			destinationSetActive = true;
			if (pointerOriginTransformFollowGameObject != null)
			{
				pointerOriginTransformFollowGameObject.SetActive(value: true);
				pointerOriginTransformFollow.Follow();
			}
		}
	}

	private void TurnOffBeam(VRTK_ControllerReference givenControllerReference)
	{
		if (enabled && isActive && (holdButtonToActivate || (!holdButtonToActivate && beamEnabledState >= 2)))
		{
			controllerReference = givenControllerReference;
			DisableBeam();
		}
	}

	private void DisableBeam()
	{
		TogglePointer(state: false);
		isActive = false;
		beamEnabledState = 0;
		if (pointerOriginTransformFollowGameObject != null)
		{
			pointerOriginTransformFollowGameObject.SetActive(value: false);
		}
	}
}
