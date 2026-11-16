using Galaxia;
using UnityEngine;

[RequireComponent(typeof(Galaxy))]
public class GalaxyAnimationController : MonoBehaviour
{
	private Galaxy galaxy;

	[SerializeField]
	private float speed = 0.1f;

	private void Start()
	{
		galaxy = GetComponent<Galaxy>();
	}

	private void Update()
	{
		galaxy.SetParticlesTime(Time.time * speed);
		galaxy.UpdateParticlesImmediately();
	}
}
