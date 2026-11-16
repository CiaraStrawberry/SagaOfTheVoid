using System;

namespace Devdog.General;

[AttributeUsage(AttributeTargets.Field)]
public class ArrayControlOptionsAttribute : Attribute
{
	public bool canRemoveItems = true;

	public bool canAddItems = true;
}
