using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Navigating Unit", 200)]
public sealed class NavigatingUnitQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.NavigatingUnit(gameObject, !isPrefab);
		return null;
	}
}
