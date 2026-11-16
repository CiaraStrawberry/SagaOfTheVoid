using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Navigating Unit on Patrol", 202)]
public sealed class NavigatingUnitOnPatrolQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.NavigatingUnitOnPatrol(gameObject, !isPrefab);
		return null;
	}
}
