using TrueSync.Physics3D;
using UnityEngine;

namespace TrueSync;

public class TSCollision
{
	public TSContactPoint[] contacts = new TSContactPoint[1];

	public TSCollider collider;

	public GameObject gameObject;

	public TSRigidBody rigidbody;

	public TSTransform transform;

	public TSVector relativeVelocity;

	internal void Update(GameObject otherGO, Contact c)
	{
		if (gameObject == null)
		{
			gameObject = otherGO;
			collider = gameObject.GetComponent<TSCollider>();
			rigidbody = gameObject.GetComponent<TSRigidBody>();
			transform = collider.tsTransform;
		}
		if (c != null)
		{
			if (contacts[0] == null)
			{
				contacts[0] = new TSContactPoint();
			}
			relativeVelocity = c.CalculateRelativeVelocity();
			contacts[0].normal = c.Normal;
			contacts[0].point = c.p1;
		}
	}
}
