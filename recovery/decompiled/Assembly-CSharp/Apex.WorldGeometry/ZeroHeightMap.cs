using UnityEngine;

namespace Apex.WorldGeometry;

public class ZeroHeightMap : IHeightMap
{
	private Bounds _bounds = new Bounds(Vector3.zero, Vector3.zero);

	public Bounds bounds => _bounds;

	public bool isGridBound => false;

	public float granularity => 0f;

	public float SampleHeight(Vector3 position)
	{
		return -10000f;
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		height = -10000f;
		return false;
	}

	public bool Contains(Vector3 pos)
	{
		return false;
	}
}
