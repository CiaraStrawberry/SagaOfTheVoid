using System.Collections;
using UnityEngine;

namespace Devdog.General;

public static class CoroutineUtility
{
	public static IEnumerator WaitRealtime(float waitTime)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + waitTime)
		{
			yield return null;
		}
	}
}
