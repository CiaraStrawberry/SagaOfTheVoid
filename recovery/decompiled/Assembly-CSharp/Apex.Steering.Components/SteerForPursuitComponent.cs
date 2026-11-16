using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Pursuit", 1027)]
[ApexComponent("Steering")]
public class SteerForPursuitComponent : SteeringComponent
{
	[Tooltip("The target used for pursuit, i.e. the target that this unit will attempt to pursue")]
	public Transform target;

	[Tooltip("The radius within which the pursuing unit may start pursuing the target (set to 0 to always pursue).")]
	public float awarenessRadius = 10f;

	[Tooltip("The radius at which the unit will slow to a stop and move no further towards the target.")]
	public float stopRadius = 1.5f;

	[Tooltip("The time over which to stop as permitted by deceleration capabilities.")]
	public float stopTimeFrame = 0.1f;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (target == null)
		{
			return;
		}
		Vector3 position = target.position;
		Vector3 position2 = input.unit.position;
		float sqrMagnitude = (position - position2).sqrMagnitude;
		if (awarenessRadius > 0f && sqrMagnitude > awarenessRadius * awarenessRadius)
		{
			return;
		}
		if (sqrMagnitude <= stopRadius * stopRadius)
		{
			output.desiredAcceleration = Arrive(stopTimeFrame, input);
			return;
		}
		Vector3 zero = Vector3.zero;
		IUnitFacade unitFacade = target.GetUnitFacade(createIfMissing: false);
		if (unitFacade == null)
		{
			zero = position;
		}
		else
		{
			float magnitude = (position - position2).magnitude;
			float magnitude2 = unitFacade.velocity.magnitude;
			float num = 0.1f;
			if (magnitude2 > 0f)
			{
				num = magnitude / magnitude2 / 2f;
			}
			zero = position + unitFacade.velocity * num;
		}
		if (zero.sqrMagnitude != 0f)
		{
			output.desiredAcceleration = Seek(zero, input);
			output.maxAllowedSpeed = input.unit.maximumSpeed;
		}
	}

	public override void Stop()
	{
		target = null;
	}
}
