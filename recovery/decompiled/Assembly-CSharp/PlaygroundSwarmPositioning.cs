using UnityEngine;

public class PlaygroundSwarmPositioning : MonoBehaviour
{
	public float swarmStrength = 0.1f;

	public float swarmSpeed = 6f;

	public Transform swarmTransform;

	private Vector3 _velocity;

	private Transform _thisTransform;

	private void Start()
	{
		_thisTransform = transform;
	}

	private void Update()
	{
		float num = Time.time * swarmSpeed;
		Vector3 position = _thisTransform.position;
		_velocity.x += Mathf.PerlinNoise(num, position.z) - 0.5f;
		_velocity.y += Mathf.PerlinNoise(num + 1f, position.x) - 0.5f;
		_velocity.z += Mathf.PerlinNoise(num + 2f, position.y) - 0.5f;
		_velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * swarmSpeed);
		_thisTransform.position = Vector3.Lerp(position, swarmTransform.position + _velocity * swarmStrength, Time.deltaTime * swarmSpeed);
	}
}
