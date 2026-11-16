using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("Space Graphics Toolkit/SGT Debris Spawner")]
public class SgtDebrisSpawner : MonoBehaviour
{
	public static List<SgtDebrisSpawner> AllDebrisSpawners = new List<SgtDebrisSpawner>();

	[Tooltip("If this transform is inside the radius then debris will begin spawning")]
	public Transform Follower;

	[Tooltip("How quickly the debris shows after it spawns")]
	public float ShowSpeed = 10f;

	[Tooltip("The distance from the follower that debris begins spawning")]
	public float ShowDistance = 0.9f;

	[Tooltip("The distance from the follower that debris gets hidden")]
	public float HideDistance = 1f;

	[Tooltip("The maximum amount of debris that can be spawned")]
	public int SpawnLimit = 50;

	[Tooltip("The minimum amount of seconds between debris spawns")]
	public float SpawnRateMin = 0.5f;

	[Tooltip("The maximum amount of seconds between debris spawns")]
	public float SpawnRateMax = 1f;

	[Tooltip("The minimum scale multiplier applied to spawned debris")]
	public float SpawnScaleMin = 1f;

	[Tooltip("The maximum scale multiplier applied to spawned debris")]
	public float SpawnScaleMax = 1f;

	[Tooltip("Should all the debris be automatically spawned at the start?")]
	public bool SpawnOnAwake;

	[Tooltip("These prefabs are randomly picked from when spawning new debris")]
	public List<SgtDebris> Prefabs;

	[Tooltip("These shapes define where the debris can spawn, if this is empty then they will spawn everywhere")]
	[FormerlySerializedAs("Locations")]
	public List<SgtShape> Shapes;

	public List<SgtDebris> Debris;

	private float spawnCooldown;

	private Vector3 followerPosition;

	private Vector3 followerVelocity;

	private float minScale = 0.001f;

	private SgtDebris targetPrefab;

	[ContextMenu("Clear Debris")]
	public void ClearDebris()
	{
		if (Debris == null)
		{
			return;
		}
		for (int num = Debris.Count - 1; num >= 0; num--)
		{
			SgtDebris sgtDebris = Debris[num];
			if (sgtDebris != null)
			{
				Despawn(sgtDebris, num);
			}
			else
			{
				Debris.RemoveAt(num);
			}
		}
	}

	[ContextMenu("Spawn Debris Inside")]
	public void SpawnDebrisInside()
	{
		SpawnDebris(inside: true);
	}

	public void SpawnDebris(bool inside)
	{
		if (Prefabs == null || Prefabs.Count <= 0 || !(Follower != null))
		{
			return;
		}
		int index = Random.Range(0, Prefabs.Count - 1);
		SgtDebris sgtDebris = Prefabs[index];
		if (sgtDebris != null)
		{
			SgtDebris sgtDebris2 = Spawn(sgtDebris);
			Vector3 vector = Random.insideUnitSphere * HideDistance + followerVelocity;
			float hideDistance = HideDistance;
			hideDistance = ((!inside) ? Random.Range(ShowDistance, HideDistance) : Random.Range(0f, HideDistance));
			if (vector.sqrMagnitude <= 0f)
			{
				vector = Random.onUnitSphere;
			}
			sgtDebris2.Show = 0f;
			sgtDebris2.Prefab = sgtDebris;
			sgtDebris2.Scale = sgtDebris.transform.localScale * Random.Range(SpawnScaleMin, SpawnScaleMax);
			sgtDebris2.transform.SetParent(transform, worldPositionStays: false);
			sgtDebris2.transform.position = Follower.transform.position + vector.normalized * hideDistance;
			sgtDebris2.transform.rotation = Random.rotationUniform;
			sgtDebris2.transform.localScale = sgtDebris.transform.localScale * minScale;
			if (sgtDebris2.OnSpawn != null)
			{
				sgtDebris2.OnSpawn();
			}
			if (Debris == null)
			{
				Debris = new List<SgtDebris>();
			}
			Debris.Add(sgtDebris2);
		}
	}

	[ContextMenu("Spawn All Debris Inside")]
	public void SpawnAllDebrisInside()
	{
		if (SpawnLimit > 0)
		{
			int num = ((Debris != null) ? Debris.Count : 0);
			for (int i = num; i < SpawnLimit; i++)
			{
				SpawnDebrisInside();
			}
		}
	}

