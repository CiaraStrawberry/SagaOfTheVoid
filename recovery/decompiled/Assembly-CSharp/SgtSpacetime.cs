using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Spacetime")]
public class SgtSpacetime : MonoBehaviour
{
	public enum DisplacementType
	{
		Pinch,
		Offset
	}

	public static List<SgtSpacetime> AllSpacetimes = new List<SgtSpacetime>();

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The color brightness")]
	public float Brightness = 1f;

	[Tooltip("The render queue group")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset")]
	public int RenderQueueOffset;

	[Tooltip("The main texture applied to the spacetime")]
	public Texture2D MainTex;

	[Tooltip("How many times should the spacetime texture be tiled?")]
	public int Tile = 1;

	[Tooltip("The ambient color")]
	public Color AmbientColor = Color.white;

	[Tooltip("The ambient brightness")]
	public float AmbientBrightness = 0.25f;

	[Tooltip("The displacement color")]
	public Color DisplacementColor = Color.white;

	[Tooltip("The displacement brightness")]
	public float DisplacementBrightness = 1f;

	[Tooltip("The color of the highlight")]
	public Color HighlightColor = Color.white;

	[Tooltip("The brightness of the highlight")]
	public float HighlightBrightness = 0.1f;

	[Tooltip("The sharpness of the highlight")]
	public float HighlightPower = 1f;

	[Tooltip("The scale of the highlight")]
	public float HighlightScale = 3f;

	[Tooltip("How should the vertices in the spacetime get displaced when a well is nearby?")]
	public DisplacementType Displacement;

	[Tooltip("Should the displacement effect additively stack if wells overlap?")]
	public bool Accumulate;

	[Tooltip("The pinch power")]
	public float Power = 3f;

	[Tooltip("How strong the fading is")]
	public float FadeScale = 1f;

	[Tooltip("The offset direction/vector for vertices within range of a well")]
	public Vector3 Offset = new Vector3(0f, -1f, 0f);

	[Tooltip("Automatically use all active and enabled wells in the scene?")]
	public bool UseAllWells = true;

	[Tooltip("Filter all the wells to require the same layer at this GameObject")]
	public bool RequireSameLayer;

	[Tooltip("Filter all the wells to require the same tag at this GameObject")]
	public bool RequireSameTag;

	[Tooltip("Filter all the wells to require a name that contains this")]
	public string RequireNameContains;

	[Tooltip("The wells currently being checked by the spacetime")]
	public List<SgtSpacetimeWell> Wells;

	[Tooltip("The renderers this spacetime is being applied to")]
	public List<MeshRenderer> Renderers;

	[NonSerialized]
	public Material Material;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	private bool updateWellsCalled;

	[NonSerialized]
	private Vector4[] gauPos = new Vector4[12];

	[NonSerialized]
	private Vector4[] gauDat = new Vector4[12];

	[NonSerialized]
	private Vector4[] ripPos = new Vector4[1];

	[NonSerialized]
	private Vector4[] ripDat = new Vector4[1];

	[NonSerialized]
	private Vector4[] twiPos = new Vector4[1];

	[NonSerialized]
	private Vector4[] twiDat = new Vector4[1];

	[NonSerialized]
	private Matrix4x4[] twiMat = new Matrix4x4[1];

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Spacetime (Generated)", "Hidden/SgtSpacetime");
			ApplyMaterial();
		}
		Color value = SgtHelper.Brighten(AmbientColor, AmbientBrightness);
		Color value2 = SgtHelper.Brighten(DisplacementColor, DisplacementBrightness);
		Color value3 = SgtHelper.Brighten(HighlightColor, HighlightBrightness);
		Material.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetColor("_Color", SgtHelper.Brighten(Color, Brightness));
		Material.SetColor("_AmbientColor", value);
		Material.SetColor("_DisplacementColor", value2);
		Material.SetColor("_HighlightColor", value3);
		Material.SetFloat("_HighlightPower", HighlightPower);
		Material.SetFloat("_HighlightScale", HighlightScale);
		Material.SetFloat("_Tile", Tile);
		if (Displacement == DisplacementType.Pinch)
		{
			Material.SetFloat("_Power", Power);
		}
		if (Displacement == DisplacementType.Offset)
		{
			SgtHelper.EnableKeyword("SGT_A", Material);
			Material.SetVector("_Offset", Offset);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A", Material);
		}
		if (Accumulate)
		{
			SgtHelper.EnableKeyword("SGT_B", Material);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B", Material);
		}
	}

	[ContextMenu("Update Wells")]
	public void UpdateWells()
	{
		if (Material != null)
		{
			int gaussianCount = 0;
			int rippleCount = 0;
			int twistCount = 0;
			WriteWells(ref gaussianCount, ref rippleCount, ref twistCount);
			if ((gaussianCount & 1) != 0)
			{
				SgtHelper.EnableKeyword("SGT_C", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("SGT_C", Material);
			}
			if ((gaussianCount & 2) != 0)
			{
				SgtHelper.EnableKeyword("SGT_D", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("SGT_D", Material);
			}
			if ((gaussianCount & 4) != 0)
			{
				SgtHelper.EnableKeyword("SGT_E", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("SGT_E", Material);
			}
			if ((gaussianCount & 8) != 0)
			{
				SgtHelper.EnableKeyword("LIGHT_0", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("LIGHT_0", Material);
			}
			if ((rippleCount & 1) != 0)
			{
				SgtHelper.EnableKeyword("LIGHT_1", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("LIGHT_1", Material);
			}
			if ((twistCount & 1) != 0)
			{
				SgtHelper.EnableKeyword("SHADOW_1", Material);
			}
			else
			{
				SgtHelper.DisableKeyword("SHADOW_1", Material);
			}
		}
	}

	[ContextMenu("Apply Material")]
	public void ApplyMaterial()
	{
		for (int num = Renderers.Count - 1; num >= 0; num--)
		{
			MeshRenderer meshRenderer = Renderers[num];
			if (meshRenderer != null && meshRenderer.sharedMaterial != Material)
			{
				meshRenderer.sharedMaterial = Material;
			}
		}
	}

	[ContextMenu("Remove Material")]
	public void RemoveMaterial()
	{
		if (Renderers == null)
		{
			return;
		}
		for (int num = Renderers.Count - 1; num >= 0; num--)
		{
			MeshRenderer meshRenderer = Renderers[num];
			if (meshRenderer != null && meshRenderer.sharedMaterial == Material)
			{
				meshRenderer.sharedMaterial = null;
			}
		}
	}

	[ContextMenu("Add Well")]
	public SgtSpacetimeWell AddWell()
	{
		return SgtSpacetimeWell.Create(this);
	}

	public void AddWell(SgtSpacetimeWell well)
	{
		if (Wells == null)
		{
			Wells = new List<SgtSpacetimeWell>();
		}
		if (!Wells.Contains(well))
		{
			Wells.Add(well);
		}
	}

	public void AddRenderer(MeshRenderer renderer)
	{
		if (!(renderer != null))
		{
			return;
		}
		if (Renderers == null)
		{
			Renderers = new List<MeshRenderer>();
		}
		if (!Renderers.Contains(renderer))
		{
			if (renderer.sharedMaterial != Material)
			{
				renderer.sharedMaterial = Material;
			}
			Renderers.Add(renderer);
		}
	}

	public void RemoveRenderer(MeshRenderer renderer)
	{
		if (renderer != null && Renderers != null)
		{
			if (renderer.sharedMaterial == Material)
			{
				renderer.sharedMaterial = null;
			}
			Renderers.Remove(renderer);
		}
	}

	protected virtual void OnEnable()
	{
		AllSpacetimes.Add(this);
		if (startCalled)
		{
			CheckUpdateCalls();
		}
		ApplyMaterial();
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			if (Renderers == null)
			{
				AddRenderer(GetComponent<MeshRenderer>());
			}
			CheckUpdateCalls();
		}
	}

	protected virtual void Update()
	{
		UpdateWells();
	}

	protected virtual void OnDisable()
	{
		AllSpacetimes.Remove(this);
		RemoveMaterial();
	}

	private void WriteWells(ref int gaussianCount, ref int rippleCount, ref int twistCount)
	{
		List<SgtSpacetimeWell> list = ((!UseAllWells) ? Wells : SgtSpacetimeWell.AllWells);
		if (list == null)
		{
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			SgtSpacetimeWell sgtSpacetimeWell = list[num];
			if (SgtHelper.Enabled(sgtSpacetimeWell) && sgtSpacetimeWell.Radius > 0f && (sgtSpacetimeWell.Distribution != SgtSpacetimeWell.DistributionType.Gaussian || gaussianCount < gauPos.Length) && (sgtSpacetimeWell.Distribution != SgtSpacetimeWell.DistributionType.Ripple || rippleCount < ripPos.Length) && (sgtSpacetimeWell.Distribution != SgtSpacetimeWell.DistributionType.Twist || twistCount < twiPos.Length) && (!UseAllWells || ((!RequireSameLayer || gameObject.layer == sgtSpacetimeWell.gameObject.layer) && (!RequireSameTag || !(tag != sgtSpacetimeWell.tag)) && (string.IsNullOrEmpty(RequireNameContains) || sgtSpacetimeWell.name.Contains(RequireNameContains)))))
			{
				Vector3 position = sgtSpacetimeWell.transform.position;
				switch (sgtSpacetimeWell.Distribution)
				{
				case SgtSpacetimeWell.DistributionType.Gaussian:
				{
					int num4 = gaussianCount++;
					ref Vector4 reference6 = ref gauPos[num4];
					reference6 = new Vector4(position.x, position.y, position.z, sgtSpacetimeWell.Radius);
					ref Vector4 reference7 = ref gauDat[num4];
					reference7 = new Vector4(sgtSpacetimeWell.Strength, 0f, 0f, 0f);
					break;
				}
				case SgtSpacetimeWell.DistributionType.Ripple:
				{
					int num3 = rippleCount++;
					ref Vector4 reference4 = ref ripPos[num3];
					reference4 = new Vector4(position.x, position.y, position.z, sgtSpacetimeWell.Radius);
					ref Vector4 reference5 = ref ripDat[num3];
					reference5 = new Vector4(sgtSpacetimeWell.Strength, sgtSpacetimeWell.Frequency, sgtSpacetimeWell.Offset, 0f);
					break;
				}
				case SgtSpacetimeWell.DistributionType.Twist:
				{
					int num2 = twistCount++;
					ref Vector4 reference = ref twiPos[num2];
					reference = new Vector4(position.x, position.y, position.z, sgtSpacetimeWell.Radius);
					ref Vector4 reference2 = ref twiDat[num2];
					reference2 = new Vector4(sgtSpacetimeWell.Strength, sgtSpacetimeWell.Frequency, sgtSpacetimeWell.HoleSize, sgtSpacetimeWell.HolePower);
					ref Matrix4x4 reference3 = ref twiMat[num2];
					reference3 = sgtSpacetimeWell.transform.worldToLocalMatrix;
					break;
				}
				}
			}
		}
		Material.SetVectorArray("_GauPos", gauPos);
		Material.SetVectorArray("_GauDat", gauDat);
		Material.SetVectorArray("_RipPos", ripPos);
		Material.SetVectorArray("_RipDat", ripDat);
		Material.SetVectorArray("_TwiPos", twiPos);
		Material.SetVectorArray("_TwiDat", twiDat);
		Material.SetMatrixArray("_TwiMat", twiMat);
	}

	private void CheckUpdateCalls()
	{
		if (!updateMaterialCalled)
		{
			UpdateMaterial();
		}
		if (!updateWellsCalled)
		{
			UpdateWells();
		}
	}
}
