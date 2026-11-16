using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("Excelsior/CSFHI/Example UI")]
public class ExampleUI : MonoBehaviour
{
	public Text displayedName;

	private InterfaceAnimManager current;

	private int indexInterface;

	public InterfaceAnimManager[] holoInterfaceList;

	private ExampleCam currentCam;

	public string searchTarget = "HoloInterfaces";

	public string searchCam = "currentCam";

	private int indexScene;

	private static bool allDelaysStatus = true;

	private static bool allImgEffectsStatus = true;

	public List<string> loadableScenes = new List<string>();

	public GameObject fullscreenSwith;

	public AudioSource audioSource;

	private void Start()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		if (holoInterfaceList[indexInterface] != null)
		{
			CallInterface(holoInterfaceList[indexInterface]);
		}
		InitCam();
	}

	private void playSound()
	{
		if ((bool)audioSource && audioSource.enabled)
		{
			audioSource.Play();
		}
	}

	public void DoOutFullscreen()
	{
		if (Screen.fullScreen)
		{
			Screen.fullScreen = false;
		}
		playSound();
	}

	public void DoChangeEnvironment()
	{
		indexScene++;
		if (indexScene >= loadableScenes.Count)
		{
			indexScene = 0;
		}
		playSound();
		SceneManager.LoadScene(loadableScenes[indexScene]);
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		InitCam();
	}

	private void InitCam()
	{
		if (!(searchCam != string.Empty))
		{
			return;
		}
		currentCam = GameObject.Find(searchCam).GetComponent<ExampleCam>();
		if ((bool)currentCam && searchTarget != string.Empty)
		{
			currentCam.mouseOrbit.target = GameObject.Find(searchTarget).transform;
		}
		if (!allImgEffectsStatus)
		{
			foreach (MonoBehaviour imageEffects in currentCam.ImageEffectsList)
			{
				imageEffects.enabled = allImgEffectsStatus;
			}
		}
		if (!allDelaysStatus)
		{
			InterfaceAnimManager[] array = holoInterfaceList;
			foreach (InterfaceAnimManager interfaceAnimManager in array)
			{
				interfaceAnimManager.useDelays = allDelaysStatus;
			}
		}
	}

	public void DoSwitchAutoCam()
	{
		currentCam.mouseOrbit.enabled = !currentCam.mouseOrbit.enabled;
	}

	public void DoSwitchCameraEffects()
	{
		allImgEffectsStatus = !allImgEffectsStatus;
		foreach (MonoBehaviour imageEffects in currentCam.ImageEffectsList)
		{
			imageEffects.enabled = allImgEffectsStatus;
		}
		if ((bool)audioSource && audioSource.enabled)
		{
			audioSource.Play();
		}
	}

	public void DoSwitchAnimDelays()
	{
		allDelaysStatus = !allDelaysStatus;
		InterfaceAnimManager[] array = holoInterfaceList;
		foreach (InterfaceAnimManager interfaceAnimManager in array)
		{
			interfaceAnimManager.useDelays = allDelaysStatus;
		}
		if ((bool)audioSource && audioSource.enabled)
		{
			audioSource.Play();
		}
	}

	public void DoAppear()
	{
		if ((bool)current)
		{
			current.startAppear();
			current.gameObject.SetActive(value: true);
		}
		playSound();
	}

	public void DoDisappear()
	{
		if ((bool)current)
		{
			current.startDisappear();
		}
		playSound();
	}

	public void DoNext()
	{
		indexInterface++;
		if (indexInterface >= holoInterfaceList.Length)
		{
			indexInterface = 0;
		}
		playSound();
		CallInterface(holoInterfaceList[indexInterface]);
	}

	public void DoPrevious()
	{
		indexInterface--;
		if (indexInterface < 0)
		{
			indexInterface = holoInterfaceList.Length - 1;
		}
		playSound();
		CallInterface(holoInterfaceList[indexInterface]);
	}

	private void CallInterface(InterfaceAnimManager _interface)
	{
		if ((bool)current)
		{
			current.startDisappear(direct: true);
		}
		current = _interface;
		if ((bool)_interface)
		{
			current.gameObject.SetActive(value: true);
			current.startAppear();
			UpdateDisplayedInfo();
		}
	}

	public void UpdateDisplayedInfo()
	{
		if ((bool)current)
		{
			displayedName.text = current.name.ToString();
		}
		else
		{
			displayedName.text = string.Empty;
		}
	}

	private void Update()
	{
		fullscreenSwith.SetActive(Screen.fullScreen);
	}
}
