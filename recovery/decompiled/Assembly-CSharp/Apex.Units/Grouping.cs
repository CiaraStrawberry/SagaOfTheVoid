using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Utilities;

namespace Apex.Units;

public class Grouping<T> : IGrouping<T> where T : IGroupable<T>
{
	private DynamicArray<TransientGroup<T>> _members;

	public int groupCount => _members.count;

	public int memberCount
	{
		get
		{
			int num = 0;
			int count = _members.count;
			for (int i = 0; i < count; i++)
			{
				num += _members[i].count;
			}
			return num;
		}
	}

	public TransientGroup<T> this[int index] => _members[index];

	public Grouping(int capacity)
	{
		_members = new DynamicArray<TransientGroup<T>>(capacity);
	}

	public Grouping(IEnumerable<TransientGroup<T>> members)
	{
		Ensure.ArgumentNotNull(members, "members");
		_members = new DynamicArray<TransientGroup<T>>(members);
	}

	public virtual void Add(TransientGroup<T> group)
	{
		_members.Add(group);
	}

	public virtual void Remove(TransientGroup<T> group)
	{
		_members.Remove(group);
	}

	public void Add(T member)
	{
		IGroupingStrategy<T> groupingStrategy = GroupingManager.GetGroupingStrategy<T>();
		if (groupingStrategy == null)
		{
			throw new InvalidOperationException("No strategy exists for this type of member.");
		}
		int count = _members.count;
		for (int i = 0; i < count; i++)
		{
			TransientGroup<T> transientGroup = _members[i];
			if (transientGroup.count != 0 && groupingStrategy.BelongsToSameGroup(transientGroup[0], member))
			{
				transientGroup.Add(member);
				return;
			}
		}
		TransientGroup<T> transientGroup2 = groupingStrategy.CreateGroup(1);
		transientGroup2.Add(member);
		Add(transientGroup2);
	}

	public void Remove(T member)
	{
		IGroupingStrategy<T> groupingStrategy = GroupingManager.GetGroupingStrategy<T>();
		if (groupingStrategy == null)
		{
			throw new InvalidOperationException("No strategy exists for this type of member.");
		}
		int count = _members.count;
		for (int i = 0; i < count; i++)
		{
			TransientGroup<T> transientGroup = _members[i];
			if (transientGroup.count != 0 && groupingStrategy.BelongsToSameGroup(transientGroup[0], member))
			{
				transientGroup.Remove(member);
				break;
			}
		}
	}
}
