using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("Apex/Unit/Navigation/Wander", 1033)]
[ApexComponent("Behaviours")]
public class WanderComponent : ExtendedMonoBehaviour
{
	public float radius = 10f;

	public float minimumDistance = 4f;

	public float lingerForSeconds;

	protected override void OnStartAndEnable()
	{
		IUnitFacade unitFacade = this.GetUnitFacade();
		if (unitFacade == null)
		{
			Debug.LogError("WanderBehaviour requires a component that implements IMovable.");
		}
		unitFacade.Wander(radius, minimumDistance, lingerForSeconds);
	}

	private void OnDisable()
	{
		this.GetUnitFacade()?.StopWander();
	}
}
