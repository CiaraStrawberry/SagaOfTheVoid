using System.Collections.Generic;
using UnityEngine;

namespace Forge3D;

public class F3DShotgun : MonoBehaviour
{
	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(16);

	private void OnParticleCollision(GameObject other)
	{
		int num = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
		for (int i = 0; i < num; i++)
		{
			F3DAudioController.instance.ShotGunHit(collisionEvents[i].intersection);
			if ((bool)other.GetComponent<Rigidbody>())
			{
				Vector3 intersection = collisionEvents[i].intersection;
				Vector3 force = collisionEvents[i].velocity.normalized * 50f;
				other.GetComponent<Rigidbody>().AddForceAtPosition(force, intersection);
			}
		}
	}
}
