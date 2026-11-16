using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public sealed class HeightMapSampler : ISampleHeights, ISampleHeightsSimple
{
	public HeightSamplingMode samplingStrategy => HeightSamplingMode.HeightMap;

	public float SampleHeight(Vector3 position)
	{
		return SampleHeight(position, null);
	}

	public float SampleHeight(Vector3 position, CellMatrix matrix)
	{
		IHeightMap heightMap = HeightMapManager.instance.GetHeightMap(position);
		return heightMap.SampleHeight(position);
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		return TrySampleHeight(position, null, out height);
	}

	public bool TrySampleHeight(Vector3 position, CellMatrix matrix, out float height)
	{
		IHeightMap heightMap = HeightMapManager.instance.GetHeightMap(position);
		return heightMap.TrySampleHeight(position, out height);
	}
}
