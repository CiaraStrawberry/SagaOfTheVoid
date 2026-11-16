using System;
using ParticlePlayground;

[Serializable]
public class SequenceRange
{
	public PlaygroundParticlesC particles;

	public float minRange;

	public float maxRange;

	public bool IsWithin(float f)
	{
		return f >= minRange && f <= maxRange;
	}
}
