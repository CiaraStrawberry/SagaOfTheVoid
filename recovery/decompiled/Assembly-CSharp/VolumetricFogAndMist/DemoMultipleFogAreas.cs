using UnityEngine;

namespace VolumetricFogAndMist;

public class DemoMultipleFogAreas : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			CreateCloud();
		}
		else if (Input.GetKeyDown(KeyCode.B))
		{
			CreateBoxFog();
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			VolumetricFog.RemoveAllFogAreas();
		}
	}

	private void CreateCloud()
	{
		Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 100f + Random.insideUnitSphere * 50f;
		if (position.y < 10f)
		{
			position.y = 10f;
		}
		float radius = Random.value * 50f + 85f;
		VolumetricFog volumetricFog = VolumetricFog.CreateFogArea(position, radius);
		volumetricFog.color = new Color(0.6f, 0.57f, 0.5f, 1f);
	}

	private void CreateBoxFog()
	{
		Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 100f + Random.insideUnitSphere * 50f;
		if (position.y < 10f)
		{
			position.y = 10f;
		}
		VolumetricFog volumetricFog = VolumetricFog.CreateFogArea(boxSize: new Vector3(Random.value * 50f + 35f, Random.value * 10f + 15f, Random.value * 50f + 35f), position: position);
		volumetricFog.color = new Color(0.6f, 0.57f, 0.5f, 1f);
		volumetricFog.noiseScale = 2f;
	}
}
