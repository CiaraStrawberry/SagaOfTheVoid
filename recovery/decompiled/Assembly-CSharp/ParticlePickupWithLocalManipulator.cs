using ParticlePlayground;
using UnityEngine;

public class ParticlePickupWithLocalManipulator : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public int manipulatorIndex;

	private ManipulatorObjectC manipulator;

	private int pickups;

	private void Start()
	{
		manipulator = PlaygroundC.GetManipulator(manipulatorIndex, particles);
		manipulator.trackParticles = true;
		manipulator.sendEventEnter = true;
		manipulator.trackingMethod = TrackingMethod.ManipulatorId;
		manipulator.particleEventEnter += OnManipulatorEnter;
	}

	private void OnManipulatorEnter(PlaygroundEventParticle particle)
	{
		particles.Kill(particle.particleId);
		pickups++;
	}
}
