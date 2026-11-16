using TrueSync;
using UnityEngine;

public class BallBehavior : TrueSyncBehaviour
{
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public override void OnSyncedStart()
	{
		ResetProperties();
	}

	public override void OnSyncedUpdate()
	{
		tsTransform2D.rotation -= (FP)5;
	}

	public void OnSyncedCollisionEnter(TSCollision2D other)
	{
		if (other.gameObject.tag == "player")
		{
			tsRigidBody2D.AddForce(new TSVector2(0, -500));
		}
		if (other.gameObject.tag != "goal")
		{
			audioSource.Play();
		}
	}

	private void GoalScored()
	{
		ResetProperties();
	}

	public void ResetProperties()
	{
		tsRigidBody2D.position = new TSVector2(0, 5);
		tsRigidBody2D.velocity = TSVector2.zero;
	}
}
