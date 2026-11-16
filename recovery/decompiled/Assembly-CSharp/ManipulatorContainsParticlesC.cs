using ParticlePlayground;
using UnityEngine;

public class ManipulatorContainsParticlesC : MonoBehaviour
{
	public int manipulatorNumber;

	public PlaygroundParticlesC particles;

	public bool localManipulator;

	private ManipulatorObjectC manipulator;

	private void Start()
	{
		if (localManipulator)
		{
			manipulator = PlaygroundC.GetManipulator(manipulatorNumber, particles);
		}
		else
		{
			manipulator = PlaygroundC.GetManipulator(manipulatorNumber);
		}
	}

	private void Update()
	{
		for (int i = 0; i < particles.particleCache.Length; i++)
		{
			if (manipulator.Contains(particles.particleCache[i].position, manipulator.transform.position))
			{
				IsWithin(i);
			}
		}
	}

	private void IsWithin(int i)
	{
		Debug.Log("Particle " + i + " is within manipulator");
	}
}
