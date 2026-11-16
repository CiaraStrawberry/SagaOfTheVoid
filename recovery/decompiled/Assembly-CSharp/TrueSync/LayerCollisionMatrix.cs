using UnityEngine;

namespace TrueSync;

public class LayerCollisionMatrix
{
	public static bool CollisionEnabled(int layerA, int layerB)
	{
		TrueSyncConfig config = TrueSyncManager.Config;
		if (config == null)
		{
			return true;
		}
		return !config.GetIgnoreLayerCollision(layerA, layerB);
	}

	public static bool CollisionEnabled(GameObject goA, GameObject goB)
	{
		return CollisionEnabled(goA.layer, goB.layer);
	}
}
