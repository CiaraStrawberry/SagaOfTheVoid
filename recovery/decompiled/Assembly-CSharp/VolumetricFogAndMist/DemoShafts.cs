using UnityEngine;

namespace VolumetricFogAndMist;

public class DemoShafts : MonoBehaviour
{
	private VolumetricFog fog;

	private void Start()
	{
		fog = VolumetricFog.instance;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			switch (fog.preset)
			{
			case FOG_PRESET.Clear:
				fog.preset = FOG_PRESET.Mist;
				break;
			case FOG_PRESET.Mist:
				fog.preset = FOG_PRESET.WindyMist;
				break;
			case FOG_PRESET.WindyMist:
				fog.preset = FOG_PRESET.GroundFog;
				break;
			case FOG_PRESET.GroundFog:
				fog.preset = FOG_PRESET.FrostedGround;
				break;
			case FOG_PRESET.FrostedGround:
				fog.preset = FOG_PRESET.FoggyLake;
				break;
			case FOG_PRESET.FoggyLake:
				fog.preset = FOG_PRESET.Fog;
				break;
			case FOG_PRESET.Fog:
				fog.preset = FOG_PRESET.HeavyFog;
				break;
			case FOG_PRESET.HeavyFog:
				fog.preset = FOG_PRESET.LowClouds;
				break;
			case FOG_PRESET.LowClouds:
				fog.preset = FOG_PRESET.SeaClouds;
				break;
			case FOG_PRESET.SeaClouds:
				fog.preset = FOG_PRESET.Smoke;
				break;
			case FOG_PRESET.Smoke:
				fog.preset = FOG_PRESET.ToxicSwamp;
				break;
			case FOG_PRESET.ToxicSwamp:
				fog.preset = FOG_PRESET.SandStorm1;
				break;
			case FOG_PRESET.SandStorm1:
				fog.preset = FOG_PRESET.SandStorm2;
				break;
			case FOG_PRESET.SandStorm2:
				fog.preset = FOG_PRESET.Mist;
				break;
			}
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			fog.enabled = !fog.enabled;
		}
		fog.sun.transform.Rotate(Vector3.left, Time.deltaTime);
	}

	private void OnGUI()
	{
		Rect position = new Rect(10f, 10f, Screen.width - 20, 30f);
		GUI.Label(position, "Move around with WASD or cursor keys, space to jump, F key to change fog style, T to toggle fog on/off.");
		position = new Rect(10f, 30f, Screen.width - 20, 30f);
		GUI.Label(position, "Current fog preset: " + VolumetricFog.instance.GetCurrentPresetName());
	}
}
