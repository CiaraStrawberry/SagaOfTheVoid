using TrueSync.Physics2D;
using UnityEngine;

namespace TrueSync;

public class TSCollision2D
{
	public TSContactPoint2D[] contacts = new TSContactPoint2D[1];

	public TSCollider2D collider;

	public GameObject gameObject;

	public TSRigidBody2D rigidbody;

	public TSTransform2D transform;

	public TSVector2 relativeVelocity;

	internal void Update(GameObject otherGO, Contact c)
	{
		if (gameObject == null)
		{
			gameObject = otherGO;
			collider = gameObject.GetComponent<TSCollider2D>();
			rigidbody = gameObject.GetComponent<TSRigidBody2D>();
			transform = collider.tsTransform;
		}
		if (c != null)
		{
			if (contacts[0] == null)
			{
				contacts[0] = new TSContactPoint2D();
			}
			c.GetWorldManifold(out var normal, out var points);
			contacts[0].normal = normal;
			contacts[0].point = points[0];
			relativeVelocity = c.CalculateRelativeVelocity();
		}
	}
}
