using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

[AddComponentMenu("")]
public class TurnableUnitComponent : MonoBehaviour
{
	public Axis ignoreAxis = Axis.Y;

	private void Awake()
	{
		Debug.LogWarning("The TurnableUnitComponent component is no longer valid and should be removed. Use the 'Upgrade Scene' tool in the Tools -> Apex menu to fix up the scene.");
	}
}
