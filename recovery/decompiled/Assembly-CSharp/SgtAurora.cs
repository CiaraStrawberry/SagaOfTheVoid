using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Aurora")]
public class SgtAurora : MonoBehaviour
{
	[Tooltip("The main texture")]
	public Texture MainTex;

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The Color.rgb values are multiplied by this")]
	public float Brightness = 1f;

	[Tooltip("The render queue group of the aurora material")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset of the aurora material")]
	public int RenderQueueOffset;

	[Tooltip("The distance the aurora mesh is moved toward the rendering camera in world space")]
	public float CameraOffset;

	[Tooltip("The random seed used when generating the aurora mesh")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The inner radius of the aurora mesh in local space")]
	public float RadiusMin = 1f;

	[Tooltip("The outer radius of the aurora mesh in local space")]
	public float RadiusMax = 1.1f;

	[Tooltip("The amount of aurora paths")]
	public int PathCount = 8;

	[Tooltip("The detail of each aurora path")]
	public int PathDetail = 100;

	[Tooltip("The sharpness of the fading at the start and ends of the aurora paths")]
	public float TrailEdgeFade = 1f;

	[Tooltip("The minimum length of each aurora path")]
	[Range(0f, 1f)]
	public float PathLengthMin = 0.1f;

	[Tooltip("The maximum length of each aurora path")]
	[Range(0f, 1f)]
	public float PathLengthMax = 0.1f;

	[Tooltip("The minimum distance between the pole at the aurora path start point")]
	[Range(0f, 1f)]
	public float StartMin = 0.1f;

	[Tooltip("The maximum distance between the pole at the aurora path start point")]
	[Range(0f, 1f)]
	public float StartMax = 0.5f;

	[Tooltip("The probability that the aurora path will begin closer to the pole")]
	public float StartBias = 1f;

	[Tooltip("The probability that the aurora path will start on the northern pole")]
	[Range(0f, 1f)]
	public float StartTop = 0.5f;

	[Tooltip("The amount of waypoints the aurora path will follow based on its length")]
	[Range(1f, 100f)]
	public int PointDetail = 10;

	[Tooltip("The strength of the aurora waypoint twisting")]
	public float PointSpiral = 1f;

	[Tooltip("The strength of the aurora waypoint random displacement")]
	[Range(0f, 1f)]
	public float PointJitter = 1f;

	[Tooltip("The amount of times the main texture is tiled based on its length")]
	public float TrailTile = 10f;

	[Tooltip("The flatness of the aurora path")]
	[Range(0.1f, 1f)]
	public float TrailHeights = 1f;

	[Tooltip("The amount of height changes in the aurora path")]
	public int TrailHeightsDetail = 10;

	[Tooltip("The possible colors given to the top half of the aurora path")]
	public Gradient Colors;

	[Tooltip("The amount of color changes an aurora path can have based on its length")]
	public int ColorsDetail = 10;

	[Tooltip("The minimum opacity multiplier of the aurora path colors")]
	[Range(0f, 1f)]
	public float ColorsAlpha = 0.5f;

	[Tooltip("The amount of alpha changes in the aurora path")]
	public float ColorsAlphaBias = 2f;

	[Tooltip("Should the aurora fade out when the camera gets near?")]
	public bool FadeNear;

	[Tooltip("The lookup table used to calculate the fading amount based on the distance")]
	public Texture FadeNearTex;

	[Tooltip("The radius of the fading effect in world space")]
	public float FadeNearRadius = 2f;

	[Tooltip("The thickness of the fading effect in world space")]
	public float FadeNearThickness = 2f;

	[Tooltip("Enabled aurora path animation")]
	public bool Anim;

	[Tooltip("The current age/offset of the animation")]
	public float AnimOffset;

	[Tooltip("The speed of the animation")]
	public float AnimSpeed = 1f;

	[Tooltip("The strength of the aurora path position changes in local space")]
	public float AnimStrength = 0.01f;

	[Tooltip("The amount of the animation strength changes along the aurora path based on its length")]
	public int AnimStrengthDetail = 10;

	[Tooltip("The maximum angle step between sections of the aurora path")]
	public float AnimAngle = 0.01f;

	[Tooltip("The amount of the animation angle changes along the aurora path based on its length")]
	public int AnimAngleDetail = 10;

	public List<SgtAuroraModel> Models;

	[NonSerialized]
	public Material Material;

	[NonSerialized]
	public List<Mesh> Meshes;

	[SerializeField]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	protected bool updateMeshesAndModelsCalled;

	private static List<Vector3> positions = new List<Vector3>();

	private static List<Vector4> coords0 = new List<Vector4>();

	private static List<Color> colors = new List<Color>();

	private static List<Vector3> normals = new List<Vector3>();

	private static List<int> indices = new List<int>();

	public void UpdateMainTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_MainTex", MainTex);
		}
	}

