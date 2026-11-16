using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Spiral Starfield")]
public class SgtSpiralStarfield : SgtPointStarfield
{
	[Tooltip("The random seed used when generating the stars")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The radius of the starfield")]
	public float Radius = 1f;

	[Tooltip("The amount of spiral arms")]
	public int ArmCount = 1;

	[Tooltip("The amound each arm twists")]
	public float Twist = 1f;

	[Tooltip("The thickness of the spirals")]
	public AnimationCurve Thickness;

	[Tooltip("The amount of stars that will be generated in the starfield")]
	public int StarCount = 1000;

	[Tooltip("The minimum radius of stars in the starfield")]
	public float StarRadiusMin;

	[Tooltip("The maximum radius of stars in the starfield")]
	public float StarRadiusMax = 0.05f;

	[Tooltip("How likely the size picking will pick smaller stars over larger ones (1 = default/linear)")]
	public float StarRadiusBias = 1f;

	[Tooltip("The maximum amount a star's size can pulse over time. A value of 1 means the star can potentially pulse between its maximum size, and 0")]
	[Range(0f, 1f)]
	public float StarPulseMax = 1f;

	private static Keyframe[] defaultThicknessKeyframes = new Keyframe[2]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(1f, 0.25f)
	};

	private static float armStep;

	private static float twistStep;

	public static SgtSpiralStarfield CreateSpiralStarfield(int layer = 0, Transform parent = null)
	{
		return CreateSpiralStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSpiralStarfield CreateSpiralStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Spiral Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtSpiralStarfield>();
	}

	protected override void Start()
	{
		base.Start();
		if (Thickness == null)
		{
			Thickness = new AnimationCurve();
			Thickness.keys = defaultThicknessKeyframes;
			UpdateMeshesAndModels();
		}
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		if (Thickness != null)
		{
			armStep = 360f * SgtHelper.Reciprocal(ArmCount);
			twistStep = 360f * Twist;
			return StarCount;
		}
		return 0;
	}

	protected override void NextQuad(ref SgtPointStar star, int starIndex)
	{
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		float num = 1f - Random.insideUnitSphere.magnitude;
		insideUnitSphere *= (1f - num) * Thickness.Evaluate(Random.value);
		insideUnitSphere += Quaternion.AngleAxis((float)starIndex * armStep + num * twistStep, Vector3.up) * Vector3.forward * num;
		star.Variant = Random.Range(int.MinValue, int.MaxValue);
		star.Color = Color.white;
		star.Radius = Mathf.Lerp(StarRadiusMin, StarRadiusMax, Mathf.Pow(Random.value, StarRadiusBias));
		star.Angle = Random.Range(-180f, 180f);
		star.Position = insideUnitSphere * Radius;
		star.PulseRange = Random.value * StarPulseMax;
		star.PulseSpeed = Random.value;
		star.PulseOffset = Random.value;
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}
}
