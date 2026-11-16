using ParticlePlayground;
using UnityEngine;

public class FollowerEventExample : MonoBehaviour
{
	public PlaygroundFollow followScript;

	private void OnEnable()
	{
		if (followScript != null)
		{
			followScript.sendEvents = true;
			followScript.followerEventBirth += OnFollowerBirth;
			followScript.followerEventDeath += OnFollowerDeath;
		}
	}

	private void OnDisable()
	{
		if (followScript != null)
		{
			followScript.followerEventBirth -= OnFollowerBirth;
			followScript.followerEventDeath -= OnFollowerDeath;
		}
	}

	private void OnFollowerBirth(PlaygroundFollower follower)
	{
		Debug.Log("Follower " + follower.gameObject.name + " is now tracking the particle id: " + follower.particleId);
	}

	private void OnFollowerDeath(PlaygroundFollower follower)
	{
		Debug.Log("Follower " + follower.gameObject.name + " died at position " + follower.transform.position);
	}
}
