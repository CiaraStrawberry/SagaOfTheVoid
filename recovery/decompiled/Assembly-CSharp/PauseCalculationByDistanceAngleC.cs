using ParticlePlayground;
using UnityEngine;

public class PauseCalculationByDistanceAngleC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform target;

	public float distance = 100f;

	public float angle = 90f;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		particles.calculate = Vector3.Distance(particles.particleSystemTransform.position, target.position) < distance && Vector3.Angle(particles.particleSystemTransform.position - target.position, target.forward) < angle;
	}
}
