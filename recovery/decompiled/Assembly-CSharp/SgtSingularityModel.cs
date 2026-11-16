using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtSingularityModel : MonoBehaviour
{
	[Tooltip("The singularity this belongs to")]
	public SgtSingularity Singularity;

	[NonSerialized]
	private MeshFilter meshFilter;

	[NonSerialized]
	private MeshRenderer meshRenderer;

	public void SetMesh(Mesh mesh)
	{
		if (meshFilter == null)
		{
			meshFilter = gameObject.GetComponent<MeshFilter>();
		}
		if (meshFilter.sharedMesh != mesh)
		{
			meshFilter.sharedMesh = mesh;
		}
	}

	public void SetMaterial(Material material)
	{
		if (meshRenderer == null)
		{
			meshRenderer = gameObject.GetComponent<MeshRenderer>();
		}
		if (meshRenderer.sharedMaterial != material)
		{
			meshRenderer.sharedMaterial = material;
		}
	}

	public static SgtSingularityModel Create(SgtSingularity singularity)
	{
		SgtSingularityModel sgtSingularityModel = SgtComponentPool<SgtSingularityModel>.Pop(singularity.transform, "Model", singularity.gameObject.layer);
		sgtSingularityModel.Singularity = singularity;
		return sgtSingularityModel;
	}

	public static void Pool(SgtSingularityModel model)
	{
		if (model != null)
		{
			model.Singularity = null;
			SgtComponentPool<SgtSingularityModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtSingularityModel model)
	{
		if (model != null)
		{
			model.Singularity = null;
			model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Singularity == null)
		{
			Pool(this);
		}
	}
}
