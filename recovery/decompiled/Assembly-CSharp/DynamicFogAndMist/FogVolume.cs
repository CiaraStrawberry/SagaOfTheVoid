using UnityEngine;

namespace DynamicFogAndMist;

public class FogVolume : MonoBehaviour
{
	[Tooltip("Target alpha for fog when camera enters this fog volume")]
	[Range(0f, 1f)]
	public float targetFogAlpha = 0.5f;

	[Tooltip("Target alpha for sky haze when camera enters this fog volume")]
	[Range(0f, 1f)]
	public float targetSkyHazeAlpha = 0.5f;

	[Tooltip("Set this to zero for changing fog alpha immediately upon enter/exit fog volume.")]
	public float transitionDuration = 3f;

	private DynamicFog fog;

	private bool cameraInside;

	private void Start()
	{
		fog = DynamicFog.instance;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!cameraInside && other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera)
		{
			cameraInside = true;
			fog.SetTargetAlpha(targetFogAlpha, targetSkyHazeAlpha, transitionDuration);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (cameraInside && other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera)
		{
			cameraInside = false;
			fog.ClearTargetAlpha(transitionDuration);
		}
	}
}
