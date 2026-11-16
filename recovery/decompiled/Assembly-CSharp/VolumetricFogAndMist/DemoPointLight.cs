using UnityEngine;

namespace VolumetricFogAndMist;

public class DemoPointLight : MonoBehaviour
{
	public Vector3 attractionPos = new Vector3(-58f, 3f, -19f);

	private Vector3[] fairyDirections = new Vector3[6];

	private void Update()
	{
		for (int i = 0; i < 6; i++)
		{
			GameObject pointLight = VolumetricFog.instance.GetPointLight(i);
			if (pointLight != null)
			{
				pointLight.transform.position += fairyDirections[i];
				Vector3 position = pointLight.transform.position;
				if (position.x > attractionPos.x)
				{
					fairyDirections[i].x -= 0.01f;
				}
				else
				{
					fairyDirections[i].x += 0.01f;
				}
				if (position.y > attractionPos.y + 1f)
				{
					fairyDirections[i].y -= 0.001f;
				}
				else
				{
					fairyDirections[i].y += 0.001f;
				}
				if (position.z > attractionPos.z)
				{
					fairyDirections[i].z -= 0.01f;
				}
				else
				{
					fairyDirections[i].z += 0.01f;
				}
			}
		}
	}
}
