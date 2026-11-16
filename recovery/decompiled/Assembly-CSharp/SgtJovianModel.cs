using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtJovianModel : MonoBehaviour
{
	[Tooltip("The jovian this belongs to")]
	public SgtJovian Jovian;

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

	public static SgtJovianModel Create(SgtJovian jovian)
	{
		SgtJovianModel sgtJovianModel = SgtComponentPool<SgtJovianModel>.Pop(jovian.transform, "Jovian Model", jovian.gameObject.layer);
		sgtJovianModel.Jovian = jovian;
		return sgtJovianModel;
	}

	public static void Pool(SgtJovianModel model)
	{
		if (model != null)
		{
			model.Jovian = null;
			SgtComponentPool<SgtJovianModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtJovianModel model)
	{
		if (model != null)
		{
			model.Jovian = null;
			model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Jovian == null)
		{
			Pool(this);
		}
	}
}
