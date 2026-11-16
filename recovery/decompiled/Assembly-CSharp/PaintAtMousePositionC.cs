using ParticlePlayground;
using UnityEngine;

public class PaintAtMousePositionC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public COLLISIONTYPEC collisionType;

	public Color color = Color.white;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
		if (particles != null)
		{
			particles.source = SOURCEC.Paint;
			particles.paint.collisionType = collisionType;
			particles.calculateDeltaMovement = false;
		}
		else
		{
			Debug.Log("Please assign a particle system to this script.", gameObject);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, 1000f))
			{
				PlaygroundC.Paint(particles, hitInfo.point, hitInfo.normal, hitInfo.transform, color);
			}
		}
	}
}
