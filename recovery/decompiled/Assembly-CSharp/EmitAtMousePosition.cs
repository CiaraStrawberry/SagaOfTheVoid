using ParticlePlayground;
using UnityEngine;

public class EmitAtMousePosition : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Vector3 velocity;

	public Color32 color = Color.white;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, 1000f))
			{
				particles.Emit(hitInfo.point, velocity, color);
			}
		}
	}
}
