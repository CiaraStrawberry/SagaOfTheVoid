using UnityEngine;

public class HelpButtonSender : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Help()
	{
		if (GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs[0].GetActive())
		{
			sethelpactive(activetoset: false);
		}
		else
		{
			sethelpactive(activetoset: true);
		}
	}

	private void sethelpactive(bool activetoset)
	{
		GameObject[] array = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null) || !(gameObject.gameObject != null))
			{
				continue;
			}
			foreach (Transform componentInChild in gameObject.GetComponentInChildren<Transform>(includeInactive: true))
			{
				gameObject.SetActive(value: true);
			}
			gameObject.SetActive(activetoset);
		}
	}
}
