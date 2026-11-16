using ParticlePlayground;
using UnityEngine;

public class KillParticleWithinEventRangeC : MonoBehaviour
{
	public float killRange = 1f;

	public PlaygroundParticlesC particles;

	private PlaygroundEventC playgroundEvent;

	private Transform thisTransform;

	private Vector3 thisPosition;

	private void Start()
	{
		thisTransform = transform;
		playgroundEvent = PlaygroundC.GetEvent(0, particles);
		AddEventListener();
	}

	private void Update()
	{
		thisPosition = thisTransform.position;
	}

	private void ParticleEvent(PlaygroundEventParticle particle)
	{
		if (Vector3.Distance(particle.position, thisPosition) <= killRange)
		{
			particles.Kill(particle.particleId);
		}
	}

	private void AddEventListener()
	{
		playgroundEvent.particleEvent += ParticleEvent;
	}

	private void RemoveEventListener()
	{
		playgroundEvent.particleEvent -= ParticleEvent;
	}
}
