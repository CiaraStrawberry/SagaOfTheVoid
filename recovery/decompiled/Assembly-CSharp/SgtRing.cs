using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring")]
public class SgtRing : MonoBehaviour
{
	public static List<SgtRing> AllRings = new List<SgtRing>();

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The Color.rgb values are multiplied by this")]
	public float Brightness = 1f;

	[Tooltip("The render queue group for this ring")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this ring")]
	public int RenderQueueOffset;

	[Tooltip("The texture applied to the ring (left side = inside, right side = outside)")]
	public Texture MainTex;

	[Tooltip("The mesh applied to each ring model")]
	public Mesh Mesh;

	[Tooltip("The amount of segments this ring is split into")]
	[FormerlySerializedAs("SegmentCount")]
	public int Segments = 8;

	[Tooltip("Should the ring have a detail texture?")]
	public bool Detail;

	[Tooltip("The detail texture applied to the ring")]
	public Texture DetailTex;

	[Tooltip("The detail texture horizontal tiling")]
	public float DetailScaleX = 1f;

	[Tooltip("The detail texture vertical tiling")]
	public int DetailScaleY = 1;

	[Tooltip("The UV offset of the detail texture")]
	public Vector2 DetailOffset;

	[Tooltip("The scroll speed of the detail texture UV offset")]
	public Vector2 DetailSpeed;

	[Tooltip("The amount the detail texture is twisted around the ring")]
	public float DetailTwist;

	[Tooltip("The amount the twisting is pushed to the outer edge")]
	public float DetailTwistBias = 1f;

	[Tooltip("Fade out as the camera approaches?")]
	public bool Fade;

	[Tooltip("The lookup table used to calculate the fade")]
	public Texture FadeTex;

	[Tooltip("The distance the fading begins from in world space")]
	public float FadeDistance = 1f;

	[Tooltip("Should light scatter through the rings?")]
	public bool Scattering;

	[Tooltip("The sharpness of the front scattered light")]
	public float ScatteringMie = 8f;

	[Tooltip("The scattering brightness multiplier")]
	public float ScatteringStrength = 25f;

	[Tooltip("Does this receive light?")]
	public bool Lit;

	[Tooltip("The lookup table used to calculate the lighting")]
	public Texture LightingTex;

	[Tooltip("The lights shining on this ring (max = 1 light, 2 scatter)")]
	public List<Light> Lights;

	[Tooltip("The shadows casting on this ring (max = 2)")]
	public List<SgtShadow> Shadows;

	[FormerlySerializedAs("Segments")]
	public List<SgtRingModel> Models;

	[NonSerialized]
	public Material Material;

	[SerializeField]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	private bool updateModelsCalled;

	protected virtual string ShaderName => "Hidden/SgtRing";

	public virtual void UpdateMainTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_MainTex", MainTex);
		}
	}

	public virtual void UpdateFadeTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_FadeTex", FadeTex);
		}
	}

	public virtual void UpdateLightingTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	[ContextMenu("Update Material")]
	public virtual void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Ring (Generated)", ShaderName);
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtRingModel sgtRingModel = Models[num];
					if (sgtRingModel != null)
					{
						sgtRingModel.SetMaterial(Material);
					}
				}
			}
		}
		Color value = SgtHelper.Brighten(Color, Brightness);
		int renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.renderQueue = renderQueue;
		Material.SetColor("_Color", value);
		Material.SetTexture("_MainTex", MainTex);
		if (Detail)
		{
			SgtHelper.EnableKeyword("SGT_B", Material);
			Material.SetTexture("_DetailTex", DetailTex);
			Material.SetVector("_DetailScale", new Vector2(DetailScaleX, DetailScaleY));
			Material.SetFloat("_DetailTwist", DetailTwist);
			Material.SetFloat("_DetailTwistBias", DetailTwistBias);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B", Material);
		}
		if (Fade)
		{
			SgtHelper.EnableKeyword("SGT_C", Material);
			Material.SetTexture("_FadeTex", FadeTex);
			Material.SetFloat("_FadeDistanceRecip", SgtHelper.Reciprocal(FadeDistance));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_C", Material);
		}
		if (Lit)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
		if (Scattering)
		{
			SgtHelper.EnableKeyword("SGT_A", Material);
			Material.SetFloat("_ScatteringMie", ScatteringMie * ScatteringMie);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A", Material);
		}
		UpdateMaterialNonSerialized();
	}

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtRingModel sgtRingModel = Models[num];
			if (sgtRingModel != null)
			{
				sgtRingModel.SetMesh(Mesh);
			}
		}
	}

	[ContextMenu("Update Models")]
	public void UpdateModels()
	{
		updateModelsCalled = true;
		float num = SgtHelper.Divide(360f, Segments);
		for (int i = 0; i < Segments; i++)
		{
			SgtRingModel orAddModel = GetOrAddModel(i);
			float y = num * (float)i;
			Quaternion rotation = Quaternion.Euler(0f, y, 0f);
			orAddModel.SetMesh(Mesh);
			orAddModel.SetMaterial(Material);
			orAddModel.SetRotation(rotation);
		}
		if (Models != null)
		{
			int num2 = Mathf.Max(0, Segments);
			for (int num3 = Models.Count - 1; num3 >= num2; num3--)
			{
				SgtRingModel.Pool(Models[num3]);
				Models.RemoveAt(num3);
			}
		}
	}

	public static SgtRing CreateRing(int layer = 0, Transform parent = null)
	{
		return CreateRing(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtRing CreateRing(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Ring", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtRing>();
	}

	protected virtual void OnEnable()
	{
		AllRings.Add(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtRingModel sgtRingModel = Models[num];
				if (sgtRingModel != null)
				{
					sgtRingModel.gameObject.SetActive(value: true);
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

	protected virtual void LateUpdate()
	{
		if (!(Material != null))
		{
			return;
		}
		SgtHelper.SetTempMaterial(Material);
		SgtHelper.WriteLights(Lit, Lights, 2, transform.position, null, null, SgtHelper.Brighten(Color, Brightness), ScatteringStrength);
		SgtHelper.WriteShadows(Shadows, 2);
		if (Detail)
		{
			if (Application.isPlaying)
			{
				DetailOffset += DetailSpeed * Time.deltaTime;
			}
			Material.SetVector("_DetailOffset", DetailOffset);
		}
	}

	protected virtual void OnDisable()
	{
		AllRings.Remove(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtRingModel sgtRingModel = Models[num];
			if (sgtRingModel != null)
			{
				sgtRingModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtRingModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private void UpdateMaterialNonSerialized()
	{
		SgtHelper.SetTempMaterial(Material);
		SgtHelper.WriteShadowsNonSerialized(Shadows, 2);
	}

	private void CameraPreRender(Camera camera)
	{
		if (Material != null)
		{
			UpdateMaterialNonSerialized();
		}
	}

	private SgtRingModel GetOrAddModel(int index)
	{
		SgtRingModel sgtRingModel = null;
		if (Models == null)
		{
			Models = new List<SgtRingModel>();
		}
		if (index < Models.Count)
		{
			sgtRingModel = Models[index];
			if (sgtRingModel == null)
			{
				sgtRingModel = SgtRingModel.Create(this);
				Models[index] = sgtRingModel;
			}
		}
		else
		{
			sgtRingModel = SgtRingModel.Create(this);
			Models.Add(sgtRingModel);
		}
		return sgtRingModel;
	}

	private void CheckUpdateCalls()
	{
		if (!updateMaterialCalled)
		{
			UpdateMaterial();
		}
		if (!updateModelsCalled)
		{
			UpdateModels();
		}
	}
}
