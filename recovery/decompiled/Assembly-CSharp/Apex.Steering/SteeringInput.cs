using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public sealed class SteeringInput
{
	public float gravity;

	public float maxAcceleration;

	public float maxDeceleration;

	public float maxAngularAcceleration;

	public Vector3 currentPlanarVelocity;

	public Vector3 currentSpatialVelocity;

	public Vector3 currentFullVelocity;

	public float currentAngularSpeed;

	public float desiredSpeed;

	public float desiredAngularSpeed;

	public IGrid grid;

	public IUnitFacade unit;

	public float deltaTime;
}
