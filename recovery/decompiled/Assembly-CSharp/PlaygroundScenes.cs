using System.Collections;
using ParticlePlayground;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaygroundScenes : MonoBehaviour
{
	public bool showInfo = true;

	public bool listParticleSystems = true;

	public bool canSwitchSceneByClick = true;

	public PlaygroundParticlesC[] particles;

	private string productLabel = "Particle Playground";

	private string sceneLabel;

	private string contentLabel;

	private string systemInfoLabel;

	private string fpsLabel = "wait...";

	private string timeLabel;

	private string totalParticlesLabel;

	private int totalParticles;

	private int totalParticleSystems;

	private float updateRate = 2f;

	private float fps;

	private float highestFps;

	private float lowestFps = 9999f;

	private float deltaCount;

	private int frameCount;

	private bool ready;

	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		yield return null;
		PlaygroundScenes playgroundScenes = this;
		string text = playgroundScenes.productLabel;
		playgroundScenes.productLabel = text + " " + PlaygroundC.version + PlaygroundC.specialVersion;
		sceneLabel = SceneManager.GetActiveScene().name + " Scene";
		InvokeRepeating("UpdateParticlesLabel", 0f, 1f);
		systemInfoLabel = SystemInfo.deviceModel + "\n" + SystemInfo.processorType + "\n(" + SystemInfo.processorCount + " cores, " + SystemInfo.systemMemorySize + "MB RAM)\n" + Screen.width + "x" + Screen.height + " @" + Screen.currentResolution.refreshRate + " (" + SystemInfo.graphicsMemorySize + "MB VRAM)";
		ready = true;
	}

	private void Update()
	{
		if (!ready)
		{
			return;
		}
		if (showInfo)
		{
			frameCount++;
			deltaCount += Time.deltaTime;
			if (deltaCount > 1f / updateRate)
			{
				fps = (float)frameCount / deltaCount;
				frameCount = 0;
				deltaCount -= 1f / updateRate;
				if (fps > highestFps)
				{
					highestFps = fps;
				}
				if (fps < lowestFps)
				{
					lowestFps = fps;
				}
				fpsLabel = "FPS: " + fps.ToString("f0") + "\nHighest: " + highestFps.ToString("f0") + "\nLowest: " + lowestFps.ToString("f0");
			}
		}
		if ((((Input.GetMouseButtonDown(0) && canSwitchSceneByClick) || Input.touchCount > 0) && canSwitchSceneByClick) || Input.GetKeyDown(KeyCode.Return))
		{
			LoadNext();
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void LoadNext()
	{
		SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCount);
	}

	private void OnGUI()
	{
		GUILayout.Label(productLabel);
		if (showInfo)
		{
			GUILayout.Label(sceneLabel);
			GUILayout.Label(contentLabel);
			GUILayout.Label(totalParticlesLabel);
			GUILayout.Label(systemInfoLabel);
			GUILayout.Label(fpsLabel);
			GUILayout.Label(timeLabel);
		}
	}

	public void UpdateParticlesLabel()
	{
		totalParticles = 0;
		totalParticleSystems = 0;
		contentLabel = string.Empty;
		PlaygroundParticlesC[] array = particles;
		foreach (PlaygroundParticlesC playgroundParticlesC in array)
		{
			if (listParticleSystems)
			{
				string text = contentLabel;
				contentLabel = text + playgroundParticlesC.particleSystemTransform.name + " (" + playgroundParticlesC.particleCount + " " + SourceMethod(playgroundParticlesC) + " particles)\n";
			}
			totalParticles += playgroundParticlesC.particleCount;
			totalParticleSystems++;
		}
		totalParticlesLabel = "Total " + totalParticles + " particles (" + totalParticleSystems + ")";
		timeLabel = "Seconds: " + Time.timeSinceLevelLoad.ToString("f0");
	}

	private string SourceMethod(PlaygroundParticlesC source)
	{
		return source.source switch
		{
			SOURCEC.Paint => "painted", 
			SOURCEC.Projection => "projected", 
			SOURCEC.SkinnedWorldObject => "skinned mesh", 
			SOURCEC.State => "state", 
			SOURCEC.Transform => "transform", 
			SOURCEC.WorldObject => "mesh", 
			SOURCEC.Script => (source.eventControlledBy.Count <= 0) ? "scripted" : "event", 
			_ => string.Empty, 
		};
	}
}
