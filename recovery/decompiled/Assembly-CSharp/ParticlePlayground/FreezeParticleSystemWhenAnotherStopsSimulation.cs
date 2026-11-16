using System.Collections;
using UnityEngine;

namespace ParticlePlayground;

public class FreezeParticleSystemWhenAnotherStopsSimulation : MonoBehaviour
{
	public PlaygroundParticlesC conditionalParticleSystem;

	public PlaygroundParticlesC freezeParticleSystem;

	public float extraWaitingTime;

	private float _originalTimeScale;

	private void OnEnable()
	{
		if (Mathf.Approximately(_originalTimeScale, 0f))
		{
			_originalTimeScale = freezeParticleSystem.particleTimescale;
		}
		else
		{
			freezeParticleSystem.particleTimescale = _originalTimeScale;
		}
		StartCoroutine(FreezeRoutine());
	}

	private void OnDisable()
	{
		StopCoroutine("FreezeRoutine");
	}

	private IEnumerator FreezeRoutine()
	{
		if (!(conditionalParticleSystem == null) && !(freezeParticleSystem == null))
		{
			while (!conditionalParticleSystem.IsReady() || conditionalParticleSystem.IsAlive())
			{
				yield return null;
			}
			if (extraWaitingTime > 0f)
			{
				yield return new WaitForSeconds(extraWaitingTime);
			}
			freezeParticleSystem.particleTimescale = 0f;
		}
	}
}
