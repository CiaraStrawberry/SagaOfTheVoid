using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtSkysphereModel : MonoBehaviour
{
	[Tooltip("The skysphere this belongs to")]
	public SgtSkysphere Skysphere;

	[NonSerialized]
	private MeshFilter meshFilter;

	[NonSerialized]
	private MeshRenderer meshRenderer;

	private bool saveSet;

	private Vector3 saveLocalPosition;

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

	public void Save()
	{
		if (!saveSet)
		{
			saveSet = true;
			saveLocalPosition = transform.localPosition;
		}
	}

	public void Restore()
	{
		if (saveSet)
		{
			saveSet = false;
			transform.localPosition = saveLocalPosition;
		}
	}

	public static SgtSkysphereModel Create(SgtSkysphere skysphere)
	{
		SgtSkysphereModel sgtSkysphereModel = SgtComponentPool<SgtSkysphereModel>.Pop(skysphere.transform, "Model", skysphere.gameObject.layer);
		sgtSkysphereModel.Skysphere = skysphere;
		return sgtSkysphereModel;
	}

	public static void Pool(SgtSkysphereModel model)
	{
		if (model != null)
		{
			model.Skysphere = null;
			SgtComponentPool<SgtSkysphereModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtSkysphereModel model)
	{
		if (model != null)
		{
			model.Skysphere = null;
			model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Skysphere == null)
		{
			Pool(this);
		}
	}
}
