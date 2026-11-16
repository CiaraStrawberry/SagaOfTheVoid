using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SgtQuads : MonoBehaviour
{
	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The amount the Color.rgb values are multiplied by")]
	public float Brightness = 1f;

	[Tooltip("The main texture of this material")]
	public Texture MainTex;

	[Tooltip("The layout of cells in the texture")]
	public SgtQuadsLayoutType Layout;

	[Tooltip("The amount of columns in the texture")]
	public int LayoutColumns = 1;

	[Tooltip("The amount of rows in the texture")]
	public int LayoutRows = 1;

	[Tooltip("The rects of each cell in the texture")]
	public List<Rect> Rects;

	[Tooltip("The render queue group for this material")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this material")]
	public int RenderQueueOffset;

	[HideInInspector]
	public List<SgtQuadsModel> Models;

	[NonSerialized]
	public Material Material;

	[SerializeField]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	private bool updateMeshesAndModelsCalled;

	protected static List<Vector4> tempCoords = new List<Vector4>();

	protected abstract string ShaderName { get; }

	public void UpdateMainTex()
	{
		if (Material != null)
		{
			Material.SetTexture("_MainTex", MainTex);
		}
	}

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Starfield (Generated)", ShaderName);
			if (Models != null)
			{
				for (int num = Models.Count - 1; num >= 0; num--)
				{
					SgtQuadsModel sgtQuadsModel = Models[num];
					if (sgtQuadsModel != null)
					{
						sgtQuadsModel.SetMaterial(Material);
					}
				}
			}
		}
		BuildMaterial();
	}

	[ContextMenu("Update Meshes and Models")]
	public void UpdateMeshesAndModels()
	{
		updateMeshesAndModelsCalled = true;
		int num = BeginQuads();
		int num2 = 0;
		if (num > 0)
		{
			BuildRects();
			ConvertRectsToCoords();
			while (num > 0)
			{
				int num3 = Mathf.Min(num, SgtHelper.QuadsPerMesh);
				SgtQuadsModel orNewModel = GetOrNewModel(num2);
				Mesh orNewMesh = GetOrNewMesh(orNewModel);
				orNewModel.SetMaterial(Material);
				BuildMesh(orNewMesh, num2 * SgtHelper.QuadsPerMesh, num3);
				num2++;
				num -= num3;
			}
		}
		if (Models != null)
		{
			for (int num4 = Models.Count - 1; num4 >= num2; num4--)
			{
				SgtQuadsModel.Pool(Models[num4]);
				Models.RemoveAt(num4);
			}
		}
		EndQuads();
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			StartOnce();
		}
	}

	protected virtual void OnEnable()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtQuadsModel sgtQuadsModel = Models[num];
				if (sgtQuadsModel != null)
				{
					sgtQuadsModel.gameObject.SetActive(value: true);
				}
			}
		}
		if (startCalled)
		{
			CheckUpdateCalls();
		}
	}

	protected virtual void OnDisable()
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
				sgtQuadsModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtQuadsModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	protected abstract int BeginQuads();

	protected abstract void EndQuads();

	protected virtual void BuildMaterial()
	{
		Material.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetColor("_Color", SgtHelper.Brighten(Color, Color.a * Brightness));
		Material.SetFloat("_Scale", transform.lossyScale.x);
		Material.SetFloat("_ScaleRecip", SgtHelper.Reciprocal(transform.lossyScale.x));
	}

	protected virtual void StartOnce()
	{
		CheckUpdateCalls();
	}

	protected void BuildRects()
	{
		if (Layout != SgtQuadsLayoutType.Grid)
		{
			return;
		}
		if (Rects == null)
		{
			Rects = new List<Rect>();
		}
		Rects.Clear();
		if (LayoutColumns <= 0 || LayoutRows <= 0)
		{
			return;
		}
		float num = SgtHelper.Reciprocal(LayoutColumns);
		float num2 = SgtHelper.Reciprocal(LayoutRows);
		for (int i = 0; i < LayoutRows; i++)
		{
			float y = (float)i * num2;
			for (int j = 0; j < LayoutColumns; j++)
			{
				float x = (float)j * num;
				Rect item = new Rect(x, y, num, num2);
				Rects.Add(item);
			}
		}
	}

	protected abstract void BuildMesh(Mesh mesh, int starIndex, int starCount);

	protected static void ExpandBounds(ref bool minMaxSet, ref Vector3 min, ref Vector3 max, Vector3 position, float radius)
	{
		Vector3 vector = new Vector3(radius, radius, radius);
		if (!minMaxSet)
		{
			minMaxSet = true;
			min = position - vector;
			max = position + vector;
		}
		min = Vector3.Min(min, position - vector);
		max = Vector3.Max(max, position + vector);
	}

	private void ConvertRectsToCoords()
	{
		tempCoords.Clear();
		if (Rects != null)
		{
			for (int i = 0; i < Rects.Count; i++)
			{
				Rect rect = Rects[i];
				tempCoords.Add(new Vector4(rect.xMin, rect.yMin, rect.xMax, rect.yMax));
			}
		}
		if (tempCoords.Count == 0)
		{
			tempCoords.Add(default);
		}
	}

	private SgtQuadsModel GetOrNewModel(int index)
	{
		SgtQuadsModel sgtQuadsModel = null;
		if (Models == null)
		{
			Models = new List<SgtQuadsModel>();
		}
		if (index < Models.Count)
		{
			sgtQuadsModel = Models[index];
		}
		else
		{
			Models.Add(sgtQuadsModel);
		}
		if (sgtQuadsModel == null || sgtQuadsModel.Quads != this)
		{
			SgtQuadsModel sgtQuadsModel2 = SgtQuadsModel.Create(this);
			Models[index] = sgtQuadsModel2;
			sgtQuadsModel = sgtQuadsModel2;
			sgtQuadsModel.SetMaterial(Material);
		}
		return sgtQuadsModel;
	}

	private Mesh GetOrNewMesh(SgtQuadsModel model)
	{
		Mesh mesh = model.Mesh;
		if (mesh == null)
		{
			mesh = SgtHelper.CreateTempMesh("Quads Mesh (Generated)");
			model.SetMesh(mesh);
		}
		else
		{
			mesh.Clear(keepVertexLayout: false);
		}
		return mesh;
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
