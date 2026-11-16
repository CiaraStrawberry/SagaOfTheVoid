using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class FlatClearanceCell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked) : FlatCell(parent, position, matrixPosX, matrixPosZ, blocked), IHaveClearance
{
	private class FlatClearanceCellFactory : ICellFactory
	{
		public Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		{
			return new FlatClearanceCell(parent, position, matrixPosX, matrixPosZ, blocked);
		}
	}

	public new static readonly ICellFactory factory = new FlatClearanceCellFactory();

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
