using ParticlePlayground;
using UnityEngine;

public class EmitWhenMoving : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform movingTransform;

	private Vector3 previousPosition;

	private void Start()
	{
		if (particles == null)
		{
			GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		if (movingTransform.position != previousPosition)
		{
			particles.emit = true;
			previousPosition = movingTransform.position;
		}
		else
		{
			particles.emit = false;
		}
	}
}
