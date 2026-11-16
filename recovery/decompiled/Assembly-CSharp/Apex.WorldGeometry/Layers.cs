using UnityEngine;

namespace Apex.WorldGeometry;

public static class Layers
{
	public static LayerMask terrain { get; set; }

	public static LayerMask blocks { get; set; }

	public static LayerMask units { get; set; }

	public static bool InLayer(GameObject go, LayerMask layer)
	{
		return ((int)layer & (1 << go.layer)) != 0;
	}
}
