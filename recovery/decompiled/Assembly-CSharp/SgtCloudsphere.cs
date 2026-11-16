using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Cloudsphere")]
public class SgtCloudsphere : MonoBehaviour
{
	public static List<SgtCloudsphere> AllCloudspheres = new List<SgtCloudsphere>();

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The Color.rgb values are multiplied by this")]
	public float Brightness = 1f;

	[Tooltip("The radius of the cloudsphere meshes specified below")]
	public float MeshRadius = 1f;

	[Tooltip("The meshes used to build the cloudsphere (should be a sphere)")]
	public List<Mesh> Meshes;

	[Tooltip("The render queue group for this cloudsphere")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this cloudsphere")]
	public int RenderQueueOffset;

	[Tooltip("The desired radius of the cloudsphere in local coordinates")]
	public float Radius = 1.5f;

	[Tooltip("Should the clouds fade out when the camera gets near?")]
	public bool Fade;

	[Tooltip("The lookup table used to calculate the fade")]
	public Texture FadeTex;

	[Tooltip("The distance the fading begins from in world space")]
	public float FadeDistance = 1f;

	[Tooltip("The amount the clouds gets moved toward the current camera")]
	[FormerlySerializedAs("ObserverOffset")]
	public float CameraOffset;

	[Tooltip("The cubemap used to render the clouds")]
	public Cubemap MainTex;

	[Tooltip("The lookup table used for depth color and opacity (bottom = no depth/space, top = maximum depth/center)")]
	public Texture2D DepthTex;

	[Tooltip("Does this cloudsphere receive light?")]
	public bool Lit;

	[Tooltip("The lookup table used to calculate the lighting color and brightness")]
	public Texture LightingTex;

	[Tooltip("The lights shining on this cloudsphere")]
	public List<Light> Lights;

	[Tooltip("The shadows casting on this cloudsphere")]
	public List<SgtShadow> Shadows;

	[NonSerialized]
	public Material Material;

	[SerializeField]
	public List<SgtCloudsphereModel> Models;

	[SerializeField]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	private bool updateModelsCalled;

	public void UpdateDeptchTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_DepthTex", DepthTex);
		}
	}

	public void UpdateFadeTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_FadeTex", FadeTex);
		}
	}

	public void UpdateLightingTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Cloudsphere (Generated)", "Hidden/SgtCloudsphere");
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtCloudsphereModel sgtCloudsphereModel = Models[num];
					if (sgtCloudsphereModel != null)
					{
						sgtCloudsphereModel.SetMaterial(Material);
					}
				}
			}
		}
		int renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Color value = SgtHelper.Brighten(Color, Brightness);
		Material.renderQueue = renderQueue;
		Material.SetColor("_Color", value);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetTexture("_DepthTex", DepthTex);
		if (Fade)
		{
			SgtHelper.EnableKeyword("SGT_A", Material);
			Material.SetTexture("_FadeTex", FadeTex);
			Material.SetFloat("_FadeDistanceRecip", SgtHelper.Reciprocal(FadeDistance));
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A", Material);
		}
		if (Lit)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	[ContextMenu("Update Models")]
	public void UpdateModels()
	{
		updateModelsCalled = true;
		int num = ((Meshes != null) ? Meshes.Count : 0);
		float scale = SgtHelper.Divide(Radius, MeshRadius);
		for (int i = 0; i < num; i++)
		{
			Mesh mesh = Meshes[i];
			SgtCloudsphereModel orAddModel = GetOrAddModel(i);
			orAddModel.SetMesh(mesh);
			orAddModel.SetMaterial(Material);
			orAddModel.SetScale(scale);
		}
		if (Models != null)
		{
			for (int num2 = Models.Count - 1; num2 >= num; num2--)
			{
				SgtCloudsphereModel.Pool(Models[num2]);
				Models.RemoveAt(num2);
			}
		}
	}

	public static SgtCloudsphere CreateCloudsphere(int layer = 0, Transform parent = null)
	{
		return CreateCloudsphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCloudsphere CreateCloudsphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Cloudsphere", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtCloudsphere>();
	}

	protected virtual void OnEnable()
	{
		AllCloudspheres.Add(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtCloudsphereModel sgtCloudsphereModel = Models[num];
				if (sgtCloudsphereModel != null)
				{
					sgtCloudsphereModel.gameObject.SetActive(value: true);
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
		if (Material != null)
		{
			SgtHelper.SetTempMaterial(Material);
			SgtHelper.WriteLights(Lit, Lights, 2, transform.position, null, null, SgtHelper.Brighten(Color, Brightness), 1f);
			SgtHelper.WriteShadows(Shadows, 2);
		}
	}

	protected virtual void OnDisable()
	{
		AllCloudspheres.Remove(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtCloudsphereModel sgtCloudsphereModel = Models[num];
			if (sgtCloudsphereModel != null)
			{
				sgtCloudsphereModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtCloudsphereModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private void CameraPreCull(Camera camera)
	{
		if (Material != null)
		{
			UpdateMaterialNonSerialized();
		}
		if (CameraOffset == 0f || Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtCloudsphereModel sgtCloudsphereModel = Models[num];
			if (sgtCloudsphereModel != null)
			{
				sgtCloudsphereModel.Revert();
				Transform transform = sgtCloudsphereModel.transform;
				Vector3 normalized = (transform.position - camera.transform.position).normalized;
				transform.position += normalized * CameraOffset;
				sgtCloudsphereModel.Save(camera);
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
			SgtCloudsphereModel sgtCloudsphereModel = Models[num];
			if (sgtCloudsphereModel != null)
			{
				sgtCloudsphereModel.Restore(camera);
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
			SgtCloudsphereModel sgtCloudsphereModel = Models[num];
			if (sgtCloudsphereModel != null)
			{
				sgtCloudsphereModel.Revert();
			}
		}
	}

	private void UpdateMaterialNonSerialized()
	{
		SgtHelper.SetTempMaterial(Material);
		SgtHelper.WriteShadowsNonSerialized(Shadows, 2);
	}

	private SgtCloudsphereModel GetOrAddModel(int index)
	{
		SgtCloudsphereModel sgtCloudsphereModel = null;
		if (Models == null)
		{
			Models = new List<SgtCloudsphereModel>();
		}
		if (index < Models.Count)
		{
			sgtCloudsphereModel = Models[index];
			if (sgtCloudsphereModel == null)
			{
				sgtCloudsphereModel = SgtCloudsphereModel.Create(this);
				Models[index] = sgtCloudsphereModel;
			}
		}
		else
		{
			sgtCloudsphereModel = SgtCloudsphereModel.Create(this);
			Models.Add(sgtCloudsphereModel);
		}
		return sgtCloudsphereModel;
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
