using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

public class EventListenerEnableSystems : MonoBehaviour
{
	public PlaygroundParticlesC mainParticles;

	public GameObject particleSystemPoolPrefab;

	public int quantity = 10;

	private int currentEnabled;

	private List<PlaygroundParticlesC> cachedParticles;

	private List<Vector3> queuedSystems = new List<Vector3>();

	private void Start()
	{
		PlaygroundEventC playgroundEventC = PlaygroundC.GetEvent(0, mainParticles);
		playgroundEventC.broadcastType = EVENTBROADCASTC.EventListeners;
		playgroundEventC.particleEvent += OnParticleEvent;
		cachedParticles = new List<PlaygroundParticlesC>();
		for (int i = 0; i < quantity; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate((Object)particleSystemPoolPrefab);
			cachedParticles.Add(gameObject.GetComponent<PlaygroundParticlesC>());
			cachedParticles[i].particleSystemGameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (queuedSystems.Count > 0)
		{
			Vector3[] array = queuedSystems.ToArray();
			queuedSystems.Clear();
			Vector3[] array2 = array;
			foreach (Vector3 position in array2)
			{
				EnableParticleSystem(position);
			}
		}
	}

	private void OnParticleEvent(PlaygroundEventParticle particle)
	{
		queuedSystems.Add(particle.position);
	}

	public void EnableParticleSystem(Vector3 position)
	{
		cachedParticles[currentEnabled].particleSystemTransform.position = position;
		cachedParticles[currentEnabled].Emit(setEmission: true);
		currentEnabled++;
		currentEnabled %= cachedParticles.Count;
	}
}
