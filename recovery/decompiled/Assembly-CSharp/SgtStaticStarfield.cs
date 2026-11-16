using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Static Starfield")]
public class SgtStaticStarfield : SgtQuads
{
	[Tooltip("The random seed used when generating the stars")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The radius of the starfield")]
	public float Radius = 1f;

	[Tooltip("Should more stars be placed near the horizon?")]
	[Range(0f, 1f)]
	public float Symmetry = 1f;

	[Tooltip("The amount of stars that will be generated in the starfield")]
	public int StarCount = 1000;

	[Tooltip("The minimum radius of stars in the starfield")]
	public float StarRadiusMin;

	[Tooltip("The maximum radius of stars in the starfield")]
	public float StarRadiusMax = 0.05f;

	[Tooltip("How likely the size picking will pick smaller stars over larger ones (1 = default/linear)")]
	public float StarRadiusBias = 1f;

	[Tooltip("Each star is given a random color from this gradient")]
	public Gradient StarColors;

	protected override string ShaderName => "Hidden/SgtStaticStarfield";

	public static SgtStaticStarfield CreateStaticStarfield(int layer = 0, Transform parent = null)
	{
		return CreateStaticStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtStaticStarfield CreateStaticStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Static Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtStaticStarfield>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
	}

	protected override int BeginQuads()
	{
		SgtHelper.BeginRandomSeed(Seed);
		return StarCount;
	}

	protected virtual void NextQuad(ref SgtStaticStar quad, int starIndex)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		insideUnitSphere.y *= Symmetry;
		quad.Variant = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		quad.Radius = Mathf.Lerp(StarRadiusMin, StarRadiusMax, Mathf.Pow(UnityEngine.Random.value, StarRadiusBias));
		quad.Position = insideUnitSphere.normalized * Radius;
		if (StarColors != null)
		{
			quad.Color = StarColors.Evaluate(UnityEngine.Random.value);
		}
		else
		{
			quad.Color = Color.white;
		}
	}

	protected override void EndQuads()
	{
		SgtHelper.EndRandomSeed();
	}

	protected override void BuildMesh(Mesh mesh, int starIndex, int starCount)
	{
		Vector3[] array = new Vector3[starCount * 4];
		Color[] array2 = new Color[starCount * 4];
		Vector2[] array3 = new Vector2[starCount * 4];
		int[] array4 = new int[starCount * 6];
		bool minMaxSet = false;
		Vector3 min = default;
		Vector3 max = default;
		for (int i = 0; i < starCount; i++)
		{
			NextQuad(ref SgtStaticStar.Temp, starIndex + i);
			int num = i * 4;
			int num2 = i * 6;
			float radius = SgtStaticStar.Temp.Radius;
			Vector4 vector = SgtQuads.tempCoords[SgtHelper.Mod(SgtStaticStar.Temp.Variant, SgtQuads.tempCoords.Count)];
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.back, SgtStaticStar.Temp.Position);
			Vector3 vector2 = quaternion * Vector3.up * radius;
			Vector3 vector3 = quaternion * Vector3.right * radius;
			SgtQuads.ExpandBounds(ref minMaxSet, ref min, ref max, SgtStaticStar.Temp.Position, radius);
			ref Vector3 reference = ref array[num];
			reference = SgtStaticStar.Temp.Position - vector2 - vector3;
			ref Vector3 reference2 = ref array[num + 1];
			reference2 = SgtStaticStar.Temp.Position - vector2 + vector3;
			ref Vector3 reference3 = ref array[num + 2];
			reference3 = SgtStaticStar.Temp.Position + vector2 - vector3;
			ref Vector3 reference4 = ref array[num + 3];
			reference4 = SgtStaticStar.Temp.Position + vector2 + vector3;
			ref Color reference5 = ref array2[num];
			ref Color reference6 = ref array2[num + 1];
			ref Color reference7 = ref array2[num + 2];
			ref Color reference8 = ref array2[num + 3];
			reference5 = (reference6 = (reference7 = (reference8 = SgtStaticStar.Temp.Color)));
			ref Vector2 reference9 = ref array3[num];
			reference9 = new Vector2(vector.x, vector.y);
			ref Vector2 reference10 = ref array3[num + 1];
			reference10 = new Vector2(vector.z, vector.y);
			ref Vector2 reference11 = ref array3[num + 2];
			reference11 = new Vector2(vector.x, vector.w);
			ref Vector2 reference12 = ref array3[num + 3];
			reference12 = new Vector2(vector.z, vector.w);
			array4[num2] = num;
			array4[num2 + 1] = num + 1;
			array4[num2 + 2] = num + 2;
			array4[num2 + 3] = num + 3;
			array4[num2 + 4] = num + 2;
			array4[num2 + 5] = num + 1;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		mesh.bounds = SgtHelper.NewBoundsFromMinMax(min, max);
	}

	protected virtual void CameraPreCull(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtQuadsModel sgtQuadsModel = Models[num];
			if (sgtQuadsModel != null)
			{
				sgtQuadsModel.Revert();
				sgtQuadsModel.transform.position = camera.transform.position;
				sgtQuadsModel.Save(camera);
			}
		}
	}

	protected void CameraPreRender(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtQuadsModel sgtQuadsModel = Models[num];
			if (sgtQuadsModel != null)
			{
				sgtQuadsModel.Restore(camera);
			}
		}
	}

	protected void CameraPostRender(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtQuadsModel sgtQuadsModel = Models[num];
			if (sgtQuadsModel != null)
			{
				sgtQuadsModel.Revert();
			}
		}
	}
}
