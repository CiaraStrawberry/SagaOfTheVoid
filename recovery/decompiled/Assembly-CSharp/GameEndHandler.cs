using TrueSync;
using UnityEngine;
using UnityEngine.UI;

public class GameEndHandler : TrueSyncBehaviour
{
	public int winScore;

	public Text resultText;

	public GameObject resultScreen;

	private void Start()
	{
		resultScreen.SetActive(value: false);
	}

	private void GoalScored(GoalBehavior goal)
	{
		if (goal.score >= winScore)
		{
			gameObject.SetActive(value: false);
			if (localOwner.Id == goal.owner.Id)
			{
				HandleWin();
			}
			else
			{
				HandleLoss();
			}
		}
	}

	private void HandleWin()
	{
		resultText.text = "YOU WIN";
		ShowMessageScreen();
	}

	private void HandleLoss()
	{
		resultText.text = "YOU LOSE";
		ShowMessageScreen();
	}

	private void ShowMessageScreen()
	{
		resultScreen.SetActive(value: true);
	}
}
