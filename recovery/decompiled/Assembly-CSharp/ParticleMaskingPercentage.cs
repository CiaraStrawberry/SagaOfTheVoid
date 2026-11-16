using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleMaskingPercentage : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public float maskingPercentage;

	private void Start()
	{
		particles.applyParticleMask = true;
	}

	private void Update()
	{
		maskingPercentage = Mathf.Clamp(maskingPercentage, 0f, 100f);
		particles.particleMask = Mathf.RoundToInt((float)particles.particleCount * (maskingPercentage / 100f));
	}
}
