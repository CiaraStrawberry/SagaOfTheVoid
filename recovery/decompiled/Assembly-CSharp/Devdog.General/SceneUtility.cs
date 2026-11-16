using UnityEngine;
using UnityEngine.SceneManagement;

namespace Devdog.General;

public static class SceneUtility
{
	public static AsyncOperation LoadSceneAsync(string name)
	{
		AsyncOperation asyncOperation = null;
		return SceneManager.LoadSceneAsync(name);
	}

	public static AsyncOperation LoadSceneAsync(int id)
	{
		AsyncOperation asyncOperation = null;
		return SceneManager.LoadSceneAsync(id);
	}

	public static void LoadScene(string name)
	{
		SceneManager.LoadScene(name);
	}

	public static void LoadScene(int id)
	{
		SceneManager.LoadScene(id);
	}
}
