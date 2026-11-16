using ParticlePlayground;
using UnityEngine;

public class PlaygroundTurbulenceOnGameObjects : MonoBehaviour
{
	public Transform[] transforms;

	public bool turbulence3d;

	public float turbulenceFieldScale = 1f;

	public float turbulenceTimeScale = 1f;

	public float turbulenceStrength = 1f;

	public float damping = 1f;

	public bool returnToOrigin = true;

	public float returnSpeed = 1f;

	private SimplexNoise simplex = new SimplexNoise();

	private Vector3[] originPositions;

	private Vector3[] velocity;

	private bool[] hasTurbulence;

	private Vector3 zero = Vector3.zero;

	private void Start()
	{
		originPositions = new Vector3[transforms.Length];
		velocity = new Vector3[transforms.Length];
		hasTurbulence = new bool[transforms.Length];
		for (int i = 0; i < transforms.Length; i++)
		{
			ref Vector3 reference = ref originPositions[i];
			reference = transforms[i].position;
			ref Vector3 reference2 = ref velocity[i];
			reference2 = Vector3.zero;
			hasTurbulence[i] = true;
		}
	}

	private void Update()
	{
		Turbulence();
	}

	private void Turbulence()
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			if (hasTurbulence[i])
			{
				velocity[i].x += SimplexValue(transforms[i].position.x, transforms[i].position.y, transforms[i].position.z);
				velocity[i].y += SimplexValue(transforms[i].position.z, transforms[i].position.x, transforms[i].position.y);
				if (turbulence3d)
				{
					velocity[i].z += SimplexValue(transforms[i].position.y, transforms[i].position.z, transforms[i].position.x);
				}
				ref Vector3 reference = ref velocity[i];
				reference = Vector3.Lerp(velocity[i], zero, Time.deltaTime * damping);
				transforms[i].position += velocity[i] * Time.deltaTime;
				if (returnToOrigin)
				{
					transforms[i].position = Vector3.Lerp(transforms[i].position, originPositions[i], Time.deltaTime * returnSpeed);
				}
			}
		}
	}

	private float SimplexValue(float x, float y, float z)
	{
		if (turbulence3d)
		{
			return (float)simplex.noise(x * turbulenceFieldScale, y * turbulenceFieldScale, z * turbulenceFieldScale, Time.time * turbulenceTimeScale) * Time.deltaTime * turbulenceStrength;
		}
		return (float)simplex.noise(x * turbulenceFieldScale, y * turbulenceFieldScale, Time.time * turbulenceTimeScale) * Time.deltaTime * turbulenceStrength;
	}

	public void StopTurbulenceOnTransform(Transform stopOnTransform)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			if (transforms[i] == stopOnTransform)
			{
				hasTurbulence[i] = false;
			}
		}
	}

	public void StartTurbulenceOnTransform(Transform startOnTransform)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			if (transforms[i] == startOnTransform)
			{
				hasTurbulence[i] = true;
			}
		}
	}
}
