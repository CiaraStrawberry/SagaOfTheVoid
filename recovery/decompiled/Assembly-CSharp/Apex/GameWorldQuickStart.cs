using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Game World", 100)]
public sealed class GameWorldQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		return QuickStarts.GameWorld(gameObject);
	}
}
