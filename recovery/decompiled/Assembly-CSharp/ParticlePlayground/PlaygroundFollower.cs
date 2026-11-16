using UnityEngine;

namespace ParticlePlayground;

public class PlaygroundFollower
{
	public bool enabled = true;

	public float lifetime;

	public Transform transform;

	public GameObject gameObject;

	public TrailRenderer trailRenderer;

	public int particleId;

	public PlaygroundFollower(Transform setTransform, GameObject setGameObject, TrailRenderer setTrailRenderer, float setLifetime, int setParticleId)
	{
		transform = setTransform;
		gameObject = setGameObject;
		trailRenderer = setTrailRenderer;
		lifetime = setLifetime;
		particleId = setParticleId;
	}

	public PlaygroundFollower Clone()
	{
		return new PlaygroundFollower(transform, gameObject, trailRenderer, lifetime, particleId);
	}
}
