using ParticlePlayground;

namespace UnityEngine;

public class PlaygroundProceduralMeshTarget : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public float manipulatorSize = 100f;

	public float manipulatorStrength = 20f;

	public float proceduralMeshSize = 3f;

	public int proceduralMeshRecursion = 1;

	public float rotationSpeed = 10f;

	private ManipulatorObjectC manipulator;

	private int subdivisionFactor;

	private void Start()
	{
		IsoSphere.CreateMesh(gameObject, proceduralMeshRecursion, proceduralMeshSize, visible: false);
		if (particles != null)
		{
			manipulator = PlaygroundC.ManipulatorObject(gameObject.transform, particles);
			manipulator.type = MANIPULATORTYPEC.Property;
			manipulator.property.type = MANIPULATORPROPERTYTYPEC.MeshTarget;
			manipulator.property.meshTarget.gameObject = gameObject;
			manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Linear;
		}
		else
		{
			Debug.Log("You must assign a Particle Playground system to this script.", gameObject);
		}
	}

	private void Update()
	{
		particles.particleSystemTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
		manipulator.size = manipulatorSize;
		manipulator.strength = manipulatorStrength;
		transform.RotateAround(Vector3.zero, Vector3.up, (0f - rotationSpeed) * Time.deltaTime);
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0f, 0f, 220f, 125f), "Manipulator");
		GUI.Label(new Rect(10f, 30f, 200f, 30f), "Strength: " + manipulatorStrength.ToString("f1"));
		manipulatorStrength = GUI.HorizontalSlider(new Rect(10f, 55f, 200f, 30f), manipulatorStrength, 0f, 30f);
		if (GUI.Button(new Rect(10f, 85f, 200f, 30f), "Transition: " + manipulator.property.transition))
		{
			if (manipulator.property.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
			{
				manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Lerp;
			}
			else
			{
				manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Linear;
			}
		}
		GUI.Box(new Rect(240f, 0f, 220f, 125f), "Turbulence");
		GUI.Label(new Rect(250f, 30f, 200f, 30f), "Strength: " + particles.turbulenceStrength.ToString("f1"));
		particles.turbulenceStrength = GUI.HorizontalSlider(new Rect(250f, 55f, 200f, 30f), particles.turbulenceStrength, 0f, 50f);
		if (GUI.Button(new Rect(250f, 85f, 200f, 30f), "Type: " + particles.turbulenceType))
		{
			if (particles.turbulenceType == TURBULENCETYPE.Perlin)
			{
				particles.turbulenceType = TURBULENCETYPE.Simplex;
			}
			else
			{
				particles.turbulenceType = TURBULENCETYPE.Perlin;
			}
		}
		GUI.Box(new Rect(Screen.width - 220, 0f, 220f, 125f), "Mesh");
		GUI.Label(new Rect(Screen.width - 210, 30f, 200f, 30f), "Rotation Speed: " + rotationSpeed.ToString("f1"));
		rotationSpeed = GUI.HorizontalSlider(new Rect(Screen.width - 210, 55f, 200f, 30f), rotationSpeed, -100f, 100f);
		if (GUI.Button(new Rect(Screen.width - 210, 85f, 200f, 30f), "Subdivide x" + subdivisionFactor))
		{
			subdivisionFactor++;
			subdivisionFactor %= 4;
			if (subdivisionFactor == 0)
			{
				IsoSphere.CreateMesh(gameObject, 1, proceduralMeshSize, visible: false);
			}
			else
			{
				SubdivideMesh.Subdivide(gameObject.GetComponent<MeshFilter>().mesh);
			}
			UpdateTargetManipulator();
		}
	}

	private void UpdateTargetManipulator()
	{
		manipulator.property.meshTarget.Update();
	}
}
