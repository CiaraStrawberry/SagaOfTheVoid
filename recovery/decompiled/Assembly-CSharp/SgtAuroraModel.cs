using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtAuroraModel : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Vector3 LocalPosition;
	}

	[Tooltip("The aurora this belongs to")]
	public SgtAurora Aurora;

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

	public static SgtAuroraModel Create(SgtAurora aurora)
	{
		SgtAuroraModel sgtAuroraModel = SgtComponentPool<SgtAuroraModel>.Pop(aurora.transform, "Plane", aurora.gameObject.layer);
		sgtAuroraModel.Aurora = aurora;
		return sgtAuroraModel;
	}

	public static void Pool(SgtAuroraModel model)
	{
		if (model != null)
		{
			model.Aurora = null;
			SgtComponentPool<SgtAuroraModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtAuroraModel model)
	{
		if (model != null)
		{
			model.Aurora = null;
			model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Aurora == null)
		{
			Pool(this);
		}
	}
}
