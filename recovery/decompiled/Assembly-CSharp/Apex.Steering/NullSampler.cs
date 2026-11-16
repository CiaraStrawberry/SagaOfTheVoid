using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public sealed class NullSampler : ISampleHeights, ISampleHeightsSimple
{
	public HeightSamplingMode samplingStrategy => HeightSamplingMode.NoHeightSampling;

	public float SampleHeight(Vector3 position)
	{
		CellMatrix matrix = GridManager.instance.GetGrid(position)?.cellMatrix;
		return SampleHeight(position, matrix);
	}

	public float SampleHeight(Vector3 position, CellMatrix matrix)
	{
		return matrix?.origin.y ?? (-10000f);
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		CellMatrix matrix = GridManager.instance.GetGrid(position)?.cellMatrix;
		return TrySampleHeight(position, matrix, out height);
	}

	public bool TrySampleHeight(Vector3 position, CellMatrix matrix, out float height)
	{
		if (matrix != null)
		{
			height = matrix.origin.y;
			return true;
		}
		height = -10000f;
		return false;
	}
}
