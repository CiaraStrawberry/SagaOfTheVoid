using Apex.WorldGeometry;

namespace Apex.PathFinding;

public class DefaultCellCostStrategy : ICellCostStrategy
{
	public int GetCellCost(IGridCell cell, object unitProperties)
	{
		return cell.cost;
	}
}
