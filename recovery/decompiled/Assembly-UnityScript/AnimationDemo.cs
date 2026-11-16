using System;
using UnityEngine;

[Serializable]
public class AnimationDemo : MonoBehaviour
{
	public GameObject door1;

	public GameObject door2;

	public GameObject wallElectronics;

	public GameObject wallConsole;

	public GameObject controlPanelScreen;

	public Texture controlPanelLocked;

	public Texture controlPanelUnlocked;

	public virtual void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width / 2 - 125, 50f, 100f, 30f), "Open") && !door1.GetComponent<Animation>().isPlaying && !door2.GetComponent<Animation>().isPlaying && !wallElectronics.GetComponent<Animation>().isPlaying && !wallConsole.GetComponent<Animation>().isPlaying)
		{
			door1.GetComponent<Animation>()["Door_01"].speed = 1f;
			door2.GetComponent<Animation>()["Door_02"].speed = 1f;
			door1.GetComponent<Animation>()["Door_01"].time = 0f;
			door2.GetComponent<Animation>()["Door_02"].time = 0f;
			wallElectronics.GetComponent<Animation>()["WallElectronics_001_Lever"].speed = 1f;
			wallConsole.GetComponent<Animation>()["WallConsole"].speed = -1f;
			wallElectronics.GetComponent<Animation>()["WallElectronics_001_Lever"].time = 0f;
			wallConsole.GetComponent<Animation>()["WallConsole"].time = wallConsole.GetComponent<Animation>()["WallConsole"].length;
			door1.GetComponent<Animation>().Play();
			door2.GetComponent<Animation>().Play();
			wallElectronics.GetComponent<Animation>().Play();
			wallConsole.GetComponent<Animation>().Play();
			controlPanelScreen.GetComponent<Renderer>().material.mainTexture = controlPanelUnlocked;
		}
		if (GUI.Button(new Rect(Screen.width / 2 + 25, 50f, 100f, 30f), "Close") && !door1.GetComponent<Animation>().isPlaying && !door2.GetComponent<Animation>().isPlaying && !wallElectronics.GetComponent<Animation>().isPlaying && !wallConsole.GetComponent<Animation>().isPlaying)
		{
			door1.GetComponent<Animation>()["Door_01"].speed = -1f;
			door2.GetComponent<Animation>()["Door_02"].speed = -1f;
			door1.GetComponent<Animation>()["Door_01"].time = door1.GetComponent<Animation>()["Door_01"].length;
			door2.GetComponent<Animation>()["Door_02"].time = door2.GetComponent<Animation>()["Door_02"].length;
			wallElectronics.GetComponent<Animation>()["WallElectronics_001_Lever"].speed = -1f;
			wallConsole.GetComponent<Animation>()["WallConsole"].speed = 1f;
			wallElectronics.GetComponent<Animation>()["WallElectronics_001_Lever"].time = wallElectronics.GetComponent<Animation>()["WallElectronics_001_Lever"].length;
			wallConsole.GetComponent<Animation>()["WallConsole"].time = 0f;
			door1.GetComponent<Animation>().Play();
			door2.GetComponent<Animation>().Play();
			wallElectronics.GetComponent<Animation>().Play();
			wallConsole.GetComponent<Animation>().Play();
			controlPanelScreen.GetComponent<Renderer>().material.mainTexture = controlPanelLocked;
		}
	}

	public virtual void Main()
	{
	}
}
