using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Units;

public class DefaultSteeringUnitGroupingStrategy : IGroupingStrategy<IUnitFacade>
{
	public IGrouping<IUnitFacade> CreateGrouping(IEnumerable members)
	{
		IEnumerable<IUnitFacade> enumerable = members.ToUnitFacades();
		if (enumerable.Count() == 1)
		{
			return new DefaultTransientUnitGroup(enumerable);
		}
		return new DefaultSteeringTransientUnitGroup(enumerable);
	}

	public TransientGroup<IUnitFacade> CreateGroup(int capacity)
	{
		return new DefaultSteeringTransientUnitGroup(capacity);
	}

	public bool BelongsToSameGroup(IUnitFacade lhs, IUnitFacade rhs)
	{
		return true;
	}
}
