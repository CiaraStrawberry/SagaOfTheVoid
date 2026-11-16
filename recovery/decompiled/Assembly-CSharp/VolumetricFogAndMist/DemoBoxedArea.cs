using UnityEngine;

namespace VolumetricFogAndMist;

public class DemoBoxedArea : MonoBehaviour
{
	private void Update()
	{
		VolumetricFog instance = VolumetricFog.instance;
		if (Input.GetKeyDown(KeyCode.T))
		{
			instance.enabled = !instance.enabled;
		}
	}
}
