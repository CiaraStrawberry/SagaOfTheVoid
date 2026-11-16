using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering;

public abstract class OrientationComponent : ExtendedMonoBehaviour, IOrientationBehaviour
{
	[MinCheck(0, tooltip = "The priority of this orientation behaviour relative to others. Only the behaviour with the highest priority will influence the orientation of the unit.")]
	public int priority;

	[RangeX(0f, 1f, tooltip = "The distance within which the unit will start to slow down its rotation speed to smoothly 'arrive' at the designated rotation")]
	public float slowingDistance = 0.8f;

	[Tooltip("The algorithm used to slow the unit's rotation")]
	public SlowingAlgorithm slowingAlgorithm = SlowingAlgorithm.Logarithmic;

	int IOrientationBehaviour.priority => priority;

	protected override void OnStartAndEnable()
	{
		SteerableUnitComponent component = GetComponent<SteerableUnitComponent>();
		if (component == null)
		{
			Debug.LogError(gameObject.name + " : A SteerableUnitComponent is required on all units");
		}
		component.RegisterOrientationBehavior(this);
	}

	protected virtual void OnDisable()
	{
		SteerableUnitComponent component = GetComponent<SteerableUnitComponent>();
		if (component != null)
		{
			component.UnregisterOrientationBehavior(this);
		}
	}

	public abstract void GetOrientation(SteeringInput input, OrientationOutput output);

	protected float GetAngularAcceleration(Vector3 targetOrientation, SteeringInput input)
	{
		float num = Vector3.Dot(input.unit.forward, targetOrientation);
		if (num > 0.99999f)
		{
			return (0f - input.currentAngularSpeed) / Time.fixedDeltaTime;
		}
		float num2 = input.desiredAngularSpeed;
		float num3 = 1f - slowingDistance;
		if (num > slowingDistance)
		{
			num2 = ((slowingAlgorithm != SlowingAlgorithm.Logarithmic) ? (num2 * ((1f - num) / num3)) : (num2 * Mathf.Log10(9f / num3 * (1f - num) + 1f)));
			if (num2 < 0.1f)
			{
				return 0f;
			}
		}
		float maxAngularAcceleration = input.maxAngularAcceleration;
		num2 = (num2 - input.currentAngularSpeed) / Time.fixedDeltaTime;
		return Mathf.Clamp(num2, 0f - maxAngularAcceleration, maxAngularAcceleration);
	}
}
