using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricFogAndMist;

public class DemoDepthBlur : MonoBehaviour
{
	private void Start()
	{
		GameObject gameObject = GameObject.Find("Elephant");
		gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
	}

	private void Update()
	{
		VolumetricFog instance = VolumetricFog.instance;
		if (Input.GetKeyDown(KeyCode.T))
		{
			instance.fogBlur = !instance.fogBlur;
		}
	}
}
