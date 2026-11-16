using System;
using System.Collections;
using System.Collections.Generic;

namespace Devdog.General.ThirdParty.UniLinq;

public class Lookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, ILookup<TKey, TElement>, IEnumerable
{
	private IGrouping<TKey, TElement> nullGrouping;

	private Dictionary<TKey, IGrouping<TKey, TElement>> groups;

	public int Count => (nullGrouping != null) ? (groups.Count + 1) : groups.Count;

	public IEnumerable<TElement> this[TKey key]
	{
		get
		{
			if (key == null && nullGrouping != null)
			{
				return nullGrouping;
			}
			if (key != null && groups.TryGetValue(key, out var value))
			{
				return value;
			}
			return new TElement[0];
		}
	}

	internal Lookup(Dictionary<TKey, List<TElement>> lookup, IEnumerable<TElement> nullKeyElements)
	{
		groups = new Dictionary<TKey, IGrouping<TKey, TElement>>(lookup.Comparer);
		foreach (KeyValuePair<TKey, List<TElement>> item in lookup)
		{
			groups.Add(item.Key, new Grouping<TKey, TElement>(item.Key, item.Value));
		}
		if (nullKeyElements != null)
		{
			nullGrouping = new Grouping<TKey, TElement>(default, nullKeyElements);
		}
	}

	public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
	{
		if (nullGrouping != null)
		{
			yield return resultSelector(nullGrouping.Key, nullGrouping);
		}
		foreach (IGrouping<TKey, TElement> group in groups.Values)
		{
			yield return resultSelector(group.Key, group);
		}
	}

	public bool Contains(TKey key)
	{
		return (key == null) ? (nullGrouping != null) : groups.ContainsKey(key);
	}

	public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
	{
		if (nullGrouping != null)
		{
			yield return nullGrouping;
		}
		foreach (IGrouping<TKey, TElement> value in groups.Values)
		{
			yield return value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
