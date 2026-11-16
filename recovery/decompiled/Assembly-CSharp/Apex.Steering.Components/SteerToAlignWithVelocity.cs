using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to Align with Velocity", 1033)]
[ApexComponent("Steering")]
public class SteerToAlignWithVelocity : OrientationComponent
{
	public float minimumSpeedToTurn = 0.01f;

	public bool alignWithElevation;

	public override void GetOrientation(SteeringInput input, OrientationOutput output)
	{
		if (input.currentPlanarVelocity.sqrMagnitude < minimumSpeedToTurn * minimumSpeedToTurn)
		{
			output.desiredAngularAcceleration = (0f - input.currentAngularSpeed) / Time.fixedDeltaTime;
		}
		else
		{
			output.desiredAngularAcceleration = GetAngularAcceleration(output.desiredOrientation = input.currentFullVelocity.normalized, input);
		}
	}

	public void CloneFrom(SteerToAlignWithVelocity steerToAlign)
	{
		priority = steerToAlign.priority;
		slowingDistance = steerToAlign.slowingDistance;
		slowingAlgorithm = steerToAlign.slowingAlgorithm;
		minimumSpeedToTurn = steerToAlign.minimumSpeedToTurn;
		alignWithElevation = steerToAlign.alignWithElevation;
	}
}
