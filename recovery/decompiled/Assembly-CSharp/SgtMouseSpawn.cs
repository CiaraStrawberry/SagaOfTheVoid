using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Mouse Spawn")]
public class SgtMouseSpawn : MonoBehaviour
{
	[Tooltip("The camera used to spawn in front of (default = MainCamera)")]
	public Camera Camera;

	[Tooltip("The key that needs to be held down to look")]
	public KeyCode Require = KeyCode.Mouse0;

	public GameObject Prefab;

	protected virtual void Update()
	{
		Camera camera = Camera;
		if (camera == null)
		{
			camera = Camera.main;
		}
		if (camera != null && (Require == KeyCode.None || Input.GetKeyDown(Require)))
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo = default;
			if (Physics.Raycast(ray, out hitInfo))
			{
				Object.Instantiate(Prefab, hitInfo.point, Quaternion.identity);
			}
		}
	}
}
