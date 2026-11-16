using Apex.WorldGeometry;

namespace Apex.PathFinding;

public interface ICellCostStrategy
{
	int GetCellCost(IGridCell cell, object unitProperties);
}
