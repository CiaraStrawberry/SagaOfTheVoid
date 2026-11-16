using ParticlePlayground;
using UnityEngine;

public class ParticlePickupWithGlobalManipulator : MonoBehaviour
{
	public int manipulatorIndex;

	private ManipulatorObjectC manipulator;

	private int pickups;

	private void Start()
	{
		manipulator = PlaygroundC.GetManipulator(manipulatorIndex);
		manipulator.trackParticles = true;
		manipulator.sendEventEnter = true;
		manipulator.trackingMethod = TrackingMethod.ManipulatorId;
		manipulator.particleEventEnter += OnManipulatorEnter;
	}

	private void OnManipulatorEnter(PlaygroundEventParticle particle)
	{
		PlaygroundC.reference.particleSystems[particle.particleSystemId].Kill(particle.particleId);
		pickups++;
	}
}
