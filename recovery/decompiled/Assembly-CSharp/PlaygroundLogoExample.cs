using System.Collections;
using ParticlePlayground;
using UnityEngine;

public class PlaygroundLogoExample : MonoBehaviour
{
	public float waitBeforeTurbulence = 3f;

	public float waitBeforeStopEmission;

	public float waitWhenDone = 4f;

	public float turbulenceIncreaseSpeed = 2f;

	public float turbulenceMaxStrength = 4f;

	public bool repeat;

	private PlaygroundParticlesC particles;

	private IEnumerator Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
		particles.emit = true;
		particles.turbulenceStrength = 0f;
		yield return new WaitForSeconds(waitBeforeTurbulence);
		while (particles.turbulenceStrength < turbulenceMaxStrength)
		{
			particles.turbulenceStrength += turbulenceIncreaseSpeed * Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(waitBeforeStopEmission);
		particles.emit = false;
		yield return new WaitForSeconds(waitWhenDone);
		if (repeat)
		{
			StartCoroutine(Start());
		}
	}
}
