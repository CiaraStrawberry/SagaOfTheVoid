using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to Look At", 1036)]
[ApexComponent("Steering")]
public class SteerToLookAt : OrientationComponent
{
	public override void GetOrientation(SteeringInput input, OrientationOutput output)
	{
		if (input.unit.lookAt.HasValue)
		{
			Vector3 vector = (output.desiredOrientation = (input.unit.lookAt.Value - input.unit.position).OnlyXZ());
			output.desiredAngularAcceleration = GetAngularAcceleration(vector.normalized, input);
		}
	}

	public void CloneFrom(SteerToLookAt steerToLook)
	{
		priority = steerToLook.priority;
		slowingDistance = steerToLook.slowingDistance;
		slowingAlgorithm = steerToLook.slowingAlgorithm;
	}
}
