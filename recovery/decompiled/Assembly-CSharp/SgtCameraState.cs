using System.Collections.Generic;
using UnityEngine;

public class SgtCameraState
{
	public Camera Camera;

	public static T Save<T>(ref List<T> cameraStates, Camera camera) where T : SgtCameraState, new()
	{
		if (cameraStates == null)
		{
			cameraStates = new List<T>();
		}
		for (int num = cameraStates.Count - 1; num >= 0; num--)
		{
			T val = cameraStates[num];
			if (val == null)
			{
				cameraStates.RemoveAt(num);
			}
			else if (val.Camera == null)
			{
				SgtClassPool<T>.Add(val);
				cameraStates.RemoveAt(num);
			}
			else if (val.Camera == camera)
			{
				return val;
			}
		}
		T val2 = SgtClassPool<T>.Pop() ?? new T();
		val2.Camera = camera;
		cameraStates.Add(val2);
		return val2;
	}

	public static T Restore<T>(List<T> cameraStates, Camera camera) where T : SgtCameraState
	{
		if (cameraStates != null)
		{
			for (int num = cameraStates.Count - 1; num >= 0; num--)
			{
				T val = cameraStates[num];
				if (val.Camera == camera)
				{
					return val;
				}
			}
		}
		return (T)null;
	}

	public static void Clear<T>(List<T> cameraStates) where T : SgtCameraState
	{
		if (cameraStates != null)
		{
			for (int num = cameraStates.Count - 1; num >= 0; num--)
			{
				SgtClassPool<T>.Add(cameraStates[num]);
			}
			cameraStates.Clear();
		}
	}
}
