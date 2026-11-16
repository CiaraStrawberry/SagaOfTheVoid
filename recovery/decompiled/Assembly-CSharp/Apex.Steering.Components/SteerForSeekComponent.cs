using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Seek", 1028)]
[ApexComponent("Steering")]
public class SteerForSeekComponent : ArrivalBase
{
	[Tooltip("The target used for seeking")]
	public Transform target;

	[Tooltip("The radius within which the seeking unit may start seeking the target (set to 0 to always seek).")]
	public float awarenessRadius = 10f;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (!(target == null))
		{
			Vector3 position = target.position;
			if ((position - input.unit.position).sqrMagnitude < awarenessRadius * awarenessRadius || awarenessRadius <= 0f)
			{
				output.desiredAcceleration = Arrive(position, input);
			}
		}
	}

	public override void Stop()
	{
		target = null;
	}
}
