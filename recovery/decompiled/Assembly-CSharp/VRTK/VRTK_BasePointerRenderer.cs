using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace VRTK;

public abstract class VRTK_BasePointerRenderer : MonoBehaviour
{
	public enum VisibilityStates
	{
		OnWhenActive,
		AlwaysOn,
		AlwaysOff
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

	[Header("General Renderer Settings")]
	[Tooltip("An optional Play Area Cursor generator to add to the destination position of the pointer tip.")]
	public VRTK_PlayAreaCursor playareaCursor;

	[Tooltip("A custom raycaster to use for the pointer's raycasts to ignore.")]
	public VRTK_CustomRaycast customRaycast;

	[Tooltip("**OBSOLETE [Use customRaycast]** The layers for the pointer's raycasts to ignore.")]
	[Obsolete("`VRTK_BasePointerRenderer.layersToIgnore` is no longer used in the `VRTK_BasePointerRenderer` class, use the `customRaycast` parameter instead. This parameter will be removed in a future version of VRTK.")]
	public LayerMask layersToIgnore = 4;

	[Tooltip("Specifies the smoothing to be applied to the pointer origin when positioning the pointer tip.")]
	public PointerOriginSmoothingSettings pointerOriginSmoothingSettings = new PointerOriginSmoothingSettings();

	[Header("General Appearance Settings")]
	[Tooltip("The colour to change the pointer materials when the pointer collides with a valid object. Set to `Color.clear` to bypass changing material colour on valid collision.")]
	public Color validCollisionColor = Color.green;

	[Tooltip("The colour to change the pointer materials when the pointer is not colliding with anything or with an invalid object. Set to `Color.clear` to bypass changing material colour on invalid collision.")]
	public Color invalidCollisionColor = Color.red;

	[Tooltip("Determines when the main tracer of the pointer renderer will be visible.")]
	public VisibilityStates tracerVisibility;

	[Tooltip("Determines when the cursor/tip of the pointer renderer will be visible.")]
	public VisibilityStates cursorVisibility;

	protected const float BEAM_ADJUST_OFFSET = 0.0001f;

	protected VRTK_Pointer controllingPointer;

	protected RaycastHit destinationHit = default;

	protected Material defaultMaterial;

	protected Color currentColor;

	protected VRTK_PolicyList invalidListPolicy;

	protected float navMeshCheckDistance;

	protected bool headsetPositionCompensation;

	protected GameObject objectInteractor;

	protected GameObject objectInteractorAttachPoint;

	protected GameObject pointerOriginTransformFollowGameObject;

	protected VRTK_TransformFollow pointerOriginTransformFollow;

	protected VRTK_InteractGrab controllerGrabScript;

	protected Rigidbody savedAttachPoint;

	protected bool attachedToInteractorAttachPoint;

	protected float savedBeamLength;

	protected List<GameObject> makeRendererVisible;

	protected bool tracerVisible;

	protected bool cursorVisible;

	public abstract GameObject[] GetPointerObjects();

	public virtual void InitalizePointer(VRTK_Pointer givenPointer, VRTK_PolicyList givenInvalidListPolicy, float givenNavMeshCheckDistance, bool givenHeadsetPositionCompensation)
	{
		controllingPointer = givenPointer;
		invalidListPolicy = givenInvalidListPolicy;
		navMeshCheckDistance = givenNavMeshCheckDistance;
		headsetPositionCompensation = givenHeadsetPositionCompensation;
		if (controllingPointer != null && controllingPointer.interactWithObjects && (bool)controllingPointer.controller && !objectInteractor)
		{
			controllerGrabScript = controllingPointer.controller.GetComponent<VRTK_InteractGrab>();
			CreateObjectInteractor();
		}
	}

	public virtual void ResetPointerObjects()
	{
		DestroyPointerObjects();
		CreatePointerObjects();
	}

	public virtual void Toggle(bool pointerState, bool actualState)
	{
		if (pointerState)
		{
			destinationHit = default;
		}
		else if (controllingPointer != null)
		{
			controllingPointer.ResetActivationTimer();
			PointerExit(destinationHit);
		}
		ToggleInteraction(pointerState);
		ToggleRenderer(pointerState, actualState);
	}

	public virtual void ToggleInteraction(bool state)
	{
		ToggleObjectInteraction(state);
	}

	public virtual void UpdateRenderer()
	{
		if (playareaCursor != null)
		{
			playareaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
			playareaCursor.ToggleState(IsCursorVisible());
		}
	}

	public virtual RaycastHit GetDestinationHit()
	{
		return destinationHit;
	}

	public virtual bool ValidPlayArea()
	{
		return playareaCursor == null || !playareaCursor.IsActive() || !playareaCursor.HasCollided();
	}

	public virtual bool IsVisible()
	{
		return IsTracerVisible() || IsCursorVisible();
	}

	public virtual bool IsTracerVisible()
	{
		return tracerVisibility == VisibilityStates.AlwaysOn || tracerVisible;
	}

