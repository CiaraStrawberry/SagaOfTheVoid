using ParticlePlayground;

namespace UnityEngine;

public class PlaygroundCycleSnapshots : MonoBehaviour
{
	private PlaygroundParticlesC particles;

	private PlaygroundScenes sceneScript;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		sceneScript = Object.FindObjectOfType<PlaygroundScenes>();
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 160, 10f, 150f, 32f), particles.snapshots[particles.loadFrom % particles.snapshots.Count].name))
		{
			particles.Load(++particles.loadFrom);
			if (sceneScript != null)
			{
				sceneScript.UpdateParticlesLabel();
			}
		}
	}
}
