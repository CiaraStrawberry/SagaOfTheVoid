using System.Collections.Generic;
using UnityEngine;

namespace Apex.WorldGeometry;

public class HeightMapManager
{
	public static readonly HeightMapManager instance = new HeightMapManager();

	private static readonly IHeightMap _zero = new ZeroHeightMap();

	private List<IHeightMap> _offGridHeightMaps;

	private List<IHeightMap> _onGridHeightMaps;

	public bool areHeightMapsEnabled => _onGridHeightMaps.Count > 0 || _offGridHeightMaps.Count > 0;

	public HeightMapManager()
	{
		_offGridHeightMaps = new List<IHeightMap>();
		_onGridHeightMaps = new List<IHeightMap>();
	}

	public void RegisterMap(IHeightMap heightMap)
	{
		if (heightMap.isGridBound)
		{
			_onGridHeightMaps.AddUnique(heightMap);
		}
		else
		{
			_offGridHeightMaps.AddUnique(heightMap);
		}
	}

	public void UnregisterMap(IHeightMap heightMap)
	{
		if (heightMap.isGridBound)
		{
			_onGridHeightMaps.Remove(heightMap);
		}
		else
		{
			_offGridHeightMaps.Remove(heightMap);
		}
	}

	public IHeightMap GetHeightMap(Vector3 pos)
	{
		int count = _onGridHeightMaps.Count;
		for (int i = 0; i < count; i++)
		{
			IHeightMap heightMap = _onGridHeightMaps[i];
			if (heightMap.bounds.Contains(pos))
			{
				return heightMap;
			}
		}
		count = _offGridHeightMaps.Count;
		for (int j = 0; j < count; j++)
		{
			IHeightMap heightMap2 = _offGridHeightMaps[j];
			if (heightMap2.bounds.Contains(pos))
			{
				return heightMap2;
			}
		}
		return _zero;
	}
}
