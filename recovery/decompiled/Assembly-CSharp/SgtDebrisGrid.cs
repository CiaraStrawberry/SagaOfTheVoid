using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Debris Grid")]
public class SgtDebrisGrid : MonoBehaviour
{
	[Tooltip("The transform the debris will spawn around (e.g. MainCamera)")]
	public Transform Target;

	[Tooltip("The shapes the debris will spawn inside")]
	public SgtShapeGroup SpawnInside;

	[Tooltip("The distance from the follower that debris begins spawning")]
	public float ShowDistance = 0.9f;

	[Tooltip("The distance from the follower that debris gets hidden")]
	public float HideDistance = 1f;

	[Tooltip("The size of each cell in world space")]
	public double CellSize = 1.0;

	[Tooltip("How far from the center of each cell the debris can be spawned. This should be decreated to stop debris intersecting")]
	[Range(0f, 0.5f)]
	public float CellNoise = 0.5f;

	[Tooltip("The maximum expected amount of debris based on the cell size settings")]
	public float DebrisCountTarget = 100f;

	[Tooltip("The minimum scale multiplier of the debris")]
	public float ScaleMin = 1f;

	[Tooltip("The maximum scale multiplier of the debris")]
	public float ScaleMax = 2f;

	[Tooltip("The amount the random scale is biased toward the min or max values")]
	public float ScaleBias = 2f;

	[Tooltip("Should the debris be given a random rotation, or inherit from the prefab that spawned it?")]
	public bool RandomRotation = true;

	[Tooltip("Random seed used when generating the debris")]
	[SgtSeed]
	public int Seed;

	[Tooltip("These prefabs are randomly picked from when spawning new debris")]
	public List<SgtDebris> Prefabs;

	public List<SgtDebris> Debris;

	private float minScale = 0.001f;

	[SerializeField]
	private SgtBoundsL bounds;

	private SgtDebris targetPrefab;

	[ContextMenu("Clear Debris")]
	public void ClearDebris()
	{
		if (Debris != null)
		{
			for (int num = Debris.Count - 1; num >= 0; num--)
			{
				SgtDebris sgtDebris = Debris[num];
				if (sgtDebris != null)
				{
					Despawn(sgtDebris);
				}
			}
			Debris.Clear();
		}
		bounds.Clear();
	}

	public void UpdateDebris()
	{
		long num = (long)Math.Ceiling(SgtHelper.Divide(HideDistance, (float)CellSize));
		if (Target != null && CellSize > 0.0 && Prefabs != null && DebrisCountTarget > 0f && num > 0)
		{
			Vector3 position = Target.position;
			long x = (long)Math.Round((double)position.x / CellSize);
			long y = (long)Math.Round((double)position.y / CellSize);
			long z = (long)Math.Round((double)position.z / CellSize);
			SgtBoundsL sgtBoundsL = new SgtBoundsL(x, y, z, num);
			if (sgtBoundsL != bounds)
			{
				float num2 = DebrisCountTarget / (float)(num * num * num);
				float min = (float)CellSize * (0.5f - CellNoise);
				float max = (float)CellSize * (0.5f + CellNoise);
				for (long num3 = sgtBoundsL.minZ; num3 <= sgtBoundsL.maxZ; num3++)
				{
					for (long num4 = sgtBoundsL.minY; num4 <= sgtBoundsL.maxY; num4++)
					{
						for (long num5 = sgtBoundsL.minX; num5 <= sgtBoundsL.maxX; num5++)
						{
							if (bounds.Contains(num5, num4, num3))
							{
								continue;
							}
							long num6 = Seed ^ (num5 * 256) ^ (num4 * 65536) ^ (num3 * 16777216);
							SgtHelper.BeginRandomSeed((int)(num6 % int.MaxValue));
							if (UnityEngine.Random.value < num2)
							{
								double num7 = (double)num5 * CellSize + (double)UnityEngine.Random.Range(min, max);
								double num8 = (double)num4 * CellSize + (double)UnityEngine.Random.Range(min, max);
								double num9 = (double)num3 * CellSize + (double)UnityEngine.Random.Range(min, max);
								Vector3 vector = new Vector3((float)num7, (float)num8, (float)num9);
								if (SpawnInside == null || UnityEngine.Random.value < SpawnInside.GetDensity(vector))
								{
									Spawn(num5, num4, num3, vector);
								}
							}
							SgtHelper.EndRandomSeed();
						}
					}
				}
				bounds = sgtBoundsL;
				if (Debris != null)
				{
					for (int num10 = Debris.Count - 1; num10 >= 0; num10--)
					{
						SgtDebris sgtDebris = Debris[num10];
						if (sgtDebris == null)
						{
							Debris.RemoveAt(num10);
						}
						else if (!bounds.Contains(sgtDebris.Cell))
						{
							Despawn(sgtDebris, num10);
						}
					}
				}
			}
			UpdateDebrisScale(position);
		}
		else
		{
			ClearDebris();
		}
	}

