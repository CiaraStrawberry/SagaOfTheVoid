using ParticlePlayground;
using UnityEngine;

public class EmitByAmountC : MonoBehaviour
{
	public int amount = 10;

	public Vector3 position;

	public Vector3 minimumVelocity = new Vector3(-10f, -10f, -10f);

	public Vector3 maximumVelocity = new Vector3(10f, 10f, 10f);

	public Color color = Color.white;

	public PlaygroundParticlesC particles;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			particles.Emit(amount, position, minimumVelocity, maximumVelocity, color);
		}
	}
}
