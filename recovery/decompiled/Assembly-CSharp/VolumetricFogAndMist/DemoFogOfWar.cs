using UnityEngine;
using UnityEngine.UI;

namespace VolumetricFogAndMist;

public class DemoFogOfWar : MonoBehaviour
{
	private bool fogCuttingOn = true;

	private VolumetricFog fog;

	private Text status;

	private void Start()
	{
		fog = VolumetricFog.instance;
		fog.fogOfWarEnabled = fogCuttingOn;
		fog.ResetFogOfWar();
		status = GameObject.Find("Status").GetComponent<Text>();
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
		else if (Input.GetKeyDown(KeyCode.C))
		{
			fogCuttingOn = !fogCuttingOn;
			fog.fogOfWarEnabled = fogCuttingOn;
			fog.ResetFogOfWar();
		}
		else if (Input.GetKeyDown(KeyCode.R))
		{
			fog.ResetFogOfWar();
		}
		if (fogCuttingOn)
		{
			fog.SetFogOfWarAlpha(Camera.main.transform.position, 4f, 0f);
		}
		string text = VolumetricFog.instance.GetCurrentPresetName();
		if (fogCuttingOn)
		{
			text += "  *** FOG CUTTING ON ***";
		}
		status.text = text;
	}

	private void AssignCustomTexture()
	{
		VolumetricFog instance = VolumetricFog.instance;
		Texture2D texture2D = Resources.Load<Texture2D>("Textures/alphaDemo");
		Color32[] pixels = texture2D.GetPixels32();
		instance.fogOfWarTextureSize = 64;
		instance.fogOfWarTextureData = pixels;
	}
}
