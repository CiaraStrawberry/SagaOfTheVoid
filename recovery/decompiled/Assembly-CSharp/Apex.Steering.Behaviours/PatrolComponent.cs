using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("Apex/Unit/Navigation/Patrol", 1029)]
[ApexComponent("Behaviours")]
public class PatrolComponent : ExtendedMonoBehaviour
{
	public bool randomize;

	public bool reverse;

	public float lingerForSeconds;

	public PatrolPointsComponent route;

	protected override void OnStartAndEnable()
	{
		IUnitFacade unitFacade = this.GetUnitFacade();
		if (unitFacade == null)
		{
			Debug.LogError("PatrolComponent requires a component that implements IMovable.");
		}
		unitFacade.Patrol(route.worldPoints, randomize, reverse, lingerForSeconds);
	}

	private void OnDisable()
	{
		this.GetUnitFacade()?.StopPatrol();
	}
}
