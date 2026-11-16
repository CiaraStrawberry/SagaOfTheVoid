using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class EETooltipAttribute : Attribute
{
	public string tooltip = string.Empty;

	public EETooltipAttribute(string tooltip)
	{
		this.tooltip = tooltip;
	}
}
