using ParticlePlayground;
using UnityEngine;

public class ProtectParticlesFromManipulator : MonoBehaviour
{
	public ParticleProtectionRange[] protectedParticles;

	public PlaygroundParticlesC particles;

	public int manipulatorNumber;

	private ManipulatorObjectC manipulator;

	private void Start()
	{
		manipulator = PlaygroundC.GetManipulator(manipulatorNumber, particles);
		SetProtectedParticles();
	}

	private void SetProtectedParticles()
	{
		ParticleProtectionRange[] array = protectedParticles;
		foreach (ParticleProtectionRange particleProtectionRange in array)
		{
			for (int j = particleProtectionRange.start; j < particleProtectionRange.end; j++)
			{
				manipulator.AddNonAffectedParticle(particles.particleSystemId, j);
			}
		}
	}

	private void RemoveProtectedParticles()
	{
		ParticleProtectionRange[] array = protectedParticles;
		foreach (ParticleProtectionRange particleProtectionRange in array)
		{
			for (int j = particleProtectionRange.start; j < particleProtectionRange.end; j++)
			{
				manipulator.RemoveNonAffectedParticle(particles.particleSystemId, j);
			}
		}
	}
}
