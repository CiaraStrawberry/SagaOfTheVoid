using System;
using Apex.Common;
using Apex.DataStructures;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[RequireComponent(typeof(SteeringScanner))]
[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Separation", 1029)]
[ApexComponent("Steering")]
public class SteerForSeparationComponent : ArrivalBase
{
	public enum SeparationMode
	{
		AvoidCollision,
		MaintainFullSeparation,
		Disable
	}

	[MinCheck(1, tooltip = "The maximum surrounding units to consider when calculating the separation vector. This is only honoured if the scanner sorts units by distance.")]
	public int maximumUnitsToConsider = 10;

	[MinCheck(0.01f, label = "Separation Distance", tooltip = "The distance at which units are at rest with each other, i.e. if the distance between two units center + radius is less than this value, they will separate.")]
	public float separationDistance = 0.5f;

	[MinCheck(0.0001f, label = "Minimum Force Magnitude", tooltip = "A factor used for ignoring separation vectors of less magnitude than this value.")]
	public float minimumForceMagnitude = 0.05f;

	[RangeX(0f, 1f, tooltip = "How much force is applied on separation vectors - as a percentage of the maximum acceleration, when not swarming (arrival).")]
	public float separationStrength = 0.25f;

	[Tooltip("Controls the behaviour when the unit is next to blocked cells. Keeping full separation may result in unwanted oscillation.")]
	public SeparationMode blockedNeighboursBehaviour;

	[Tooltip("If true, draws the last separation vector as a Gizmo.")]
	public bool drawGizmos;

	private Vector3 _lastSeparationVector;

	private SteeringScanner _scanner;

	[SerializeField]
	[AttributeProperty("Ignored Units", "Defines which types of units are ignored by this unit in relation to separation")]
	private int _ignoredUnits;

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
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		_lastSeparationVector = Vector3.zero;
		IUnitFacade unit = input.unit;
		Vector3 position = unit.position;
		float num = separationDistance;
		if (blockedNeighboursBehaviour != SeparationMode.MaintainFullSeparation && input.grid != null)
		{
			Cell cell = input.grid.GetCell(position, adjustToBounds: true);
			if (cell != null)
			{
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i == 0 && j == 0)
						{
							continue;
						}
						Cell neighbour = cell.GetNeighbour(i, j);
						if (neighbour != null && !neighbour.IsWalkableFromWithClearance(cell, unit))
						{
							if (blockedNeighboursBehaviour == SeparationMode.Disable)
							{
								return;
							}
							num = 0f;
							j = (i = 2);
						}
					}
				}
			}
		}
		Vector3 zero = Vector3.zero;
		int num2 = 0;
		IIterable<IUnitFacade> units = _scanner.units;
		int num3 = ((!_scanner.sortUnitsWithDistance) ? units.count : Math.Min(units.count, maximumUnitsToConsider));
		for (int k = 0; k < num3; k++)
		{
			IUnitFacade unitFacade = units[k];
			if (!unitFacade.isAlive || (_ignoredUnits & (int)unitFacade.attributes) > 0 || (!unit.hasArrivedAtDestination && (unit.transientGroup == null || !object.ReferenceEquals(unit.transientGroup, unitFacade.transientGroup) || unitFacade.hasArrivedAtDestination)) || unitFacade.determination < unit.determination)
			{
				continue;
			}
			Vector3 position2 = unitFacade.position;
			Vector3 vector = position - position2;
			float num4 = num + unit.radius + unitFacade.radius;
			if (!(vector.sqrMagnitude >= num4 * num4))
			{
				num2++;
				float magnitude = vector.magnitude;
				if (magnitude > 0f)
				{
					zero += vector / magnitude * (num4 - magnitude);
				}
			}
		}
		if (num2 != 0)
		{
			zero /= (float)num2;
			if (!(zero.sqrMagnitude <= minimumForceMagnitude * minimumForceMagnitude))
			{
				float maxAcceleration = ((!unit.hasArrivedAtDestination) ? (separationStrength * input.maxAcceleration) : input.maxAcceleration);
				output.desiredAcceleration = (_lastSeparationVector = Seek(position + zero, input, maxAcceleration));
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos && _lastSeparationVector.sqrMagnitude != 0f)
		{
			Vector3 position = transform.position;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(position, position + _lastSeparationVector);
		}
	}
}
