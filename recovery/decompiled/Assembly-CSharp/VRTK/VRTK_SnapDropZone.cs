using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Highlighters;

namespace VRTK;

[ExecuteInEditMode]
public class VRTK_SnapDropZone : MonoBehaviour
{
	public enum SnapTypes
	{
		UseKinematic,
		UseJoint,
		UseParenting
	}

	[Tooltip("A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.")]
	public GameObject highlightObjectPrefab;

	[Tooltip("The Snap Type to apply when a valid interactable object is dropped within the snap zone.")]
	public SnapTypes snapType;

	[Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.")]
	public float snapDuration;

	[Tooltip("If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.")]
	public bool applyScalingOnSnap;

	[Tooltip("The colour to use when showing the snap zone is active.")]
	public Color highlightColor;

	[Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
	public bool highlightAlwaysActive;

	[Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
	public VRTK_PolicyList validObjectListPolicy;

	[Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
	public bool displayDropZoneInEditor = true;

	[Tooltip("The game object to snap into the dropzone when the drop zone is enabled. The game object must be valid in any given policy list to snap.")]
	public GameObject defaultSnappedObject;

	protected GameObject previousPrefab;

	protected GameObject highlightContainer;

	protected GameObject highlightObject;

	protected GameObject highlightEditorObject;

	protected List<GameObject> currentValidSnapObjects = new List<GameObject>();

	protected GameObject currentSnappedObject;

	protected VRTK_BaseHighlighter objectHighlighter;

	protected bool willSnap;

	protected bool isSnapped;

	protected bool wasSnapped;

	protected bool isHighlighted;

	protected Coroutine transitionInPlace;

	protected bool originalJointCollisionState;

	protected const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";

	protected const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";

	protected const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

	public event SnapDropZoneEventHandler ObjectEnteredSnapDropZone;

	public event SnapDropZoneEventHandler ObjectExitedSnapDropZone;

	public event SnapDropZoneEventHandler ObjectSnappedToDropZone;

	public event SnapDropZoneEventHandler ObjectUnsnappedFromDropZone;

	public virtual void OnObjectEnteredSnapDropZone(SnapDropZoneEventArgs e)
	{
		if (ObjectEnteredSnapDropZone != null)
		{
			ObjectEnteredSnapDropZone(this, e);
		}
	}

	public virtual void OnObjectExitedSnapDropZone(SnapDropZoneEventArgs e)
	{
		if (ObjectExitedSnapDropZone != null)
		{
			ObjectExitedSnapDropZone(this, e);
		}
	}

	public virtual void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
	{
		if (ObjectSnappedToDropZone != null)
		{
			ObjectSnappedToDropZone(this, e);
		}
	}

	public virtual void OnObjectUnsnappedFromDropZone(SnapDropZoneEventArgs e)
	{
		UnsnapObject();
		if (ObjectUnsnappedFromDropZone != null)
		{
			ObjectUnsnappedFromDropZone(this, e);
		}
	}

	public virtual SnapDropZoneEventArgs SetSnapDropZoneEvent(GameObject interactableObject)
	{
		SnapDropZoneEventArgs result = default;
		result.snappedObject = interactableObject;
		return result;
	}

	public virtual void InitaliseHighlightObject(bool removeOldObject = false)
	{
		if (removeOldObject)
		{
			DeleteHighlightObject();
		}
		ChooseDestroyType(transform.Find(ObjectPath("EditorHighlightObject")));
		highlightEditorObject = null;
		GenerateObjects();
	}

	public virtual void ForceSnap(GameObject objectToSnap)
	{
		VRTK_InteractableObject componentInParent = objectToSnap.GetComponentInParent<VRTK_InteractableObject>();
		if (componentInParent != null)
		{
			componentInParent.SaveCurrentState();
			StopCoroutine("AttemptForceSnapAtEndOfFrame");
			if (componentInParent.IsGrabbed())
			{
				componentInParent.ForceStopInteracting();
				StartCoroutine(AttemptForceSnapAtEndOfFrame(objectToSnap));
			}
			else
			{
				AttemptForceSnap(objectToSnap);
			}
		}
	}

	public virtual void ForceUnsnap()
	{
		if (isSnapped && currentSnappedObject != null)
		{
			VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(currentSnappedObject, grabState: false);
			if (vRTK_InteractableObject != null)
			{
				vRTK_InteractableObject.ToggleSnapDropZone(this, state: false);
			}
		}
	}

	public virtual bool ValidSnappableObjectIsHovering()
	{
		return currentValidSnapObjects.Count > 0;
	}

	public virtual bool IsObjectHovering(GameObject checkObject)
	{
		return currentValidSnapObjects.Contains(checkObject);
	}

	public virtual List<GameObject> GetHoveringObjects()
	{
		return currentValidSnapObjects;
	}

	public virtual GameObject GetCurrentSnappedObject()
	{
		return currentSnappedObject;
	}

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			InitaliseHighlightObject();
		}
	}

	protected virtual void OnApplicationQuit()
	{
		if (objectHighlighter != null)
		{
			objectHighlighter.Unhighlight();
		}
	}

	protected virtual void OnEnable()
	{
		if (!VRTK_SharedMethods.IsEditTime() && Application.isPlaying && defaultSnappedObject != null)
		{
			ForceSnap(defaultSnappedObject);
		}
	}

	protected virtual void Update()
	{
		CheckSnappedItemExists();
		CheckPrefabUpdate();
		CreateHighlightersInEditor();
		CheckCurrentValidSnapObjectStillValid();
		previousPrefab = highlightObjectPrefab;
		SetObjectHighlight();
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (!isSnapped)
		{
			ToggleHighlight(collider, state: true);
		}
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (IsObjectHovering(collider.gameObject))
		{
			ToggleHighlight(collider, state: false);
		}
		if (currentSnappedObject == collider.gameObject)
		{
			ForceUnsnap();
		}
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
		if (!isSnapped && (bool)ValidSnapObject(collider.gameObject, grabState: true))
		{
			AddCurrentValidSnapObject(collider.gameObject);
		}
		if (IsObjectHovering(collider.gameObject))
		{
			if (!isSnapped)
			{
				ToggleHighlight(collider, state: true);
			}
			SnapObject(collider);
		}
	}

	protected virtual VRTK_InteractableObject ValidSnapObject(GameObject checkObject, bool grabState, bool checkGrabState = true)
	{
		VRTK_InteractableObject componentInParent = checkObject.GetComponentInParent<VRTK_InteractableObject>();
		return (!(componentInParent != null) || (checkGrabState && componentInParent.IsGrabbed() != grabState) || VRTK_PolicyList.Check(componentInParent.gameObject, validObjectListPolicy)) ? null : componentInParent;
	}

	protected virtual string ObjectPath(string name)
	{
		return "HighlightContainer/" + name;
	}

	protected virtual void CheckSnappedItemExists()
	{
		if (isSnapped && currentSnappedObject == null)
		{
			OnObjectUnsnappedFromDropZone(SetSnapDropZoneEvent(currentSnappedObject));
		}
	}

	protected virtual void CheckPrefabUpdate()
	{
		if (previousPrefab != null && previousPrefab != highlightObjectPrefab)
		{
			DeleteHighlightObject();
		}
	}

	protected virtual void SetObjectHighlight()
	{
		if (highlightAlwaysActive && !isSnapped && !isHighlighted)
		{
			highlightObject.SetActive(value: true);
		}
	}

	protected virtual void CreateHighlightersInEditor()
	{
		if (VRTK_SharedMethods.IsEditTime())
		{
			GenerateHighlightObject();
			if (snapType == SnapTypes.UseJoint && GetComponent<Joint>() == null)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
			}
			GenerateEditorHighlightObject();
			ForceSetObjects();
			if (highlightEditorObject != null)
			{
				highlightEditorObject.SetActive(displayDropZoneInEditor);
			}
		}
	}

	protected virtual void CheckCurrentValidSnapObjectStillValid()
	{
		for (int i = 0; i < currentValidSnapObjects.Count; i++)
		{
			VRTK_InteractableObject componentInParent = currentValidSnapObjects[i].GetComponentInParent<VRTK_InteractableObject>();
			if (componentInParent != null && componentInParent.GetStoredSnapDropZone() != null && componentInParent.GetStoredSnapDropZone() != gameObject)
			{
				RemoveCurrentValidSnapObject(currentValidSnapObjects[i]);
				if (isHighlighted && highlightObject != null && !highlightAlwaysActive)
				{
					highlightObject.SetActive(value: false);
				}
			}
		}
	}

	protected virtual void ForceSetObjects()
	{
		if (highlightEditorObject == null)
		{
			Transform transform = base.transform.Find(ObjectPath("EditorHighlightObject"));
			highlightEditorObject = ((!transform) ? null : transform.gameObject);
		}
		if (highlightObject == null)
		{
			Transform transform2 = base.transform.Find(ObjectPath("HighlightObject"));
			highlightObject = ((!transform2) ? null : transform2.gameObject);
		}
		if (highlightContainer == null)
		{
			Transform transform3 = base.transform.Find("HighlightContainer");
			highlightContainer = ((!transform3) ? null : transform3.gameObject);
		}
	}

	protected virtual void GenerateContainer()
	{
		if (highlightContainer == null || transform.Find("HighlightContainer") == null)
		{
			highlightContainer = new GameObject("HighlightContainer");
			highlightContainer.transform.SetParent(transform);
			highlightContainer.transform.localPosition = Vector3.zero;
			highlightContainer.transform.localRotation = Quaternion.identity;
			highlightContainer.transform.localScale = Vector3.one;
		}
	}

	protected virtual void SetContainer()
	{
		Transform transform = base.transform.Find("HighlightContainer");
		if (transform != null)
		{
			highlightContainer = transform.gameObject;
		}
	}

	protected virtual void GenerateObjects()
	{
		GenerateHighlightObject();
		if (highlightObject != null && objectHighlighter == null)
		{
			InitialiseHighlighter();
		}
	}

	protected virtual void SnapObject(Collider collider)
	{
		VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(collider.gameObject, grabState: false);
		if (willSnap && !isSnapped && vRTK_InteractableObject != null && !vRTK_InteractableObject.IsInSnapDropZone())
		{
			if (highlightObject != null)
			{
				highlightObject.SetActive(value: false);
			}
			Vector3 newLocalScale = GetNewLocalScale(vRTK_InteractableObject);
			if (transitionInPlace != null)
			{
				StopCoroutine(transitionInPlace);
			}
			isSnapped = true;
			currentSnappedObject = vRTK_InteractableObject.gameObject;
			transitionInPlace = StartCoroutine(UpdateTransformDimensions(vRTK_InteractableObject, highlightContainer, newLocalScale, snapDuration));
			vRTK_InteractableObject.ToggleSnapDropZone(this, state: true);
		}
		isSnapped = (!isSnapped || !vRTK_InteractableObject || !vRTK_InteractableObject.IsGrabbed()) && isSnapped;
		wasSnapped = false;
	}

	protected virtual void UnsnapObject()
	{
		isSnapped = false;
		wasSnapped = true;
		currentSnappedObject = null;
		ResetSnapDropZoneJoint();
		if (transitionInPlace != null)
		{
			StopCoroutine(transitionInPlace);
		}
	}

	protected virtual Vector3 GetNewLocalScale(VRTK_InteractableObject ioCheck)
	{
		Vector3 result = ioCheck.transform.localScale;
		if (applyScalingOnSnap)
		{
			ioCheck.StoreLocalScale();
			result = Vector3.Scale(ioCheck.transform.localScale, transform.localScale);
		}
		return result;
	}

	protected virtual IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, GameObject endSettings, Vector3 endScale, float duration)
	{
		float elapsedTime = 0f;
		Transform ioTransform = ioCheck.transform;
		Vector3 startPosition = ioTransform.position;
		Quaternion startRotation = ioTransform.rotation;
		Vector3 startScale = ioTransform.localScale;
		bool storedKinematicState = ioCheck.isKinematic;
		ioCheck.isKinematic = true;
		while (elapsedTime <= duration)
		{
			elapsedTime += Time.deltaTime;
			if (ioTransform != null && endSettings != null)
			{
				ioTransform.position = Vector3.Lerp(startPosition, endSettings.transform.position, elapsedTime / duration);
				ioTransform.rotation = Quaternion.Lerp(startRotation, endSettings.transform.rotation, elapsedTime / duration);
				ioTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
			}
			yield return null;
		}
		ioTransform.position = endSettings.transform.position;
		ioTransform.rotation = endSettings.transform.rotation;
		ioTransform.localScale = endScale;
		ioCheck.isKinematic = storedKinematicState;
		SetDropSnapType(ioCheck);
	}

