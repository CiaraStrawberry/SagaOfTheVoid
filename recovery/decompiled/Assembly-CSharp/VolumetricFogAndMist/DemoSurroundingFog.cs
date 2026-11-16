using UnityEngine;
using UnityStandardAssets.Cameras;

namespace VolumetricFogAndMist;

public class DemoSurroundingFog : MonoBehaviour
{
	private FreeLookCam cam;

	private void Start()
	{
		cam = GetComponent<FreeLookCam>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			cam.enabled = !cam.enabled;
		}
	}
}
