using System.Collections;
using UnityEngine;

namespace VRTK;

public class VRTK_DestinationPoint : VRTK_DestinationMarker
{
	public enum RotationTypes
	{
		NoRotation,
		RotateWithNoHeadsetOffset,
		RotateWithHeadsetOffset
	}

	[Header("Destination Point Settings")]
	[Tooltip("The GameObject to use to represent the default cursor state.")]
	public GameObject defaultCursorObject;

	[Tooltip("The GameObject to use to represent the hover cursor state.")]
	public GameObject hoverCursorObject;

	[Tooltip("The GameObject to use to represent the locked cursor state.")]
	public GameObject lockedCursorObject;

	[Tooltip("An optional transform to determine the destination location for the destination marker. This can be useful to offset the destination location from the destination point. If this is left empty then the destiantion point transform will be used.")]
	public Transform destinationLocation;

	[Tooltip("If this is checked then after teleporting, the play area will be snapped to the origin of the destination point. If this is false then it's possible to teleport to anywhere within the destination point collider.")]
	public bool snapToPoint = true;

	[Tooltip("If this is checked, then the pointer cursor will be hidden when a valid destination point is hovered over.")]
	public bool hidePointerCursorOnHover = true;

	[Tooltip("Determines if the play area will be rotated to the rotation of the destination point upon the destination marker being set.")]
	public RotationTypes snapToRotation;

	public static VRTK_DestinationPoint currentDestinationPoint;

	protected Collider pointCollider;

	protected bool createdCollider;

	protected Rigidbody pointRigidbody;

	protected bool createdRigidbody;

	protected Coroutine initaliseListeners;

	protected bool isActive;

	protected VRTK_BasePointerRenderer.VisibilityStates storedCursorState;

	protected Coroutine setDestination;

	protected bool currentTeleportState;

	protected Transform playArea;

	protected Transform headset;

	public virtual void ResetDestinationPoint()
	{
		ResetPoint();
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CreateColliderIfRequired();
		SetupRigidbody();
		initaliseListeners = StartCoroutine(ManageDestinationMarkersAtEndOfFrame());
		ResetPoint();
		currentTeleportState = enableTeleport;
		currentDestinationPoint = null;
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		headset = VRTK_DeviceFinder.HeadsetTransform();
		destinationLocation = ((!(destinationLocation != null)) ? transform : destinationLocation);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (initaliseListeners != null)
		{
			StopCoroutine(initaliseListeners);
		}
		if (setDestination != null)
		{
			StopCoroutine(setDestination);
		}
		ManageDestinationMarkers(state: false);
		if (createdCollider)
		{
			Object.Destroy(pointCollider);
		}
		if (createdRigidbody)
		{
			Object.Destroy(pointRigidbody);
		}
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		if (enableTeleport != currentTeleportState)
		{
			ResetPoint();
		}
		currentTeleportState = enableTeleport;
	}

	protected virtual void CreateColliderIfRequired()
	{
		pointCollider = GetComponentInChildren<Collider>();
		createdCollider = false;
		if (pointCollider == null)
		{
			pointCollider = gameObject.AddComponent<SphereCollider>();
			createdCollider = true;
		}
		pointCollider.isTrigger = true;
	}

	protected virtual void SetupRigidbody()
	{
		pointRigidbody = GetComponent<Rigidbody>();
		createdRigidbody = false;
		if (pointRigidbody == null)
		{
			pointRigidbody = gameObject.AddComponent<Rigidbody>();
			createdRigidbody = true;
		}
		pointRigidbody.isKinematic = true;
		pointRigidbody.useGravity = false;
	}