	public virtual bool IsCursorVisible()
	{
		return cursorVisibility == VisibilityStates.AlwaysOn || cursorVisible;
	}

	public virtual bool IsValidCollision()
	{
		return currentColor != invalidCollisionColor;
	}

	public virtual GameObject GetObjectInteractor()
	{
		return objectInteractor;
	}

	protected abstract void CreatePointerObjects();

	protected abstract void DestroyPointerObjects();

	protected abstract void ToggleRenderer(bool pointerState, bool actualState);

	protected virtual void OnEnable()
	{
		defaultMaterial = Resources.Load("WorldPointer") as Material;
		makeRendererVisible = new List<GameObject>();
		CreatePointerOriginTransformFollow();
		CreatePointerObjects();
	}

	protected virtual void OnDisable()
	{
		DestroyPointerObjects();
		if (objectInteractor != null)
		{
			UnityEngine.Object.Destroy(objectInteractor);
		}
		controllerGrabScript = null;
		UnityEngine.Object.Destroy(pointerOriginTransformFollowGameObject);
	}

	protected virtual void OnValidate()
	{
		pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference);
		pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference);
	}

	protected virtual void FixedUpdate()
	{
		if (controllingPointer != null && controllingPointer.interactWithObjects && objectInteractor != null && objectInteractor.activeInHierarchy)
		{
			UpdateObjectInteractor();
		}
		UpdatePointerOriginTransformFollow();
	}

	protected virtual void ToggleObjectInteraction(bool state)
	{
		if (!(controllingPointer != null) || !controllingPointer.interactWithObjects)
		{
			return;
		}
		if (state && controllingPointer.grabToPointerTip && controllerGrabScript != null && objectInteractorAttachPoint != null)
		{
			savedAttachPoint = controllerGrabScript.controllerAttachPoint;
			controllerGrabScript.controllerAttachPoint = objectInteractorAttachPoint.GetComponent<Rigidbody>();
			attachedToInteractorAttachPoint = true;
		}
		if (!state && controllingPointer.grabToPointerTip && controllerGrabScript != null)
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
		if (objectInteractor != null)
		{
			objectInteractor.SetActive(state);
		}
	}

	protected virtual void UpdateObjectInteractor()
	{
		objectInteractor.transform.position = destinationHit.point;
	}

	protected virtual void UpdatePointerOriginTransformFollow()
	{
		pointerOriginTransformFollow.gameObject.SetActive(controllingPointer != null);
		if (controllingPointer != null)
		{
			pointerOriginTransformFollow.gameObjectToFollow = ((!(controllingPointer.customOrigin == null)) ? controllingPointer.customOrigin : transform).gameObject;
			pointerOriginTransformFollow.enabled = controllingPointer != null;
			pointerOriginTransformFollowGameObject.SetActive(controllingPointer != null);
			pointerOriginTransformFollow.smoothsPosition = pointerOriginSmoothingSettings.smoothsPosition;
			pointerOriginTransformFollow.maxAllowedPerFrameDistanceDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference;
			pointerOriginTransformFollow.smoothsRotation = pointerOriginSmoothingSettings.smoothsRotation;
			pointerOriginTransformFollow.maxAllowedPerFrameAngleDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference;
		}
	}

	protected Transform GetOrigin(bool smoothed = true)
	{
		Transform result;
		if (smoothed)
		{
			result = pointerOriginTransformFollow.gameObjectToChange.transform;
		}
		else
		{
			result = ((!(controllingPointer.customOrigin == null)) ? controllingPointer.customOrigin : transform);
		}
		return result;
	}

	protected virtual void PointerEnter(RaycastHit givenHit)
	{
		controllingPointer.PointerEnter(givenHit);
	}

	protected virtual void PointerExit(RaycastHit givenHit)
	{
		controllingPointer.PointerExit(givenHit);
	}

	protected virtual bool ValidDestination()
	{
		bool flag = false;
		if (destinationHit.transform != null)
		{
			flag = NavMesh.SamplePosition(destinationHit.point, out var _, navMeshCheckDistance, -1);
		}
		if (navMeshCheckDistance == 0f)
		{
			flag = true;
		}
		return flag && (bool)destinationHit.transform && !VRTK_PolicyList.Check(destinationHit.transform.gameObject, invalidListPolicy);
	}

	protected virtual void ToggleElement(GameObject givenObject, bool pointerState, bool actualState, VisibilityStates givenVisibility, ref bool currentVisible)
	{
		if (givenObject != null)
		{
			currentVisible = givenVisibility == VisibilityStates.AlwaysOn || pointerState;
			givenObject.SetActive(currentVisible);
			if (givenVisibility == VisibilityStates.AlwaysOff)
			{
				currentVisible = false;
				ToggleRendererVisibility(givenObject, state: false);
			}
			else if (actualState && givenVisibility != VisibilityStates.AlwaysOn)
			{
				ToggleRendererVisibility(givenObject, state: false);
				AddVisibleRenderer(givenObject);
			}
			else
			{
				ToggleRendererVisibility(givenObject, state: true);
			}
		}
	}

	protected virtual void AddVisibleRenderer(GameObject givenObject)
	{
		if (!makeRendererVisible.Contains(givenObject))
		{
			makeRendererVisible.Add(givenObject);
		}
	}

	protected virtual void MakeRenderersVisible()
	{
		for (int i = 0; i < makeRendererVisible.Count; i++)
		{
			ToggleRendererVisibility(makeRendererVisible[i], state: true);
			makeRendererVisible.Remove(makeRendererVisible[i]);
		}
	}

	protected virtual void ToggleRendererVisibility(GameObject givenObject, bool state)
	{
		if (givenObject != null)
		{
			Renderer[] componentsInChildren = givenObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = state;
			}
		}
	}

	protected virtual void SetupMaterialRenderer(GameObject givenObject)
	{
		if (givenObject != null)
		{
			MeshRenderer component = givenObject.GetComponent<MeshRenderer>();
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.material = defaultMaterial;
		}
	}

	protected virtual void ChangeColor(Color givenColor)
	{
		if ((playareaCursor != null && playareaCursor.IsActive() && playareaCursor.HasCollided()) || !ValidDestination() || (controllingPointer != null && !controllingPointer.CanSelect()))
		{
			givenColor = invalidCollisionColor;
		}
		if (givenColor != Color.clear)
		{
			currentColor = givenColor;
			ChangeMaterial(givenColor);
		}
	}

	protected virtual void ChangeMaterial(Color givenColor)
	{
		if (playareaCursor != null)
		{
			playareaCursor.SetMaterialColor(givenColor, IsValidCollision());
		}
	}

	protected virtual void ChangeMaterialColor(GameObject givenObject, Color givenColor)
	{
		if (!(givenObject != null))
		{
			return;
		}
		Renderer[] componentsInChildren = givenObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.material != null)
			{
				renderer.material.EnableKeyword("_EMISSION");
				if (renderer.material.HasProperty("_Color"))
				{
					renderer.material.color = givenColor;
				}
				if (renderer.material.HasProperty("_EmissionColor"))
				{
					renderer.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(givenColor, 50f));
				}
			}
		}
	}

	protected virtual void CreateObjectInteractor()
	{
		objectInteractor = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BasePointerRenderer_ObjectInteractor_Container"));
		objectInteractor.transform.SetParent(controllingPointer.controller.transform);
		objectInteractor.transform.localPosition = Vector3.zero;
		objectInteractor.layer = LayerMask.NameToLayer("Ignore Raycast");
		VRTK_PlayerObject.SetPlayerObject(objectInteractor, VRTK_PlayerObject.ObjectTypes.Pointer);
		GameObject gameObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BasePointerRenderer_ObjectInteractor_Collider"));
		gameObject.transform.SetParent(objectInteractor.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
		if (controllingPointer.grabToPointerTip)
		{
			objectInteractorAttachPoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BasePointerRenderer_ObjectInteractor_AttachPoint"));
			objectInteractorAttachPoint.transform.SetParent(objectInteractor.transform);
			objectInteractorAttachPoint.transform.localPosition = Vector3.zero;
			objectInteractorAttachPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
			Rigidbody rigidbody = objectInteractorAttachPoint.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.freezeRotation = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			VRTK_PlayerObject.SetPlayerObject(objectInteractorAttachPoint, VRTK_PlayerObject.ObjectTypes.Pointer);
		}
		ScaleObjectInteractor(Vector3.one * 0.025f);
		objectInteractor.SetActive(value: false);
	}

	protected virtual void ScaleObjectInteractor(Vector3 scaleAmount)
	{
		if (objectInteractor != null)
		{
			objectInteractor.transform.localScale = scaleAmount;
		}
	}

	protected virtual void CreatePointerOriginTransformFollow()
	{
		pointerOriginTransformFollowGameObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "BasePointerRenderer_Origin_Smoothed"));
		pointerOriginTransformFollow = pointerOriginTransformFollowGameObject.AddComponent<VRTK_TransformFollow>();
		pointerOriginTransformFollow.enabled = false;
		pointerOriginTransformFollow.followsScale = false;
	}

	protected virtual float OverrideBeamLength(float currentLength)
	{
		if (controllerGrabScript == null || !controllerGrabScript.GetGrabbedObject())
		{
			savedBeamLength = 0f;
		}
		if (controllingPointer != null && controllingPointer.interactWithObjects && controllingPointer.grabToPointerTip && attachedToInteractorAttachPoint && controllerGrabScript != null && (bool)controllerGrabScript.GetGrabbedObject())
		{
			savedBeamLength = ((savedBeamLength != 0f) ? savedBeamLength : currentLength);
			return savedBeamLength;
		}
		return currentLength;
	}

	protected virtual void UpdateDependencies(Vector3 location)
	{
		if (playareaCursor != null)
		{
			playareaCursor.SetPlayAreaCursorTransform(location);
		}
	}
}
