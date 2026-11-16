using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

public class EmitFromTransformsC : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public int emissionAmount = 1;

	public float emissionRate = 0.1f;

	public Color32 color = Color.white;

	public Vector3 randomPositionMin;

	public Vector3 randomPositionMax;

	public Vector3 randomVelocityMin;

	public Vector3 randomVelocityMax;

	public List<Transform> transforms;

	private float lastTimeUpdated;

	private void Update()
	{
		if (!(Time.time > lastTimeUpdated + emissionRate))
		{
			return;
		}
		foreach (Transform transform in transforms)
		{
			particles.Emit(emissionAmount, transform.position + randomPositionMin, transform.position + randomPositionMax, randomVelocityMin, randomVelocityMax, color);
		}
		lastTimeUpdated = Time.time;
	}
}
