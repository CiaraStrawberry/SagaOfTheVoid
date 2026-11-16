using System.Collections;

namespace Apex.Units;

public class DefaultUnitGroupingStrategy : IGroupingStrategy<IUnitFacade>
{
	public IGrouping<IUnitFacade> CreateGrouping(IEnumerable members)
	{
		return new DefaultTransientUnitGroup(members.ToUnitFacades());
	}

	public TransientGroup<IUnitFacade> CreateGroup(int capacity)
	{
		return new DefaultTransientUnitGroup(capacity);
	}

	public bool BelongsToSameGroup(IUnitFacade lhs, IUnitFacade rhs)
	{
		return true;
	}
}
