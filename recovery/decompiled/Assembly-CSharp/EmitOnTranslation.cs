using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
public class EmitOnTranslation : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform movingTransform;

	public float minSpace = 0.1f;

	private bool isReady;

	private Vector3 previousPosition;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
		if (movingTransform == null)
		{
			movingTransform = transform;
		}
		if (particles != null && movingTransform != null)
		{
			isReady = true;
		}
		previousPosition = movingTransform.position;
	}

	private void Update()
	{
		if (isReady)
		{
			EmissionMoveCheck();
		}
	}

	private void EmissionMoveCheck()
	{
		if (movingTransform.position != previousPosition)
		{
			float num = 0f;
			float num2 = Vector3.Distance(previousPosition, movingTransform.position);
			if (num2 < minSpace)
			{
				return;
			}
			Vector3 vector = previousPosition;
			Vector3 zero = Vector3.zero;
			Color white = Color.white;
			for (; num < num2; num += minSpace)
			{
				vector = Vector3.Lerp(previousPosition, movingTransform.position, num / num2);
				particles.Emit(vector, zero, white);
			}
		}
		previousPosition = movingTransform.position;
	}
}
