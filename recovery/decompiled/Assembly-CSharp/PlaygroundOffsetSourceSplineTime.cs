using ParticlePlayground;
using UnityEngine;

public class PlaygroundOffsetSourceSplineTime : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public float offsetSpeed = 1f;

	public bool showGui = true;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		particles.splineTimeOffset = (particles.splineTimeOffset + offsetSpeed * Time.deltaTime) % 1f;
	}

	private void OnGUI()
	{
		if (showGui)
		{
			GUI.Box(new Rect(Screen.width - 220, 0f, 220f, 70f), "Spline Offset");
			GUI.Label(new Rect(Screen.width - 210, 30f, 200f, 30f), "Offset Speed: " + offsetSpeed.ToString("f1"));
			offsetSpeed = GUI.HorizontalSlider(new Rect(Screen.width - 210, 55f, 200f, 30f), offsetSpeed, 0f, 2f);
		}
	}
}
