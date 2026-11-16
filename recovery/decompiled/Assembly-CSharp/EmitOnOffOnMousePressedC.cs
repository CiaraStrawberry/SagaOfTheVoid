using ParticlePlayground;
using UnityEngine;

public class EmitOnOffOnMousePressedC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	private void Update()
	{
		particles.emit = Input.GetMouseButton(0);
	}
}
