using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Navigating Unit Wandering", 203)]
public sealed class NavigatingUnitWanderingQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.NavigatingUnitWandering(gameObject, !isPrefab);
		return null;
	}
}
