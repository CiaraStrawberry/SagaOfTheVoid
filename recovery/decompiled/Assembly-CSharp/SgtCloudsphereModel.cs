using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtCloudsphereModel : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Vector3 LocalPosition;
	}

	[Tooltip("The cloudsphere this belongs to")]
	public SgtCloudsphere Cloudsphere;

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

	public void SetScale(float scale)
	{
		SgtHelper.SetLocalScale(transform, scale);
	}

	public static SgtCloudsphereModel Create(SgtCloudsphere cloudsphere)
	{
		SgtCloudsphereModel sgtCloudsphereModel = SgtComponentPool<SgtCloudsphereModel>.Pop(cloudsphere.transform, "Model", cloudsphere.gameObject.layer);
		sgtCloudsphereModel.Cloudsphere = cloudsphere;
		return sgtCloudsphereModel;
	}

	public static void Pool(SgtCloudsphereModel model)
	{
		if (model != null)
		{
			model.Cloudsphere = null;
			SgtComponentPool<SgtCloudsphereModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtCloudsphereModel model)
	{
		if (model != null)
		{
			model.Cloudsphere = null;
			model.gameObject.SetActive(value: true);
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

	protected virtual void Update()
	{
		if (Cloudsphere == null)
		{
			Pool(this);
		}
	}
}
