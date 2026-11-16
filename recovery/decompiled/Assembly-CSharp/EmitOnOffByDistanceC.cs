using ParticlePlayground;
using UnityEngine;

public class EmitOnOffByDistanceC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform target;

	public float distance = 10f;

	private void Update()
	{
		particles.emit = Vector3.Distance(target.position, particles.particleSystemTransform.position) <= distance;
	}
}
