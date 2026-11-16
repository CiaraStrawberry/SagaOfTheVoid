using UnityEngine;

public class SpawnPrefabOnClickC : MonoBehaviour
{
	public GameObject prefab;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, 100f))
			{
				Object.Instantiate(prefab, hitInfo.point, Quaternion.identity);
			}
		}
	}
}
