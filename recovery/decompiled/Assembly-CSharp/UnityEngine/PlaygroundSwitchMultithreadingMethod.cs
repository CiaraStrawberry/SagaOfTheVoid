using ParticlePlayground;

namespace UnityEngine;

public class PlaygroundSwitchMultithreadingMethod : MonoBehaviour
{
	private ManipulatorObjectC globalManipulator;

	private void Start()
	{
		globalManipulator = PlaygroundC.GetManipulator(0);
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo, 1000f))
		{
			globalManipulator.transform.transform.position = hitInfo.point;
		}
	}

	private void OnGUI()
	{
		if (PlaygroundC.reference.manipulators.Count > 0)
		{
			GUI.Label(new Rect(Screen.width - 620, 10f, 300f, 25f), "Global Manipulator:");
			globalManipulator.enabled = GUI.Toggle(new Rect(Screen.width - 620, 35f, 300f, 20f), globalManipulator.enabled, "Enabled");
			if (GUI.Button(new Rect(Screen.width - 620, 55f, 300f, 20f), "Type: " + globalManipulator.type))
			{
				SwitchManipulator();
			}
			GUI.Label(new Rect(Screen.width - 620, 80f, 120f, 30f), "Strength");
			globalManipulator.strength = GUI.HorizontalSlider(new Rect(Screen.width - 560, 85f, 210f, 30f), globalManipulator.strength, 0f, 10f);
			GUI.Label(new Rect(Screen.width - 340, 80f, 60f, 30f), globalManipulator.strength.ToString("f1"));
		}
		GUI.Label(new Rect(Screen.width - 310, 10f, 300f, 25f), "Multithreading:");
		if (GUI.Button(new Rect(Screen.width - 310, 35f, 300f, 20f), "Thread Pool Method: " + PlaygroundC.reference.threadPoolMethod))
		{
			SwitchMultithreading.NextThreadPoolMethod();
		}
		if (GUI.Button(new Rect(Screen.width - 310, 55f, 300f, 20f), "Particle System Method: " + PlaygroundC.reference.threadMethod))
		{
			SwitchMultithreading.NextParticleSystemMethod();
		}
		if (GUI.Button(new Rect(Screen.width - 310, 75f, 300f, 20f), "Skinned Mesh Method: " + PlaygroundC.reference.skinnedMeshThreadMethod))
		{
			SwitchMultithreading.NextSkinnedMeshMethod();
		}
		if (GUI.Button(new Rect(Screen.width - 310, 95f, 300f, 20f), "Turbulence Method: " + PlaygroundC.reference.turbulenceThreadMethod))
		{
			SwitchMultithreading.NextTurbulenceMethod();
		}
	}

	private void SwitchManipulator()
	{
		switch (globalManipulator.type)
		{
		case MANIPULATORTYPEC.Attractor:
			globalManipulator.type = MANIPULATORTYPEC.AttractorGravitational;
			break;
		case MANIPULATORTYPEC.AttractorGravitational:
			globalManipulator.type = MANIPULATORTYPEC.Repellent;
			break;
		case MANIPULATORTYPEC.Repellent:
			globalManipulator.type = MANIPULATORTYPEC.Vortex;
			break;
		case MANIPULATORTYPEC.Vortex:
			globalManipulator.type = MANIPULATORTYPEC.Attractor;
			break;
		case MANIPULATORTYPEC.Property:
		case MANIPULATORTYPEC.Combined:
			break;
		}
	}
}
