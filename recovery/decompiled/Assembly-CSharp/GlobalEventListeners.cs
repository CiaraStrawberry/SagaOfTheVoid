using ParticlePlayground;
using UnityEngine;

public class GlobalEventListeners : MonoBehaviour
{
	private void OnEnable()
	{
		PlaygroundC.particleEventBirth += OnEventParticleBirth;
		PlaygroundC.particleEventDeath += OnEventParticleDeath;
		PlaygroundC.particleEventCollision += OnEventParticleCollision;
		PlaygroundC.particleEventTime += OnEventParticleTime;
	}

	private void OnDisable()
	{
		PlaygroundC.particleEventBirth -= OnEventParticleBirth;
		PlaygroundC.particleEventDeath -= OnEventParticleDeath;
		PlaygroundC.particleEventCollision -= OnEventParticleCollision;
		PlaygroundC.particleEventTime -= OnEventParticleTime;
	}

	private void OnEventParticleBirth(PlaygroundEventParticle particle)
	{
		Debug.Log(string.Concat("A particle came to life at ", particle.position, " with a size of ", particle.size));
	}

	private void OnEventParticleDeath(PlaygroundEventParticle particle)
	{
		Debug.Log(string.Concat("A particle died at ", particle.position, " with the velocity of ", particle.velocity, ". It was born in ", particle.targetPosition));
	}

	private void OnEventParticleCollision(PlaygroundEventParticle particle)
	{
		Debug.Log(string.Concat("A particle collided at ", particle.position, " with ", particle.collisionTransform.name, ". The id of the particle was ", particle.particleId, " from particle system ", particle.particleSystemId));
	}

	private void OnEventParticleTime(PlaygroundEventParticle particle)
	{
		Debug.Log(string.Concat("A particle called in at ", particle.position, " with the lifetime of ", particle.life));
	}
}