	protected virtual void SetDropSnapType(VRTK_InteractableObject ioCheck)
	{
		switch (snapType)
		{
		case SnapTypes.UseKinematic:
			ioCheck.SaveCurrentState();
			ioCheck.isKinematic = true;
			break;
		case SnapTypes.UseParenting:
			ioCheck.SaveCurrentState();
			ioCheck.isKinematic = true;
			ioCheck.transform.SetParent(transform);
			break;
		case SnapTypes.UseJoint:
			SetSnapDropZoneJoint(ioCheck.GetComponent<Rigidbody>());
			break;
		}
		OnObjectSnappedToDropZone(SetSnapDropZoneEvent(ioCheck.gameObject));
	}

	protected virtual void SetSnapDropZoneJoint(Rigidbody snapTo)
	{
		Joint component = GetComponent<Joint>();
		if (component == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
		}
		else if (snapTo == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_SnapDropZone", "Rigidbody", "the `VRTK_InteractableObject`"));
		}
		else
		{
			component.connectedBody = snapTo;
			originalJointCollisionState = component.enableCollision;
			component.enableCollision = true;
		}
	}

	protected virtual void ResetSnapDropZoneJoint()
	{
		Joint component = GetComponent<Joint>();
		if (component != null)
		{
			component.enableCollision = originalJointCollisionState;
		}
	}

