using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apex.Utilities;

public static class Utils
{
	public static void CopyProps(object from, object to)
	{
		Ensure.ArgumentNotNull(from, "from");
		Ensure.ArgumentNotNull(to, "to");
		Type type = from.GetType();
		Type type2 = to.GetType();
		IEnumerable<PropertyInfo> enumerable = from p in type.GetProperties()
			where p.CanRead
			select p;
		Dictionary<string, PropertyInfo> dictionary = (from p in type2.GetProperties()
			where p.CanWrite
			select p).ToDictionary((PropertyInfo p) => p.Name);
		foreach (PropertyInfo item in enumerable)
		{
			if (dictionary.TryGetValue(item.Name, out var value))
			{
				value.SetValue(to, item.GetValue(from, null), null);
			}
		}
	}
}
