using Apex.Units;
using Apex.Utilities;

namespace Apex.Steering.Messages;

public class UnitsSelectedMessage
{
	public IGrouping<IUnitFacade> selectedUnits { get; private set; }

	public UnitsSelectedMessage(IGrouping<IUnitFacade> units)
	{
		Ensure.ArgumentNotNull(units, "units");
		selectedUnits = units;
	}
}
