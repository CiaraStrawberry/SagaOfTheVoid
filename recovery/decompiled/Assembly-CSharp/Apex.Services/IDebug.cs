using System;
using UnityEngine;

namespace Apex.Services;

public interface IDebug
{
	bool developerConsoleVisible { get; set; }

	bool isDebugBuild { get; }

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest);

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration);

	void DrawLine(Vector3 start, Vector3 end, Color color);

	void DrawLine(Vector3 start, Vector3 end);

	void DrawRay(Vector3 start, Vector3 dir, Color color, float duration);

	void DrawRay(Vector3 start, Vector3 dir, Color color);

	void DrawRay(Vector3 start, Vector3 dir);

	void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest);

	void Break();

	void DebugBreak();

	void Log(object message);

	void Log(object message, UnityEngine.Object context);

	void LogError(object message);

	void LogError(object message, UnityEngine.Object context);

	void ClearDeveloperConsole();

	void LogException(Exception exception);

	void LogException(Exception exception, UnityEngine.Object context);

	void LogWarning(object message);

	void LogWarning(object message, UnityEngine.Object context);
}