	protected virtual IEnumerator ManageDestinationMarkersAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		if (enabled)
		{
			ManageDestinationMarkers(state: true);
		}
	}

	protected virtual void ManageDestinationMarkers(bool state)
	{
		ManageDestinationMarkerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
		ManageDestinationMarkerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);
		foreach (VRTK_DestinationMarker registeredDestinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
		{
			ManageDestinationMarkerListeners(registeredDestinationMarker.gameObject, state);
		}
	}

	protected virtual void ManageDestinationMarkerListeners(GameObject markerMaker, bool register)
	{
		if (!(markerMaker != null))
		{
			return;
		}
		VRTK_DestinationMarker[] componentsInChildren = markerMaker.GetComponentsInChildren<VRTK_DestinationMarker>();
		foreach (VRTK_DestinationMarker vRTK_DestinationMarker in componentsInChildren)
		{
			if (!(vRTK_DestinationMarker == this))
			{
				if (register)
				{
					vRTK_DestinationMarker.DestinationMarkerEnter += DoDestinationMarkerEnter;
					vRTK_DestinationMarker.DestinationMarkerExit += DoDestinationMarkerExit;
					vRTK_DestinationMarker.DestinationMarkerSet += DoDestinationMarkerSet;
				}
				else
				{
					vRTK_DestinationMarker.DestinationMarkerEnter -= DoDestinationMarkerEnter;
					vRTK_DestinationMarker.DestinationMarkerExit -= DoDestinationMarkerExit;
					vRTK_DestinationMarker.DestinationMarkerSet -= DoDestinationMarkerSet;
				}
			}
		}
	}

	protected virtual void DoDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
	{
		if (!isActive && e.raycastHit.transform == transform)
		{
			isActive = true;
			ToggleCursor(sender, state: false);
			EnablePoint();
			OnDestinationMarkerEnter(SetDestinationMarkerEvent(0f, e.raycastHit.transform, e.raycastHit, e.raycastHit.transform.position, e.controllerReference, forceDestinationPosition: false, GetRotation()));
		}
	}

	protected virtual void DoDestinationMarkerExit(object sender, DestinationMarkerEventArgs e)
	{
		if (isActive && e.raycastHit.transform == transform)
		{
			isActive = false;
			ToggleCursor(sender, state: true);
			ResetPoint();
			OnDestinationMarkerExit(SetDestinationMarkerEvent(0f, e.raycastHit.transform, e.raycastHit, e.raycastHit.transform.position, e.controllerReference, forceDestinationPosition: false, GetRotation()));
		}
	}

	protected virtual void DoDestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
	{
		if (e.raycastHit.transform == transform)
		{
			currentDestinationPoint = this;
			if (snapToPoint)
			{
				e.raycastHit.point = destinationLocation.position;
				setDestination = StartCoroutine(DoDestinationMarkerSetAtEndOfFrame(e));
			}
		}
		else if (currentDestinationPoint != this)
		{
			ResetPoint();
		}
	}

	protected virtual IEnumerator DoDestinationMarkerSetAtEndOfFrame(DestinationMarkerEventArgs e)
	{
		yield return new WaitForEndOfFrame();
		if (enabled)
		{
			e.raycastHit.point = destinationLocation.position;
			DisablePoint();
			OnDestinationMarkerSet(SetDestinationMarkerEvent(e.distance, transform, e.raycastHit, destinationLocation.position, e.controllerReference, forceDestinationPosition: false, GetRotation()));
		}
	}

	protected virtual void ToggleCursor(object sender, bool state)
	{
		if (hidePointerCursorOnHover && sender.GetType().Equals(typeof(VRTK_Pointer)))
		{
			VRTK_Pointer vRTK_Pointer = (VRTK_Pointer)sender;
			if (!state)
			{
				storedCursorState = vRTK_Pointer.pointerRenderer.cursorVisibility;
				vRTK_Pointer.pointerRenderer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
			}
			else
			{
				vRTK_Pointer.pointerRenderer.cursorVisibility = storedCursorState;
			}
		}
	}

	protected virtual void EnablePoint()
	{
		ToggleObject(lockedCursorObject, state: false);
		ToggleObject(defaultCursorObject, state: false);
		ToggleObject(hoverCursorObject, state: true);
	}

	protected virtual void DisablePoint()
	{
		pointCollider.enabled = false;
		ToggleObject(lockedCursorObject, state: false);
		ToggleObject(defaultCursorObject, state: false);
		ToggleObject(hoverCursorObject, state: false);
	}

	protected virtual void ResetPoint()
	{
		currentDestinationPoint = null;
		ToggleObject(hoverCursorObject, state: false);
		if (enableTeleport)
		{
			pointCollider.enabled = true;
			ToggleObject(defaultCursorObject, state: true);
			ToggleObject(lockedCursorObject, state: false);
		}
		else
		{
			pointCollider.enabled = false;
			ToggleObject(lockedCursorObject, state: true);
			ToggleObject(defaultCursorObject, state: false);
		}
	}

	protected virtual void ToggleObject(GameObject givenObject, bool state)
	{
		if (givenObject != null)
		{
			givenObject.SetActive(state);
		}
	}

	protected virtual Quaternion? GetRotation()
	{
		if (snapToRotation == RotationTypes.NoRotation)
		{
			return null;
		}
		float num = ((snapToRotation != RotationTypes.RotateWithHeadsetOffset || !(playArea != null) || !(headset != null)) ? 0f : (playArea.eulerAngles.y - headset.eulerAngles.y));
		return Quaternion.Euler(0f, destinationLocation.localEulerAngles.y + num, 0f);
	}
}
