using System;
using UnityEngine;

namespace Apex.Common;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class AttributePropertyAttribute : PropertyAttribute
{
	public string label { get; private set; }

	public string tooltip { get; private set; }

	public AttributePropertyAttribute()
	{
	}

	public AttributePropertyAttribute(string label)
	{
		this.label = label;
	}

	public AttributePropertyAttribute(string label, string tooltip)
	{
		this.label = label;
		this.tooltip = tooltip;
	}
}
