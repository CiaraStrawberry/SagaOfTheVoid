using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Nebula Starfield")]
public class SgtNebulaStarfield : SgtPointStarfield
{
	[Tooltip("The random seed used when generating the stars")]
	[SgtSeed]
	public int Seed;

	[Tooltip("This texture used to color the nebula particles")]
	public Texture SourceTex;

	[Tooltip("This brightness of the sampled SourceTex pixel for a particle to be spawned")]
	[Range(0f, 1f)]
	public float Threshold = 0.1f;

	[Tooltip("The amount of times a nebula point is randomly sampled, before the brightest sample is used")]
	[Range(1f, 5f)]
	public int Samples = 2;

	[Tooltip("This allows you to randomly offset each nebula particle position")]
	[Range(0f, 1f)]
	public float Jitter;

	[Tooltip("The calculation used to find the height offset of a particle in the nebula")]
	public SgtNebulaSource HeightSource;

	[Tooltip("The calculation used to find the scale modified of each particle in the nebula")]
	public SgtNebulaSource ScaleSource;

	[Tooltip("The size of the generated nebula")]
	public Vector3 Size = new Vector3(1f, 1f, 1f);

	[Tooltip("The brightness of the nebula when viewed from the side (good for galaxies)")]
	public float HorizontalBrightness = 0.25f;

	[Tooltip("The relationship between the Brightness and HorizontalBrightness relative to the viweing angle")]
	public float HorizontalPower = 1f;

	[Tooltip("The amount of stars that will be generated in the starfield")]
	public int StarCount = 1000;

	[Tooltip("The minimum radius of stars in the starfield")]
	public float StarRadiusMin;

	[Tooltip("The maximum radius of stars in the starfield")]
	public float StarRadiusMax = 0.05f;

	[Tooltip("The maximum amount a star's size can pulse over time. A value of 1 means the star can potentially pulse between its maximum size, and 0")]
	[Range(0f, 1f)]
	public float StarPulseMax = 1f;

	private static Texture2D sourceTex2D;

	private static Vector3 halfSize;

	public static SgtNebulaStarfield CreateNebulaStarfield(int layer = 0, Transform parent = null)
	{
		return CreateNebulaStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtNebulaStarfield CreateNebulaStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Nebula Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtNebulaStarfield>();
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		sourceTex2D = SourceTex as Texture2D;
		if (sourceTex2D != null && Samples > 0)
		{
			halfSize = Size * 0.5f;
			return StarCount;
		}
		return 0;
	}

	protected override void NextQuad(ref SgtPointStar quad, int starIndex)
	{
		for (int num = Samples - 1; num >= 0; num--)
		{
			float num2 = Random.Range(0f, 1f);
			float num3 = Random.Range(0f, 1f);
			Color pixelBilinear = sourceTex2D.GetPixelBilinear(num2, num3);
			float grayscale = pixelBilinear.grayscale;
			if (grayscale > Threshold || num == 0)
			{
				Vector3 position = -halfSize + Random.insideUnitSphere * Jitter * StarRadiusMax;
				position.x += Size.x * num2;
				position.y += Size.y * GetWeight(HeightSource, pixelBilinear, 0.5f);
				position.z += Size.z * num3;
				quad.Variant = Random.Range(int.MinValue, int.MaxValue);
				quad.Color = pixelBilinear;
				quad.Radius = Random.Range(StarRadiusMin, StarRadiusMax) * GetWeight(ScaleSource, pixelBilinear, 1f);
				quad.Angle = Random.Range(-180f, 180f);
				quad.Position = position;
				quad.PulseRange = Random.value * StarPulseMax;
				quad.PulseSpeed = Random.value;
				quad.PulseOffset = Random.value;
				break;
			}
		}
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}

	protected override void CameraPreCull(Camera camera)
	{
		base.CameraPreCull(camera);
		if (Material != null)
		{
			Vector3 normalized = (transform.position - camera.transform.position).normalized;
			float f = Mathf.Abs(Vector3.Dot(transform.up, normalized));
			float num = Mathf.Lerp(HorizontalBrightness, Brightness, Mathf.Pow(f, HorizontalPower));
			Color value = SgtHelper.Brighten(Color, Color.a * num);
			Material.SetColor("_Color", value);
		}
	}

	private float GetWeight(SgtNebulaSource source, Color pixel, float defaultWeight)
	{
		return source switch
		{
			SgtNebulaSource.Red => pixel.r, 
			SgtNebulaSource.Green => pixel.g, 
			SgtNebulaSource.Blue => pixel.b, 
			SgtNebulaSource.Alpha => pixel.a, 
			SgtNebulaSource.AverageRgb => (pixel.r + pixel.g + pixel.b) / 3f, 
			SgtNebulaSource.MinRgb => Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b)), 
			SgtNebulaSource.MaxRgb => Mathf.Max(pixel.r, Mathf.Max(pixel.g, pixel.b)), 
			_ => defaultWeight, 
		};
	}
}
