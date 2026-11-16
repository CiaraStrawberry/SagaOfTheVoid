using System;

namespace EasyEditor;

[AttributeUsage(AttributeTargets.Class)]
public class GroupsAttribute : Attribute
{
	public string[] groups = new string[1] { string.Empty };

	public GroupsAttribute(params string[] groupList)
	{
		groups = groupList;
	}
}
