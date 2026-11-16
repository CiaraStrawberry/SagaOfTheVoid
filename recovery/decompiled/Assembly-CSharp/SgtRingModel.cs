using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtRingModel : MonoBehaviour
{
	[Tooltip("The ring this belongs to")]
	public SgtRing Ring;

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

	public void SetRotation(Quaternion rotation)
	{
		SgtHelper.SetLocalRotation(transform, rotation);
	}

	public static SgtRingModel Create(SgtRing ring)
	{
		SgtRingModel sgtRingModel = SgtComponentPool<SgtRingModel>.Pop(ring.transform, "Ring Model", ring.gameObject.layer);
		sgtRingModel.Ring = ring;
		return sgtRingModel;
	}

	public static void Pool(SgtRingModel segment)
	{
		if (segment != null)
		{
			segment.Ring = null;
			SgtComponentPool<SgtRingModel>.Add(segment);
		}
	}

	public static void MarkForDestruction(SgtRingModel segment)
	{
		if (segment != null)
		{
			segment.Ring = null;
			segment.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Ring == null)
		{
			Pool(this);
		}
	}
}
