using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMapLoader : MonoBehaviour
{
	private void Start()
	{
		if (Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
			return;
		}
		Object.DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene(5);
	}

	private void Update()
	{
	}
}
