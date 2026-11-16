using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemsSequence : MonoBehaviour
{
	public float input;

	public float maximumSequenceRange = 100f;

	public List<SequenceRange> sequences = new List<SequenceRange>();

	private void Update()
	{
		Sequencer();
	}

	private void Sequencer()
	{
		input = Mathf.Repeat(input, maximumSequenceRange);
		for (int i = 0; i < sequences.Count; i++)
		{
			sequences[i].particles.emit = sequences[i].IsWithin(input);
		}
	}
}
