using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class InspectorAttribute : Attribute
{
	public string group = string.Empty;

	public string groupDescription = string.Empty;

	public bool displayHeader = true;

	public bool foldable;

	public int order = 100;

	public string rendererType;

	public string id = string.Empty;

	public string label = string.Empty;

	public static InspectorAttribute DefaultInspectorAttribute => new InspectorAttribute();
}
