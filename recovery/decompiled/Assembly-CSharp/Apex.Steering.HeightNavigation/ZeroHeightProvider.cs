using Apex.Units;
using Apex.WorldGeometry;

namespace Apex.Steering.HeightNavigation;

public class ZeroHeightProvider : IUnitHeightProvider
{
	public float GetHeightDelta(SteeringInput input)
	{
		IGrid grid = input.grid;
		if (grid == null)
		{
			return 0f;
		}
		IUnitFacade unit = input.unit;
		return grid.origin.y - (unit.position.y - unit.baseToPositionOffset);
	}
}
