using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class SgtBelt : SgtQuads
{
	[Tooltip("The height texture of this belt")]
	public Texture HeightTex;

	[Tooltip("The amount of seconds this belt has been animating for")]
	[FormerlySerializedAs("Age")]
	public float OrbitOffset;

	[Tooltip("The animation speed of this belt")]
	[FormerlySerializedAs("TimeScale")]
	public float OrbitSpeed = 1f;

	[Tooltip("Does this receive light?")]
	public bool Lit;

	[Tooltip("The lookup table used to calculate the lighting")]
	public Texture LightingTex;

	[Tooltip("The lights shining on this belt (max = 1 light, 2 scatter)")]
	public List<Light> Lights;

	[Tooltip("The shadows casting on this belt (max = 2)")]
	public List<SgtShadow> Shadows;

	protected override string ShaderName => "Hidden/SgtBelt";

	public SgtCustomBelt MakeEditableCopy(int layer = 0, Transform parent = null)
	{
		return MakeEditableCopy(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public SgtCustomBelt MakeEditableCopy(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Editable Belt Copy", layer, parent, localPosition, localRotation, localScale);
		SgtCustomBelt sgtCustomBelt = SgtHelper.AddComponent<SgtCustomBelt>(gameObject, recordUndo: false);
		List<SgtBeltAsteroid> list = new List<SgtBeltAsteroid>();
		int num = BeginQuads();
		for (int i = 0; i < num; i++)
		{
			SgtBeltAsteroid quad = SgtClassPool<SgtBeltAsteroid>.Pop() ?? new SgtBeltAsteroid();
			NextQuad(ref quad, i);
			list.Add(quad);
		}
		EndQuads();
		if (Lights != null)
		{
			sgtCustomBelt.Lights = new List<Light>(Lights);
		}
		if (Shadows != null)
		{
			sgtCustomBelt.Shadows = new List<SgtShadow>(Shadows);
		}
		sgtCustomBelt.Color = Color;
		sgtCustomBelt.Brightness = Brightness;
		sgtCustomBelt.MainTex = MainTex;
		sgtCustomBelt.HeightTex = HeightTex;
		sgtCustomBelt.Layout = Layout;
		sgtCustomBelt.LayoutColumns = LayoutColumns;
		sgtCustomBelt.LayoutRows = LayoutRows;
		if (Rects != null)
		{
			sgtCustomBelt.Rects = new List<Rect>(Rects);
		}
		sgtCustomBelt.RenderQueue = RenderQueue;
		sgtCustomBelt.RenderQueueOffset = RenderQueueOffset;
		sgtCustomBelt.OrbitOffset = OrbitOffset;
		sgtCustomBelt.OrbitSpeed = OrbitSpeed;
		sgtCustomBelt.Asteroids = list;
		sgtCustomBelt.UpdateMaterial();
		sgtCustomBelt.UpdateMeshesAndModels();
		return sgtCustomBelt;
	}

	public virtual void UpdateLightingTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	protected override void OnEnable()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		base.OnEnable();
	}

	protected virtual void LateUpdate()
	{
		if (Application.isPlaying)
		{
			OrbitOffset += Time.deltaTime * OrbitSpeed;
		}
		if (Material != null)
		{
			Material.SetFloat("_Age", OrbitOffset);
		}
		if (Material != null)
		{
			SgtHelper.SetTempMaterial(Material);
			SgtHelper.WriteLights(Lit, Lights, 2, transform.position, transform, null, SgtHelper.Brighten(Color, Brightness), 1f);
			SgtHelper.WriteShadows(Shadows, 2);
		}
	}

	protected override void OnDisable()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		base.OnDisable();
	}

	protected override void BuildMaterial()
	{
		base.BuildMaterial();
		Material.SetTexture("_HeightTex", HeightTex);
		Material.SetFloat("_Age", OrbitOffset);
		if (Lit)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	protected abstract void NextQuad(ref SgtBeltAsteroid quad, int starIndex);

	protected override void BuildMesh(Mesh mesh, int asteroidIndex, int asteroidCount)
	{
		Vector3[] array = new Vector3[asteroidCount * 4];
		Color[] array2 = new Color[asteroidCount * 4];
		Vector3[] array3 = new Vector3[asteroidCount * 4];
		Vector4[] array4 = new Vector4[asteroidCount * 4];
		Vector2[] array5 = new Vector2[asteroidCount * 4];
		Vector2[] array6 = new Vector2[asteroidCount * 4];
		int[] array7 = new int[asteroidCount * 6];
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < asteroidCount; i++)
		{
			NextQuad(ref SgtBeltAsteroid.Temp, asteroidIndex + i);
			int num3 = i * 4;
			int num4 = i * 6;
			float radius = SgtBeltAsteroid.Temp.Radius;
			float orbitDistance = SgtBeltAsteroid.Temp.OrbitDistance;
			float height = SgtBeltAsteroid.Temp.Height;
			Vector4 vector = SgtQuads.tempCoords[SgtHelper.Mod(SgtBeltAsteroid.Temp.Variant, SgtQuads.tempCoords.Count)];
			num = Mathf.Max(num, orbitDistance + radius);
			num2 = Mathf.Max(num2, height + radius);
			ref Vector3 reference = ref array[num3];
			ref Vector3 reference2 = ref array[num3 + 1];
			ref Vector3 reference3 = ref array[num3 + 2];
			ref Vector3 reference4 = ref array[num3 + 3];
			reference = (reference2 = (reference3 = (reference4 = new Vector3(SgtBeltAsteroid.Temp.OrbitAngle, orbitDistance, SgtBeltAsteroid.Temp.OrbitSpeed))));
			ref Color reference5 = ref array2[num3];
			ref Color reference6 = ref array2[num3 + 1];
			ref Color reference7 = ref array2[num3 + 2];
			ref Color reference8 = ref array2[num3 + 3];
			reference5 = (reference6 = (reference7 = (reference8 = SgtBeltAsteroid.Temp.Color)));
			ref Vector3 reference9 = ref array3[num3];
			reference9 = new Vector3(-1f, 1f, 0f);
			ref Vector3 reference10 = ref array3[num3 + 1];
			reference10 = new Vector3(1f, 1f, 0f);
			ref Vector3 reference11 = ref array3[num3 + 2];
			reference11 = new Vector3(-1f, -1f, 0f);
			ref Vector3 reference12 = ref array3[num3 + 3];
			reference12 = new Vector3(1f, -1f, 0f);
			ref Vector4 reference13 = ref array4[num3];
			ref Vector4 reference14 = ref array4[num3 + 1];
			ref Vector4 reference15 = ref array4[num3 + 2];
			ref Vector4 reference16 = ref array4[num3 + 3];
			reference13 = (reference14 = (reference15 = (reference16 = new Vector4(SgtBeltAsteroid.Temp.Angle / (float)Math.PI, SgtBeltAsteroid.Temp.Spin / (float)Math.PI, 0f, 0f))));
			ref Vector2 reference17 = ref array5[num3];
			reference17 = new Vector2(vector.x, vector.y);
			ref Vector2 reference18 = ref array5[num3 + 1];
			reference18 = new Vector2(vector.z, vector.y);
			ref Vector2 reference19 = ref array5[num3 + 2];
			reference19 = new Vector2(vector.x, vector.w);
			ref Vector2 reference20 = ref array5[num3 + 3];
			reference20 = new Vector2(vector.z, vector.w);
			ref Vector2 reference21 = ref array6[num3];
			ref Vector2 reference22 = ref array6[num3 + 1];
			ref Vector2 reference23 = ref array6[num3 + 2];
			ref Vector2 reference24 = ref array6[num3 + 3];
			reference21 = (reference22 = (reference23 = (reference24 = new Vector2(radius, height))));
			array7[num4] = num3;
			array7[num4 + 1] = num3 + 1;
			array7[num4 + 2] = num3 + 2;
			array7[num4 + 3] = num3 + 3;
			array7[num4 + 4] = num3 + 2;
			array7[num4 + 5] = num3 + 1;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.normals = array3;
		mesh.tangents = array4;
		mesh.uv = array5;
		mesh.uv2 = array6;
		mesh.triangles = array7;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(num * 2f, num2 * 2f, num * 2f));
	}

	private void ObserverPreRender(SgtObserver observer)
	{
		if (Material != null)
		{
			Material.SetFloat("_CameraRollAngle", observer.RollAngle * ((float)Math.PI / 180f));
		}
	}

	protected void CameraPreRender(Camera camera)
	{
		if (Material != null)
		{
			SgtObserver sgtObserver = SgtObserver.Find(camera);
			if (sgtObserver != null)
			{
				Material.SetFloat("_CameraRollAngle", sgtObserver.RollAngle * ((float)Math.PI / 180f));
			}
		}
	}

	protected void CameraPostRender(Camera camera)
	{
		if (Material != null)
		{
			Material.SetFloat("_CameraRollAngle", 0f);
		}
	}
}
