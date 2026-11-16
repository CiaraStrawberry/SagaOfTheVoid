using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class StandardCell : Cell
{
	private class StandardCellFacory : ICellFactory
	{
		public Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		{
			return new StandardCell(parent, position, matrixPosX, matrixPosZ, blocked);
		}
	}

	public static readonly ICellFactory factory = new StandardCellFacory();

	private NeighbourPosition _heightBlockedFrom;

	public NeighbourPosition heightBlockedFrom
	{
		get
		{
			return _heightBlockedFrom;
		}
		set
		{
			_heightBlockedFrom = value;
		}
	}

	internal NeighbourPosition heightIntializedFrom { get; set; }

	public StandardCell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		: base(parent, position, matrixPosX, matrixPosZ, blocked)
	{
	}

	public override bool IsWalkableFromAllDirections(IUnitProperties unitProps)
	{
		if (!IsWalkableWithClearance(unitProps))
		{
			return false;
		}
		return _heightBlockedFrom == NeighbourPosition.None;
	}

	public override bool IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps)
	{
		if (!IsWalkable(unitProps.attributes))
		{
			return false;
		}
		NeighbourPosition relativePositionTo = neighbour.GetRelativePositionTo(this);
		return (_heightBlockedFrom & relativePositionTo) == 0;
	}

	public override bool IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps)
	{
		if (!IsWalkableWithClearance(unitProps))
		{
			return false;
		}
		NeighbourPosition relativePositionTo = neighbour.GetRelativePositionTo(this);
		return (_heightBlockedFrom & relativePositionTo) == 0;
	}
}
