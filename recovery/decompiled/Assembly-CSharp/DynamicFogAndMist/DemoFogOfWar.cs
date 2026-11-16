using UnityEngine;

namespace DynamicFogAndMist;

public class DemoFogOfWar : MonoBehaviour
{
	private bool fogCuttingOn = true;

	private static GUIStyle labelStyle;

	private void Update()
	{
		DynamicFog instance = DynamicFog.instance;
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
				instance.preset = FOG_PRESET.Fog;
				break;
			case FOG_PRESET.Fog:
				instance.preset = FOG_PRESET.HeavyFog;
				break;
			case FOG_PRESET.HeavyFog:
				instance.preset = FOG_PRESET.SandStorm;
				break;
			case FOG_PRESET.SandStorm:
				instance.preset = FOG_PRESET.Mist;
				break;
			}
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			instance.enabled = !instance.enabled;
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			fogCuttingOn = !fogCuttingOn;
			instance.fogOfWarEnabled = fogCuttingOn;
			instance.ResetFogOfWar();
		}
		else if (Input.GetKeyDown(KeyCode.R))
		{
			instance.ResetFogOfWar();
		}
		if (fogCuttingOn)
		{
			instance.SetFogOfWarAlpha(Camera.main.transform.position, 4f, 0f);
		}
	}

	private void OnGUI()
	{
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.normal.textColor = Color.black;
		}
		Rect position = new Rect(10f, 10f, Screen.width - 20, 30f);
		GUI.Label(position, "Move around with WASD or cursor keys, space to jump, F key to change fog style, T to toggle fog on/off, C to toggle fog cutting, R to reset fog.", labelStyle);
		position = new Rect(10f, 30f, Screen.width - 20, 30f);
		GUI.Label(position, "Current fog preset: " + DynamicFog.instance.GetCurrentPresetName(), labelStyle);
		if (fogCuttingOn)
		{
			position = new Rect(10f, 50f, Screen.width - 20, 30f);
			GUI.Label(position, "FOG CUTTING ON", labelStyle);
		}
	}
}
