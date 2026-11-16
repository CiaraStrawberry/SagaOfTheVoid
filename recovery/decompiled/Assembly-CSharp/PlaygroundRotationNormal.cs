using ParticlePlayground;
using UnityEngine;

public class PlaygroundRotationNormal : MonoBehaviour
{
	private PlaygroundParticlesC particles;

	private Transform cameraTransform;

	private bool isUpsideDown;

	private bool isBackward;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		cameraTransform = Camera.main.transform;
	}

	private void Update()
	{
		if (particles.rotateTowardsDirection)
		{
			isUpsideDown = Vector3.Dot(cameraTransform.up, Vector3.up) < 0f;
			isBackward = Vector3.Dot(cameraTransform.forward, Vector3.forward) < 0f;
			if (!isUpsideDown && !isBackward)
			{
				particles.rotationNormal = new Vector3(0f, 0f, -1f);
				particles.initialRotationMin = 0f;
			}
			else if (!isUpsideDown && isBackward)
			{
				particles.rotationNormal = new Vector3(0f, 0f, 1f);
				particles.initialRotationMin = 0f;
			}
			else if (isUpsideDown && !isBackward)
			{
				particles.rotationNormal = new Vector3(0f, 0f, -1f);
				particles.initialRotationMin = 180f;
			}
			else if (isUpsideDown && isBackward)
			{
				particles.rotationNormal = new Vector3(0f, 0f, 1f);
				particles.initialRotationMin = 180f;
			}
			particles.initialRotationMax = particles.initialRotationMin;
		}
	}
}
