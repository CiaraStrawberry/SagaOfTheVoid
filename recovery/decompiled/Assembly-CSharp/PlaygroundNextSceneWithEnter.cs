using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaygroundNextSceneWithEnter : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			LoadNext();
		}
	}

	private void LoadNext()
	{
		SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCount);
	}
}
