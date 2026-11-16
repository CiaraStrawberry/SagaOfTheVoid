using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PlaygroundParticlesC))]
public class PlaygroundPresetLaserC : MonoBehaviour
{
	public float laserMaxDistance = 100f;

	public Gradient laserColor;

	public int particleCount = 1000;

	public LayerMask collisionLayer = -1;

	private PlaygroundParticlesC particles;

	private int previousParticleCount;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		laserColor = particles.lifetimeColor;
		previousParticleCount = particleCount;
	}

	private void Update()
	{
		if (Physics.Raycast(particles.sourceTransform.position, particles.sourceTransform.forward, out var hitInfo, laserMaxDistance, collisionLayer))
		{
			particles.overflowOffset.z = Vector3.Distance(particles.sourceTransform.position, hitInfo.point) / (float)(1 + particles.particleCount);
		}
		else
		{
			particles.overflowOffset.z = laserMaxDistance / (float)(1 + particles.particleCount);
		}
		if (particleCount != previousParticleCount)
		{
			PlaygroundC.SetParticleCount(particles, particleCount);
			previousParticleCount = particleCount;
		}
		if (laserColor != particles.lifetimeColor)
		{
			particles.lifetimeColor = laserColor;
		}
	}
}
