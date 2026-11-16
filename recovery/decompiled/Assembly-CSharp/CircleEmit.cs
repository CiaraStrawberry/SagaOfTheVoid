using System;
using System.Collections;
using ParticlePlayground;
using UnityEngine;

public class CircleEmit : MonoBehaviour
{
	public float velocity = 1f;

	public Color color = Color.white;

	public float width = 1f;

	public float height = 1f;

	private IEnumerator Start()
	{
		PlaygroundParticlesC particles = GetComponent<PlaygroundParticlesC>();
		while (!particles.IsReady())
		{
			yield return null;
		}
		Vector3 position = particles.particleSystemTransform.position;
		for (int i = 0; i < particles.particleCount; i++)
		{
			float num = 360f * ((float)i * 1f / ((float)particles.particleCount * 1f));
			Vector3 vector = new Vector3(Mathf.Cos((float)Math.PI / 180f * num) * width, Mathf.Sin((float)Math.PI / 180f * num) * height, 0f);
			particles.Emit(position, vector * velocity, color);
		}
	}
}
