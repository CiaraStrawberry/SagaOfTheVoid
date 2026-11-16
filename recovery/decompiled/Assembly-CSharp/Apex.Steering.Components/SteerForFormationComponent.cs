using Apex.LoadBalancing;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Formation", 1025)]
[ApexComponent("Steering")]
public class SteerForFormationComponent : ArrivalBase, ILoadBalanced
{
	[Tooltip("If true, draws debug information including the desired formation position and the formation indices.")]
	public bool debugDraw;

	[MinCheck(0, label = "Sampled Cell Count", tooltip = "How many cells ahead the formation samples. If any sampled cells are blocked, the formation is dropped temporarily.")]
	public int sampledCellCount = 2;

	[MinCheck(0.01f, label = "Sampling Update Interval", tooltip = "How often cell sampling is done, load balanced, towards the formation position and towards the model unit.")]
	public float samplingUpdateInterval = 0.25f;

	[MinCheck(1f, label = "Max Formation Radius", tooltip = "The maximum distance at which this unit still responds to its formation. Outside of this radius (from the group) the formation is ignored.")]
	public float maxFormationRadius = 40f;

	[Tooltip("Controls whether the unit will drop its formation on arrival and instead allow other steering components to control the unit's behaviour.")]
	public bool dropFormationOnArrival;

	private IUnitFacade _unitData;

	private bool _stopped;

	private IGrid _grid;

	private IPositioned _formationPos;

	public bool repeat => enabled;

	protected override void OnStartAndEnable()
	{
		base.OnStartAndEnable();
		NavLoadBalancer.steering.Add(this, samplingUpdateInterval, delayFirstUpdate: true);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		NavLoadBalancer.steering.Remove(this);
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		input.unit.formationPos = null;
		_formationPos = null;
		_unitData = input.unit;
		_grid = input.grid;
		if (_grid == null || !(_unitData.transientGroup is DefaultSteeringTransientUnitGroup { count: not 0, modelUnit: { } modelUnit } defaultSteeringTransientUnitGroup) || _unitData.formationIndex < 0 || (_unitData.hasArrivedAtDestination && dropFormationOnArrival))
		{
			return;
		}
		_formationPos = defaultSteeringTransientUnitGroup.GetFormationPosition(_unitData.formationIndex);
		if (_formationPos != null && !_stopped)
		{
			float num = Mathf.Min(1.5f * input.desiredSpeed, _unitData.maximumSpeed);
			_unitData.formationPos = _formationPos;
			Vector3 desiredAcceleration = Arrive(_formationPos.position, input, num);
			if (modelUnit.hasArrivedAtDestination && hasArrived)
			{
				_unitData.hasArrivedAtDestination = true;
				return;
			}
			output.maxAllowedSpeed = num;
			output.desiredAcceleration = desiredAcceleration;
		}
	}

	public float? ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (_unitData == null || _unitData.transientGroup == null || _grid == null || _formationPos == null)
		{
			_stopped = true;
			return null;
		}
		TransientGroup<IUnitFacade> transientGroup = _unitData.transientGroup;
		IUnitFacade modelUnit = transientGroup.modelUnit;
		if (modelUnit == null || !modelUnit.isAlive)
		{
			_stopped = true;
			return null;
		}
		Vector3 position = _unitData.position;
		Vector3 position2 = modelUnit.position;
		Vector3 vector = position2 - position;
		if (vector.sqrMagnitude > maxFormationRadius * maxFormationRadius)
		{
			_stopped = true;
			return null;
		}
		Cell cell = _grid.GetCell(position, adjustToBounds: false);
		if (cell == null || !cell.IsWalkableWithClearance(_unitData))
		{
			_stopped = true;
			return null;
		}
		Cell cell2 = _grid.GetCell(_formationPos.position, adjustToBounds: false);
		if (cell2 == null || !cell2.IsWalkableFromWithClearance(cell, _unitData))
		{
			_stopped = true;
			return null;
		}
		float cellSize = _grid.cellSize;
		Cell neighbour = cell;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		int num = Mathf.CeilToInt(magnitude / cellSize);
		for (int i = 1; i <= num; i++)
		{
			float num2 = (float)i * cellSize;
			Vector3 position3 = position + vector2 * num2;
			Cell cell3 = _grid.GetCell(position3, adjustToBounds: false);
			if (cell3 == null || !cell3.IsWalkableFromWithClearance(neighbour, _unitData))
			{
				_stopped = true;
				return null;
			}
			neighbour = cell3;
		}
		Vector3 vector3 = _formationPos.position - position;
		Vector3 normalized = vector3.normalized;
		float num3 = vector3.magnitude + _unitData.radius;
		for (int j = 1; j <= sampledCellCount && (float)j * cellSize < num3; j++)
		{
			float num4 = (float)j * cellSize;
			Vector3 position4 = position + normalized * num4;
			Cell cell4 = _grid.GetCell(position4, adjustToBounds: false);
			if (cell4 == null || !cell4.IsWalkableFromWithClearance(cell, _unitData))
			{
				_stopped = true;
				return null;
			}
		}
		if (transientGroup is DefaultSteeringTransientUnitGroup { vectorField: not null } defaultSteeringTransientUnitGroup && defaultSteeringTransientUnitGroup.vectorField.GetFieldCellAtPos(cell).pathPortalIndex > 0)
		{
			_stopped = true;
			return null;
		}
		_stopped = false;
		return null;
	}

	private void OnDrawGizmos()
	{
		if (!debugDraw || _unitData == null || _unitData.formationPos == null)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(_unitData.formationPos.position, _unitData.radius);
		if (!(_unitData.transientGroup is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup))
		{
			return;
		}
		IPositioned finalFormationPosition = defaultSteeringTransientUnitGroup.GetFinalFormationPosition(_unitData.formationIndex);
		if (finalFormationPosition != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(finalFormationPosition.position, _unitData.radius);
			if (_unitData.hasArrivedAtDestination)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(_unitData.position, _unitData.radius);
			}
		}
	}
}
