using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Singularity")]
public class SgtSingularity : MonoBehaviour
{
	public enum EdgeFadeType
	{
		None,
		Center,
		Fragment
	}

	public static List<SgtSingularity> AllSingularities = new List<SgtSingularity>();

	[Tooltip("The meshes used to build the singularity mesh (should be a sphere)")]
	public List<Mesh> Meshes;

	[Tooltip("The render queue group for this singularity")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this singularity")]
	public int RenderQueueOffset;

	[Tooltip("How much the singulaity distorts the screen")]
	public float PinchPower = 10f;

	[Tooltip("How large the pinch start point is")]
	[Range(0f, 0.5f)]
	public float PinchOffset = 0.02f;

	[Tooltip("To prevent rendering issues the singularity can be faded out as it approaches the edges of the screen. This allows you to set how the fading is calculated")]
	public EdgeFadeType EdgeFade = EdgeFadeType.Fragment;

	[Tooltip("How sharp the fading effect is")]
	public float EdgeFadePower = 2f;

	[Tooltip("The color of the pinched hole")]
	public Color HoleColor = Color.black;

	[Tooltip("How sharp the hole color gradient is")]
	public float HolePower = 2f;

	[Tooltip("Enable this if you want the singulairty to tint nearby space")]
	public bool Tint;

	[Tooltip("The color of the tint")]
	public Color TintColor = Color.red;

	[Tooltip("How sharp the tint color gradient is")]
	public float TintPower = 2f;

	[Tooltip("The models used to render the full jovian")]
	[FormerlySerializedAs("models")]
	public List<SgtSingularityModel> Models;

	[NonSerialized]
	public Material Material;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	private bool updateModelsCalled;

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Singulairty (Generated)", "Hidden/SgtSingularity");
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtSingularityModel sgtSingularityModel = Models[num];
					if (sgtSingularityModel != null)
					{
						sgtSingularityModel.SetMaterial(Material);
					}
				}
			}
		}
		Material.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.SetVector("_Center", SgtHelper.NewVector4(transform.position, 1f));
		Material.SetFloat("_PinchPower", PinchPower);
		Material.SetFloat("_PinchScale", SgtHelper.Reciprocal(1f - PinchOffset));
		Material.SetFloat("_PinchOffset", PinchOffset);
		Material.SetFloat("_HolePower", HolePower);
		Material.SetColor("_HoleColor", HoleColor);
		SgtHelper.SetTempMaterial(Material);
		if (Tint)
		{
			SgtHelper.EnableKeyword("SGT_A");
			Material.SetFloat("_TintPower", TintPower);
			Material.SetColor("_TintColor", TintColor);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A");
		}
		if (EdgeFade == EdgeFadeType.Center)
		{
			SgtHelper.EnableKeyword("SGT_B");
			Material.SetFloat("_EdgeFadePower", EdgeFadePower);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B");
		}
		if (EdgeFade == EdgeFadeType.Fragment)
		{
			SgtHelper.EnableKeyword("SGT_C");
			Material.SetFloat("_EdgeFadePower", EdgeFadePower);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_C");
		}
	}

	[ContextMenu("Update Models")]
	public void UpdateModels()
	{
		updateModelsCalled = true;
		int num = ((Meshes != null) ? Meshes.Count : 0);
		for (int i = 0; i < num; i++)
		{
			Mesh mesh = Meshes[i];
			SgtSingularityModel orAddModel = GetOrAddModel(i);
			orAddModel.SetMesh(mesh);
			orAddModel.SetMaterial(Material);
		}
		if (Models != null)
		{
			for (int num2 = Models.Count - 1; num2 >= num; num2--)
			{
				SgtSingularityModel.Pool(Models[num2]);
				Models.RemoveAt(num2);
			}
		}
	}

	public static SgtSingularity CreateSingularity(int layer = 0, Transform parent = null)
	{
		return CreateSingularity(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSingularity CreateSingularity(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Singularity", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtSingularity>();
	}

	protected virtual void OnEnable()
	{
		AllSingularities.Add(this);
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtSingularityModel sgtSingularityModel = Models[num];
				if (sgtSingularityModel != null)
				{
					sgtSingularityModel.gameObject.SetActive(value: true);
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

	protected virtual void OnDisable()
	{
		AllSingularities.Remove(this);
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtSingularityModel sgtSingularityModel = Models[num];
			if (sgtSingularityModel != null)
			{
				sgtSingularityModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtSingularityModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private SgtSingularityModel GetOrAddModel(int index)
	{
		SgtSingularityModel sgtSingularityModel = null;
		if (Models == null)
		{
			Models = new List<SgtSingularityModel>();
		}
		if (index < Models.Count)
		{
			sgtSingularityModel = Models[index];
			if (sgtSingularityModel == null)
			{
				sgtSingularityModel = SgtSingularityModel.Create(this);
				Models[index] = sgtSingularityModel;
			}
		}
		else
		{
			sgtSingularityModel = SgtSingularityModel.Create(this);
			Models.Add(sgtSingularityModel);
		}
		return sgtSingularityModel;
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
