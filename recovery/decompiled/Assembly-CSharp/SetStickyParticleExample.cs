using System.Collections;
using ParticlePlayground;
using UnityEngine;

public class SetStickyParticleExample : MonoBehaviour
{
	public int stickyParticle;

	public Transform stickyTransform;

	public Vector3 stickyOffset;

	private PlaygroundParticlesC particles;

	private IEnumerator Start()
	{
		particles = GetComponent<PlaygroundParticlesC>();
		if (stickyParticle > -1)
		{
			while (!particles.IsReady())
			{
				yield return null;
			}
			particles.SetSticky(stickyParticle, stickyTransform.position + stickyOffset, stickyTransform.up, particles.stickyCollisionsSurfaceOffset, stickyTransform);
		}
	}

	private void Update()
	{
		particles.UpdateSticky(stickyParticle);
	}

	public void SetSticky(int index, Vector3 position, Vector3 normal, float offset, Transform parent)
	{
		particles.SetSticky(index, position, normal, offset, parent);
	}

	public void ClearAllSticky()
	{
		particles.ClearCollisions();
	}

	public void ClearSticky(int index)
	{
		particles.ClearCollisions(index);
	}
}
