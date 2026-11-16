using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Atmosphere")]
public class SgtAtmosphere : MonoBehaviour
{
	public static List<SgtAtmosphere> AllAtmospheres = new List<SgtAtmosphere>();

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The Color.rgb values are multiplied by this")]
	public float Brightness = 1f;

	[Tooltip("The render queue group of the surface and sky materials")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset of the surface and sky materials")]
	public int RenderQueueOffset;

	[Tooltip("The height of the sky above the surface in local space")]
	public float Height = 0.1f;

	[Tooltip("If you want an extra-thin or extra-thick density, you can adjust that here (0 = default)")]
	[FormerlySerializedAs("Fog")]
	public float InnerFog;

	[Tooltip("If you want an extra-thin or extra-thick density, you can adjust that here (0 = default)")]
	[FormerlySerializedAs("Fog")]
	public float OuterFog;

	[Tooltip("The opacity multiplier of the sky when the camera is inside the sky mesh")]
	public float Sky = 1f;

	[Tooltip("The amount the atmosphere gets moved toward the current camera")]
	public float CameraOffset;

	[Tooltip("Does this atmosphere receive light?")]
	public bool Lit;

	[Tooltip("The lookup texture used to calculate the lighting color")]
	public Texture LightingTex;

	[Tooltip("Should lights scatter through the atmosphere?")]
	public bool Scattering;

	[Tooltip("Should lights scatter through the atmosphere onto the ground?")]
	public bool GroundScattering;

	[Tooltip("The lookup texture used to calculate the scattering brightness")]
	public Texture ScatteringTex;

	[Tooltip("The scattering brightness multiplier")]
	public float ScatteringStrength = 3f;

	[Tooltip("The sharpness of the front scattered light")]
	public float ScatteringMie = 50f;

	[Tooltip("The brightness of the front and back scattered light")]
	public float ScatteringRayleigh = 0.1f;

	[Tooltip("The lights shining on this atmosphere (max = 1 light, 2 = scattering)")]
	public List<Light> Lights;

	[Tooltip("The shadows casting on this atmosphere (max = 2)")]
	public List<SgtShadow> Shadows;

	[Tooltip("The lookup table used to calculate the optical depth of the surface material")]
	public Texture InnerDepthTex;

	[Tooltip("The radius of the inner renderers (surface) in local coordinates")]
	public float InnerMeshRadius = 1f;

	[Tooltip("The renderers that are used to render the surface mesh")]
	public List<MeshRenderer> InnerRenderers;

	[Tooltip("The lookup table used to calculate the optical depth of the sky material")]
	public Texture2D OuterDepthTex;

	[Tooltip("The radius of the OuterMeshes in local space")]
	public float OuterMeshRadius = 1f;

	[Tooltip("The meshes used to render the sky (should form a sphere)")]
	public List<Mesh> OuterMeshes;

	public List<SgtAtmosphereOuter> Outers;

	[NonSerialized]
	public Material InnerMaterial;

	[NonSerialized]
	public Material OuterMaterial;

	[SerializeField]
	protected bool awakeCalled;

	[SerializeField]
	protected bool startCalled;

	[NonSerialized]
	protected bool updateMaterialsCalled;

	[NonSerialized]
	protected bool updateOutersCalled;

	private SgtTerrain terrain;

	public float OuterRadius => InnerMeshRadius + Height;

	public void UpdateDepthTex()
	{
		if (InnerMaterial != null)
		{
			InnerMaterial.SetTexture("_DepthTex", InnerDepthTex);
		}
		if (OuterMaterial != null)
		{
			OuterMaterial.SetTexture("_DepthTex", OuterDepthTex);
		}
	}

	public void UpdateLightingTex()
	{
		if (InnerMaterial != null)
		{
			InnerMaterial.SetTexture("_LightingTex", LightingTex);
		}
		if (OuterMaterial != null)
		{
			OuterMaterial.SetTexture("_LightingTex", LightingTex);
		}
	}

	public void UpdateScatteringTex()
	{
		if (InnerMaterial != null)
		{
			InnerMaterial.SetTexture("_ScatteringTex", ScatteringTex);
		}
		if (OuterMaterial != null)
		{
			OuterMaterial.SetTexture("_ScatteringTex", ScatteringTex);
		}
	}

	public void UpdateTerrainMaterials()
	{
		for (int num = SgtTerrain.AllTerrains.Count - 1; num >= 0; num--)
		{
			SgtTerrain sgtTerrain = SgtTerrain.AllTerrains[num];
			if (sgtTerrain.Atmosphere == this)
			{
				sgtTerrain.UpdateMaterials();
			}
		}
	}

	[ContextMenu("Update Materials")]
	public void UpdateMaterials()
	{
		updateMaterialsCalled = true;
		if (InnerMaterial == null)
		{
			InnerMaterial = SgtHelper.CreateTempMaterial("Atmosphere Inner (Generated)", "Hidden/SgtAtmosphereInner");
			if (InnerRenderers != null)
			{
				for (int num = InnerRenderers.Count - 1; num >= 0; num--)
				{
					MeshRenderer meshRenderer = InnerRenderers[num];
					if (meshRenderer != null)
					{
						SgtHelper.AddMaterial(meshRenderer, InnerMaterial);
					}
				}
			}
			UpdateTerrainMaterials();
		}
		if (OuterMaterial == null)
		{
			OuterMaterial = SgtHelper.CreateTempMaterial("Atmosphere Outer (Generated)", "Hidden/SgtAtmosphereOuter");
			if (Outers != null)
			{
				for (int num2 = Outers.Count - 1; num2 >= 0; num2--)
				{
					SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num2];
					if (sgtAtmosphereOuter != null)
					{
						sgtAtmosphereOuter.SetMaterial(OuterMaterial);
					}
				}
			}
		}
		Color value = SgtHelper.Brighten(Color, Brightness);
		int renderQueue = (int)(RenderQueue + RenderQueueOffset);
		InnerMaterial.renderQueue = renderQueue;
		OuterMaterial.renderQueue = renderQueue;
		InnerMaterial.SetColor("_Color", value);
		OuterMaterial.SetColor("_Color", value);
		InnerMaterial.SetTexture("_DepthTex", InnerDepthTex);
		OuterMaterial.SetTexture("_DepthTex", OuterDepthTex);
		if (Lit)
		{
			InnerMaterial.SetTexture("_LightingTex", LightingTex);
			OuterMaterial.SetTexture("_LightingTex", LightingTex);
			if (Scattering)
			{
				OuterMaterial.SetTexture("_ScatteringTex", ScatteringTex);
				OuterMaterial.SetFloat("_ScatteringMie", ScatteringMie);
				OuterMaterial.SetFloat("_ScatteringRayleigh", ScatteringRayleigh);
				SgtHelper.EnableKeyword("SGT_B", OuterMaterial);
				if (GroundScattering)
				{
					InnerMaterial.SetTexture("_ScatteringTex", ScatteringTex);
					InnerMaterial.SetFloat("_ScatteringMie", ScatteringMie);
					InnerMaterial.SetFloat("_ScatteringRayleigh", ScatteringRayleigh);
					SgtHelper.EnableKeyword("SGT_B", InnerMaterial);
				}
				else
				{
					SgtHelper.DisableKeyword("SGT_B", InnerMaterial);
				}
			}
			else
			{
				SgtHelper.DisableKeyword("SGT_B", InnerMaterial);
				SgtHelper.DisableKeyword("SGT_B", OuterMaterial);
			}
		}
		SgtHelper.SetTempMaterial(InnerMaterial, OuterMaterial);
		UpdateMaterialNonSerialized();
	}

