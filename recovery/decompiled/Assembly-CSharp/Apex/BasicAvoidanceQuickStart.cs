using UnityEngine;

namespace Apex;

[AddComponentMenu("Apex/Quick Starts/Navigation/Basic Avoidance", 300)]
public sealed class BasicAvoidanceQuickStart : ApexQuickStartComponent
{
	public override GameObject Apply(bool isPrefab)
	{
		QuickStarts.BasicAvoidance(gameObject, !isPrefab);
		return null;
	}
}
