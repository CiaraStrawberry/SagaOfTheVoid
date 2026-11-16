using System.Collections;
using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PlaygroundParticlesC))]
public class PlaygroundPresetCircleShotC : MonoBehaviour
{
	public enum WhenDoneCircleShot
	{
		Nothing,
		Inactivate,
		Destroy
	}

	public int numberOfParticles = 30;

	public float force = 10f;

	public int cycles = 1;

	public Vector3 rotationNormal = new Vector3(0f, 0f, 1f);

	public float minimumLifetime = 3f;

	public float maximumLifetime = 3f;

	public Color color = Color.white;

	public float yieldBeforeEmission;

	public float yieldBetweenShots;

	public float yieldBetweenCycles;

	public WhenDoneCircleShot whenDone;

	private Transform thisTransform;

	private PlaygroundParticlesC particles;

	private bool isSameLifetime;

	private void Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		thisTransform = transform;
		isSameLifetime = minimumLifetime == maximumLifetime;
		StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		while (!particles.Initialized())
		{
			yield return null;
		}
		float rotationSpeed = 360f / (float)numberOfParticles;
		particles.particleCount = numberOfParticles * cycles;
		if (yieldBeforeEmission > 0f)
		{
			float timeDone = PlaygroundC.globalTime + yieldBeforeEmission;
			while (PlaygroundC.globalTime < timeDone)
			{
				yield return null;
			}
		}
		for (int c = 0; c < cycles; c++)
		{
			for (int p = 0; p < numberOfParticles; p++)
			{
				particles.Emit(thisTransform.position, thisTransform.right * force, (!isSameLifetime) ? Random.Range(minimumLifetime, maximumLifetime) : maximumLifetime, color);
				thisTransform.Rotate(rotationNormal * rotationSpeed);
				if (yieldBetweenShots > 0f)
				{
					float timeDone = PlaygroundC.globalTime + yieldBetweenShots;
					while (PlaygroundC.globalTime < timeDone)
					{
						yield return null;
					}
				}
			}
			if (yieldBetweenCycles > 0f)
			{
				float timeDone = PlaygroundC.globalTime + yieldBetweenCycles;
				while (PlaygroundC.globalTime < timeDone)
				{
					yield return null;
				}
			}
		}
		switch (whenDone)
		{
		case WhenDoneCircleShot.Inactivate:
			yield return new WaitForSeconds(maximumLifetime);
			gameObject.SetActive(value: false);
			break;
		case WhenDoneCircleShot.Destroy:
			yield return new WaitForSeconds(maximumLifetime);
			Object.Destroy(gameObject);
			break;
		}
	}
}
