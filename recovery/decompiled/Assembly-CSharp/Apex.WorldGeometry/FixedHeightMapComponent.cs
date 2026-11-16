using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Fixed Height Map", 1031)]
[ApexComponent("Game World")]
public class FixedHeightMapComponent : ExtendedMonoBehaviour, IHeightMap
{
	public float height;

	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	Bounds IHeightMap.bounds => bounds;

	public bool isGridBound => false;

	public float granularity => 1f;

	public float SampleHeight(Vector3 position)
	{
		return height;
	}

	public bool TrySampleHeight(Vector3 position, out float height)
	{
		height = this.height;
		return true;
	}

	public bool Contains(Vector3 pos)
	{
		return bounds.Contains(pos);
	}

	protected override void OnStartAndEnable()
	{
		HeightMapManager.instance.RegisterMap(this);
	}

	private void OnDisable()
	{
		HeightMapManager.instance.UnregisterMap(this);
	}
}
