using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Evasion", 1023)]
[ApexComponent("Steering")]
public class SteerForEvasionComponent : SteeringComponent
{
	[Tooltip("The target for evasion, i.e. the target that this unit will attempt to evade.")]
	public Transform target;

	[Tooltip("The radius within which the evading unit may start evading its target (set to 0 to always evade from target).")]
	public float awarenessRadius = 10f;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (target == null)
		{
			return;
		}
		Vector3 position = target.position;
		Vector3 position2 = input.unit.position;
		if (awarenessRadius > 0f && (position - position2).sqrMagnitude > awarenessRadius * awarenessRadius)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		IUnitFacade unitFacade = target.GetUnitFacade();
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
			output.desiredAcceleration = Flee(zero, input);
			output.maxAllowedSpeed = input.unit.maximumSpeed;
		}
	}

	public override void Stop()
	{
		target = null;
	}
}
