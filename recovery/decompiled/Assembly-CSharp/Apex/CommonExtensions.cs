using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apex.Common;
using Apex.PathFinding;
using Apex.Units;
using UnityEngine;

namespace Apex;

public static class CommonExtensions
{
	public static T MaxScored<T, TResult>(this IEnumerable<T> list, Func<T, TResult> scorer) where TResult : IComparable<TResult>
	{
		TResult other = default;
		T result = default;
		foreach (T item in list)
		{
			TResult val = scorer(item);
			if (val.CompareTo(other) > 0)
			{
				other = val;
				result = item;
			}
		}
		return result;
	}

	public static bool HasAny(this IHaveAttributes entity, AttributeMask attributes)
	{
		return ((int)entity.attributes & (int)attributes) > 0;
	}

	public static bool HasAll(this IHaveAttributes entity, AttributeMask attributes)
	{
		return ((int)entity.attributes & (int)attributes) == attributes;
	}

	public static IEnumerable<T> All<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			TransientGroup<T> grp = grouping[i];
			int memberCount = grp.count;
			for (int j = 0; j < memberCount; j++)
			{
				yield return grp[j];
			}
		}
	}

	public static void SetPreferredSpeed<T>(this IGrouping<T> grouping, float speed) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].SetPreferredSpeed(speed);
		}
	}

	public static void Stop<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].Stop();
		}
	}

	public static void Wait<T>(this IGrouping<T> grouping, float? seconds) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].Wait(seconds);
		}
	}

	public static void Resume<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].Resume();
		}
	}

	public static void MoveTo<T>(this IGrouping<T> grouping, Vector3 position, bool append) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].MoveTo(position, append);
		}
	}

	public static void MoveAlong<T>(this IGrouping<T> grouping, Path path) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].MoveAlong(path);
		}
	}

	public static void MoveAlong<T>(this IGrouping<T> grouping, Path path, ReplanCallback onReplan) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].MoveAlong(path, onReplan);
		}
	}

	public static void EnableMovementOrders<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].EnableMovementOrders();
		}
	}

	public static void DisableMovementOrders<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].DisableMovementOrders();
		}
	}

	public static void Add<T>(this IGrouping<T> grouping, params T[] members) where T : IGroupable<T>
	{
		int num = members.Length;
		for (int i = 0; i < num; i++)
		{
			grouping.Add(members[i]);
		}
	}

	public static void Add<T>(this IGrouping<T> grouping, IEnumerable<T> members) where T : IGroupable<T>
	{
		foreach (T member in members)
		{
			grouping.Add(member);
		}
	}

	public static void Remove<T>(this IGrouping<T> grouping, params T[] members) where T : IGroupable<T>
	{
		int num = members.Length;
		for (int i = 0; i < num; i++)
		{
			grouping.Remove(members[i]);
		}
	}

	public static void Remove<T>(this IGrouping<T> grouping, IEnumerable<T> members) where T : IGroupable<T>
	{
		foreach (T member in members)
		{
			grouping.Remove(member);
		}
	}

	public static IPathRequest CreatePathRequest(this IUnitFacade unit, Vector3 to, Action<PathResult> callback)
	{
		CallbackPathRequest callbackPathRequest = new CallbackPathRequest(callback);
		callbackPathRequest.from = unit.position;
		callbackPathRequest.to = to;
		callbackPathRequest.requesterProperties = unit;
		callbackPathRequest.pathFinderOptions = unit.pathFinderOptions;
		return callbackPathRequest;
	}

	public static IEnumerable<IUnitFacade> ToUnitFacades(this IEnumerable list)
	{
		return from object m in list
			let unit = ResolveUnit(m)
			where unit != null
			select unit;
	}

	private static IUnitFacade ResolveUnit(object member)
	{
		if (member is IUnitFacade result)
		{
			return result;
		}
		if (member is IGameObjectComponent goc)
		{
			return goc.GetUnitFacade();
		}
		Component component = member as Component;
		if (component != null)
		{
			return component.GetUnitFacade();
		}
		GameObject gameObject = member as GameObject;
		if (gameObject != null)
		{
			return gameObject.GetUnitFacade();
		}
		return null;
	}
}
