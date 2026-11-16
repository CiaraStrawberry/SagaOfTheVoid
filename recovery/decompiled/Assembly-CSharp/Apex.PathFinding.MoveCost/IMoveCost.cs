using Apex.WorldGeometry;

namespace Apex.PathFinding.MoveCost;

public interface IMoveCost
{
	int baseMoveCost { get; }

	int GetMoveCost(IPositioned current, IPositioned other);

	int GetHeuristic(IPositioned current, IPositioned goal);
}
