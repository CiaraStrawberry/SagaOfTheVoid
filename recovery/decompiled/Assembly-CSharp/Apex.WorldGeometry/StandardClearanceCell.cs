using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class StandardClearanceCell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked) : StandardCell(parent, position, matrixPosX, matrixPosZ, blocked), IHaveClearance
{
	private class StandardClearanceCellFactory : ICellFactory
	{
		public Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		{
			return new StandardClearanceCell(parent, position, matrixPosX, matrixPosZ, blocked);
		}
	}

	public new static readonly ICellFactory factory = new StandardClearanceCellFactory();

	private float _clearance = float.MaxValue;

	public float clearance
	{
		get
		{
			return _clearance;
		}
		set
		{
			_clearance = value;
		}
	}

	public override bool IsWalkableWithClearance(IUnitProperties unitProps)
	{
		if (unitProps.radius > _clearance)
		{
			return false;
		}
		return base.IsWalkableWithClearance(unitProps);
	}
}
