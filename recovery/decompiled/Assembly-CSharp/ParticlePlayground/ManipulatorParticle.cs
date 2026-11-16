using System;

namespace ParticlePlayground;

[Serializable]
public class ManipulatorParticle
{
	public int particleSystemId;

	public int particleId;

	public ManipulatorParticle(int setParticleSystemId, int setParticleId)
	{
		particleSystemId = setParticleSystemId;
		particleId = setParticleId;
	}
}
