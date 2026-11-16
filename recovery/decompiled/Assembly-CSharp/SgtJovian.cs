using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Jovian")]
public class SgtJovian : MonoBehaviour
{
	public static List<SgtJovian> AllJovians = new List<SgtJovian>();

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The Color.rgb values are multiplied by this")]
	public float Brightness = 1f;

	[Tooltip("The render queue group for this jovian")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this jovian")]
	public int RenderQueueOffset;

	[Tooltip("The main cube map texture used to render the jovian")]
	public Cubemap MainTex;

	[Tooltip("The lookup table used for depth color and opacity (bottom = no depth/space, top = maximum depth/center)")]
	public Texture2D DepthTex;

	[Tooltip("The opacity multiplier of the sky when the camera is inside the sky mesh")]
	public float Sky = 1f;

	[Tooltip("Does this jovian receive light?")]
	public bool Lit;

	[Tooltip("The lookup table used to calculate the lighting color and brightness")]
	public Texture LightingTex;

	[Tooltip("The lights shining on this jovian")]
	public List<Light> Lights;

	[Tooltip("The shadows casting on this jovian")]
	public List<SgtShadow> Shadows;

	[Tooltip("Should lights scatter through the atmosphere?")]
	public bool Scattering;

	[Tooltip("The color applied to the jovian based on the lighting (bottom = dark side, top = light side)")]
	public Texture ScatteringTex;

	[Tooltip("The scattering brightness multiplier")]
	public float ScatteringStrength = 3f;

	[Tooltip("The radius of the jovian meshes specified below")]
	public float MeshRadius = 1f;

	[Tooltip("The meshes used to build the jovian (should be a sphere)")]
	public List<Mesh> Meshes;

	[FormerlySerializedAs("models")]
	public List<SgtJovianModel> Models;

	[NonSerialized]
	public Material Material;

	public virtual void UpdateDepthTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_DepthTex", DepthTex);
		}
	}

	public virtual void UpdateLightingTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
	}

	public virtual void UpdateScatteringTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_ScatteringTex", ScatteringTex);
		}
	}

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Jovian Material (Generated)", "Hidden/SgtJovian");
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtJovianModel sgtJovianModel = Models[num];
					if (sgtJovianModel != null)
					{
						sgtJovianModel.SetMaterial(Material);
					}
				}
			}
		}
		Material.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetTexture("_DepthTex", DepthTex);
		Material.SetColor("_Color", SgtHelper.Brighten(Color, Brightness));
		if (Lit)
		{
			Material.SetTexture("_LightingTex", LightingTex);
		}
		SgtHelper.SetTempMaterial(Material);
		if (Scattering)
		{
			Material.SetTexture("_ScatteringTex", ScatteringTex);
			SgtHelper.EnableKeyword("SGT_B");
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B");
		}
		UpdateMaterialNonSerialized();
	}

	[ContextMenu("Update Models")]
	public void UpdateModels()
	{
		int num = ((Meshes != null) ? Meshes.Count : 0);
		for (int i = 0; i < num; i++)
		{
			Mesh mesh = Meshes[i];
			SgtJovianModel orAddModel = GetOrAddModel(i);
			orAddModel.SetMesh(mesh);
			orAddModel.SetMaterial(Material);
		}
		if (Models != null)
		{
			for (int num2 = Models.Count - 1; num2 >= num; num2--)
			{
				SgtJovianModel.Pool(Models[num2]);
				Models.RemoveAt(num2);
			}
		}
	}

	public static SgtJovian CreateJovian(int layer = 0, Transform parent = null)
	{
		return CreateJovian(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtJovian CreateJovian(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Jovian", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtJovian>();
	}

	protected virtual void OnEnable()
	{
		AllJovians.Add(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtJovianModel sgtJovianModel = Models[num];
			if (sgtJovianModel != null)
			{
				sgtJovianModel.gameObject.SetActive(value: true);
			}
		}
	}

	protected virtual void Start()
	{
		if (Material == null)
		{
			UpdateMaterial();
		}
	}

	protected virtual void LateUpdate()
	{
		if (Material != null)
		{
			SgtHelper.SetTempMaterial(Material);
			SgtHelper.WriteLights(Lit, Lights, 2, transform.position, transform, null, SgtHelper.Brighten(Color, Brightness), ScatteringStrength);
			SgtHelper.WriteShadows(Shadows, 2);
		}
	}

	protected virtual void OnDisable()
	{
		AllJovians.Remove(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtJovianModel sgtJovianModel = Models[num];
			if (sgtJovianModel != null)
			{
				sgtJovianModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtJovianModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private void UpdateMaterialNonSerialized()
	{
		Matrix4x4 value = transform.localToWorldMatrix * SgtHelper.Scaling(MeshRadius * 2f);
		Material.SetMatrix("_WorldToLocal", value.inverse);
		Material.SetMatrix("_LocalToWorld", value);
		SgtHelper.SetTempMaterial(Material);
		SgtHelper.WriteShadowsNonSerialized(Shadows, 2);
	}

	private void CameraPreRender(Camera camera)
	{
		if (!(Material != null))
		{
			return;
		}
		Vector3 position = camera.transform.position;
		float magnitude = transform.InverseTransformPoint(position).magnitude;
		float num = SgtHelper.Divide(magnitude, MeshRadius);
		if (num > 1f)
		{
			SgtHelper.EnableKeyword("SGT_A", Material);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A", Material);
			if (DepthTex != null)
			{
				Material.SetFloat("_Sky", Sky * DepthTex.GetPixelBilinear(1f - num, 0f).a);
			}
		}
		UpdateMaterialNonSerialized();
	}

	private SgtJovianModel GetOrAddModel(int index)
	{
		SgtJovianModel sgtJovianModel = null;
		if (Models == null)
		{
			Models = new List<SgtJovianModel>();
		}
		if (index < Models.Count)
		{
			sgtJovianModel = Models[index];
			if (sgtJovianModel == null)
			{
				sgtJovianModel = SgtJovianModel.Create(this);
				Models[index] = sgtJovianModel;
			}
		}
		else
		{
			sgtJovianModel = SgtJovianModel.Create(this);
			Models.Add(sgtJovianModel);
		}
		return sgtJovianModel;
	}
}
