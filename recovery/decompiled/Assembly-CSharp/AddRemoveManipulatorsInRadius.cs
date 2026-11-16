using ParticlePlayground;
using UnityEngine;

public class AddRemoveManipulatorsInRadius : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform originTransform;

	public float radius = 10f;

	public LayerMask layer;

	public float checkRate = 1f;

	private void Start()
	{
		InvokeRepeating("AddRemoveInRange", 0.01f, checkRate);
	}

	private void AddRemoveInRange()
	{
		RemoveOutOfRange();
		Collider[] array = Physics.OverlapSphere(originTransform.position, radius, layer);
		for (int i = 0; i < array.Length; i++)
		{
			if (!ManipulatorHasTransform(array[i].transform))
			{
				ManipulatorObjectC manipulatorObjectC = PlaygroundC.ManipulatorObject(array[i].transform, particles);
				manipulatorObjectC.type = MANIPULATORTYPEC.Repellent;
				manipulatorObjectC.size = 5f;
				manipulatorObjectC.strength = 3f;
			}
		}
	}

	private void RemoveOutOfRange()
	{
		for (int i = 0; i < particles.manipulators.Count; i++)
		{
			if (Vector3.Distance(originTransform.position, particles.manipulators[i].transform.transform.position) > radius)
			{
				particles.manipulators.RemoveAt(i);
				if (i > 0)
				{
					i--;
				}
			}
		}
	}

	private bool ManipulatorHasTransform(Transform comparer)
	{
		for (int i = 0; i < particles.manipulators.Count; i++)
		{
			if (particles.manipulators[i].transform.transform == comparer)
			{
				return true;
			}
		}
		return false;
	}
}
