using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PaintPositionC
{
	public Vector3 position;

	public Color32 color;

	public Transform parent;

	public Vector3 initialPosition;

	public Vector3 initialNormal;

	public Quaternion parentRotation;
}
