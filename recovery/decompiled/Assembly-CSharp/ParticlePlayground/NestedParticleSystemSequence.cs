using UnityEngine;

namespace ParticlePlayground;

public class NestedParticleSystemSequence : MonoBehaviour
{
	public SystemSequence[] sequences;

	private void Start()
	{
		for (int i = 0; i < sequences.Length; i++)
		{
			StartCoroutine(sequences[i].StartDelayedEmission());
		}
	}
}
