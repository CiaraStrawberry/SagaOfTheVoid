namespace Apex.Units;

public interface IGrouping<T> where T : IGroupable<T>
{
	int groupCount { get; }

	int memberCount { get; }

	TransientGroup<T> this[int index] { get; }

	void Add(T member);

	void Remove(T member);
}
