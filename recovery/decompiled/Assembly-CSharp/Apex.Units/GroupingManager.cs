using System;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Units;

public static class GroupingManager
{
	private static readonly IDictionary<Type, object> _groupingStrategies = new Dictionary<Type, object>();

	public static void RegisterGroupingStrategy<T>(IGroupingStrategy<T> strat) where T : IGroupable<T>
	{
		_groupingStrategies[typeof(T)] = strat;
	}

	public static IGroupingStrategy<T> GetGroupingStrategy<T>() where T : IGroupable<T>
	{
		if (_groupingStrategies.TryGetValue(typeof(T), out var value))
		{
			return (IGroupingStrategy<T>)value;
		}
		return null;
	}

	public static IGrouping<T> CreateGrouping<T>(params T[] members) where T : IGroupable<T>
	{
		return CreateGrouping((IEnumerable<T>)members);
	}

	public static IGrouping<T> CreateGrouping<T>(IEnumerable<T> members) where T : IGroupable<T>
	{
		return GetGroupingStrategy<T>()?.CreateGrouping(members);
	}

	public static TransientGroup<T> CreateGroup<T>(int capacity) where T : IGroupable<T>
	{
		return GetGroupingStrategy<T>()?.CreateGroup(capacity);
	}

	public static TransientGroup<T> CreateGroup<T>(IEnumerable<T> members) where T : IGroupable<T>
	{
		IGroupingStrategy<T> groupingStrategy = GetGroupingStrategy<T>();
		if (groupingStrategy == null)
		{
			return null;
		}
		List<T> list = members.ToList();
		TransientGroup<T> transientGroup = groupingStrategy.CreateGroup(list.Count);
		foreach (T item in list)
		{
			transientGroup.Add(item);
		}
		return transientGroup;
	}
}