	protected virtual void AddCurrentValidSnapObject(GameObject givenObject)
	{
		if (!currentValidSnapObjects.Contains(givenObject))
		{
			currentValidSnapObjects.Add(givenObject);
		}
	}

	protected virtual void RemoveCurrentValidSnapObject(GameObject givenObject)
	{
		if (currentValidSnapObjects.Contains(givenObject))
		{
			currentValidSnapObjects.Remove(givenObject);
		}
	}

	protected virtual void AttemptForceSnap(GameObject objectToSnap)
	{
		willSnap = true;
		AddCurrentValidSnapObject(objectToSnap);
		OnTriggerStay(objectToSnap.GetComponentInChildren<Collider>());
	}

	protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(GameObject objectToSnap)
	{
		yield return new WaitForEndOfFrame();
		AttemptForceSnap(objectToSnap);
	}

	protected virtual void ToggleHighlight(Collider collider, bool state)
	{
		VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(collider.gameObject, grabState: true, state);
		if (!(highlightObject != null) || !(vRTK_InteractableObject != null))
		{
			return;
		}
		highlightObject.SetActive(state);
		vRTK_InteractableObject.SetSnapDropZoneHover(this, state);
		willSnap = state;
		isHighlighted = state;
		if (state)
		{
			if (!IsObjectHovering(collider.gameObject) || wasSnapped)
			{
				OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
			}
			AddCurrentValidSnapObject(collider.gameObject);
		}
		else
		{
			OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
			RemoveCurrentValidSnapObject(collider.gameObject);
		}
	}

