using ParticlePlayground;
using UnityEngine;

public class FlickerParticles : MonoBehaviour
{
	[Range(0.01f, 1f)]
	public float flickerInterval = 0.1f;

	private PlaygroundParticlesC particles;

	private float _lastUpdate;

	private void OnEnable()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
		particles.particleMask = particles.particleCount;
	}

	private void OnDisable()
	{
		particles.applyParticleMask = false;
	}

	private void Update()
	{
		if (Time.time >= _lastUpdate + flickerInterval)
		{
			particles.applyParticleMask = !particles.applyParticleMask;
			_lastUpdate = Time.time;
		}
	}
}