	public void UpdateDebrisScale(Vector3 worldPoint)
	{
		if (Debris == null)
		{
			return;
		}
		float num = HideDistance * HideDistance;
		float num2 = ShowDistance * ShowDistance;
		for (int num3 = Debris.Count - 1; num3 >= 0; num3--)
		{
			SgtDebris sgtDebris = Debris[num3];
			if (sgtDebris != null)
			{
				Transform transform = sgtDebris.transform;
				float num4 = Vector3.SqrMagnitude(transform.position - worldPoint);
				if (num4 >= num)
				{
					if (sgtDebris.State != SgtDebris.StateType.Hide)
					{
						sgtDebris.State = SgtDebris.StateType.Hide;
						transform.localScale = sgtDebris.Scale * minScale;
					}
				}
				else if (num4 <= num2)
				{
					if (sgtDebris.State != SgtDebris.StateType.Show)
					{
						sgtDebris.State = SgtDebris.StateType.Show;
						transform.localScale = sgtDebris.Scale;
					}
				}
				else
				{
					sgtDebris.State = SgtDebris.StateType.Fade;
					transform.localScale = sgtDebris.Scale * Mathf.Max(Mathf.InverseLerp(HideDistance, ShowDistance, Mathf.Sqrt(num4)), minScale);
				}
			}
		}
	}

	public static SgtDebrisGrid CreateDebrisGrid(int layer = 0, Transform parent = null)
	{
		return CreateDebrisGrid(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtDebrisGrid CreateDebrisGrid(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Debris Grid", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtDebrisGrid>();
	}

	protected virtual void Update()
	{
		UpdateDebris();
	}

	private void Spawn(long x, long y, long z, Vector3 point)
	{
		int index = UnityEngine.Random.Range(0, Prefabs.Count);
		SgtDebris sgtDebris = Prefabs[index];
		if (sgtDebris != null)
		{
			SgtDebris sgtDebris2 = Spawn(sgtDebris);
			sgtDebris2.Cell.x = x;
			sgtDebris2.Cell.y = y;
			sgtDebris2.Cell.z = z;
			sgtDebris2.transform.position = point;
			if (RandomRotation)
			{
				sgtDebris2.transform.localRotation = UnityEngine.Random.rotation;
			}
			else
			{
				sgtDebris2.transform.localRotation = sgtDebris.transform.rotation;
			}
			sgtDebris2.State = SgtDebris.StateType.Fade;
			sgtDebris2.Scale = sgtDebris.transform.localScale * Mathf.Lerp(ScaleMin, ScaleMax, SgtHelper.Pow(UnityEngine.Random.value, ScaleBias));
			if (sgtDebris2.OnSpawn != null)
			{
				sgtDebris2.OnSpawn();
			}
			Debris.Add(sgtDebris2);
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
				sgtDebris.transform.SetParent(transform, worldPositionStays: false);
				return sgtDebris;
			}
		}
		return UnityEngine.Object.Instantiate(prefab, transform);
	}

	private void Despawn(SgtDebris debris)
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
	}

	private void Despawn(SgtDebris debris, int index)
	{
		Despawn(debris);
		Debris.RemoveAt(index);
	}

	private bool DebrisMatch(SgtDebris debris)
	{
		return debris != null && debris.Prefab == targetPrefab;
	}
}
