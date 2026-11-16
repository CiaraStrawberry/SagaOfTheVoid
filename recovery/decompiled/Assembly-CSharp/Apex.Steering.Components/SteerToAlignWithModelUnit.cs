using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to Align With Model Unit", 1032)]
[ApexComponent("Steering")]
public class SteerToAlignWithModelUnit : OrientationComponent
{
	public override void GetOrientation(SteeringInput input, OrientationOutput output)
	{
		if (input.currentPlanarVelocity.sqrMagnitude > 0f)
		{
			return;
		}
		TransientGroup<IUnitFacade> transientGroup = input.unit.transientGroup;
		if (transientGroup != null)
		{
			IUnitFacade modelUnit = transientGroup.modelUnit;
			if (modelUnit != null)
			{
				output.desiredAngularAcceleration = GetAngularAcceleration(output.desiredOrientation = modelUnit.forward, input);
			}
		}
	}
}
