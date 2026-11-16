using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolTipController : MonoBehaviour
{
	private void Awake()
	{
		GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.Add(gameObject);
	}

	private void OnEnable()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
		{
			gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
