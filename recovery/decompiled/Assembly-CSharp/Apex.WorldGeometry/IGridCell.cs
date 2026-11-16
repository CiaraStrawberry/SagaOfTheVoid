using Apex.Common;
using Apex.DataStructures;
using Apex.Units;

namespace Apex.WorldGeometry;

public interface IGridCell : IPositioned
{
	CellMatrix parent { get; }

	int matrixPosX { get; }

	int matrixPosZ { get; }

	int cost { get; }

	NeighbourPosition neighbours { get; }

	bool isPermanentlyBlocked { get; set; }

	bool IsWalkable(AttributeMask mask);

	bool IsWalkableWithClearance(IUnitProperties unitProps);

	bool IsWalkableFromAllDirections(IUnitProperties unitProps);

	bool IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps);

	bool IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps);

	NeighbourPosition GetRelativePositionTo(IGridCell other);

	VectorXZ GetDirectionTo(IGridCell other);

	Cell GetNeighbour(VectorXZ offset);

	Cell GetNeighbour(int dx, int dz);

	int GetNeighbours(Cell[] neighboursBuffer);
}
