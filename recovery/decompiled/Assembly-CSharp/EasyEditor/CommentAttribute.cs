using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class CommentAttribute : Attribute
{
	public string comment = string.Empty;

	public CommentAttribute(string comment)
	{
		this.comment = comment;
	}
}
