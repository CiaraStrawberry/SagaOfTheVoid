using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace VolumetricFogAndMist;

public class DemoWalk : MonoBehaviour
{
	private Text status;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Elephant");
		if (gameObject != null)
		{
			gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
		}
		status = GameObject.Find("Status").GetComponent<Text>();
	}

	private void Update()
	{
		VolumetricFog instance = VolumetricFog.instance;
		if (Input.GetKeyDown(KeyCode.F))
		{
			switch (instance.preset)
			{
			case FOG_PRESET.Clear:
			case FOG_PRESET.Custom:
				instance.preset = FOG_PRESET.Mist;
				break;
			case FOG_PRESET.Mist:
				instance.preset = FOG_PRESET.WindyMist;
				break;
			case FOG_PRESET.WindyMist:
				instance.preset = FOG_PRESET.GroundFog;
				break;
			case FOG_PRESET.GroundFog:
				instance.preset = FOG_PRESET.FrostedGround;
				break;
			case FOG_PRESET.FrostedGround:
				instance.preset = FOG_PRESET.FoggyLake;
				break;
			case FOG_PRESET.FoggyLake:
				instance.preset = FOG_PRESET.Fog;
				break;
			case FOG_PRESET.Fog:
				instance.preset = FOG_PRESET.HeavyFog;
				break;
			case FOG_PRESET.HeavyFog:
				instance.preset = FOG_PRESET.LowClouds;
				break;
			case FOG_PRESET.LowClouds:
				instance.preset = FOG_PRESET.SeaClouds;
				break;
			case FOG_PRESET.SeaClouds:
				instance.preset = FOG_PRESET.Smoke;
				break;
			case FOG_PRESET.Smoke:
				instance.preset = FOG_PRESET.ToxicSwamp;
				break;
			case FOG_PRESET.ToxicSwamp:
				instance.preset = FOG_PRESET.SandStorm1;
				break;
			case FOG_PRESET.SandStorm1:
				instance.preset = FOG_PRESET.SandStorm2;
				break;
			case FOG_PRESET.SandStorm2:
				instance.preset = FOG_PRESET.Mist;
				break;
			}
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			instance.enabled = !instance.enabled;
		}
		status.text = "Current fog preset: " + VolumetricFog.instance.GetCurrentPresetName();
	}
}