	protected virtual void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string givenName)
	{
		GenerateContainer();
		Vector3 localScale = transform.localScale;
		transform.localScale = Vector3.one;
		clonedObject = Object.Instantiate(objectBlueprint, highlightContainer.transform);
		clonedObject.name = givenName;
		clonedObject.transform.localPosition = Vector3.zero;
		clonedObject.transform.localRotation = Quaternion.identity;
		transform.localScale = localScale;
		CleanHighlightObject(clonedObject);
	}

	protected virtual void GenerateHighlightObject()
	{
		if (highlightObjectPrefab != null && highlightObject == null && base.transform.Find(ObjectPath("HighlightObject")) == null)
		{
			CopyObject(highlightObjectPrefab, ref highlightObject, "HighlightObject");
		}
		Transform transform = base.transform.Find(ObjectPath("HighlightObject"));
		if (transform != null && highlightObject == null)
		{
			highlightObject = transform.gameObject;
		}
		if (highlightObjectPrefab == null && highlightObject != null)
		{
			DeleteHighlightObject();
		}
		if (highlightObject != null)
		{
			highlightObject.SetActive(value: false);
		}
		SetContainer();
	}

	protected virtual void DeleteHighlightObject()
	{
		ChooseDestroyType(transform.Find("HighlightContainer"));
		highlightContainer = null;
		highlightObject = null;
		objectHighlighter = null;
	}

	protected virtual void GenerateEditorHighlightObject()
	{
		if (highlightObject != null && highlightEditorObject == null && transform.Find(ObjectPath("EditorHighlightObject")) == null)
		{
			CopyObject(highlightObject, ref highlightEditorObject, "EditorHighlightObject");
			Renderer[] componentsInChildren = highlightEditorObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.material = Resources.Load("SnapDropZoneEditorObject") as Material;
			}
			highlightEditorObject.SetActive(value: true);
		}
	}

	protected virtual void CleanHighlightObject(GameObject objectToClean)
	{
		VRTK_SnapDropZone[] componentsInChildren = objectToClean.GetComponentsInChildren<VRTK_SnapDropZone>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ChooseDestroyType(componentsInChildren[i].gameObject);
		}
		string[] array = new string[5] { "Transform", "MeshFilter", "MeshRenderer", "SkinnedMeshRenderer", "VRTK_GameObjectLinker" };
		Component[] componentsInChildren2 = objectToClean.GetComponentsInChildren<Component>(includeInactive: true);
		foreach (Component component in componentsInChildren2)
		{
			bool flag = false;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (component.GetType().ToString().Contains("." + text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ChooseDestroyType(component);
			}
		}
	}

	protected virtual void InitialiseHighlighter()
	{
		VRTK_BaseHighlighter activeHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
		if (activeHighlighter == null)
		{
			highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
		}
		else
		{
			VRTK_SharedMethods.CloneComponent(activeHighlighter, highlightObject);
		}
		objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
		objectHighlighter.unhighlightOnDisable = false;
		objectHighlighter.Initialise(highlightColor);
		objectHighlighter.Highlight(highlightColor);
		if (!objectHighlighter.UsesClonedObject())
		{
			return;
		}
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!VRTK_PlayerObject.IsPlayerObject(renderer.gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
			{
				renderer.enabled = false;
			}
		}
	}

	protected virtual void ChooseDestroyType(Transform deleteTransform)
	{
		if (deleteTransform != null)
		{
			ChooseDestroyType(deleteTransform.gameObject);
		}
	}

	protected virtual void ChooseDestroyType(GameObject deleteObject)
	{
		if (VRTK_SharedMethods.IsEditTime())
		{
			if (deleteObject != null)
			{
				Object.DestroyImmediate(deleteObject);
			}
		}
		else if (deleteObject != null)
		{
			Object.Destroy(deleteObject);
		}
	}

	protected virtual void ChooseDestroyType(Component deleteComponent)
	{
		if (VRTK_SharedMethods.IsEditTime())
		{
			if (deleteComponent != null)
			{
				Object.DestroyImmediate(deleteComponent);
			}
		}
		else if (deleteComponent != null)
		{
			Object.Destroy(deleteComponent);
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if (highlightObject != null && !displayDropZoneInEditor)
		{
			Vector3 size = VRTK_SharedMethods.GetBounds(highlightObject.transform).size * 1.05f;
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(highlightObject.transform.position, size);
		}
	}
}
