using Galaxia;
using UnityEngine;

public class GalaxyDemoManager : MonoBehaviour
{
	public GalaxyPrefab[] galaxyPrefabs;

	public Galaxy Galaxy;

	public OffscreenGalaxyRenderer OffscreenGalaxyRenderer;

	private void Start()
	{
	}

	private void OnGUI()
	{
		float num = 10f;
		for (int i = 0; i < galaxyPrefabs.Length; i++)
		{
			if (GUI.Button(new Rect(10f, num, 128f, 32f), galaxyPrefabs[i].name))
			{
				Galaxy.GalaxyPrefab = galaxyPrefabs[i];
			}
			num += 37f;
		}
		Galaxy.GPU = GUI.Toggle(new Rect(10f, num, 128f, 32f), Galaxy.GPU, "GPU");
		num += 37f;
		OffscreenGalaxyRenderer.enabled = GUI.Toggle(new Rect(10f, num, 128f, 32f), OffscreenGalaxyRenderer.enabled, "Off-Screen Particles");
		string empty = string.Empty;
		empty = "Shader model " + SystemInfo.graphicsShaderLevel;
		empty = ((SystemInfo.graphicsShaderLevel < 40) ? (empty + " (Does not support DirectX11)") : (empty + " (Supports DirectX11)"));
		empty = empty + "\n " + SystemInfo.graphicsDeviceVersion;
		GUI.Label(text: (!SystemInfo.graphicsDeviceVersion.Contains("OpenGL")) ? (empty + " (Uses Direct3D)") : (empty + " (Uses OpenGL)"), position: new Rect(Screen.width - 128, 10f, 128f, 128f));
	}

	private void Update()
	{
	}
}
