using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
	public Vector3 diveTorque;

	public Vector3 rollTorque;

	public float accelerationSpeed = 1f;

	public float maxVelocity = 100f;

	public float maxBoostVelocity = 250f;

	public float maxAngularVelocity = 50f;

	private Rigidbody _rigidbody;

	protected void Awake()
	{
		_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
		_rigidbody.maxAngularVelocity = maxAngularVelocity;
	}

	protected void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			_rigidbody.AddRelativeTorque(diveTorque * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			_rigidbody.AddRelativeTorque(-diveTorque * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.A))
		{
			_rigidbody.AddRelativeTorque(rollTorque * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D))
		{
			_rigidbody.AddRelativeTorque(-rollTorque * Time.deltaTime);
		}
		float magnitude = _rigidbody.velocity.magnitude;
		if (magnitude > 0f)
		{
			Vector3 vector = _rigidbody.velocity / magnitude;
			Vector3 vector2 = vector * (magnitude * 0.95f);
			Vector3 vector3 = transform.forward * (magnitude * 0.05f);
			Vector3 velocity = vector3 + vector2;
			_rigidbody.velocity = velocity;
		}
		_rigidbody.AddRelativeForce(transform.forward * accelerationSpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.LeftShift))
		{
			LimitMaxVelocity(maxBoostVelocity);
		}
		else
		{
			LimitMaxVelocity(maxVelocity);
		}
	}

	private void LimitMaxVelocity(float max)
	{
		float magnitude = _rigidbody.velocity.magnitude;
		if (magnitude > max)
		{
			Vector3 vector = _rigidbody.velocity / magnitude;
			_rigidbody.velocity = vector * max;
		}
	}
}
