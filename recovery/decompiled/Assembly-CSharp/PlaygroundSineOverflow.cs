using ParticlePlayground;
using UnityEngine;

public class PlaygroundSineOverflow : MonoBehaviour
{
	private PlaygroundParticlesC particles;

	private float amount = 0.1f;

	private Vector3 initialOverflow;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		initialOverflow = particles.overflowOffset;
	}

	private void Update()
	{
		particles.overflowOffset = initialOverflow + new Vector3(Mathf.Cos(Time.time) * amount, Mathf.Sin(Time.time) * amount, Mathf.Cos(Time.time) * amount);
	}
}
