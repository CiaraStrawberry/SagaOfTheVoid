using UnityEngine;

namespace Apex.Services;

public sealed class LayerMaskWrapper : ILayerMask
{
	private LayerMask _actual;

	public int value
	{
		get
		{
			return _actual.value;
		}
		set
		{
			_actual.value = value;
		}
	}

	public string LayerToName(int layer)
	{
		return LayerMask.LayerToName(layer);
	}

	public int NameToLayer(string layerName)
	{
		return LayerMask.NameToLayer(layerName);
	}

	public int GetMask(string[] layerNames)
	{
		return LayerMask.GetMask(layerNames);
	}
}
