using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Terrain Height Map", 1043)]
[ApexComponent("Game World")]
public sealed class TerrainHeightMap : MonoBehaviour, IHeightMap
{
	private Bounds _bounds;

	private float _granularity;

	public Terrain terrain;

	public Bounds bounds => _bounds;

	public bool isGridBound => false;

	public float granularity => _granularity;

	private void Start()
	{
		if (terrain == null)
		{
			Debug.LogError("You must assign a Terrain asset to the TerrainHeightMap.");
			enabled = false;
			return;
		}
		TerrainData terrainData = terrain.terrainData;
		Vector3 position = terrain.GetPosition();
		Vector3 center = new Vector3(position.x + terrainData.size.x / 2f, position.y + terrainData.size.y / 2f, position.z + terrainData.size.z / 2f);
		_bounds = new Bounds(center, terrainData.size);
		Vector3 heightmapScale = terrain.terrainData.heightmapScale;
		_granularity = (heightmapScale.x + heightmapScale.z) / 2f;
	}

	private void OnEnable()
	{
		HeightMapManager.instance.RegisterMap(this);
	}

	private void OnDisable()
	{
		HeightMapManager.instance.UnregisterMap(this);
	}

	public float SampleHeight(Vector3 position)
	{
		return terrain.SampleHeight(position);
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		height = terrain.SampleHeight(position);
		return true;
	}

	public bool Contains(Vector3 pos)
	{
		return _bounds.Contains(pos);
	}
}
