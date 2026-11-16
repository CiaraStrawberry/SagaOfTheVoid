using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnLoad : MonoBehaviour
{
	private void Start()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		if (gameObject.name != "GameObjectives" && Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (this != null && gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	private void Update()
	{
	}
}
