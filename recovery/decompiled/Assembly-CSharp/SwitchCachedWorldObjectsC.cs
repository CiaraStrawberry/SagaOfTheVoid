using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

public class SwitchCachedWorldObjectsC : MonoBehaviour
{
	public Transform[] worldObjects;

	public PlaygroundParticlesC particles;

	private List<WorldObject> cachedWorldObjects;

	private void Start()
	{
		cachedWorldObjects = new List<WorldObject>();
		Transform[] array = worldObjects;
		foreach (Transform meshTransform in array)
		{
			cachedWorldObjects.Add(PlaygroundC.WorldObject(meshTransform));
		}
		SwitchWorldObject(0);
	}

	private void SwitchWorldObject(int switchTo)
	{
		particles.worldObject = cachedWorldObjects[switchTo];
	}
}
