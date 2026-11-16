using UnityEngine;

namespace EasyEditor;

public class EnumFlagAttribute : PropertyAttribute
{
	public string enumName = string.Empty;

	public EnumFlagAttribute()
	{
	}

	public EnumFlagAttribute(string name)
	{
		enumName = name;
	}
}
