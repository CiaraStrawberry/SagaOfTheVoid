using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public interface IUnitHeightProviderFactory
{
	IUnitHeightProvider Create(Collider c);
}
