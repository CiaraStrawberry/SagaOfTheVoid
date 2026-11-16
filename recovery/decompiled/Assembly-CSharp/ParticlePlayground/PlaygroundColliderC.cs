using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundColliderC
{
	public bool enabled = true;

	public Transform transform;

	public Plane plane = default;

	public Vector3 offset;

	public void UpdatePlane()
	{
		if ((bool)transform)
		{
			plane.SetNormalAndPosition(transform.up, transform.position + offset);
		}
	}

	public PlaygroundColliderC Clone()
	{
		PlaygroundColliderC playgroundColliderC = new PlaygroundColliderC();
		playgroundColliderC.enabled = enabled;
		playgroundColliderC.transform = transform;
		playgroundColliderC.plane = new Plane(plane.normal, plane.distance);
		playgroundColliderC.offset = offset;
		return playgroundColliderC;
	}
}
