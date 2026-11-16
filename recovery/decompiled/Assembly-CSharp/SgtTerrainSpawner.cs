using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Spawner")]
public class SgtTerrainSpawner : SgtTerrainModifier
{
	[Serializable]
	public class Face
	{
		public SgtTerrainLevel Level;

		public SgtRectL Rect;

		public List<SgtTerrainObject> Clones;

		public void Clear()
		{
			if (Clones == null)
			{
				return;
			}
			for (int num = Clones.Count - 1; num >= 0; num--)
			{
				SgtTerrainObject sgtTerrainObject = Clones[num];
				if (sgtTerrainObject != null)
				{
					Despawn(sgtTerrainObject);
				}
			}
			Clones.Clear();
		}

		public void Clear(SgtTerrain.Ring ring)
		{
			if (Clones == null)
			{
				return;
			}
			for (int num = Clones.Count - 1; num >= 0; num--)
			{
				SgtTerrainObject sgtTerrainObject = Clones[num];
				if (sgtTerrainObject != null)
				{
					if (!ring.Outer.Contains(sgtTerrainObject.X, sgtTerrainObject.Y))
					{
						Despawn(sgtTerrainObject);
						Clones.RemoveAt(num);
					}
				}
				else
				{
					Clones.RemoveAt(num);
				}
			}
		}
	}

	[Tooltip("The patch depth required for these objects to spawn")]
	public int Depth;

	[Tooltip("This decides how many prefabs get spawned based on a random 0..1 sample on the x axis")]
	[Range(0f, 1f)]
	public float SpawnProbability;

	public float HeightMin = 1f;

	public float HeightMax = 1.1f;

	[Tooltip("The prefabs we want to spawn on the terrain patch")]
	public List<SgtTerrainObject> Prefabs;

	[SerializeField]
	private List<Face> faces;

	[SerializeField]
	private List<SgtTerrainObject> terrainObjects;

	private static SgtTerrainObject targetPrefab;

	protected override void OnEnable()
	{
		base.OnEnable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateLevel = (SgtTerrain.CalculateQuadDelegate)Delegate.Combine(sgtTerrain.OnCalculateLevel, new SgtTerrain.CalculateQuadDelegate(CalculateLevel));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateLevel = (SgtTerrain.CalculateQuadDelegate)Delegate.Combine(sgtTerrain.OnCalculateLevel, new SgtTerrain.CalculateQuadDelegate(CalculateLevel));
		Clear();
	}

	private Face GetFace(SgtTerrainLevel level)
	{
		if (faces == null)
		{
			faces = new List<Face>();
		}
		for (int num = faces.Count - 1; num >= 0; num--)
		{
			Face face = faces[num];
			if (face != null && face.Level != null)
			{
				if (face.Level == level)
				{
					return face;
				}
			}
			else
			{
				faces.RemoveAt(num);
			}
		}
		return null;
	}

	public void Clear()
	{
		if (faces != null)
		{
			for (int num = faces.Count - 1; num >= 0; num--)
			{
				faces[num]?.Clear();
			}
			faces.Clear();
		}
	}

	private void AddObject(Face face, SgtTerrain.Ring ring, long x, long y)
	{
		if (Prefabs == null || Prefabs.Count <= 0)
		{
			return;
		}
		int index = UnityEngine.Random.Range(0, Prefabs.Count);
		SgtTerrainObject sgtTerrainObject = Prefabs[index];
		if (!(sgtTerrainObject != null))
		{
			return;
		}
		SgtVector3D sgtVector3D = ring.ExtentsX / ring.Detail;
		SgtVector3D sgtVector3D2 = ring.ExtentsY / ring.Detail;
		SgtVector3D localPoint = ring.Center + x * sgtVector3D + y * sgtVector3D2;
		float localHeight = terrain.GetLocalHeight(localPoint);
		if (localHeight >= HeightMin && localHeight < HeightMax)
		{
			SgtTerrainObject sgtTerrainObject2 = Spawn(sgtTerrainObject);
			sgtTerrainObject2.Prefab = sgtTerrainObject;
			sgtTerrainObject2.X = x;
			sgtTerrainObject2.Y = y;
			sgtTerrainObject2.Spawn(terrain, face.Level, localPoint);
			if (face.Clones == null)
			{
				face.Clones = new List<SgtTerrainObject>();
			}
			face.Clones.Add(sgtTerrainObject2);
		}
	}

	private void CalculateLevel(SgtTerrainLevel level, SgtTerrain.Ring ring)
	{
		Face face = GetFace(level);
		if (level.Index == Depth && ring != null)
		{
			if (face == null)
			{
				face = new Face();
				face.Level = level;
				faces.Add(face);
			}
			face.Clear(ring);
			for (long num = ring.Outer.minY; num <= ring.Outer.maxY; num++)
			{
				for (long num2 = ring.Outer.minX; num2 <= ring.Outer.maxX; num2++)
				{
					if ((num2 < face.Rect.minX || num2 >= face.Rect.maxX || num < face.Rect.minY || num >= face.Rect.maxY) && UnityEngine.Random.value < SpawnProbability)
					{
						AddObject(face, ring, num2, num);
					}
				}
			}
			face.Rect = ring.Outer;
		}
		else if (face != null)
		{
			face.Clear();
			faces.Remove(face);
		}
	}

	private static SgtTerrainObject Despawn(SgtTerrainObject prefab)
	{
		if (prefab.Pool)
		{
			SgtComponentPool<SgtTerrainObject>.Add(prefab);
		}
		else
		{
			prefab.Despawn();
		}
		return null;
	}

	private static SgtTerrainObject Spawn(SgtTerrainObject prefab)
	{
		if (prefab.Pool)
		{
			targetPrefab = prefab;
			SgtTerrainObject sgtTerrainObject = SgtComponentPool<SgtTerrainObject>.Pop(ObjectMatch);
			if (sgtTerrainObject != null)
			{
				return sgtTerrainObject;
			}
		}
		return UnityEngine.Object.Instantiate(prefab);
	}

	private static bool ObjectMatch(SgtTerrainObject instance)
	{
		return instance != null && instance.Prefab == targetPrefab;
	}
}
