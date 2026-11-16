using ParticlePlayground;
using PlaygroundSplines;
using UnityEngine;

public class PlaygroundDragSplineObject : MonoBehaviour
{
	private static Camera mainCam;

	private static Transform mainCamTransform;

	private static bool isDragging;

	private static ManipulatorObjectC manipulator;

	private static PlaygroundTurbulenceOnGameObjects turbulenceScript;

	public PlaygroundSpline mySpline;

	public Material enterMaterial;

	private Material exitMaterial;

	private Transform t;

	private Renderer r;

	private float camZ;

	private void Start()
	{
		if (mainCam == null)
		{
			mainCam = Camera.main;
			mainCamTransform = mainCam.transform;
			PlaygroundParticlesC[] array = Object.FindObjectsOfType<PlaygroundParticlesC>();
			PlaygroundParticlesC[] array2 = array;
			foreach (PlaygroundParticlesC playgroundParticlesC in array2)
			{
				if (playgroundParticlesC.manipulators.Count > 0)
				{
					manipulator = playgroundParticlesC.manipulators[0];
					break;
				}
			}
			turbulenceScript = Object.FindObjectOfType<PlaygroundTurbulenceOnGameObjects>();
		}
		r = GetComponent<Renderer>();
		t = transform;
		exitMaterial = r.sharedMaterial;
		camZ = 0f - mainCamTransform.position.z + t.position.z;
	}

	private void OnMouseOver()
	{
		if (!isDragging)
		{
			r.sharedMaterial = enterMaterial;
		}
	}

	private void OnMouseExit()
	{
		if (!isDragging)
		{
			r.sharedMaterial = exitMaterial;
		}
	}

	private void OnMouseDrag()
	{
		if (!isDragging)
		{
			turbulenceScript.StopTurbulenceOnTransform(t);
		}
		isDragging = true;
		r.sharedMaterial = enterMaterial;
		t.position = Vector3.Lerp(t.position, mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camZ)), Time.deltaTime * 10f);
		manipulator.strength = Mathf.Lerp(manipulator.strength, 3f, Time.deltaTime);
		manipulator.size = manipulator.strength * 4f;
		float x = t.localScale.x;
		x = Mathf.Lerp(x, 1f + Mathf.Sin(Time.time * 20f) * 0.5f, Time.deltaTime * 3f);
		t.localScale = new Vector3(x, x, x);
	}

	private void OnMouseDown()
	{
		SetManipulatorTransform(t);
	}

	private void OnMouseUp()
	{
		isDragging = false;
		r.sharedMaterial = exitMaterial;
		t.localScale = new Vector3(1f, 1f, 1f);
		SetManipulatorTransform(null);
		manipulator.strength = 0f;
		manipulator.size = 0f;
		turbulenceScript.StartTurbulenceOnTransform(t);
	}

	private void SetManipulatorTransform(Transform mTransform)
	{
		manipulator.transform.transform = mTransform;
		manipulator.properties[0].splineTarget = mySpline;
	}
}
