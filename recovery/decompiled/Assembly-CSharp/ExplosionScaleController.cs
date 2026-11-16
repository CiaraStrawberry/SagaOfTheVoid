using UnityEngine;

public class ExplosionScaleController : MonoBehaviour
{
	public GameObject World;

	private ParticleSystem particle;

	private ParticleSystem.ShapeModule shape;

	private float startsize;

	private ParticleSystem.MainModule main;

	private void Start()
	{
		World = GameObject.Find("WorldScaleBase");
		particle = GetComponent<ParticleSystem>();
		shape = particle.shape;
		main = particle.main;
	}

	private void Update()
	{
	}
}
