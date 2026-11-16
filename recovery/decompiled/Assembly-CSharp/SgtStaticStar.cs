using System;
using UnityEngine;

[Serializable]
public class SgtStaticStar
{
	public static SgtStaticStar Temp = new SgtStaticStar();

	[Tooltip("The coordinate index in the asteroid texture")]
	public int Variant;

	[Tooltip("Color tint of this star")]
	public Color Color = Color.white;

	[Tooltip("Radius of this star in local space")]
	public float Radius;

	[Tooltip("Position of the star in local space")]
	public Vector3 Position;

	public void CopyFrom(SgtStaticStar other)
	{
		Variant = other.Variant;
		Color = other.Color;
		Radius = other.Radius;
		Position = other.Position;
	}
}
