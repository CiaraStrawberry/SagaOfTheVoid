using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMapLoader2 : MonoBehaviour
{
	private int a;

	private void Start()
	{
	}

	private void Update()
	{
		a++;
		if (a == 1)
		{
			SceneManager.LoadScene(0);
		}
	}
}
