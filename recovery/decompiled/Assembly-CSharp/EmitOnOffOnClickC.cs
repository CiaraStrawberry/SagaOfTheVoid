using ParticlePlayground;
using UnityEngine;

public class EmitOnOffOnClickC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			particles.Emit(!particles.emit);
		}
	}
}
