using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayerControlScript : MonoBehaviour
{
	private void Start()
	{
		if (Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
			return;
		}
		Object.DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void Update()
	{
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6 && this != null && gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}
}
