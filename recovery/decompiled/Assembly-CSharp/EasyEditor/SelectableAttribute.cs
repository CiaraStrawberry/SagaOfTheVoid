using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SelectableAttribute : Attribute
{
}
