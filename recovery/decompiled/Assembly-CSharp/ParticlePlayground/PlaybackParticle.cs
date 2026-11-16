using UnityEngine;

namespace ParticlePlayground;

public struct PlaybackParticle(Vector3 position, Vector3 velocity, float rotation, float size, float lifetime, float startLifetime, float playgroundLife, float playgroundStartLifetime, float playgroundEndLifetime, float playgroundLifetimeSubtraction, Color32 color, Vector3 sourcePosition, float startingSize)
{
	[HideInInspector]
	public Vector3 position = position;

	[HideInInspector]
	public Vector3 velocity = velocity;

	[HideInInspector]
	public float rotation = rotation;

	[HideInInspector]
	public float size = size;

	[HideInInspector]
	public float lifetime = lifetime;

	[HideInInspector]
	public float startLifetime = startLifetime;

	[HideInInspector]
	public float playgroundLife = playgroundLife;

	[HideInInspector]
	public float playgroundStartLifetime = playgroundStartLifetime;

	[HideInInspector]
	public float playgroundEndLifetime = playgroundEndLifetime;

	[HideInInspector]
	public float playgroundLifetimeSubtraction = playgroundLifetimeSubtraction;

	[HideInInspector]
	public Color32 color = color;

	[HideInInspector]
	public Vector3 sourcePosition = sourcePosition;

	[HideInInspector]
	public float startingSize = startingSize;

	public ParticleSystem.Particle CloneAsParticle(bool includeVelocity = false)
	{
		ParticleSystem.Particle result = new ParticleSystem.Particle
		{
			position = position,
			rotation = rotation,
			remainingLifetime = lifetime,
			startLifetime = startLifetime,
			startSize = size,
			startColor = color
		};
		if (includeVelocity)
		{
			result.velocity = velocity;
		}
		return result;
	}

	public SerializedParticle CloneAsSerializedParticle()
	{
		return new SerializedParticle(position, velocity, rotation, size, lifetime, startLifetime, playgroundLife, playgroundStartLifetime, playgroundEndLifetime, playgroundLifetimeSubtraction, color, sourcePosition, startingSize);
	}

	public PlaybackParticle Clone()
	{
		return new PlaybackParticle(position, velocity, rotation, size, lifetime, startLifetime, playgroundLife, playgroundStartLifetime, playgroundEndLifetime, playgroundLifetimeSubtraction, color, sourcePosition, startingSize);
	}
}
