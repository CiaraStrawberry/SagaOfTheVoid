using UnityEngine;

namespace VolumetricFogAndMist;

public class FogVolume : MonoBehaviour
{
	[Tooltip("Target fog alpha when camera enters this fog volume")]
	[Range(0f, 1f)]
	public float targetFogAlpha = 0.5f;

	[Tooltip("Target sky haze alpha when camera enters this fog volume")]
	[Range(0f, 1f)]
	public float targetSkyHazeAlpha = 0.5f;

	[Tooltip("Set this to zero for changing fog alpha immediately upon enter/exit fog volume.")]
	public float transitionDuration = 3f;

	[Tooltip("Set collider that will trigger this fog volume. If not set, this fog volume will react to any collider which has the main camera. If you use a third person controller, assign the character collider here.")]
	public Collider targetCollider;

	private bool cameraInside;

	private VolumetricFog fog;

	private void Start()
	{
		fog = VolumetricFog.instance;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!cameraInside && (other == targetCollider || other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera))
		{
			cameraInside = true;
			fog.SetTargetAlpha(targetFogAlpha, targetSkyHazeAlpha, transitionDuration);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (cameraInside && (other == targetCollider || other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera))
		{
			cameraInside = false;
			fog.ClearTargetAlpha(transitionDuration);
		}
	}
}
