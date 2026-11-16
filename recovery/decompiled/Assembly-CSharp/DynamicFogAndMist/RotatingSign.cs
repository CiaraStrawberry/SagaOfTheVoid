using UnityEngine;

namespace DynamicFogAndMist;

public class RotatingSign : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * 50f);
		if (Input.GetMouseButtonDown(0) && Input.touchCount > 0 && Input.touches[0].position.x < (float)Screen.width * 0.5f && Input.touches[0].position.y < (float)Screen.height * 0.25f)
		{
			ToggleFogStyle();
		}
	}

	private void ToggleFogStyle()
	{
		DynamicFog component = Camera.main.GetComponent<DynamicFog>();
		switch (component.preset)
		{
		case FOG_PRESET.Clear:
			component.preset = FOG_PRESET.Mist;
			break;
		case FOG_PRESET.Mist:
			component.preset = FOG_PRESET.WindyMist;
			break;
		case FOG_PRESET.WindyMist:
			component.preset = FOG_PRESET.GroundFog;
			break;
		case FOG_PRESET.GroundFog:
			component.preset = FOG_PRESET.Fog;
			break;
		case FOG_PRESET.Fog:
			component.preset = FOG_PRESET.HeavyFog;
			break;
		case FOG_PRESET.HeavyFog:
			component.preset = FOG_PRESET.SandStorm;
			break;
		case FOG_PRESET.SandStorm:
			component.preset = FOG_PRESET.Clear;
			break;
		}
	}
}
