using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class MainMenuValueHolder : MonoBehaviour
{
	public VRTK_SDKSetup loadedsetup;

	public bool testarea;

	public bool tutorial;

	public bool currentlyloading;

	public bool campaign;

	public MainMenuCampaignControlScript.mapcontainer selectedcampaign;

	public List<GameObject> TutorialObjs = new List<GameObject>();

	private void Start()
	{
		if (Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
		}
		Object.DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnApplicationQuit()
	{
		if (!Application.isEditor)
		{
			Process.GetCurrentProcess().Kill();
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode inputmode)
	{
		if (scene.buildIndex == 0)
		{
			testarea = false;
			tutorial = false;
			campaign = false;
			if (GameObject.Find("CrossLevelVariables") != null)
			{
				CrossLevelVariableHolder component = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
				component.tutorial = false;
				component.campaign = false;
			}
		}
	}

	private void Update()
	{
	}
}
