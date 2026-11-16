using System;
using Apex.Common;
using Apex.DataStructures;
using Apex.Units;
using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering.Components;

[RequireComponent(typeof(SteeringScanner))]
[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Unit Avoidance", 1030)]
[ApexComponent("Steering")]
public class SteerForUnitAvoidanceComponent : SteeringComponent, ISupportRuntimeStateChange
{
	[MinCheck(0.01f, label = "Radius Margin", tooltip = "A margin added to the unit's radius when it considers whether it overlaps with another unit.")]
	public float radiusMargin = 0.1f;

	[Label("Accumulate Avoid Vectors", "Whether to accumulate all avoid vectors (true), or only avoid the first perceived unit (false). If false, the SteeringScanner on this unit must sort units with distance. When true, groups generally form lanes when avoiding other groups. When false, groups generally interweave when avoiding other groups.")]
	public bool accumulateAvoidVectors;

	[RangeX(90.01f, 180f, label = "Head-On Collision Angle", tooltip = "The angle in degrees used for evaluating whether a collision is head-on, i.e if the angle between unit's velocities is greater than this value in degrees, the collision is considered head-on")]
	public float headOnCollisionAngle = 175f;

	[MinCheck(0.001f, label = "Minimum Avoid Vector Magnitude", tooltip = "A factor defining the minimum magnitude that an avoidance vector must have, otherwise it is ignored")]
	public float minimumAvoidVectorMagnitude = 0.25f;

	[Tooltip("When true this unit will attempt to avoid behind other units, instead of avoiding in front of them.")]
	public bool preventPassingInFront;

	[Tooltip("If true, draws the latest avoidance vector and the last avoided position as gizmos.")]
	public bool drawGizmos;

	private const float _collisionTimeFactor = 0.25f;

	[SerializeField]
	[AttributeProperty("Ignored Units", "Defines which types of units are ignored by this unit in relation to unit avoidance")]
	private int _ignoredUnits;

	private SteeringScanner _scanner;

	private float _fovReverseAngleCos;

	private float _omniAwareRadius;

	private float _cosAvoidAngle;

	private IUnitFacade _unitData;

	private Vector3 _selfCollisionPos;

	private Vector3 _lastAvoidVector;

	private Vector3 _lastAvoidPos;

	public AttributeMask ignoredUnits
	{
		get
		{
			return _ignoredUnits;
		}
		set
		{
			_ignoredUnits = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		_scanner = GetComponent<SteeringScanner>();
		if (_scanner == null)
		{
			Debug.LogError(gameObject.name + " is missing its SteeringScanner component");
		}
		_unitData = this.GetUnitFacade();
		float num = (float)Math.PI / 180f;
		_fovReverseAngleCos = Mathf.Cos((360f - _unitData.fieldOfView) / 2f * num);
		_omniAwareRadius = _unitData.radius * 2f;
		_cosAvoidAngle = Mathf.Cos(headOnCollisionAngle * num);
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		_selfCollisionPos = Vector3.zero;
		_lastAvoidVector = Vector3.zero;
		_lastAvoidPos = Vector3.zero;
		IIterable<IUnitFacade> units = _scanner.units;
		int count = units.count;
		if (count != 0)
		{
			_unitData = input.unit;
			Vector3 vector = Avoid(units, count, input.currentPlanarVelocity);
			if (!(vector.sqrMagnitude < minimumAvoidVectorMagnitude * minimumAvoidVectorMagnitude))
			{
				output.desiredAcceleration = (_lastAvoidVector = Vector3.ClampMagnitude(vector / Time.fixedDeltaTime, input.maxDeceleration));
			}
		}
	}

	private Vector3 Avoid(IIterable<IUnitFacade> units, int unitsLength, Vector3 currentVelocity)
	{
		Vector3 position = _unitData.position;
		Vector3 normalized = currentVelocity.normalized;
		Vector3 result = Vector3.zero;
		for (int i = 0; i < unitsLength; i++)
		{
			IUnitFacade unitFacade = units[i];
			if (!unitFacade.isAlive || (_ignoredUnits & (int)unitFacade.attributes) > 0 || (_unitData.transientGroup != null && object.ReferenceEquals(unitFacade.transientGroup, _unitData.transientGroup)) || unitFacade.determination < _unitData.determination)
			{
				continue;
			}
			Vector3 position2 = unitFacade.position;
			Vector3 vector = position2.DirToXZ(position);
			float magnitude = vector.magnitude;
			if (!(magnitude > _omniAwareRadius) || !(Vector3.Dot(normalized, vector / magnitude) > _fovReverseAngleCos))
			{
				float combinedRadius = _unitData.radius + unitFacade.radius + radiusMargin;
				Vector3 velocity = unitFacade.velocity;
				Vector3 avoidVector = GetAvoidVector(position, currentVelocity, normalized, _unitData, position2, velocity, unitFacade, combinedRadius);
				if (!accumulateAvoidVectors)
				{
					result = avoidVector;
					break;
				}
				result += avoidVector;
			}
		}
		return result;
	}

	private Vector3 GetAvoidVector(Vector3 selfPos, Vector3 currentVelocity, Vector3 normalVelocity, IUnitFacade unitData, Vector3 otherPos, Vector3 otherVelocity, IUnitFacade otherData, float combinedRadius)
	{
		Vector3 avoidDirectionVector = GetAvoidDirectionVector(selfPos, currentVelocity, otherPos, otherVelocity, combinedRadius);
		float magnitude = avoidDirectionVector.magnitude;
		if (magnitude == 0f)
		{
			return Vector3.zero;
		}
		float num = combinedRadius * 0.5f;
		if (num <= 0f)
		{
			return Vector3.zero;
		}
		Vector3 vector = avoidDirectionVector / magnitude;
		Vector3 vector2 = vector * num;
		float num2 = Vector3.Dot(vector, normalVelocity);
		if (num2 <= _cosAvoidAngle)
		{
			vector2 = new Vector3(vector2.z, vector2.y, 0f - vector2.x);
		}
		else if (preventPassingInFront && otherData.determination > unitData.determination && Vector3.Dot(otherVelocity, vector2) > 0f && Vector3.Dot(currentVelocity, otherVelocity) >= 0f)
		{
			vector2 = _selfCollisionPos.DirToXZ(selfPos);
		}
		float num3 = Mathf.Max(1f, selfPos.DirToXZ(_selfCollisionPos).magnitude);
		return vector2 * (currentVelocity.magnitude / num3);
	}

	private Vector3 GetAvoidDirectionVector(Vector3 selfPos, Vector3 currentVelocity, Vector3 otherPos, Vector3 otherVelocity, float combinedRadius)
	{
		float num = (currentVelocity.x - otherVelocity.x) * (currentVelocity.x - otherVelocity.x) + (currentVelocity.z - otherVelocity.z) * (currentVelocity.z - otherVelocity.z);
		float num2 = 2f * (selfPos.x - otherPos.x) * (currentVelocity.x - otherVelocity.x) + 2f * (selfPos.z - otherPos.z) * (currentVelocity.z - otherVelocity.z);
		float num3 = (selfPos.x - otherPos.x) * (selfPos.x - otherPos.x) + (selfPos.z - otherPos.z) * (selfPos.z - otherPos.z) - combinedRadius * combinedRadius;
		if (num == 0f)
		{
			return Vector3.zero;
		}
		float num4 = num2 * num2 - 4f * num * num3;
		if (num4 <= 0f)
		{
			return Vector3.zero;
		}
		float num5 = Mathf.Sqrt(num4);
		float num6 = 2f * num;
		float num7 = (0f - num2 + num5) / num6;
		float num8 = (0f - num2 - num5) / num6;
		if (num7 < 0f && num8 < 0f)
		{
			return Vector3.zero;
		}
		float num9 = 0f;
		if (num7 < 0f)
		{
			num9 = num8;
		}
		else
		{
			num9 = ((!(num8 < 0f)) ? Mathf.Min(num7, num8) : num7);
		}
		num9 += Mathf.Abs(num8 - num7) * 0.25f;
		Vector3 to = (_selfCollisionPos = selfPos + currentVelocity * num9);
		Vector3 vector = otherPos + otherVelocity * num9;
		_lastAvoidPos = otherPos;
		return vector.DirToXZ(to);
	}

	void ISupportRuntimeStateChange.ReevaluateState()
	{
		_cosAvoidAngle = Mathf.Cos(headOnCollisionAngle * ((float)Math.PI / 180f));
	}

	private void OnDrawGizmos()
	{
		if (!drawGizmos || _unitData == null || _unitData.gameObject == null)
		{
			return;
		}
		Renderer component = _unitData.gameObject.GetComponent<Renderer>();
		if (!(component == null) && !(component.material == null))
		{
			Color color = component.material.color;
			Gizmos.color = color;
			if (_lastAvoidVector.sqrMagnitude != 0f)
			{
				Gizmos.DrawLine(_unitData.position, _unitData.position + _lastAvoidVector);
			}
			if (_lastAvoidPos.sqrMagnitude != 0f)
			{
				Gizmos.DrawSphere(_selfCollisionPos, 0.25f);
			}
		}
	}
}
