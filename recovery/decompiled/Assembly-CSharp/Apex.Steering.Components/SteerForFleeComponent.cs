using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Flee", 1024)]
[ApexComponent("Steering")]
public class SteerForFleeComponent : SteeringComponent
{
	[Tooltip("The target used for fleeing, i.e. the target that this unit will attempt to flee.")]
	public Transform target;

	[Tooltip("The radius within which the fleeing unit may start fleeing away from the target (set to 0 to always flee from target)")]
	public float awarenessRadius = 10f;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (!(target == null))
		{
			Vector3 position = target.position;
			if ((position - input.unit.position).sqrMagnitude < awarenessRadius * awarenessRadius || awarenessRadius <= 0f)
			{
				output.desiredAcceleration = Flee(position, input);
				output.maxAllowedSpeed = input.unit.maximumSpeed;
			}
		}
	}

	public override void Stop()
	{
		target = null;
	}
}