	[ContextMenu("Apply Inner Material")]
	public void ApplyInnerMaterial()
	{
		if (InnerRenderers != null)
		{
			for (int num = InnerRenderers.Count - 1; num >= 0; num--)
			{
				SgtHelper.AddMaterial(InnerRenderers[num], InnerMaterial);
			}
		}
		UpdateTerrainMaterials();
	}

	[ContextMenu("Remove Inner Material")]
	public void RemoveInnerMaterial()
	{
		if (InnerRenderers != null)
		{
			for (int num = InnerRenderers.Count - 1; num >= 0; num--)
			{
				SgtHelper.RemoveMaterial(InnerRenderers[num], InnerMaterial);
			}
		}
		UpdateTerrainMaterials();
	}

	public static SgtAtmosphere CreateAtmosphere(int layer = 0, Transform parent = null)
	{
		return CreateAtmosphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtAtmosphere CreateAtmosphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Atmosphere", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtAtmosphere>();
	}

	public void AddInnerRenderer(MeshRenderer renderer)
	{
		if (renderer != null)
		{
			if (InnerRenderers == null)
			{
				InnerRenderers = new List<MeshRenderer>();
			}
			if (!InnerRenderers.Contains(renderer))
			{
				SgtHelper.AddMaterial(renderer, InnerMaterial);
				InnerRenderers.Add(renderer);
			}
		}
	}

