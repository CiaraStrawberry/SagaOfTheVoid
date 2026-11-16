using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to flee obstacle", 1035)]
[ApexComponent("Steering")]
public class FleeObstacleComponent : SteeringComponent
{
	public int fleeMaxRadius = 5;

	private Cell _targetCell;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		IGrid grid = input.grid;
		if (grid == null)
		{
			_targetCell = null;
			return;
		}
		IUnitFacade unit = input.unit;
		Vector3 position = unit.position;
		Cell cell = grid.GetCell(position, adjustToBounds: true);
		if (cell.IsWalkableWithClearance(unit))
		{
			if (_targetCell == null)
			{
				return;
			}
			Vector3 vector = position.DirToXZ(cell.position);
			float num = grid.cellSize / 2f - unit.radius;
			if (!(vector.sqrMagnitude > num * num))
			{
				_targetCell = null;
				return;
			}
			_targetCell = cell;
		}
		else if (_targetCell == null || _targetCell == cell)
		{
			_targetCell = grid.GetNearestWalkableCell(position, position, requireWalkableFromPosition: true, fleeMaxRadius, unit);
		}
		output.desiredAcceleration = Seek(_targetCell.position, input);
		output.maxAllowedSpeed = input.unit.maximumSpeed;
	}
}
