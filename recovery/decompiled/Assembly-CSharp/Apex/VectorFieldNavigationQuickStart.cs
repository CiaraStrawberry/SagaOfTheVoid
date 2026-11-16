using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Vector Field Navigation", 208)]
public sealed class VectorFieldNavigationQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.VectorFieldNavigation(gameObject, !isPrefab);
		return null;
	}
}
