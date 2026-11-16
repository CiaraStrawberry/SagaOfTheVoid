using ParticlePlayground;
using UnityEngine;

public class ParticleTeleporter : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public int manipulatorIndex;

	public Transform target;

	private ManipulatorObjectC _manipulator;

	private Vector3 _targetPosition;

	private void OnEnable()
	{
		if (_manipulator == null)
		{
			_manipulator = PlaygroundC.GetManipulator(manipulatorIndex, particles);
		}
		_targetPosition = target.position;
		_manipulator.particleEventEnter -= Teleport;
		_manipulator.enabled = true;
		_manipulator.particleEventEnter += Teleport;
	}

	private void OnDisable()
	{
		_manipulator.particleEventEnter -= Teleport;
		_manipulator.enabled = false;
	}

	private void Update()
	{
		_targetPosition = target.position;
	}

	private void Teleport(PlaygroundEventParticle eventParticle)
	{
		int particleId = eventParticle.particleId;
		Vector3 position = eventParticle.position;
		Vector3 position2 = _manipulator.transform.position;
		Vector3 position3 = _targetPosition + (position - position2);
		particles.ParticlePosition(particleId, position3);
	}
}
