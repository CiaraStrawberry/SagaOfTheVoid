using System;
using ParticlePlayground;
using UnityEngine;

public class ParticleParabola : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Vector3 startPosition;

	public Vector3 endPosition = new Vector3(10f, 0f, 0f);

	public Vector3 bending = Vector3.up;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			ShootParabola();
		}
	}

	private void ShootParabola()
	{
		for (int i = 0; i < particles.particleCount; i++)
		{
			float num = (float)i * 1f / ((float)particles.particleCount * 1f);
			Vector3 givePosition = Vector3.Lerp(startPosition, endPosition, num);
			givePosition.x += bending.x * Mathf.Sin(Mathf.Clamp01(num) * (float)Math.PI);
			givePosition.y += bending.y * Mathf.Sin(Mathf.Clamp01(num) * (float)Math.PI);
			givePosition.z += bending.z * Mathf.Sin(Mathf.Clamp01(num) * (float)Math.PI);
			particles.Emit(givePosition, Vector3.zero, Color.white);
		}
	}
}
