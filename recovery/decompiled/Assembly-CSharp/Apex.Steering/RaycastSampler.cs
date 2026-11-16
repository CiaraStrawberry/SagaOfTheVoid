using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public sealed class RaycastSampler : ISampleHeights, ISampleHeightsSimple
{
	public HeightSamplingMode samplingStrategy => HeightSamplingMode.Raycast;

	public float SampleHeight(Vector3 position)
	{
		CellMatrix matrix = GridManager.instance.GetGrid(position)?.cellMatrix;
		return SampleHeight(position, matrix);
	}

	public float SampleHeight(Vector3 position, CellMatrix matrix)
	{
		float maxDistance;
		if (matrix != null)
		{
			position.y = matrix.origin.y + matrix.upperBoundary;
			maxDistance = matrix.upperBoundary + matrix.lowerBoundary;
		}
		else
		{
			maxDistance = float.PositiveInfinity;
		}
		if (Physics.Raycast(position, Vector3.down, out var hitInfo, maxDistance, Layers.terrain))
		{
			return hitInfo.point.y;
		}
		return -10000f;
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		CellMatrix matrix = GridManager.instance.GetGrid(position)?.cellMatrix;
		return TrySampleHeight(position, matrix, out height);
	}

	public bool TrySampleHeight(Vector3 position, CellMatrix matrix, out float height)
	{
		float maxDistance;
		if (matrix != null)
		{
			position.y = matrix.origin.y + matrix.upperBoundary;
			maxDistance = matrix.upperBoundary + matrix.lowerBoundary;
			if (!matrix.bounds.Contains(position))
			{
				height = -10000f;
				return false;
			}
		}
		else
		{
			maxDistance = float.PositiveInfinity;
		}
		if (Physics.Raycast(position, Vector3.down, out var hitInfo, maxDistance, Layers.terrain))
		{
			height = hitInfo.point.y;
			return true;
		}
		height = -10000f;
		return false;
	}
}
