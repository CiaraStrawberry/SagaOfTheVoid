using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtAtmosphereOuter : MonoBehaviour
{
	[Tooltip("The atmosphere this belongs to")]
	public SgtAtmosphere Atmosphere;

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

	public void SetScale(float scale)
	{
		SgtHelper.SetLocalScale(transform, scale);
	}

	public static SgtAtmosphereOuter Create(SgtAtmosphere atmosphere)
	{
		SgtAtmosphereOuter sgtAtmosphereOuter = SgtComponentPool<SgtAtmosphereOuter>.Pop(atmosphere.transform, "Outer", atmosphere.gameObject.layer);
		sgtAtmosphereOuter.Atmosphere = atmosphere;
		return sgtAtmosphereOuter;
	}

	public static void Pool(SgtAtmosphereOuter outer)
	{
		if (outer != null)
		{
			outer.Atmosphere = null;
			SgtComponentPool<SgtAtmosphereOuter>.Add(outer);
		}
	}

	public static void MarkForDestruction(SgtAtmosphereOuter outer)
	{
		if (outer != null)
		{
			outer.Atmosphere = null;
			outer.gameObject.SetActive(value: true);
		}
	}

	protected virtual void Update()
	{
		if (Atmosphere == null)
		{
			Pool(this);
		}
	}
}
