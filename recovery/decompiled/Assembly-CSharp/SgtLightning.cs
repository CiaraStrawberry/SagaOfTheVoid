using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtLightning : MonoBehaviour
{
	[Tooltip("The lightning spawner this was spawned from")]
	public SgtLightningSpawner LightningSpawner;

	[Tooltip("The maximum amount of seconds this lightning has been active for")]
	public float Age;

	[Tooltip("The maximum amount of seconds this lightning can be active for")]
	public float Life;

	[NonSerialized]
	private MeshFilter meshFilter;

	[NonSerialized]
	private MeshRenderer meshRenderer;

	[NonSerialized]
	private Mesh mesh;

	[NonSerialized]
	private Material material;

	public Material Material => material;

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

	public static SgtLightning Create(SgtLightningSpawner lightningSpawner)
	{
		SgtLightning sgtLightning = SgtComponentPool<SgtLightning>.Pop(lightningSpawner.transform, "Lightning", lightningSpawner.gameObject.layer);
		sgtLightning.LightningSpawner = lightningSpawner;
		return sgtLightning;
	}

	public static void Pool(SgtLightning model)
	{
		if (model != null)
		{
			model.LightningSpawner = null;
			SgtComponentPool<SgtLightning>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtLightning model)
	{
		if (model != null)
		{
			model.LightningSpawner = null;
			model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(material);
	}

	protected virtual void Update()
	{
		if (LightningSpawner == null)
		{
			Pool(this);
			return;
		}
		if (Application.isPlaying)
		{
			Age += Time.deltaTime;
		}
		if (Age >= Life)
		{
			SgtComponentPool<SgtLightning>.Add(this);
		}
		else if (material != null)
		{
			material.SetFloat("_Age", SgtHelper.Divide(Age, Life));
		}
	}
}
