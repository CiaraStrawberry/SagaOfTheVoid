using System;

namespace Devdog.General;

[AttributeUsage(AttributeTargets.Field)]
public class HideGroupAttribute : Attribute
{
	public bool includeArrayChildren { get; protected set; }

	public HideGroupAttribute()
		: this(includeArrayChildren: true)
	{
	}

	public HideGroupAttribute(bool includeArrayChildren)
	{
		this.includeArrayChildren = includeArrayChildren;
	}
}
