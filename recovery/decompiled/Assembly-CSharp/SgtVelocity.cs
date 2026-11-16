using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Space Graphics Toolkit/SGT Velocity")]
public class SgtVelocity : MonoBehaviour
{
	[Tooltip("The initial and current rigidbody velocity")]
	public Vector3 Velocity;

	[HideInInspector]
	[SerializeField]
	private Vector3 expectedVelocity;

	private Rigidbody body;

	protected virtual void OnEnable()
	{
		UpdateVelocity(forceSet: true);
	}

	protected virtual void Update()
	{
		UpdateVelocity();
	}

	protected virtual void FixedUpdate()
	{
		UpdateVelocity();
	}

	private void UpdateVelocity(bool forceSet = false)
	{
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
		if (Velocity != expectedVelocity || forceSet)
		{
			body.velocity = (expectedVelocity = Velocity);
		}
		else
		{
			Velocity = (expectedVelocity = body.velocity);
		}
	}
}
