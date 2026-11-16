using UnityEngine;

namespace Apex.Units;

[AddComponentMenu("Apex/Game World/Default Unit Grouping Strategy", 1029)]
[ApexComponent("Steering")]
public class SteeringUnitGroupingStrategyFactory : MonoBehaviour, IUnitGroupingStrategyFactory
{
	public IGroupingStrategy<IUnitFacade> CreateStrategy()
	{
		return new DefaultSteeringUnitGroupingStrategy();
	}
}
