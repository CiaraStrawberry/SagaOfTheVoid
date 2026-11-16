using Galaxia;
using UnityEngine;

public class VRGalaxyManager : MonoBehaviour
{
	public Galaxy Galaxy;

	public Transform camera1;

	public Transform viewPointsGroup;

	public bool offscreenParticlesEnabled = true;

	public bool hdr = true;

	public bool adjustCameraRotation;

	private bool lastOffscreenParticlesEnabled = true;

	private bool lastHDR = true;

	private int viewsCounter;

	private void Start()
	{
		ApplyOffscreenParticleStates();
		lastOffscreenParticlesEnabled = offscreenParticlesEnabled;
		lastHDR = hdr;
	}

	private void OnGUI()
	{
		Transform child = viewPointsGroup.GetChild(viewsCounter % viewPointsGroup.childCount);
		GUILayout.Label("Currently view: " + child.name);
		GUILayout.Label("O - Toggle Offscreen Galaxy Rendering");
		GUILayout.Label("V - Cycle Views");
		GUILayout.Label("H - Toggle HDR");
		offscreenParticlesEnabled = GUILayout.Toggle(offscreenParticlesEnabled, "Offscreen Particles");
		if (offscreenParticlesEnabled)
		{
			hdr = GUILayout.Toggle(hdr, "HDR");
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			offscreenParticlesEnabled = !offscreenParticlesEnabled;
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			viewsCounter++;
			Transform child = viewPointsGroup.GetChild(viewsCounter % viewPointsGroup.childCount);
			camera1.transform.position = child.position;
			if (adjustCameraRotation)
			{
				camera1.transform.rotation = child.rotation;
			}
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			hdr = !hdr;
		}
		if (offscreenParticlesEnabled != lastOffscreenParticlesEnabled || hdr != lastHDR)
		{
			lastOffscreenParticlesEnabled = offscreenParticlesEnabled;
			lastHDR = hdr;
			ApplyOffscreenParticleStates();
		}
	}

	private void ApplyOffscreenParticleStates()
	{
		OffscreenGalaxyRenderer[] components = camera1.GetComponents<OffscreenGalaxyRenderer>();
		foreach (OffscreenGalaxyRenderer offscreenGalaxyRenderer in components)
		{
			offscreenGalaxyRenderer.enabled = offscreenParticlesEnabled;
			offscreenGalaxyRenderer.Hdr = hdr;
		}
		OffscreenGalaxyRenderer[] componentsInChildren = camera1.GetComponentsInChildren<OffscreenGalaxyRenderer>();
		foreach (OffscreenGalaxyRenderer offscreenGalaxyRenderer2 in componentsInChildren)
		{
			offscreenGalaxyRenderer2.enabled = offscreenParticlesEnabled;
			offscreenGalaxyRenderer2.Hdr = hdr;
		}
	}
}