	public void RemoveRenderer(MeshRenderer renderer)
	{
		if (renderer != null && InnerRenderers != null)
		{
			SgtHelper.RemoveMaterial(renderer, InnerMaterial);
			InnerRenderers.Remove(renderer);
		}
	}

	[ContextMenu("Update Outers")]
	public void UpdateOuters()
	{
		updateOutersCalled = true;
		int num = ((OuterMeshes != null) ? OuterMeshes.Count : 0);
		float scale = SgtHelper.Divide(OuterRadius, OuterMeshRadius);
		for (int i = 0; i < num; i++)
		{
			Mesh mesh = OuterMeshes[i];
			SgtAtmosphereOuter orAddOuter = GetOrAddOuter(i);
			orAddOuter.SetMesh(mesh);
			orAddOuter.SetMaterial(OuterMaterial);
			orAddOuter.SetScale(scale);
		}
		if (Outers == null)
		{
			return;
		}
		for (int num2 = Outers.Count - 1; num2 >= num; num2--)
		{
			SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num2];
			if (sgtAtmosphereOuter != null)
			{
				SgtAtmosphereOuter.Pool(sgtAtmosphereOuter);
			}
			Outers.RemoveAt(num2);
		}
	}

	protected virtual void OnEnable()
	{
		AllAtmospheres.Add(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (InnerRenderers != null)
		{
			for (int num = InnerRenderers.Count - 1; num >= 0; num--)
			{
				SgtHelper.ReplaceMaterial(InnerRenderers[num], InnerMaterial);
			}
		}
		if (Outers != null)
		{
			for (int num2 = Outers.Count - 1; num2 >= 0; num2--)
			{
				SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num2];
				if (sgtAtmosphereOuter != null)
				{
					sgtAtmosphereOuter.gameObject.SetActive(value: true);
				}
			}
		}
		if (startCalled)
		{
			CheckUpdateCalls();
		}
		ApplyInnerMaterial();
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			StartOnce();
		}
	}

	protected virtual void LateUpdate()
	{
		if (InnerMaterial != null && OuterMaterial != null)
		{
			SgtHelper.SetTempMaterial(InnerMaterial, OuterMaterial);
			SgtHelper.WriteLights(Lit, Lights, 2, transform.position, transform, null, SgtHelper.Brighten(Color, Brightness), ScatteringStrength);
			SgtHelper.WriteShadows(Shadows, 2);
		}
	}

	protected virtual void OnDisable()
	{
		AllAtmospheres.Remove(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		RemoveInnerMaterial();
		if (Outers == null)
		{
			return;
		}
		for (int num = Outers.Count - 1; num >= 0; num--)
		{
			SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num];
			if (sgtAtmosphereOuter != null)
			{
				sgtAtmosphereOuter.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Outers != null)
		{
			for (int num = Outers.Count - 1; num >= 0; num--)
			{
				SgtAtmosphereOuter.MarkForDestruction(Outers[num]);
			}
		}
		SgtHelper.Destroy(OuterMaterial);
		SgtHelper.Destroy(InnerMaterial);
	}

	private void CameraPreRender(Camera camera)
	{
		if (Outers != null)
		{
			Vector3 vector = camera.transform.position - transform.position;
			Vector3 localPosition = transform.InverseTransformPoint(transform.position + vector.normalized * CameraOffset);
			for (int num = Outers.Count - 1; num >= 0; num--)
			{
				SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num];
				if (sgtAtmosphereOuter != null)
				{
					sgtAtmosphereOuter.transform.localPosition = localPosition;
				}
			}
		}
		if (InnerMaterial != null && OuterMaterial != null)
		{
			Vector3 position = camera.transform.position;
			float magnitude = transform.InverseTransformPoint(position).magnitude;
			float num2 = Mathf.InverseLerp(OuterRadius, InnerMeshRadius, magnitude);
			float groundThickness = 0f;
			float skyThickness = 0f;
			float groundRadius = SgtHelper.Divide(InnerMeshRadius, OuterRadius);
			float num3 = SgtHelper.Divide(magnitude, OuterRadius);
			float num4 = 1f - InnerFog;
			float num5 = 1f - OuterFog;
			SgtHelper.CalculateAtmosphereThicknessAtHorizon(groundRadius, 1f, num3, out groundThickness, out skyThickness);
			SgtHelper.SetTempMaterial(InnerMaterial, OuterMaterial);
			if (num3 > 1f)
			{
				SgtHelper.EnableKeyword("SGT_A");
			}
			else
			{
				SgtHelper.DisableKeyword("SGT_A");
			}
			InnerMaterial.SetFloat("_HorizonLengthRecip", SgtHelper.Reciprocal(groundThickness * num4));
			OuterMaterial.SetFloat("_HorizonLengthRecip", SgtHelper.Reciprocal(skyThickness * num5));
			if (OuterDepthTex != null)
			{
				OuterMaterial.SetFloat("_Sky", Sky * OuterDepthTex.GetPixelBilinear(num2 / num5, 0f).a);
			}
			UpdateMaterialNonSerialized();
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (Outers == null)
		{
			return;
		}
		for (int num = Outers.Count - 1; num >= 0; num--)
		{
			SgtAtmosphereOuter sgtAtmosphereOuter = Outers[num];
			if (sgtAtmosphereOuter != null)
			{
				sgtAtmosphereOuter.transform.localPosition = Vector3.zero;
			}
		}
	}

	private void UpdateMaterialNonSerialized()
	{
		float xyz = SgtHelper.Divide(OuterMeshRadius, OuterRadius);
		Matrix4x4 value = SgtHelper.Scaling(xyz) * transform.worldToLocalMatrix;
		InnerMaterial.SetMatrix("_WorldToLocal", value);
		OuterMaterial.SetMatrix("_WorldToLocal", value);
	}

	private void StartOnce()
	{
		SgtTerrain component = GetComponent<SgtTerrain>();
		if (component != null)
		{
			component.Atmosphere = this;
			component.UpdateMaterials();
		}
		else if (InnerRenderers == null)
		{
			MeshRenderer componentInParent = GetComponentInParent<MeshRenderer>();
			if (componentInParent != null)
			{
				MeshFilter component2 = componentInParent.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					Mesh sharedMesh = component2.sharedMesh;
					if (sharedMesh != null)
					{
						Vector3 min = sharedMesh.bounds.min;
						Vector3 max = sharedMesh.bounds.max;
						float num = Mathf.Abs(min.x) + Mathf.Abs(min.y) + Mathf.Abs(min.z) + Mathf.Abs(max.x) + Mathf.Abs(max.y) + Mathf.Abs(max.z);
						InnerMeshRadius = num / 6f;
						InnerRenderers = new List<MeshRenderer>();
						InnerRenderers.Add(componentInParent);
					}
				}
			}
		}
		CheckUpdateCalls();
	}

	private void CheckUpdateCalls()
	{
		if (!updateMaterialsCalled)
		{
			UpdateMaterials();
		}
		if (!updateOutersCalled)
		{
			UpdateOuters();
		}
	}

	private SgtAtmosphereOuter GetOrAddOuter(int index)
	{
		SgtAtmosphereOuter sgtAtmosphereOuter = null;
		if (Outers == null)
		{
			Outers = new List<SgtAtmosphereOuter>();
		}
		if (index < Outers.Count)
		{
			sgtAtmosphereOuter = Outers[index];
			if (sgtAtmosphereOuter == null)
			{
				sgtAtmosphereOuter = SgtAtmosphereOuter.Create(this);
				Outers[index] = sgtAtmosphereOuter;
			}
		}
		else
		{
			sgtAtmosphereOuter = SgtAtmosphereOuter.Create(this);
			Outers.Add(sgtAtmosphereOuter);
		}
		return sgtAtmosphereOuter;
	}
}
