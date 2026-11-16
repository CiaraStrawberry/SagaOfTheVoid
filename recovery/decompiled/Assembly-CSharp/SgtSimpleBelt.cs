using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Simple Belt")]
public class SgtSimpleBelt : SgtBelt
{
	[Tooltip("The seed used to generate the asteroids")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The thickness of the belt in local coordinates")]
	public float Thickness;

	public float ThicknessBias = 1f;

	[Tooltip("The radius of the inner edge of the belt in local coordinates")]
	public float InnerRadius = 1f;

	[Tooltip("The speed of asteroids orbiting on the inner edge of the belt in radians")]
	public float InnerSpeed = 0.1f;

	[Tooltip("The radius of the outer edge of the belt in local coordinates")]
	public float OuterRadius = 2f;

	[Tooltip("The speed of asteroids orbiting on the outer edge of the belt in radians")]
	public float OuterSpeed = 0.05f;

	[Tooltip("The amount of asteroids generated in the belt")]
	public int AsteroidCount = 1000;

	[Tooltip("The maximum amount of angular velcoity each asteroid has")]
	public float AsteroidSpin = 1f;

	[Tooltip("The minimum asteroid radius in local coordinates")]
	public float AsteroidRadiusMin = 0.025f;

	[Tooltip("The maximum asteroid radius in local coordinates")]
	public float AsteroidRadiusMax = 0.05f;

	[Tooltip("How likely the size picking will pick smaller asteroids over larger ones (1 = default/linear)")]
	public float AsteroidRadiusBias = 1f;

	public static SgtSimpleBelt CreateSimpleBelt(int layer = 0, Transform parent = null)
	{
		return CreateSimpleBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSimpleBelt CreateSimpleBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Simple Belt", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtSimpleBelt>();
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		return AsteroidCount;
	}

	protected override void NextQuad(ref SgtBeltAsteroid asteroid, int asteroidIndex)
	{
		float t = (UnityEngine.Random.value + UnityEngine.Random.value) * 0.5f;
		asteroid.Variant = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		asteroid.Color = Color.white;
		asteroid.Radius = Mathf.Lerp(AsteroidRadiusMin, AsteroidRadiusMax, Mathf.Pow(UnityEngine.Random.value, AsteroidRadiusBias));
		asteroid.Height = Mathf.Pow(UnityEngine.Random.value, ThicknessBias) * Thickness * ((!(UnityEngine.Random.value < 0.5f)) ? 0.5f : (-0.5f));
		asteroid.Angle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		asteroid.Spin = UnityEngine.Random.Range(0f - AsteroidSpin, AsteroidSpin);
		asteroid.OrbitAngle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		asteroid.OrbitSpeed = Mathf.Lerp(InnerSpeed, OuterSpeed, t);
		asteroid.OrbitDistance = Mathf.Lerp(InnerRadius, OuterRadius, t);
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}
}
