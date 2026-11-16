using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Custom Belt")]
public class SgtCustomBelt : SgtBelt
{
	[Tooltip("The custom asteroids in this belt")]
	public List<SgtBeltAsteroid> Asteroids;

	public static SgtCustomBelt CreateCustomBelt(int layer = 0, Transform parent = null)
	{
		return CreateCustomBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCustomBelt CreateCustomBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Custom Belt", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtCustomBelt>();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Asteroids != null)
		{
			for (int num = Asteroids.Count - 1; num >= 0; num--)
			{
				SgtClassPool<SgtBeltAsteroid>.Add(Asteroids[num]);
			}
		}
	}

	protected override int BeginQuads()
	{
		if (Asteroids != null)
		{
			return Asteroids.Count;
		}
		return 0;
	}

	protected override void NextQuad(ref SgtBeltAsteroid asteroid, int asteroidIndex)
	{
		asteroid.CopyFrom(Asteroids[asteroidIndex]);
	}

	protected override void EndQuads()
	{
	}
}
