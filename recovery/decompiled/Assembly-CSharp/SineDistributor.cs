using System;
using Galaxia;
using UnityEngine;

public class SineDistributor : ParticleDistributor
{
	public float Spread = 100f;

	public float Height = 10f;

	public override void Process(ProcessContext context)
	{
		float value = UnityEngine.Random.value;
		float num = context.particle.index / (float)context.particles.Count;
		float y = Height * Mathf.Sin((float)Math.PI * 2f * num) * Mathf.Cos((float)Math.PI * 2f * value);
		context.particle.position = new Vector3(num, y, value) * Spread * context.galaxy.Size;
		ProcessProperties(context, context.particle.position, 0f);
	}
}
