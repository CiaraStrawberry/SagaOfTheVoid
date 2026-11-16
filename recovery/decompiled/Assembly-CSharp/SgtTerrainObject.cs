using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Object")]
public class SgtTerrainObject : MonoBehaviour
{
	public Action OnSpawn;

	public Action OnDespawn;

	[Tooltip("Can this particle be pooled?")]
	public bool Pool;

	[Tooltip("The minimum scale this prefab is multiplied by when spawned")]
	public float ScaleMin = 1f;

	[Tooltip("The maximum scale this prefab is multiplied by when spawned")]
	public float ScaleMax = 1.1f;

	[Tooltip("How far from the center the height samples are taken to align to the surface normal in world coordinates (0 = no alignment)")]
	public float AlignToNormal;

	public long X;

	public long Y;

	[Tooltip("The prefab this was instantiated from")]
	public SgtTerrainObject Prefab;

	public void Spawn(SgtTerrain terrain, SgtTerrainLevel level, SgtVector3D localPoint)
	{
		if (OnSpawn != null)
		{
			OnSpawn();
		}
		transform.SetParent(level.transform, worldPositionStays: false);
		localPoint = terrain.GetLocalPoint(localPoint);
		Vector3 fromDirection = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.up;
		transform.localPosition = (Vector3)localPoint;
		transform.localRotation = Quaternion.FromToRotation(fromDirection, terrain.transform.TransformDirection(transform.localPosition));
		transform.localScale = Prefab.transform.localScale * UnityEngine.Random.Range(ScaleMin, ScaleMax);
		if (AlignToNormal == 0f)
		{
		}
	}

	public void Despawn()
	{
		if (OnDespawn != null)
		{
			OnDespawn();
		}
		if (Pool)
		{
			SgtComponentPool<SgtTerrainObject>.Add(this);
		}
		else
		{
			SgtHelper.Destroy(gameObject);
		}
	}
}
