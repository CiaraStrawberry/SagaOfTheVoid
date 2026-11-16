using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Navigating Unit with Selection", 201)]
public sealed class NavigatingUnitWithSelectionQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.NavigatingUnitWithSelection(gameObject, !isPrefab);
		return null;
	}
}
