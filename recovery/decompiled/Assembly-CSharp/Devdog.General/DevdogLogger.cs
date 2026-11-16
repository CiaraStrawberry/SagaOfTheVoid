using UnityEngine;

namespace Devdog.General;

public static class DevdogLogger
{
	public enum LogType
	{
		LogVerbose,
		Log,
		Warning,
		Error
	}

	public static LogType minimaLog;

	public static void LogVerbose(string message)
	{
		if (LogType.LogVerbose >= minimaLog)
		{
			Debug.Log(message);
		}
	}

	public static void LogVerbose(string message, Object context)
	{
		if (LogType.LogVerbose >= minimaLog)
		{
			Debug.Log(message, context);
		}
	}

	public static void Log(string message)
	{
		Log(message, null);
	}

	public static void Log(string message, Object context)
	{
		if (LogType.Log >= minimaLog)
		{
			Debug.Log(message, context);
		}
	}

	public static void LogWarning(string message)
	{
		LogWarning(message, null);
	}

	public static void LogWarning(string message, Object context)
	{
		if (LogType.Warning >= minimaLog)
		{
			Debug.LogWarning(message, context);
		}
	}

	public static void LogError(string message)
	{
		LogError(message, null);
	}

	public static void LogError(string message, Object context)
	{
		if (LogType.Error >= minimaLog)
		{
			Debug.LogError(message, context);
		}
	}
}
