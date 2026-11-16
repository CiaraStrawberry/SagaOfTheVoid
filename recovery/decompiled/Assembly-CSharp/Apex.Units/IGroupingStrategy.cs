using System.Collections;

namespace Apex.Units;

public interface IGroupingStrategy<T> where T : IGroupable<T>
{
	IGrouping<T> CreateGrouping(IEnumerable members);

	TransientGroup<T> CreateGroup(int capacity);

	bool BelongsToSameGroup(T lhs, T rhs);
}
