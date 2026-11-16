using TrueSync;
using UnityEngine;
using UnityEngine.UI;

public class GoalBehavior : TrueSyncBehaviour
{
	[AddTracking]
	public int score;

	public Text scoreText;

	public GameEndHandler gameEndHandler;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		scoreText.text = "0";
	}

	public void OnSyncedTriggerEnter(TSCollision2D otherBody)
	{
		if (otherBody.gameObject.tag == "ball")
		{
			score++;
			UpdateScore();
			otherBody.gameObject.SendMessage("GoalScored");
			gameEndHandler.gameObject.SendMessage("GoalScored", this);
		}
	}

	private void UpdateScore()
	{
		audioSource.Play();
		scoreText.text = score.ToString();
	}
}
