using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class VisibilityAttribute : Attribute
{
	public string id = string.Empty;

	public object value;

	public string method = string.Empty;

	public VisibilityAttribute(string id, object value)
	{
		this.id = id;
		this.value = value;
	}

	public VisibilityAttribute(string method)
	{
		this.method = method;
	}

	public VisibilityAttribute()
	{
	}
}
