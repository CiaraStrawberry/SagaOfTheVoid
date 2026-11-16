using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Box Starfield")]
public class SgtBoxStarfield : SgtPointStarfield
{
	[Tooltip("The random seed used when generating the stars")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The size of the starfield box")]
	public Vector3 Extents = Vector3.one;

	[Tooltip("How far from the center of the box the stars will spawn from. If this is 1, then the stars will only spawn at the edges")]
	[Range(0f, 1f)]
	public float Offset;

	[Tooltip("The amount of stars that will be generated in the starfield")]
	public int StarCount = 1000;

	[Tooltip("The minimum radius of stars in the starfield")]
	public float StarRadiusMin;

	[Tooltip("The maximum radius of stars in the starfield")]
	public float StarRadiusMax = 0.05f;

	[Tooltip("The maximum amount a star's size can pulse over time. A value of 1 means the star can potentially pulse between its maximum size, and 0")]
	[Range(0f, 1f)]
	public float StarPulseMax = 1f;

	public static SgtBoxStarfield CreateBoxStarfield(int layer = 0, Transform parent = null)
	{
		return CreateBoxStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtBoxStarfield CreateBoxStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Box Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtBoxStarfield>();
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		return StarCount;
	}

	protected override void NextQuad(ref SgtPointStar star, int starIndex)
	{
		float num = Random.Range(-1f, 1f);
		float num2 = Random.Range(-1f, 1f);
		float num3 = Random.Range(Offset * 1f, 1f);
		Vector3 a = default;
		if (Random.value >= 0.5f)
		{
			num3 = 0f - num3;
		}
		switch (Random.Range(0, 3))
		{
		case 0:
			a = new Vector3(num3, num, num2);
			break;
		case 1:
			a = new Vector3(num, num3, num2);
			break;
		case 2:
			a = new Vector3(num, num2, num3);
			break;
		}
		star.Variant = Random.Range(int.MinValue, int.MaxValue);
		star.Color = Color.white;
		star.Radius = Random.Range(StarRadiusMin, StarRadiusMax);
		star.Angle = Random.Range(-180f, 180f);
		star.Position = Vector3.Scale(a, Extents);
		star.PulseRange = Random.value * StarPulseMax;
		star.PulseSpeed = Random.value;
		star.PulseOffset = Random.value;
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}
}
