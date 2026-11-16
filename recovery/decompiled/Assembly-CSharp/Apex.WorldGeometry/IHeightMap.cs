using UnityEngine;

namespace Apex.WorldGeometry;

public interface IHeightMap
{
	Bounds bounds { get; }

	bool isGridBound { get; }

	float granularity { get; }

	float SampleHeight(Vector3 position);

	bool TrySampleHeight(Vector3 position, out float height);

	bool Contains(Vector3 pos);
}
