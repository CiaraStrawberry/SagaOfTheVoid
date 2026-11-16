using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Elliptical Starfield")]
public class SgtEllipticalStarfield : SgtPointStarfield
{
	[Tooltip("The random seed used when generating the stars")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The radius of the starfield")]
	public float Radius = 1f;

	[Tooltip("Should more stars be placed near the horizon?")]
	[Range(0f, 1f)]
	public float Symmetry = 1f;

	[Tooltip("How far from the center the distribution begins")]
	[Range(0f, 1f)]
	public float Offset;

	[Tooltip("Invert the distribution?")]
	public bool Inverse;

	[Tooltip("The amount of stars that will be generated in the starfield")]
	public int StarCount = 1000;

	[Tooltip("The minimum radius of stars in the starfield")]
	public float StarRadiusMin = 0.01f;

	[Tooltip("The maximum radius of stars in the starfield")]
	public float StarRadiusMax = 0.05f;

	[Tooltip("How likely the size picking will pick smaller stars over larger ones (1 = default/linear)")]
	public float StarRadiusBias = 1f;

	[Tooltip("The maximum amount a star's size can pulse over time. A value of 1 means the star can potentially pulse between its maximum size, and 0")]
	[Range(0f, 1f)]
	public float StarPulseMax = 1f;

	public static SgtEllipticalStarfield CreateEllipticalStarfield(int layer = 0, Transform parent = null)
	{
		return CreateEllipticalStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtEllipticalStarfield CreateEllipticalStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Elliptical Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtEllipticalStarfield>();
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		return StarCount;
	}

	protected override void NextQuad(ref SgtPointStar quad, int starIndex)
	{
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		float offset = Offset;
		offset = ((!Inverse) ? (offset + insideUnitSphere.magnitude * (1f - Offset)) : (offset + (1f - insideUnitSphere.magnitude) * (1f - Offset)));
		insideUnitSphere.y *= Symmetry;
		quad.Variant = Random.Range(int.MinValue, int.MaxValue);
		quad.Color = Color.white;
		quad.Radius = Mathf.Lerp(StarRadiusMin, StarRadiusMax, Mathf.Pow(Random.value, StarRadiusBias));
		quad.Angle = Random.Range(-180f, 180f);
		quad.Position = insideUnitSphere.normalized * offset * Radius;
		quad.PulseRange = Random.value * StarPulseMax;
		quad.PulseSpeed = Random.value;
		quad.PulseOffset = Random.value;
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}
}
