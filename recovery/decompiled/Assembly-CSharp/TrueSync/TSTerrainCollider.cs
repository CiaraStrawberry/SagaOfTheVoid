using TrueSync.Physics3D;
using UnityEngine;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/TerrainCollider", 0)]
[RequireComponent(typeof(Terrain))]
public class TSTerrainCollider : TSCollider
{
	public int Resolution
	{
		get
		{
			Terrain component = GetComponent<Terrain>();
			TerrainData terrainData = component.terrainData;
			return terrainData.heightmapResolution;
		}
	}

	public Vector3 Size
	{
		get
		{
			Terrain component = GetComponent<Terrain>();
			TerrainData terrainData = component.terrainData;
			return terrainData.size;
		}
	}

	public override Shape CreateShape()
	{
		Terrain component = GetComponent<Terrain>();
		TerrainData terrainData = component.terrainData;
		int heightmapResolution = terrainData.heightmapResolution;
		float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
		FP[,] array = new FP[heights.GetLength(0), heights.GetLength(1)];
		for (int i = 0; i < heights.GetLength(0); i++)
		{
			for (int j = 0; j < heights.GetLength(1); j++)
			{
				ref FP reference = ref array[i, j];
				reference = heights[i, j];
			}
		}
		FP fP = terrainData.size.y;
		for (int k = 0; k < heightmapResolution; k++)
		{
			for (int l = 0; l < heightmapResolution; l++)
			{
				array[k, l] *= fP;
			}
		}
		for (int m = 0; m < heightmapResolution - 1; m++)
		{
			for (int n = m; n < heightmapResolution; n++)
			{
				FP fP2 = array[m, n];
				FP fP3 = array[n, m];
				array[m, n] = fP3;
				array[n, m] = fP2;
			}
		}
		return new TerrainShape(array, terrainData.size.x / (float)(heightmapResolution - 1), terrainData.size.z / (float)(heightmapResolution - 1));
	}

	protected override Vector3 GetGizmosSize()
	{
		return Vector3.one;
	}

	protected override void DrawGizmos()
	{
	}
}
