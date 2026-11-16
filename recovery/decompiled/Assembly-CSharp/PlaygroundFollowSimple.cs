using ParticlePlayground;
using UnityEngine;

public class PlaygroundFollowSimple : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform followerReference;

	public bool rotateTowardsDirection;

	private Transform[] _followers;

	private void Start()
	{
		_followers = new Transform[particles.particleCount];
		for (int i = 0; i < particles.particleCount; i++)
		{
			_followers[i] = Object.Instantiate(followerReference).transform;
			_followers[i].parent = transform;
		}
		followerReference.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (_followers.Length != particles.particleCount)
		{
			return;
		}
		for (int i = 0; i < _followers.Length; i++)
		{
			_followers[i].position = particles.particleCache[i].position;
			if (rotateTowardsDirection && particles.playgroundCache.velocity[i] != Vector3.zero)
			{
				_followers[i].rotation = Quaternion.LookRotation(particles.playgroundCache.velocity[i]);
			}
		}
	}
}
