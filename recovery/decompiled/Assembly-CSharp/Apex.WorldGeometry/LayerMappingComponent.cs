using UnityEngine;

namespace Apex.WorldGeometry;

[ExecuteInEditMode]
[AddComponentMenu("Apex/Game World/Layer Mapping", 1035)]
[ApexComponent("Game World")]
public class LayerMappingComponent : SingleInstanceComponent<LayerMappingComponent>
{
	[Tooltip("The layer(s) dedicated to static obstacles.")]
	public LayerMask staticObstacleLayer;

	[Tooltip("The layer(s) dedicated to terrain.")]
	public LayerMask terrainLayer;

	[Tooltip("The layer(s) dedicated to units.")]
	public LayerMask unitLayer;

	private void OnEnable()
	{
		Map();
	}

	private void OnValidate()
	{
		Map();
	}

	internal virtual void Map()
	{
		Layers.blocks = staticObstacleLayer;
		Layers.terrain = terrainLayer;
		Layers.units = unitLayer;
	}
}
