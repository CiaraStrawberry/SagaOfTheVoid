using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

public class SwitchCachedParticleSystemC : MonoBehaviour
{
	public GameObject particleSystemPrefab;

	public int quantity = 10;

	private int currentEnabled;

	private List<PlaygroundParticlesC> cachedParticles;

	private void Start()
	{
		cachedParticles = new List<PlaygroundParticlesC>();
		for (int i = 0; i < quantity; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate((Object)particleSystemPrefab);
			cachedParticles.Add(gameObject.GetComponent<PlaygroundParticlesC>());
			cachedParticles[i].particleSystemGameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
		{
			EnableParticleSystem(hitInfo.point);
		}
	}

	public void EnableParticleSystem(Vector3 position)
	{
		cachedParticles[currentEnabled].particleSystemTransform.position = position;
		cachedParticles[currentEnabled].Emit(setEmission: true);
		currentEnabled++;
		currentEnabled %= cachedParticles.Count;
	}
}
