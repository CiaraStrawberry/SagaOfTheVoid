using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Advanced Belt")]
public class SgtAdvancedBelt : SgtBelt
{
	[Tooltip("The random seed used when generating the asteroids")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The distribution of asteroid distances in local space")]
	public AnimationCurve DistanceDistribution;

	[Tooltip("The distribution of asteroid heights in local space")]
	public AnimationCurve HeightDistribution;

	[Tooltip("The distribution of asteroid speeds in radians per second")]
	public AnimationCurve SpeedDistribution;

	[Tooltip("The distribution of asteroid speed offsets as a multiplier")]
	public AnimationCurve SpeedNoiseDistribution;

	[Tooltip("The distribution of asteroid radii in local space")]
	public AnimationCurve RadiusDistribution;

	[Tooltip("The distribution of asteroid spin in radians")]
	public AnimationCurve SpinDistribution;

	[Tooltip("The amount of asteroids generated in this belt")]
	public int AsteroidCount = 1000;

	private static Keyframe[] defaultDistanceKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 2f)
	};

	private static Keyframe[] defaultHeightKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, -0.1f),
		new Keyframe(1f, 0.1f)
	};

	private static Keyframe[] defaultSpeedKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, 0.1f),
		new Keyframe(1f, 0.05f)
	};

	private static Keyframe[] defaultSpeedOffsetKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, 0.1f),
		new Keyframe(1f, 0.1f)
	};

	private static Keyframe[] defaultRadiusKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, 0.1f),
		new Keyframe(1f, 0.2f)
	};

	private static Keyframe[] defaultSpinKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, -0.1f),
		new Keyframe(1f, 0.1f)
	};

	public static SgtAdvancedBelt CreateAdvancedBelt(int layer = 0, Transform parent = null)
	{
		return CreateAdvancedBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtAdvancedBelt CreateAdvancedBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Advanced Belt", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtAdvancedBelt>();
	}

	protected override void StartOnce()
	{
		if (DistanceDistribution == null)
		{
			DistanceDistribution = new AnimationCurve();
			DistanceDistribution.keys = defaultDistanceKeyframes;
		}
		if (HeightDistribution == null)
		{
			HeightDistribution = new AnimationCurve();
			HeightDistribution.keys = defaultHeightKeyframes;
		}
		if (SpeedDistribution == null)
		{
			SpeedDistribution = new AnimationCurve();
			SpeedDistribution.keys = defaultSpeedKeyframes;
		}
		if (SpeedNoiseDistribution == null)
		{
			SpeedNoiseDistribution = new AnimationCurve();
			SpeedNoiseDistribution.keys = defaultSpeedOffsetKeyframes;
		}
		if (RadiusDistribution == null)
		{
			RadiusDistribution = new AnimationCurve();
			RadiusDistribution.keys = defaultRadiusKeyframes;
		}
		if (SpinDistribution == null)
		{
			SpinDistribution = new AnimationCurve();
			SpinDistribution.keys = defaultSpinKeyframes;
		}
		base.StartOnce();
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		return AsteroidCount;
	}

	protected override void NextQuad(ref SgtBeltAsteroid asteroid, int asteroidIndex)
	{
		float value = UnityEngine.Random.value;
		float num = SpeedNoiseDistribution.Evaluate(value);
		asteroid.Variant = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		asteroid.Color = Color.white;
		asteroid.Radius = RadiusDistribution.Evaluate(UnityEngine.Random.value);
		asteroid.Height = HeightDistribution.Evaluate(UnityEngine.Random.value);
		asteroid.Angle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		asteroid.Spin = SpinDistribution.Evaluate(UnityEngine.Random.value);
		asteroid.OrbitAngle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		asteroid.OrbitSpeed = SpeedDistribution.Evaluate(value) + UnityEngine.Random.Range(0f - num, num);
		asteroid.OrbitDistance = DistanceDistribution.Evaluate(value);
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}
}
