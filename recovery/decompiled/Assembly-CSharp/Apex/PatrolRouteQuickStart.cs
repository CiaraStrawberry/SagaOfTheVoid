using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Patrol Route", 102)]
public sealed class PatrolRouteQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		return QuickStarts.PatrolRoute(gameObject);
	}
}
