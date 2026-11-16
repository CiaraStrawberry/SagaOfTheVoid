using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Invalid Destination", 1034)]
[ApexComponent("Game World")]
public class InvalidDestinationComponent : MonoBehaviour
{
	public bool entireTransform = true;

	public Bounds onlySubArea;

	private void Start()
	{
		if (gameObject.layer == 0)
		{
			Debug.LogWarning("The InvalidDestinationComponent must be assigned to the terrain layer to function correctly.");
			enabled = false;
		}
	}
}
