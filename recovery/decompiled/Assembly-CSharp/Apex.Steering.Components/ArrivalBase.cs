using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering.Components;

public abstract class ArrivalBase : SteeringComponent
{
	[MinCheck(0f, tooltip = "The distance within which the unit will start to slow down for arrival.")]
	public float slowingDistance = 1.5f;

	[Tooltip("Determines whether the slowing distance is automatically calculated based on the unit's speed and deceleration capabilities.")]
	public bool autoCalculateSlowingDistance = true;

	[MinCheck(0.1f, tooltip = "The distance from the final destination where the unit will stop.")]
	public float arrivalDistance = 0.2f;

	[Tooltip("The algorithm used to slow the unit for arrival.")]
	public SlowingAlgorithm slowingAlgorithm = SlowingAlgorithm.Logarithmic;

	protected bool hasArrived { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (autoCalculateSlowingDistance)
		{
			slowingDistance = 0f;
		}
	}

	protected Vector3 Arrive(Vector3 destination, SteeringInput input)
	{
		return Arrive(destination, input, input.desiredSpeed);
	}

	protected Vector3 Arrive(Vector3 destination, SteeringInput input, float desiredSpeed)
	{
		Vector3 vector = input.unit.position.DirToXYZ(destination);
		float magnitude = vector.magnitude;
		float num = magnitude - arrivalDistance;
		hasArrived = num <= 0.01f;
		if (hasArrived)
		{
			if (autoCalculateSlowingDistance)
			{
				slowingDistance = 0f;
			}
			return -input.currentPlanarVelocity / Time.fixedDeltaTime;
		}
		if (autoCalculateSlowingDistance)
		{
			CalculateRequiredSlowingDistance(input);
		}
		float num2 = desiredSpeed;
		if (num < slowingDistance)
		{
			num2 = ((slowingAlgorithm != SlowingAlgorithm.Logarithmic) ? (num2 * (num / slowingDistance)) : (num2 * Mathf.Log10(9f / slowingDistance * num + 1f)));
		}
		Vector3 vector2 = vector / magnitude * Mathf.Max(num2, input.unit.minimumSpeed);
		float maxLength = ((!(vector2.sqrMagnitude < input.currentPlanarVelocity.sqrMagnitude)) ? input.maxAcceleration : input.maxDeceleration);
		vector2 = (vector2 - input.currentPlanarVelocity) / Time.fixedDeltaTime;
		return Vector3.ClampMagnitude(vector2, maxLength);
	}

	protected bool Arrive(Vector3 destination, float remainingDistance, SteeringInput input, SteeringOutput output)
	{
		float num = remainingDistance - arrivalDistance;
		hasArrived = num <= 0.01f;
		if (hasArrived)
		{
			return true;
		}
		Vector3 currentPlanarVelocity = input.currentPlanarVelocity;
		slowingDistance = currentPlanarVelocity.sqrMagnitude / (2f * input.maxDeceleration);
		float a = Mathf.Sqrt(2f * input.maxDeceleration * num);
		float num2 = Mathf.Min(a, input.desiredSpeed);
		Vector3 vector = input.unit.position.DirToXYZ(destination).normalized * num2;
		if (num2 < input.desiredSpeed)
		{
			output.desiredAcceleration = (vector - input.currentPlanarVelocity) / input.deltaTime;
		}
		else
		{
			output.desiredAcceleration = Vector3.ClampMagnitude((vector - input.currentPlanarVelocity) / input.deltaTime, input.maxAcceleration);
		}
		return false;
	}

	protected void CalculateRequiredSlowingDistance(SteeringInput input)
	{
		float magnitude = input.currentPlanarVelocity.magnitude;
		if (slowingAlgorithm == SlowingAlgorithm.Logarithmic)
		{
			slowingDistance = Mathf.Max(slowingDistance, Mathf.Ceil(magnitude * magnitude / (2f * input.maxDeceleration) * 1.1f) + 1f);
		}
		else
		{
			slowingDistance = Mathf.Max(slowingDistance, magnitude * magnitude / (2f * input.maxDeceleration));
		}
	}
}
