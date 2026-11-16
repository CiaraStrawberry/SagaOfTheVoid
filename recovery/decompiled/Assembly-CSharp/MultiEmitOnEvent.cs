using ParticlePlayground;
using UnityEngine;

public class MultiEmitOnEvent : MonoBehaviour
{
	public PlaygroundParticlesC particlesEvent;

	public PlaygroundParticlesC particlesEmit;

	public int emitCount = 8;

	public Vector3 randomVelocityMin = new Vector3(-1f, 0f, -1f);

	public Vector3 randomVelocityMax = new Vector3(1f, 1f, 1f);

	public Color32 color = Color.white;

	private void Start()
	{
		PlaygroundC.GetEvent(0, particlesEvent).particleEvent += EmitOnEvent;
	}

	private void EmitOnEvent(PlaygroundEventParticle particle)
	{
		particlesEmit.ThreadSafeEmit(emitCount, particle.collisionParticlePosition, randomVelocityMin, randomVelocityMax, color);
	}
}
