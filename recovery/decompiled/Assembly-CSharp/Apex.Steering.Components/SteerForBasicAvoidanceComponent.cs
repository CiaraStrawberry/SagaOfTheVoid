using System;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("")]
[ApexComponent("Steering")]
public class SteerForBasicAvoidanceComponent : SteeringComponent
{
	private BasicScanner _scanner;

	private float _minSpeedSquared;

	private float _fovReverseAngleCos;

	private float _omniAwareRadius;

	protected override void Awake()
	{
		base.Awake();
		IUnitFacade unitFacade = this.GetUnitFacade();
		_scanner = GetComponent<BasicScanner>();
		_fovReverseAngleCos = Mathf.Cos((360f - unitFacade.fieldOfView) / 2f * ((float)Math.PI / 180f));
		_omniAwareRadius = unitFacade.radius * 2f;
		_minSpeedSquared = unitFacade.minimumSpeed * unitFacade.minimumSpeed;
		int num = 1 << gameObject.layer;
		if ((num & (int)Layers.units) == 0)
		{
			Debug.LogWarning(gameObject.name + " : For local avoidance to work, your unit must be put in the unit layer(s) mapped on the Game World.");
		}
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (input.currentPlanarVelocity.sqrMagnitude < _minSpeedSquared)
		{
			return;
		}
		Collider[] units = _scanner.Units;
		float num = 0f;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		foreach (Collider collider in units)
		{
			if (collider.Equals(null) || collider.Equals(input.unit.collider))
			{
				continue;
			}
			IUnitFacade unitFacade = collider.GetUnitFacade();
			Vector3 vector;
			if (unitFacade == null)
			{
				vector = collider.transform.position;
			}
			else
			{
				Vector3 position = unitFacade.position;
				float magnitude = (position - input.unit.position).magnitude;
				float magnitude2 = unitFacade.velocity.magnitude;
				float num3 = 0.1f;
				if (magnitude2 > 0f)
				{
					num3 = magnitude / magnitude2 / 2f;
				}
				vector = position + unitFacade.velocity * num3;
			}
			Vector3 vector2 = input.unit.position - vector;
			float magnitude3 = vector2.magnitude;
			if (!(magnitude3 > _omniAwareRadius) || !(Vector3.Dot(input.unit.forward, vector2 / magnitude3) > _fovReverseAngleCos))
			{
				float num4 = 1f / (magnitude3 * magnitude3);
				num2 += num4;
				if (num4 > num)
				{
					num = num4;
				}
				zero += vector2 * num4;
			}
		}
		if (num > 0f)
		{
			zero /= num2 / num;
			output.desiredAcceleration = Seek(input.unit.position + zero, input);
		}
	}
}
