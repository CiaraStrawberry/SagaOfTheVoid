using Apex.PathFinding.MoveCost;

namespace Apex.PathFinding;

public interface IMoveCostFactory
{
	IMoveCost CreateMoveCostProvider();
}
