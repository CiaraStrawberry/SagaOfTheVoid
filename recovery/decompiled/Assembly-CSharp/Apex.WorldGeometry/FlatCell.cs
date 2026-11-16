using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class FlatCell : Cell
{
	private class FlatCellFacory : ICellFactory
	{
		public Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		{
			return new FlatCell(parent, position, matrixPosX, matrixPosZ, blocked);
		}
	}

	public static readonly ICellFactory factory = new FlatCellFacory();

	public FlatCell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		: base(parent, position, matrixPosX, matrixPosZ, blocked)
	{
	}

	public override bool IsWalkableFromAllDirections(IUnitProperties unitProps)
	{
		return IsWalkableWithClearance(unitProps);
	}

	public override bool IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps)
	{
		return IsWalkable(unitProps.attributes);
	}

	public override bool IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps)
	{
		return IsWalkableWithClearance(unitProps);
	}
}
