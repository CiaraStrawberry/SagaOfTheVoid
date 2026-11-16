using System;

namespace EasyEditor;

[Serializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
public class MessageAttribute : Attribute
{
	public string text = string.Empty;

	public MessageType messageType;

	public string method = string.Empty;

	public string id = string.Empty;

	public object value;

	public MessageAttribute(string text)
	{
		this.text = text;
	}

	public MessageAttribute()
	{
	}
}
