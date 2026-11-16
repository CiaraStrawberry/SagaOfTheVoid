using UnityEngine;

namespace ParticlePlayground;

public struct TrailParticleInfo(int particleId, Vector3 position, Vector3 velocity)
{
	public int particleId = particleId;

	public Vector3 position = position;

	public Vector3 velocity = velocity;
}
