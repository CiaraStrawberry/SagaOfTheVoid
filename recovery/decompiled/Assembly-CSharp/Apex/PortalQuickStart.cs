using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Portal", 101)]
public sealed class PortalQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		return QuickStarts.Portal(gameObject);
	}
}
