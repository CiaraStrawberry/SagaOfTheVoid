using ParticlePlayground;
using UnityEngine;

public class PlaygroundInk : MonoBehaviour
{
	public Transform controlTransform;

	private PlaygroundParticlesC particles;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
	}

	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo, 1000f))
		{
			particles.Emit(Mathf.RoundToInt(4000f * Time.deltaTime), hitInfo.point - new Vector3(0.2f, 0.2f, 0.2f), hitInfo.point + new Vector3(0.2f, 0.2f, 0.2f), new Vector3(-1f, -1f, -1f), new Vector3(1f, 1f, 1f), Color.white);
			if (controlTransform != null)
			{
				controlTransform.position = hitInfo.point;
			}
		}
	}
}
