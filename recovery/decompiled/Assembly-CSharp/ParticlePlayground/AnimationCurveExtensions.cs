using UnityEngine;

namespace ParticlePlayground;

public static class AnimationCurveExtensions
{
	public static void Reset(this AnimationCurve animationCurve)
	{
		if (animationCurve == null)
		{
			animationCurve = new AnimationCurve();
		}
		Keyframe[] array = new Keyframe[2];
		array[1].time = 1f;
		animationCurve.keys = array;
	}

	public static void Reset1(this AnimationCurve animationCurve)
	{
		if (animationCurve == null)
		{
			animationCurve = new AnimationCurve();
		}
		Keyframe[] array = new Keyframe[2];
		array[1].time = 1f;
		array[0].value = 1f;
		array[1].value = 1f;
		animationCurve.keys = array;
	}

	public static void Reset01(this AnimationCurve animationCurve)
	{
		if (animationCurve == null)
		{
			animationCurve = new AnimationCurve();
		}
		Keyframe[] array = new Keyframe[2];
		array[1].time = 1f;
		array[0].value = 0f;
		array[1].value = 1f;
		animationCurve.keys = array;
	}
}
