using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtProminencePlane : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Vector3 LocalPosition;
	}

	[Tooltip("The prominence this belongs to")]
	public SgtProminence Prominence;

	[NonSerialized]
	private MeshFilter meshFilter;

	[NonSerialized]
	private MeshRenderer meshRenderer;

	[NonSerialized]
	private Mesh mesh;

	[NonSerialized]
	private Material material;

	[NonSerialized]
	private List<CameraState> cameraStates;

	public void SetMesh(Mesh newMesh)
	{
		if (newMesh != mesh)
		{
			if (meshFilter == null)
			{
				meshFilter = gameObject.GetComponent<MeshFilter>();
			}
			meshFilter.sharedMesh = newMesh;
			mesh = newMesh;
		}
	}

	public void SetMaterial(Material newMaterial)
	{
		if (newMaterial != material)
		{
			if (meshRenderer == null)
			{
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
			}
			meshRenderer.sharedMaterial = newMaterial;
			material = newMaterial;
		}
	}

	public void SetRotation(Quaternion rotation)
	{
		SgtHelper.SetLocalRotation(transform, rotation);
	}

	public void Save(Camera camera)
	{
		CameraState cameraState = SgtCameraState.Save(ref cameraStates, camera);
		cameraState.LocalPosition = transform.localPosition;
	}

	public void Restore(Camera camera)
	{
		CameraState cameraState = SgtCameraState.Restore(cameraStates, camera);
		if (cameraState != null)
		{
			transform.localPosition = cameraState.LocalPosition;
		}
	}

	public void Revert()
	{
		transform.localPosition = Vector3.zero;
	}

	public static SgtProminencePlane Create(SgtProminence prominence)
	{
		SgtProminencePlane sgtProminencePlane = SgtComponentPool<SgtProminencePlane>.Pop(prominence.transform, "Plane", prominence.gameObject.layer);
		sgtProminencePlane.Prominence = prominence;
		return sgtProminencePlane;
	}

	public static void Pool(SgtProminencePlane plane)
	{
		if (plane != null)
		{
			plane.Prominence = null;
			SgtComponentPool<SgtProminencePlane>.Add(plane);
		}
	}

	public static void MarkForDestruction(SgtProminencePlane plane)
	{
		if (plane != null)
		{
			plane.Prominence = null;
			plane.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Prominence == null)
		{
			Pool(this);
		}
	}
}