	public void UpdateFadeNearTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_FadeNearTex", FadeNearTex);
		}
	}

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Aurora (Generated)", "Hidden/SgtAurora");
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtAuroraModel sgtAuroraModel = Models[num];
					if (sgtAuroraModel != null)
					{
						sgtAuroraModel.SetMaterial(Material);
					}
				}
			}
		}
		Color value = SgtHelper.Premultiply(SgtHelper.Brighten(Color, Brightness));
		int num2 = (int)(RenderQueue + RenderQueueOffset);
		if (Material.renderQueue != num2)
		{
			Material.renderQueue = num2;
		}
		Material.SetColor("_Color", value);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetFloat("_RadiusMin", RadiusMin);
		Material.SetFloat("_RadiusSize", RadiusMax - RadiusMin);
		SgtHelper.SetTempMaterial(Material);
		if (FadeNear)
		{
			SgtHelper.EnableKeyword("SGT_A");
			Material.SetTexture("_FadeNearTex", FadeNearTex);
			Material.SetFloat("_FadeNearRadius", FadeNearRadius);
			Material.SetFloat("_FadeNearScale", SgtHelper.Reciprocal(FadeNearThickness));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A");
		}
		if (Anim)
		{
			SgtHelper.EnableKeyword("SGT_B");
			Material.SetFloat("_AnimOffset", AnimOffset);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B");
		}
	}

	private void BakeMesh(Mesh mesh)
	{
		mesh.Clear(keepVertexLayout: false);
		mesh.SetVertices(positions);
		mesh.SetUVs(0, coords0);
		mesh.SetColors(colors);
		mesh.SetNormals(normals);
		mesh.SetTriangles(indices, 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * RadiusMax * 2f);
	}

	private Mesh GetMesh(int index)
	{
		SgtHelper.ClearCapacity(positions, 1024);
		SgtHelper.ClearCapacity(coords0, 1024);
		SgtHelper.ClearCapacity(indices, 1024);
		SgtHelper.ClearCapacity(colors, 1024);
		SgtHelper.ClearCapacity(normals, 1024);
		if (index >= Meshes.Count)
		{
			Mesh mesh = SgtHelper.CreateTempMesh("Aurora Mesh (Generated)");
			Meshes.Add(mesh);
			return mesh;
		}
		Mesh mesh2 = Meshes[index];
		if (mesh2 == null)
		{
			Mesh mesh3 = SgtHelper.CreateTempMesh("Aurora Mesh (Generated)");
			Meshes[index] = mesh3;
			mesh2 = mesh3;
		}
		return mesh2;
	}

	private Vector3 GetStart(float angle)
	{
		float num = Mathf.Lerp(StartMin, StartMax, Mathf.Pow(UnityEngine.Random.value, StartBias));
		if (UnityEngine.Random.value < StartTop)
		{
			return new Vector3(Mathf.Sin(angle) * num, 1f, Mathf.Cos(angle) * num);
		}
		return new Vector3(Mathf.Sin(angle) * num, -1f, Mathf.Cos(angle) * num);
	}

	private Vector3 GetNext(Vector3 point, float angle, float speed)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		point.x += Mathf.Sin(angle) * speed;
		point.z += Mathf.Cos(angle) * speed;
		point.x += insideUnitCircle.x * PointJitter;
		point.z += insideUnitCircle.y * PointJitter;
		return Quaternion.Euler(0f, PointSpiral, 0f) * point;
	}

	private float GetNextAngle(float angle)
	{
		return angle + UnityEngine.Random.Range(0f, AnimAngle);
	}

	private float GetNextStrength()
	{
		return UnityEngine.Random.Range(0f - AnimStrength, AnimStrength);
	}

	private Color GetNextColor()
	{
		Color result = Color.white;
		if (Colors != null)
		{
			result = Colors.Evaluate(UnityEngine.Random.value);
		}
		result.a *= Mathf.LerpUnclamped(ColorsAlpha, 1f, Mathf.Pow(UnityEngine.Random.value, ColorsAlphaBias));
		return result;
	}

	private float GetNextHeight()
	{
		return UnityEngine.Random.Range(0f, TrailHeights);
	}

	private void Shift<T>(ref T a, ref T b, ref T c, T d, ref float f)
	{
		a = b;
		b = c;
		c = d;
		f--;
	}

	private void AddPath(ref Mesh mesh, ref int meshCount, ref int vertexCount)
	{
		float num = UnityEngine.Random.Range(PathLengthMin, PathLengthMax);
		int num2 = 2 + (int)(num * (float)PathDetail);
		int num3 = num2 - 1;
		int num4 = num3 * 2 + 2;
		if (vertexCount + num4 > 65000)
		{
			BakeMesh(mesh);
			mesh = GetMesh(meshCount);
			meshCount++;
			vertexCount = 0;
		}
		float num5 = UnityEngine.Random.Range(-(float)Math.PI, (float)Math.PI);
		float speed = 1f / (float)PointDetail;
		float num6 = 1f / (float)PathDetail;
		float num7 = num6 * (float)PointDetail;
		float f = 0f;
		Vector3 a = GetStart(num5 + (float)Math.PI);
		Vector3 b = GetNext(a, num5, speed);
		Vector3 c = GetNext(b, num5, speed);
		Vector3 next = GetNext(c, num5, speed);
		float num8 = 0f;
		float num9 = -1f;
		float num10 = 2f / (float)num2;
		float num11 = num6 * TrailTile;
		float a2 = num5;
		float b2 = GetNextAngle(a2);
		float c2 = GetNextAngle(b2);
		float nextAngle = GetNextAngle(c2);
		float f2 = 0f;
		float num12 = num6 * (float)AnimAngleDetail;
		float a3 = 0f;
		float b3 = GetNextStrength();
		float c3 = GetNextStrength();
		float nextStrength = GetNextStrength();
		float f3 = 0f;
		float num13 = num6 * (float)AnimStrengthDetail;
		Color a4 = GetNextColor();
		Color b4 = GetNextColor();
		Color c4 = GetNextColor();
		Color nextColor = GetNextColor();
		float f4 = 0f;
		float num14 = num6 * (float)ColorsDetail;
		float a5 = GetNextHeight();
		float b5 = GetNextHeight();
		float c5 = GetNextHeight();
		float nextHeight = GetNextHeight();
		float f5 = 0f;
		float num15 = num6 * (float)TrailHeightsDetail;
		for (int i = 0; i < num2; i++)
		{
			while (f >= 1f)
			{
				Shift(ref a, ref b, ref c, next, ref f);
				next = GetNext(c, num5, speed);
			}
			while (f2 >= 1f)
			{
				Shift(ref a2, ref b2, ref c2, nextAngle, ref f2);
				nextAngle = GetNextAngle(c2);
			}
			while (f3 >= 1f)
			{
				Shift(ref a3, ref b3, ref c3, nextStrength, ref f3);
				nextStrength = GetNextStrength();
			}
			while (f4 >= 1f)
			{
				Shift(ref a4, ref b4, ref c4, nextColor, ref f4);
				nextColor = GetNextColor();
			}
			while (f5 >= 1f)
			{
				Shift(ref a5, ref b5, ref c5, nextHeight, ref f5);
				nextHeight = GetNextHeight();
			}
			Vector3 item = SgtHelper.HermiteInterpolate3(a, b, c, next, f);
			float z = SgtHelper.HermiteInterpolate(a2, b2, c2, nextAngle, f2);
			float w = SgtHelper.HermiteInterpolate(a3, b3, c3, nextStrength, f3);
			Color item2 = SgtHelper.HermiteInterpolate(a4, b4, c4, nextColor, f4);
			float y = SgtHelper.HermiteInterpolate(a5, b5, c5, nextHeight, f5);
			item2.a *= Mathf.SmoothStep(1f, 0f, Mathf.Pow(Mathf.Abs(num9), TrailEdgeFade));
			coords0.Add(new Vector4(num8, 0f, z, w));
			coords0.Add(new Vector4(num8, y, z, w));
			positions.Add(item);
			positions.Add(item);
			colors.Add(item2);
			colors.Add(item2);
			f += num7;
			num9 += num10;
			num8 += num11;
			f2 += num12;
			f3 += num13;
			f4 += num14;
			f5 += num15;
		}
		Vector3 vector = positions[1] - positions[0];
		normals.Add(GetNormal(vector, vector));
		normals.Add(GetNormal(vector, vector));
		for (int j = 2; j < num2; j++)
		{
			Vector3 vector2 = positions[j] - positions[j - 1];
			normals.Add(GetNormal(vector, vector2));
			normals.Add(GetNormal(vector, vector2));
			vector = vector2;
		}
		normals.Add(GetNormal(vector, vector));
		normals.Add(GetNormal(vector, vector));
		for (int k = 0; k < num3; k++)
		{
			int num16 = vertexCount + k * 2;
			indices.Add(num16);
			indices.Add(num16 + 1);
			indices.Add(num16 + 2);
			indices.Add(num16 + 3);
			indices.Add(num16 + 2);
			indices.Add(num16 + 1);
		}
		vertexCount += num4;
	}

	private Vector3 GetNormal(Vector3 a, Vector3 b)
	{
		return Vector3.Cross(a.normalized, b.normalized);
	}

	[ContextMenu("Update Meshes And Models")]
	public void UpdateMeshesAndModels()
	{
		updateMeshesAndModelsCalled = true;
		if (Meshes == null)
		{
			Meshes = new List<Mesh>();
		}
		if (Models == null)
		{
			Models = new List<SgtAuroraModel>();
		}
		if (PathDetail > 0 && PathLengthMin > 0f && PathLengthMax > 0f)
		{
			int meshCount = 1;
			Mesh mesh = GetMesh(0);
			int vertexCount = 0;
			SgtHelper.BeginRandomSeed(Seed);
			for (int i = 0; i < PathCount; i++)
			{
				AddPath(ref mesh, ref meshCount, ref vertexCount);
			}
			SgtHelper.EndRandomSeed();
			BakeMesh(mesh);
			for (int num = Meshes.Count - 1; num >= meshCount; num--)
			{
				Mesh mesh2 = Meshes[num];
				if (mesh2 != null)
				{
					mesh2.Clear(keepVertexLayout: false);
					SgtObjectPool<Mesh>.Add(mesh2);
				}
				Meshes.RemoveAt(num);
			}
		}
		for (int j = 0; j < Meshes.Count; j++)
		{
			SgtAuroraModel orAddModel = GetOrAddModel(j);
			orAddModel.SetMesh(Meshes[j]);
		}
		if (Models != null)
		{
			int num2 = Mathf.Max(0, Meshes.Count);
			for (int num3 = Models.Count - 1; num3 >= num2; num3--)
			{
				SgtAuroraModel.Pool(Models[num3]);
				Models.RemoveAt(num3);
			}
		}
	}

	public static SgtAurora CreateAurora(int layer = 0, Transform parent = null)
	{
		return CreateAurora(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtAurora CreateAurora(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Aurora", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtAurora>();
	}

	protected virtual void OnEnable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtAuroraModel sgtAuroraModel = Models[num];
				if (sgtAuroraModel != null)
				{
					sgtAuroraModel.gameObject.SetActive(value: true);
				}
			}
		}
		if (startCalled)
		{
			CheckUpdateCalls();
		}
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			CheckUpdateCalls();
		}
	}

	protected virtual void Update()
	{
		if (Anim)
		{
			if (Application.isPlaying)
			{
				AnimOffset += Time.deltaTime * AnimSpeed;
			}
			if (Material != null)
			{
				Material.SetFloat("_AnimOffset", AnimOffset);
			}
		}
	}

	protected virtual void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtAuroraModel sgtAuroraModel = Models[num];
			if (sgtAuroraModel != null)
			{
				sgtAuroraModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Meshes != null)
		{
			for (int num = Meshes.Count - 1; num >= 0; num--)
			{
				Mesh mesh = Meshes[num];
				if (mesh != null)
				{
					mesh.Clear(keepVertexLayout: false);
					SgtObjectPool<Mesh>.Add(Meshes[num]);
				}
			}
		}
		if (Models != null)
		{
			for (int num2 = Models.Count - 1; num2 >= 0; num2--)
			{
				SgtAuroraModel.MarkForDestruction(Models[num2]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private void CameraPreCull(Camera camera)
	{
		if (CameraOffset == 0f || Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtAuroraModel sgtAuroraModel = Models[num];
			if (sgtAuroraModel != null)
			{
				sgtAuroraModel.Revert();
				Transform transform = sgtAuroraModel.transform;
				Vector3 normalized = (transform.position - camera.transform.position).normalized;
				transform.position += normalized * CameraOffset;
				sgtAuroraModel.Save(camera);
			}
		}
	}

	private void CameraPreRender(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtAuroraModel sgtAuroraModel = Models[num];
			if (sgtAuroraModel != null)
			{
				sgtAuroraModel.Restore(camera);
			}
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtAuroraModel sgtAuroraModel = Models[num];
			if (sgtAuroraModel != null)
			{
				sgtAuroraModel.Revert();
			}
		}
	}

	private SgtAuroraModel GetOrAddModel(int index)
	{
		SgtAuroraModel sgtAuroraModel = null;
		if (Models == null)
		{
			Models = new List<SgtAuroraModel>();
		}
		if (index < Models.Count)
		{
			sgtAuroraModel = Models[index];
		}
		else
		{
			Models.Add(sgtAuroraModel);
		}
		if (sgtAuroraModel == null)
		{
			SgtAuroraModel sgtAuroraModel2 = SgtAuroraModel.Create(this);
			Models[index] = sgtAuroraModel2;
			sgtAuroraModel = sgtAuroraModel2;
			sgtAuroraModel.SetMaterial(Material);
		}
		return sgtAuroraModel;
	}

	private void CheckUpdateCalls()
	{
		if (!updateMaterialCalled)
		{
			UpdateMaterial();
		}
		if (!updateMeshesAndModelsCalled)
		{
			UpdateMeshesAndModels();
		}
	}
}
