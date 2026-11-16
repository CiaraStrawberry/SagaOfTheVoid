using Apex.DataStructures;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Blocked Cell Repulsion", 1021)]
[ApexComponent("Steering")]
public class SteerForBlockedCellRepulsionComponent : SteeringComponent
{
	[MinCheck(0.01f, label = "Radius Margin", tooltip = "A margin added to the unit's radius when it considers whether it overlaps with a blocked cell")]
	public float radiusMargin = 0.1f;

	[MinCheck(1f, label = "Repulsion Strength", tooltip = "A factor used for multiplying the repulsion vector, i.e. this factor explicitely sets the repulsion vector magnitude")]
	public float repulsionStrength = 200f;

	[Tooltip("If true, draws debug information, including the last cell repulsion vector.")]
	public bool drawGizmos;

	private Vector3 _lastAvoidVector;

	private IUnitFacade _unitData;

	private DynamicArray<Cell> _neighbours;

	protected override void Start()
	{
		base.Start();
		_neighbours = new DynamicArray<Cell>(4);
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		_lastAvoidVector = Vector3.zero;
		IGrid grid = input.grid;
		if (grid == null)
		{
			return;
		}
		_unitData = input.unit;
		if (!_unitData.isGrounded)
		{
			return;
		}
		Vector3 position = _unitData.position;
		Cell cell = grid.GetCell(position);
		if (cell == null || !cell.IsWalkableWithClearance(_unitData))
		{
			return;
		}
		_neighbours.Clear();
		_neighbours.Add(cell.GetNeighbour(-1, 0));
		_neighbours.Add(cell.GetNeighbour(0, -1));
		_neighbours.Add(cell.GetNeighbour(1, 0));
		_neighbours.Add(cell.GetNeighbour(0, 1));
		float num = _unitData.radius + radiusMargin + grid.cellSize / 2f;
		float num2 = num * num;
		Vector3 zero = Vector3.zero;
		int count = _neighbours.count;
		for (int i = 0; i < count; i++)
		{
			Cell cell2 = _neighbours[i];
			if (cell2 != null)
			{
				Vector3 vector = cell2.position.DirToXZ(position);
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude < num2 && !cell2.IsWalkableFromWithClearance(cell, _unitData))
				{
					zero += vector;
				}
			}
		}
		if (zero.sqrMagnitude != 0f)
		{
			output.desiredAcceleration = (_lastAvoidVector = zero.normalized * repulsionStrength);
		}
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos && _lastAvoidVector.sqrMagnitude != 0f)
		{
			Vector3 position = _unitData.position;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(position, position + _lastAvoidVector);
		}
	}
}
