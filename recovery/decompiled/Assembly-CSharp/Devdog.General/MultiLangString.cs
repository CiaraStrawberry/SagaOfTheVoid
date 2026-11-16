using System;

namespace Devdog.General;

[Serializable]
public class MultiLangString
{
	public string title;

	public string message;

	public MultiLangString()
	{
	}

	public MultiLangString(string title, string message)
	{
		this.title = title;
		this.message = message;
	}

	public override string ToString()
	{
		return MessageToString();
	}

	public string TitleToString(params object[] vars)
	{
		return ToString(title, vars);
	}

	public string MessageToString(params object[] vars)
	{
		return ToString(message, vars);
	}

	private string ToString(string msg, object[] vars)
	{
		try
		{
			return string.Format(msg, vars);
		}
		catch (Exception)
		{
			DevdogLogger.LogWarning("Invalid string.Format :: " + msg);
			return msg;
		}
	}
}
