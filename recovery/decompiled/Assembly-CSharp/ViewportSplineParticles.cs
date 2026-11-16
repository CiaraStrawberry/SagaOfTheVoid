using ParticlePlayground;
using PlaygroundSplines;
using UnityEngine;

public class ViewportSplineParticles : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public float screenOffset;

	private PlaygroundSpline spline;

	private Camera mainCamera;

	private int currentScreenX;

	private int currentScreenY;

	private float prevScreenOffset;

	private void Start()
	{
		mainCamera = Camera.main;
		particles.particleSystemTransform.parent = mainCamera.transform;
		ParticleSystem.MainModule main = particles.shurikenParticleSystem.main;
		main.simulationSpace = ParticleSystemSimulationSpace.Local;
		spline = new GameObject("Viewport Spline", typeof(PlaygroundSpline)).GetComponent<PlaygroundSpline>();
		spline.Loop = true;
		spline.Reset();
		spline.SetControlPointMode(0, BezierControlPointMode.Free);
		spline.SetControlPointMode(1, BezierControlPointMode.Free);
		spline.AddNode();
		spline.AddNode();
		spline.AddNode();
		currentScreenX = Screen.width;
		currentScreenY = Screen.height;
		SetViewportNodes();
		particles.splines.Add(spline);
		particles.source = SOURCEC.Spline;
		prevScreenOffset = screenOffset;
	}

	private void Update()
	{
		if (currentScreenX != Screen.width || currentScreenY != Screen.height || prevScreenOffset != screenOffset)
		{
			currentScreenX = Screen.width;
			currentScreenY = Screen.height;
			prevScreenOffset = screenOffset;
			SetViewportNodes();
		}
	}

	private void SetViewportNodes()
	{
		Vector3[] array = new Vector3[13];
		ref Vector3 reference = ref array[0];
		reference = mainCamera.ViewportToWorldPoint(new Vector3(screenOffset, screenOffset, 10f));
		ref Vector3 reference2 = ref array[3];
		reference2 = mainCamera.ViewportToWorldPoint(new Vector3(screenOffset, 1f - screenOffset, 10f));
		ref Vector3 reference3 = ref array[6];
		reference3 = mainCamera.ViewportToWorldPoint(new Vector3(1f - screenOffset, 1f - screenOffset, 10f));
		ref Vector3 reference4 = ref array[9];
		reference4 = mainCamera.ViewportToWorldPoint(new Vector3(1f - screenOffset, screenOffset, 10f));
		ref Vector3 reference5 = ref array[12];
		reference5 = mainCamera.ViewportToWorldPoint(new Vector3(screenOffset, screenOffset, 10f));
		ref Vector3 reference6 = ref array[1];
		reference6 = mainCamera.ViewportToWorldPoint(new Vector3(screenOffset, 0.5f, 10f));
		ref Vector3 reference7 = ref array[2];
		reference7 = mainCamera.ViewportToWorldPoint(new Vector3(screenOffset, 0.5f, 10f));
		ref Vector3 reference8 = ref array[4];
		reference8 = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f - screenOffset, 10f));
		ref Vector3 reference9 = ref array[5];
		reference9 = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f - screenOffset, 10f));
		ref Vector3 reference10 = ref array[7];
		reference10 = mainCamera.ViewportToWorldPoint(new Vector3(1f - screenOffset, 0.5f, 10f));
		ref Vector3 reference11 = ref array[8];
		reference11 = mainCamera.ViewportToWorldPoint(new Vector3(1f - screenOffset, 0.5f, 10f));
		ref Vector3 reference12 = ref array[10];
		reference12 = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, screenOffset, 10f));
		ref Vector3 reference13 = ref array[11];
		reference13 = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, screenOffset, 10f));
		spline.SetPoints(array);
	}
}
