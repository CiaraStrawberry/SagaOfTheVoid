using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering;

public abstract class SteeringComponent : ExtendedMonoBehaviour, ISteeringBehaviour
{
	[Tooltip("The weight this component's input will have in relation to other steering components.")]
	public float weight = 1f;

	[MinCheck(0, tooltip = "The priority of this steering behaviour relative to others. Only behaviours with the highest priority will influence the steering of the unit, provided they have any steering output")]
	public int priority;

	int ISteeringBehaviour.priority => priority;

	public virtual void Stop()
	{
	}

	public void GetSteering(SteeringInput input, SteeringOutput output)
	{
		GetDesiredSteering(input, output);
		if (output.hasOutput)
		{
			output.desiredAcceleration *= weight;
		}
	}

	public abstract void GetDesiredSteering(SteeringInput input, SteeringOutput output);

	protected virtual void Awake()
	{
		this.WarnIfMultipleInstances();
	}

	protected override void OnStartAndEnable()
	{
		SteerableUnitComponent component = GetComponent<SteerableUnitComponent>();
		component.RegisterSteeringBehavior(this);
	}

	protected virtual void OnDisable()
	{
		SteerableUnitComponent component = GetComponent<SteerableUnitComponent>();
		component.UnregisterSteeringBehavior(this);
	}

	protected Vector3 Seek(Vector3 destination, SteeringInput input)
	{
		return Seek(input.unit.position, destination, input);
	}

	protected Vector3 Seek(Vector3 destination, SteeringInput input, float maxAcceleration)
	{
		return Seek(input.unit.position, destination, input, maxAcceleration);
	}

	protected Vector3 Seek(Vector3 position, Vector3 destination, SteeringInput input)
	{
		return Seek(position, destination, input, input.maxAcceleration);
	}

	protected Vector3 Seek(Vector3 position, Vector3 destination, SteeringInput input, float maxAcceleration)
	{
		Vector3 vector = position.DirToXZ(destination).normalized * input.desiredSpeed;
		return Vector3.ClampMagnitude((vector - input.currentPlanarVelocity) / input.deltaTime, maxAcceleration);
	}

	protected Vector3 Flee(Vector3 from, SteeringInput input)
	{
		return Seek(from, input.unit.transform.position, input);
	}

	protected Vector3 Flee(Vector3 position, Vector3 from, SteeringInput input)
	{
		return Seek(from, position, input);
	}

	protected Vector3 Arrive(SteeringInput input)
	{
		return Vector3.ClampMagnitude(-input.currentPlanarVelocity / Time.fixedDeltaTime, input.maxDeceleration);
	}

	protected Vector3 Arrive(float timeToTarget, SteeringInput input)
	{
		timeToTarget = Mathf.Max(timeToTarget, Time.fixedDeltaTime);
		return Vector3.ClampMagnitude(-input.currentPlanarVelocity / timeToTarget, input.maxDeceleration);
	}
}
