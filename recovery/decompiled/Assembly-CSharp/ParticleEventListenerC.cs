using ParticlePlayground;
using UnityEngine;

public class ParticleEventListenerC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	private PlaygroundEventC playgroundEvent;

	private Vector3 gizmoPosition;

	private void Start()
	{
		if (!(particles == null))
		{
			playgroundEvent = PlaygroundC.GetEvent(0, particles);
			AddListener();
		}
	}

	private void AddListener()
	{
		playgroundEvent.particleEvent += DoSomething;
	}

	private void DoSomething(PlaygroundEventParticle particle)
	{
		gizmoPosition = particle.position;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(gizmoPosition, 1f);
	}
}
