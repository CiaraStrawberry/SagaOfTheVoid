using Apex.DataStructures;
using Apex.Units;
using Apex.WorldGeometry;

namespace Apex.PathFinding;

public interface IPathNode : IGridCell, IPositioned
{
	int g { get; set; }

	int h { get; set; }

	int f { get; set; }

	bool isClosed { get; set; }

	IPathNode predecessor { get; set; }

	bool hasVirtualNeighbour { get; }

	void GetWalkableNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps, bool cornerCuttingAllowed, bool preventDiagonalMoves);

	bool TryGetWalkableNeighbour(int dx, int dz, IUnitProperties unitProps, DynamicArray<IPathNode> neighbours);

	void GetVirtualNeighbours(DynamicArray<IPathNode> neighbours, IUnitProperties unitProps);

	void RegisterVirtualNeighbour(IPathNode neighbour);

	void UnregisterVirtualNeighbour(IPathNode neighbour);
}
