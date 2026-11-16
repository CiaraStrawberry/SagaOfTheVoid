using System;
using UnityEngine;

[Serializable]
public class SgtPointStar
{
	public static SgtPointStar Temp = new SgtPointStar();

	[Tooltip("The coordinate index in the asteroid texture")]
	public int Variant;

	[Tooltip("Color tint of this star")]
	public Color Color = Color.white;

	[Tooltip("Radius of this star in local space")]
	public float Radius;

	[Tooltip("Angle in degrees")]
	public float Angle;

	[Tooltip("Local position of this star relative to the starfield")]
	public Vector3 Position;

	[Tooltip("How fast this star pulses (requires AllowPulse)")]
	[Range(0f, 1f)]
	public float PulseSpeed = 1f;

	[Tooltip("How much this star can pulse in size (requires AllowPulse)")]
	[Range(0f, 1f)]
	public float PulseRange;

	[Tooltip("The original pulse offset (requires AllowPulse)")]
	[Range(0f, 1f)]
	public float PulseOffset;

	public void CopyFrom(SgtPointStar other)
	{
		Variant = other.Variant;
		Color = other.Color;
		Radius = other.Radius;
		Angle = other.Angle;
		Position = other.Position;
		PulseSpeed = other.PulseSpeed;
		PulseRange = other.PulseRange;
		PulseOffset = other.PulseOffset;
	}
}
