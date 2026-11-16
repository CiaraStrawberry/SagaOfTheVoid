using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class SgtPointStarfield : SgtQuads
{
	[Tooltip("Should the stars automatically be placed on top of the currently rendering camera?")]
	[FormerlySerializedAs("FollowObservers")]
	public bool FollowCameras;

	[Tooltip("Should the stars fade out if they're intersecting solid geometry?")]
	[Range(0f, 1000f)]
	public float Softness;

	[Tooltip("Should the stars wrap around the camera if they get too far away?")]
	public bool Wrap;

	[Tooltip("The size of the wrapped area")]
	public Vector3 WrapSize = Vector3.one;

	[Tooltip("Should the stars stretch if an observer moves?")]
	[FormerlySerializedAs("StretchToObservers")]
	public bool Stretch;

	[Tooltip("The vector of the stretching")]
	public Vector3 StretchVector;

	[Tooltip("The scale of the stretching relative to the velocity")]
	public float StretchScale = 1f;

	[Tooltip("Should the stars fade out when the camera gets near?")]
	public bool FadeNear;

	[Tooltip("The lookup table used to calculate the fading amount based on the distance")]
	public Texture FadeNearTex;

	[Tooltip("The radius of the fading effect in world coordinates")]
	public float FadeNearRadius = 2f;

	[Tooltip("The thickness of the fading effect in local coordinates")]
	public float FadeNearThickness = 2f;

	[Tooltip("Should the stars fade out when the camera gets too far away?")]
	public bool FadeFar;

	[Tooltip("The lookup table used to calculate the fading amount based on the distance")]
	public Texture FadeFarTex;

	[Tooltip("The radius of the fading effect in world coordinates")]
	public float FadeFarRadius = 2f;

	[Tooltip("The thickness of the fading effect in world coordinates")]
	public float FadeFarThickness = 2f;

	[Tooltip("Should the stars pulse in size over time?")]
	[FormerlySerializedAs("AllowPulse")]
	public bool Pulse;

	[Tooltip("The amount of seconds this starfield has been animating")]
	[FormerlySerializedAs("Age")]
	public float PulseOffset;

	[Tooltip("The animation speed of this starfield")]
	[FormerlySerializedAs("TimeScale")]
	public float PulseSpeed = 1f;

	protected override string ShaderName => "Hidden/SgtPointStarfield";

	public void UpdateFadeFarTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_FadeFarTex", FadeFarTex);
		}
	}

	public void UpdateFadeNearTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_FadeNearTex", FadeNearTex);
		}
	}

	public SgtCustomStarfield MakeEditableCopy(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Editable Starfield Copy", layer, parent, localPosition, localRotation, localScale);
		SgtCustomStarfield sgtCustomStarfield = SgtHelper.AddComponent<SgtCustomStarfield>(gameObject, recordUndo: false);
		List<SgtPointStar> list = new List<SgtPointStar>();
		int num = BeginQuads();
		for (int i = 0; i < num; i++)
		{
			SgtPointStar quad = SgtClassPool<SgtPointStar>.Pop() ?? new SgtPointStar();
			NextQuad(ref quad, i);
			list.Add(quad);
		}
		EndQuads();
		sgtCustomStarfield.Color = Color;
		sgtCustomStarfield.Brightness = Brightness;
		sgtCustomStarfield.MainTex = MainTex;
		sgtCustomStarfield.Layout = Layout;
		sgtCustomStarfield.LayoutColumns = LayoutColumns;
		sgtCustomStarfield.LayoutRows = LayoutRows;
		sgtCustomStarfield.RenderQueue = RenderQueue;
		sgtCustomStarfield.RenderQueueOffset = RenderQueueOffset;
		sgtCustomStarfield.FollowCameras = FollowCameras;
		sgtCustomStarfield.Softness = Softness;
		sgtCustomStarfield.Wrap = Wrap;
		sgtCustomStarfield.WrapSize = WrapSize;
		sgtCustomStarfield.Stretch = Stretch;
		sgtCustomStarfield.StretchVector = StretchVector;
		sgtCustomStarfield.StretchScale = StretchScale;
		sgtCustomStarfield.FadeNear = FadeNear;
		sgtCustomStarfield.FadeNearRadius = FadeNearRadius;
		sgtCustomStarfield.FadeNearThickness = FadeNearThickness;
		sgtCustomStarfield.FadeNearTex = FadeNearTex;
		sgtCustomStarfield.FadeFar = FadeFar;
		sgtCustomStarfield.FadeFarRadius = FadeFarRadius;
		sgtCustomStarfield.FadeFarThickness = FadeFarThickness;
		sgtCustomStarfield.FadeFarTex = FadeFarTex;
		sgtCustomStarfield.Pulse = Pulse;
		sgtCustomStarfield.PulseOffset = PulseOffset;
		sgtCustomStarfield.PulseSpeed = PulseSpeed;
		sgtCustomStarfield.Stars = list;
		sgtCustomStarfield.UpdateMaterial();
		sgtCustomStarfield.UpdateMeshesAndModels();
		return sgtCustomStarfield;
	}

	public SgtCustomStarfield MakeEditableCopy(int layer = 0, Transform parent = null)
	{
		return MakeEditableCopy(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	protected override void OnEnable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		base.OnEnable();
	}

	protected virtual void LateUpdate()
	{
		if (Application.isPlaying)
		{
			PulseOffset += Time.deltaTime * PulseSpeed;
		}
		if (Material != null && Pulse)
		{
			Material.SetFloat("_PulseOffset", PulseOffset);
		}
	}

	protected override void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		base.OnDisable();
	}

	protected override void BuildMaterial()
	{
		base.BuildMaterial();
		if (Wrap)
		{
			SgtHelper.EnableKeyword("SGT_B", Material);
			Material.SetVector("_WrapSize", WrapSize);
			Material.SetVector("_WrapSizeRecip", SgtHelper.Reciprocal3(WrapSize));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B", Material);
		}
		if (Stretch)
		{
			SgtHelper.EnableKeyword("SGT_C", Material);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_C", Material);
		}
		if (FadeNear)
		{
			SgtHelper.EnableKeyword("SGT_D", Material);
			Material.SetTexture("_FadeNearTex", FadeNearTex);
			Material.SetFloat("_FadeNearRadius", FadeNearRadius);
			Material.SetFloat("_FadeNearScale", SgtHelper.Reciprocal(FadeNearThickness));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_D", Material);
		}
		if (FadeFar)
		{
			SgtHelper.EnableKeyword("SGT_E", Material);
			Material.SetTexture("_FadeFarTex", FadeFarTex);
			Material.SetFloat("_FadeFarRadius", FadeFarRadius);
			Material.SetFloat("_FadeFarScale", SgtHelper.Reciprocal(FadeFarThickness));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_E", Material);
		}
		if (Pulse)
		{
			SgtHelper.EnableKeyword("LIGHT_1", Material);
			Material.SetFloat("_PulseOffset", PulseOffset);
		}
		else
		{
			SgtHelper.DisableKeyword("LIGHT_1", Material);
		}
		if (Softness > 0f)
		{
			SgtHelper.EnableKeyword("LIGHT_2", Material);
			Material.SetFloat("_InvFade", SgtHelper.Reciprocal(Softness));
		}
		else
		{
			SgtHelper.DisableKeyword("LIGHT_2", Material);
		}
	}

	protected abstract void NextQuad(ref SgtPointStar quad, int starIndex);

	protected override void BuildMesh(Mesh mesh, int starIndex, int starCount)
	{
		Vector3[] array = new Vector3[starCount * 4];
		Color[] array2 = new Color[starCount * 4];
		Vector3[] array3 = new Vector3[starCount * 4];
		Vector4[] array4 = new Vector4[starCount * 4];
		Vector2[] array5 = new Vector2[starCount * 4];
		Vector2[] array6 = new Vector2[starCount * 4];
		int[] array7 = new int[starCount * 6];
		bool minMaxSet = false;
		Vector3 min = default;
		Vector3 max = default;
		for (int i = 0; i < starCount; i++)
		{
			NextQuad(ref SgtPointStar.Temp, starIndex + i);
			int num = i * 4;
			int num2 = i * 6;
			Vector3 position = SgtPointStar.Temp.Position;
			float radius = SgtPointStar.Temp.Radius;
			float z = Mathf.Repeat(SgtPointStar.Temp.Angle / 180f, 2f) - 1f;
			Vector4 vector = SgtQuads.tempCoords[SgtHelper.Mod(SgtPointStar.Temp.Variant, SgtQuads.tempCoords.Count)];
			SgtQuads.ExpandBounds(ref minMaxSet, ref min, ref max, position, radius);
			ref Vector3 reference = ref array[num];
			ref Vector3 reference2 = ref array[num + 1];
			ref Vector3 reference3 = ref array[num + 2];
			reference = (reference2 = (reference3 = (array[num + 3] = position)));
			ref Color reference4 = ref array2[num];
			ref Color reference5 = ref array2[num + 1];
			ref Color reference6 = ref array2[num + 2];
			ref Color reference7 = ref array2[num + 3];
			reference4 = (reference5 = (reference6 = (reference7 = SgtPointStar.Temp.Color)));
			ref Vector3 reference8 = ref array3[num];
			reference8 = new Vector3(-1f, 1f, z);
			ref Vector3 reference9 = ref array3[num + 1];
			reference9 = new Vector3(1f, 1f, z);
			ref Vector3 reference10 = ref array3[num + 2];
			reference10 = new Vector3(-1f, -1f, z);
			ref Vector3 reference11 = ref array3[num + 3];
			reference11 = new Vector3(1f, -1f, z);
			ref Vector4 reference12 = ref array4[num];
			ref Vector4 reference13 = ref array4[num + 1];
			ref Vector4 reference14 = ref array4[num + 2];
			ref Vector4 reference15 = ref array4[num + 3];
			reference12 = (reference13 = (reference14 = (reference15 = new Vector4(SgtPointStar.Temp.PulseOffset, SgtPointStar.Temp.PulseSpeed, SgtPointStar.Temp.PulseRange, 0f))));
			ref Vector2 reference16 = ref array5[num];
			reference16 = new Vector2(vector.x, vector.y);
			ref Vector2 reference17 = ref array5[num + 1];
			reference17 = new Vector2(vector.z, vector.y);
			ref Vector2 reference18 = ref array5[num + 2];
			reference18 = new Vector2(vector.x, vector.w);
			ref Vector2 reference19 = ref array5[num + 3];
			reference19 = new Vector2(vector.z, vector.w);
			ref Vector2 reference20 = ref array6[num];
			reference20 = new Vector2(radius, 0.5f);
			ref Vector2 reference21 = ref array6[num + 1];
			reference21 = new Vector2(radius, -0.5f);
			ref Vector2 reference22 = ref array6[num + 2];
			reference22 = new Vector2(radius, 0.5f);
			ref Vector2 reference23 = ref array6[num + 3];
			reference23 = new Vector2(radius, -0.5f);
			array7[num2] = num;
			array7[num2 + 1] = num + 1;
			array7[num2 + 2] = num + 2;
			array7[num2 + 3] = num + 3;
			array7[num2 + 4] = num + 2;
			array7[num2 + 5] = num + 1;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.normals = array3;
		mesh.tangents = array4;
		mesh.uv = array5;
		mesh.uv2 = array6;
		mesh.triangles = array7;
		mesh.bounds = SgtHelper.NewBoundsFromMinMax(min, max);
	}

	protected virtual void CameraPreCull(Camera camera)
	{
		if ((!FollowCameras && !Wrap) || Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtQuadsModel sgtQuadsModel = Models[num];
			if (sgtQuadsModel != null)
			{
				sgtQuadsModel.Revert();
				if (FollowCameras)
				{
					sgtQuadsModel.transform.position = camera.transform.position;
				}
				sgtQuadsModel.Save(camera);
				if (Wrap)
				{
					sgtQuadsModel.transform.position = camera.transform.position;
				}
			}
		}
	}

	protected void CameraPreRender(Camera camera)
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtQuadsModel sgtQuadsModel = Models[num];
				if (sgtQuadsModel != null)
				{
					sgtQuadsModel.Restore(camera);
				}
			}
		}
		if (Material != null)
		{
			Vector3 stretchVector = StretchVector;
			SgtObserver sgtObserver = SgtObserver.Find(camera);
			if (sgtObserver != null)
			{
				Material.SetFloat("_CameraRollAngle", sgtObserver.RollAngle * ((float)Math.PI / 180f));
				stretchVector += sgtObserver.Velocity * StretchScale;
			}
			if (Stretch)
			{
				Material.SetVector("_StretchVector", stretchVector);
				Material.SetVector("_StretchDirection", stretchVector.normalized);
				Material.SetFloat("_StretchLength", stretchVector.magnitude);
			}
		}
	}

	protected void CameraPostRender(Camera camera)
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtQuadsModel sgtQuadsModel = Models[num];
				if (sgtQuadsModel != null)
				{
					sgtQuadsModel.Revert();
				}
			}
		}
		if (Material != null)
		{
			Material.SetFloat("_CameraRollAngle", 0f);
			Material.SetVector("_StretchVector", Vector3.zero);
			Material.SetVector("_StretchDirection", Vector3.zero);
			Material.SetFloat("_StretchLength", 0f);
		}
	}
}
