using ParticlePlayground;
using UnityEngine;

public class EnableDisableByDistanceC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform target;

	public float distance = 10f;

	private void Update()
	{
		particles.particleSystemGameObject.SetActive(Vector3.Distance(target.position, particles.particleSystemTransform.position) <= distance);
	}
}
