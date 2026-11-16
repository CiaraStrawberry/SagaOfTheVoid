using System.Collections.Generic;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.DataStructures;

public class HeightDictionary : IHeightLookup
{
	private IDictionary<int, float> _lookup;

	private int _sizeX;

	private int _sizeZ;

	private float _originY;

	public bool hasHeights => _lookup.Count > 0;

	public int heightCount => _lookup.Count;

	public HeightDictionary(int sizeX, int sizeZ, int heightEntryCount, float originY)
	{
		_lookup = new Dictionary<int, float>(heightEntryCount);
		_sizeX = sizeX;
		_sizeZ = sizeZ;
		_originY = originY;
	}

	public bool Add(int x, int z, float height)
	{
		int key = x * _sizeZ + z;
		if (height != _originY)
		{
			_lookup[key] = height;
		}
		return true;
	}

	public bool TryGetHeight(int x, int z, out float height)
	{
		int key = x * _sizeZ + z;
		return _lookup.TryGetValue(key, out height);
	}

	public void Cleanup()
	{
	}

	public IHeightLookup PrepareForUpdate(MatrixBounds suggestedBounds, out MatrixBounds requiredBounds)
	{
		requiredBounds = suggestedBounds;
		return this;
	}

	public void FinishUpdate(IHeightLookup updatedHeights)
	{
	}

	public void Render(Vector3 position, float pointGranularity, Color drawColor)
	{
		Bounds bounds = default;
		bounds.SetMinMax(position + new Vector3(0f, 5f, 0f), position + new Vector3((float)_sizeX * pointGranularity, 5f, (float)_sizeZ * pointGranularity));
		Gizmos.color = drawColor;
		Gizmos.DrawCube(bounds.center, bounds.size);
	}
}
