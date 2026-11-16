using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtQuadsModel : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Vector3 LocalPosition;
	}

	[Tooltip("The quads instance this belongs to")]
	public SgtQuads Quads;

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

	public Mesh Mesh => mesh;

	public void PoolMeshNow()
	{
		if (mesh != null)
		{
			if (meshFilter == null)
			{
				meshFilter = gameObject.GetComponent<MeshFilter>();
			}
			mesh.Clear(keepVertexLayout: false);
			Mesh sharedMesh = SgtObjectPool<Mesh>.Add(mesh);
			meshFilter.sharedMesh = sharedMesh;
			mesh = sharedMesh;
		}
	}

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

	public static SgtQuadsModel Create(SgtQuads quads)
	{
		SgtQuadsModel sgtQuadsModel = SgtComponentPool<SgtQuadsModel>.Pop(quads.transform, "Quads Model", quads.gameObject.layer);
		sgtQuadsModel.Quads = quads;
		return sgtQuadsModel;
	}

	public static void Pool(SgtQuadsModel model)
	{
		if (model != null)
		{
			model.Quads = null;
			model.PoolMeshNow();
			SgtComponentPool<SgtQuadsModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtQuadsModel model)
	{
		if (model != null)
		{
			model.Quads = null;
			model.PoolMeshNow();
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

	protected virtual void OnDestroy()
	{
		PoolMeshNow();
	}

	protected virtual void Update()
	{
		if (Quads == null)
		{
			Pool(this);
		}
	}
}
