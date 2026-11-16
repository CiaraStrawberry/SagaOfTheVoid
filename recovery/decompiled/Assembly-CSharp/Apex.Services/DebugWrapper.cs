using System;
using UnityEngine;

namespace Apex.Services;

public sealed class DebugWrapper : IDebug
{
	public bool developerConsoleVisible
	{
		get
		{
			return Debug.developerConsoleVisible;
		}
		set
		{
			Debug.developerConsoleVisible = value;
		}
	}

	public bool isDebugBuild => Debug.isDebugBuild;

	public void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		Debug.DrawLine(start, end, color, duration);
	}

	public void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		Debug.DrawLine(start, end, color);
	}

	public void DrawLine(Vector3 start, Vector3 end)
	{
		Debug.DrawLine(start, end);
	}

	public void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		Debug.DrawRay(start, dir, color, duration);
	}

	public void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		Debug.DrawRay(start, dir, color);
	}

	public void DrawRay(Vector3 start, Vector3 dir)
	{
		Debug.DrawRay(start, dir);
	}

	public void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
	{
		Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	public void Break()
	{
		Debug.Break();
	}

	public void DebugBreak()
	{
		Debug.DebugBreak();
	}

	public void Log(object message)
	{
		Debug.Log(message);
	}

	public void Log(object message, UnityEngine.Object context)
	{
		Debug.Log(message, context);
	}

	public void LogError(object message)
	{
		Debug.LogError(message);
	}

	public void LogError(object message, UnityEngine.Object context)
	{
		Debug.LogError(message, context);
	}

	public void ClearDeveloperConsole()
	{
		Debug.ClearDeveloperConsole();
	}

	public void LogException(Exception exception)
	{
		Debug.LogException(exception);
	}

	public void LogException(Exception exception, UnityEngine.Object context)
	{
		Debug.LogException(exception, context);
	}

	public void LogWarning(object message)
	{
		Debug.LogWarning(message);
	}

	public void LogWarning(object message, UnityEngine.Object context)
	{
		Debug.LogWarning(message, context);
	}
}
