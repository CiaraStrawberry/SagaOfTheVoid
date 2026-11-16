using System;
using System.Collections;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class SystemSequence
{
	public PlaygroundParticlesC particles;

	public float startTime;

	public IEnumerator StartDelayedEmission()
	{
		particles.particleSystemGameObject.SetActive(value: false);
		yield return new WaitForSeconds(startTime);
		particles.Emit(setEmission: true);
	}
}