	public float GetDensity(Vector3 worldPosition)
	{
		if (Shapes == null || Shapes.Count == 0)
		{
			return 1f;
		}
		float num = 0f;
		for (int num2 = Shapes.Count - 1; num2 >= 0; num2--)
		{
			SgtShape sgtShape = Shapes[num2];
			if (sgtShape != null)
			{
				float density = sgtShape.GetDensity(worldPosition);
				if (density > num)
				{
					num = density;
				}
			}
		}
		return num;
	}

	public static SgtDebrisSpawner CreateDebrisSpawner(int layer = 0, Transform parent = null)
	{
		return CreateDebrisSpawner(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtDebrisSpawner CreateDebrisSpawner(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Debris Spawner", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtDebrisSpawner>();
	}

	protected virtual void Awake()
	{
		ResetFollower();
		if (SpawnOnAwake)
		{
			SpawnAllDebrisInside();
		}
	}

	protected virtual void OnEnable()
	{
		AllDebrisSpawners.Add(this);
		ResetFollower();
	}

	protected virtual void FixedUpdate()
	{
		Vector3 vector = ((!(Follower != null)) ? Vector3.zero : Follower.position);
		followerVelocity = (vector - followerPosition) * SgtHelper.Reciprocal(Time.fixedDeltaTime);
		followerPosition = vector;
	}

	protected virtual void Update()
	{
		if (Follower == null)
		{
			ClearDebris();
			return;
		}
		Vector3 position = Follower.position;
		float density = GetDensity(position);
		if (density > 0f)
		{
			int num = ((Debris != null) ? Debris.Count : 0);
			if (num < SpawnLimit)
			{
				spawnCooldown -= Time.deltaTime;
				while (spawnCooldown <= 0f)
				{
					spawnCooldown += Random.Range(SpawnRateMin, SpawnRateMax);
					SpawnDebris(inside: false);
					num++;
					if (num >= SpawnLimit)
					{
						break;
					}
				}
			}
		}
		if (Debris == null)
		{
			return;
		}
		float b = HideDistance - ShowDistance;
		for (int num2 = Debris.Count - 1; num2 >= 0; num2--)
		{
			SgtDebris sgtDebris = Debris[num2];
			if (sgtDebris != null)
			{
				float num3 = 0f;
				float num4 = Vector3.Distance(position, sgtDebris.transform.position);
				sgtDebris.Show = SgtHelper.Dampen(sgtDebris.Show, 1f, ShowSpeed, Time.deltaTime, 0.1f);
				if (num4 < ShowDistance)
				{
					num3 = 1f;
				}
				else
				{
					num3 = ((!(num4 > HideDistance)) ? (1f - SgtHelper.Divide(num4 - ShowDistance, b)) : 0f);
				}
				sgtDebris.transform.localScale = sgtDebris.Scale * sgtDebris.Show * Mathf.Max(minScale, num3);
				if (num3 <= 0f)
				{
					Despawn(sgtDebris, num2);
				}
			}
			else
			{
				Debris.RemoveAt(num2);
			}
		}
	}

	private SgtDebris Spawn(SgtDebris prefab)
	{
		if (prefab.Pool)
		{
			targetPrefab = prefab;
			SgtDebris sgtDebris = SgtComponentPool<SgtDebris>.Pop(DebrisMatch);
			if (sgtDebris != null)
			{
				sgtDebris.transform.SetParent(null, worldPositionStays: false);
				return sgtDebris;
			}
		}
		return Object.Instantiate(prefab);
	}

	private void Despawn(SgtDebris debris, int index)
	{
		if (debris.OnDespawn != null)
		{
			debris.OnDespawn();
		}
		if (debris.Pool)
		{
			SgtComponentPool<SgtDebris>.Add(debris);
		}
		else
		{
			SgtHelper.Destroy(debris.gameObject);
		}
		Debris.RemoveAt(index);
	}

	protected virtual void OnDisable()
	{
		AllDebrisSpawners.Remove(this);
	}

	private void ResetFollower()
	{
		followerVelocity = Vector3.zero;
		followerPosition = ((!(Follower != null)) ? Vector3.zero : Follower.position);
	}

	private bool DebrisMatch(SgtDebris debris)
	{
		return debris != null && debris.Prefab == targetPrefab;
	}
}
