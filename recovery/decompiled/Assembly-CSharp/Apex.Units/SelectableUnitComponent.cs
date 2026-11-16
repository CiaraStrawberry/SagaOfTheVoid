using UnityEngine;

namespace Apex.Units;

[AddComponentMenu("")]
public class SelectableUnitComponent : ExtendedMonoBehaviour
{
	public GameObject selectionVisual;

	private void Awake()
	{
		Debug.LogWarning("The SelectableUnitComponent component is no longer valid and should be removed. Use the 'Upgrade Scene' tool in the Tools -> Apex menu to fix up the scene.");
	}
}
