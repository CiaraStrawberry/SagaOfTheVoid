using System;
using UnityEngine;

public static class SgtEase
{
	public enum Type
	{
		Linear,
		Smoothstep,
		Sinusoidial,
		Quadratic,
		Circular,
		Cubic,
		Quartic,
		Quintic,
		Exponential
	}

	public static float Evaluate(Type ease, float t)
	{
		switch (ease)
		{
		case Type.Linear:
			return t;
		case Type.Smoothstep:
			return t * t * (3f - 2f * t);
		case Type.Sinusoidial:
			return Mathf.Sin(t * ((float)Math.PI / 2f));
		case Type.Quadratic:
			return -1f * t * (t - 2f);
		case Type.Circular:
			t--;
			return Mathf.Sqrt(1f - t * t);
		case Type.Cubic:
			t--;
			return t * t * t + 1f;
		case Type.Quartic:
			t--;
			return -1f * (t * t * t * t - 1f);
		case Type.Quintic:
			t--;
			return t * t * t * t * t + 1f;
		case Type.Exponential:
			return 0f - Mathf.Pow(2f, -10f * t) + 1f;
		default:
			return t;
		}
	}
}
