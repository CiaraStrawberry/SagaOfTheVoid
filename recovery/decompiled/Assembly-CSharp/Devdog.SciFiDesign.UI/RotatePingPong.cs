using UnityEngine;

namespace Devdog.SciFiDesign.UI;

public class RotatePingPong : MonoBehaviour
{
	public Vector3 rotationMin;

	public Vector3 rotationMax;

	public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float speed = 0.2f;

	public bool randomizeSpeed;

	public float maxSpeed = 1f;

	private float _speed;

	protected void Awake()
	{
		_speed = speed;
		if (randomizeSpeed)
		{
			_speed = Random.Range(speed, maxSpeed);
		}
	}

	protected void Update()
	{
		Vector3 euler = Vector3.Lerp(Vector3.zero, rotationMax - rotationMin, curve.Evaluate(Mathf.PingPong(Time.time * _speed, 1f)));
		euler += rotationMin;
		transform.localRotation = Quaternion.Euler(euler);
	}
}
