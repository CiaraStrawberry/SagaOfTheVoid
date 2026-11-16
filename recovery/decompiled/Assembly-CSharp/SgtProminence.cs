using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Prominence")]
public class SgtProminence : MonoBehaviour
{
	public static List<SgtProminence> AllProminences = new List<SgtProminence>();

	[Tooltip("The main texture of the prominence")]
	public Texture MainTex;

	[Tooltip("The color tint of the prominence")]
	public Color Color = Color.white;

	[Tooltip("The color brightness of the prominence")]
	public float Brightness = 1f;

	[Tooltip("The render queue group of the prominence")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset of the prominence")]
	public int RenderQueueOffset;

	[Tooltip("The random seed used when generating the prominence planes")]
	[SgtSeed]
	public int Seed;

	[Tooltip("The inner radius of the prominence planes in local coordinates")]
	public float InnerRadius = 1f;

	[Tooltip("The outer radius of the prominence planes in local coordinates")]
	public float OuterRadius = 2f;

	[Tooltip("The amount of planes used to build the prominence")]
	public int PlaneCount = 8;

	[Tooltip("The amount of quads used to build each plane")]
	[FormerlySerializedAs("Detail")]
	public int PlaneDetail = 10;

	[Tooltip("Should the plane fade out when it's viewed edge-on?")]
	public bool FadeEdge;

	[Tooltip("How sharp the transition between visible and invisible is")]
	public float FadePower = 2f;

	[Tooltip("Should the plane fade out when it's in front of the star?")]
	public bool ClipNear;

	[Tooltip("How sharp the transition between visible and invisible is")]
	public float ClipPower = 2f;

	[Tooltip("How much this prominence gets shifted toward the camera when rendering in world coordinates")]
	[FormerlySerializedAs("ObserverOffset")]
	public float CameraOffset;

	[FormerlySerializedAs("planes")]
	public List<SgtProminencePlane> Planes;

	[NonSerialized]
	public Material Material;

	[NonSerialized]
	public Mesh Mesh;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	[NonSerialized]
	private bool updateMaterialCalled;

	[NonSerialized]
	protected bool updateMeshCalled;

	[NonSerialized]
	private bool updatePlanesCalled;

	public float Width => OuterRadius - InnerRadius;

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		updateMaterialCalled = true;
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Prominence (Generated)", "Hidden/SgtProminence");
			if (Planes != null)
			{
				for (int num = Planes.Count - 1; num >= 0; num--)
				{
					SgtProminencePlane sgtProminencePlane = Planes[num];
					if (sgtProminencePlane != null)
					{
						sgtProminencePlane.SetMaterial(Material);
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
		Material.SetTexture("_MainTex", MainTex);
		Material.SetColor("_Color", value);
		Material.SetVector("_WorldPosition", transform.position);
		SgtHelper.SetTempMaterial(Material);
		if (FadeEdge)
		{
			SgtHelper.EnableKeyword("SGT_A");
			Material.SetFloat("_FadePower", FadePower);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_A");
		}
		if (ClipNear)
		{
			SgtHelper.EnableKeyword("SGT_B");
			Material.SetFloat("_ClipPower", ClipPower);
		}
		else
		{
			SgtHelper.DisableKeyword("SGT_B");
		}
	}

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		updateMeshCalled = true;
		if (Mesh == null)
		{
			Mesh = SgtHelper.CreateTempMesh("Plane");
			if (Planes != null)
			{
				for (int num = Planes.Count - 1; num >= 0; num--)
				{
					SgtProminencePlane sgtProminencePlane = Planes[num];
					if (sgtProminencePlane != null)
					{
						sgtProminencePlane.SetMesh(Mesh);
					}
				}
			}
		}
		else
		{
			Mesh.Clear(keepVertexLayout: false);
		}
		if (PlaneDetail >= 2)
		{
			int num2 = Mathf.Min(PlaneDetail, SgtHelper.QuadsPerMesh / 2);
			Vector3[] array = new Vector3[num2 * 2 + 2];
			Vector3[] array2 = new Vector3[num2 * 2 + 2];
			Vector2[] array3 = new Vector2[num2 * 2 + 2];
			Vector2[] array4 = new Vector2[num2 * 2 + 2];
			int[] array5 = new int[num2 * 6];
			float num3 = SgtHelper.Divide((float)Math.PI * 2f, num2);
			float num4 = SgtHelper.Reciprocal(num2);
			for (int i = 0; i <= num2; i++)
			{
				float num5 = num4 * (float)i;
				float f = num3 * (float)i;
				float num6 = Mathf.Sin(f);
				float num7 = Mathf.Cos(f);
				int num8 = i * 2;
				ref Vector3 reference = ref array[num8];
				reference = new Vector3(num6 * InnerRadius, 0f, num7 * InnerRadius);
				ref Vector3 reference2 = ref array[num8 + 1];
				reference2 = new Vector3(num6 * OuterRadius, 0f, num7 * OuterRadius);
				ref Vector3 reference3 = ref array2[num8];
				reference3 = Vector3.up;
				ref Vector3 reference4 = ref array2[num8 + 1];
				reference4 = Vector3.up;
				ref Vector2 reference5 = ref array3[num8];
				reference5 = new Vector2(0f, num5 * InnerRadius);
				ref Vector2 reference6 = ref array3[num8 + 1];
				reference6 = new Vector2(1f, num5 * OuterRadius);
				ref Vector2 reference7 = ref array4[num8];
				reference7 = new Vector2(InnerRadius, 0f);
				ref Vector2 reference8 = ref array4[num8 + 1];
				reference8 = new Vector2(OuterRadius, 0f);
			}
			for (int j = 0; j < num2; j++)
			{
				int num9 = j * 2;
				int num10 = j * 6;
				array5[num10] = num9;
				array5[num10 + 1] = num9 + 1;
				array5[num10 + 2] = num9 + 2;
				array5[num10 + 3] = num9 + 3;
				array5[num10 + 4] = num9 + 2;
				array5[num10 + 5] = num9 + 1;
			}
			Mesh.vertices = array;
			Mesh.normals = array2;
			Mesh.uv = array3;
			Mesh.uv2 = array4;
			Mesh.triangles = array5;
		}
	}

	[ContextMenu("Update Planes")]
	public void UpdatePlanes()
	{
		updatePlanesCalled = true;
		SgtHelper.BeginRandomSeed(Seed);
		for (int i = 0; i < PlaneCount; i++)
		{
			SgtProminencePlane orAddPlane = GetOrAddPlane(i);
			orAddPlane.SetRotation(UnityEngine.Random.rotationUniform);
		}
		SgtHelper.EndRandomSeed();
		if (Planes != null)
		{
			int num = Mathf.Max(0, PlaneCount);
			for (int num2 = Planes.Count - 1; num2 >= num; num2--)
			{
				SgtProminencePlane.Pool(Planes[num2]);
				Planes.RemoveAt(num2);
			}
		}
	}

	public static SgtProminence CreateProminence(int layer = 0, Transform parent = null)
	{
		return CreateProminence(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtProminence CreateProminence(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Prominence", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtProminence>();
	}

	protected virtual void OnEnable()
	{
		AllProminences.Add(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Planes != null)
		{
			for (int num = Planes.Count - 1; num >= 0; num--)
			{
				SgtProminencePlane sgtProminencePlane = Planes[num];
				if (sgtProminencePlane != null)
				{
					sgtProminencePlane.gameObject.SetActive(value: true);
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
		AllProminences.Remove(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Planes == null)
		{
			return;
		}
		for (int num = Planes.Count - 1; num >= 0; num--)
		{
			SgtProminencePlane sgtProminencePlane = Planes[num];
			if (sgtProminencePlane != null)
			{
				sgtProminencePlane.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Planes != null)
		{
			for (int num = Planes.Count - 1; num >= 0; num--)
			{
				SgtProminencePlane.MarkForDestruction(Planes[num]);
			}
		}
		if (Mesh != null)
		{
			Mesh.Clear(keepVertexLayout: false);
			SgtObjectPool<Mesh>.Add(Mesh);
		}
		SgtHelper.Destroy(Material);
	}

	private void CameraPreCull(Camera camera)
	{
		if (CameraOffset == 0f || Planes == null)
		{
			return;
		}
		for (int num = Planes.Count - 1; num >= 0; num--)
		{
			SgtProminencePlane sgtProminencePlane = Planes[num];
			if (sgtProminencePlane != null)
			{
				sgtProminencePlane.Revert();
				Transform transform = sgtProminencePlane.transform;
				Vector3 normalized = (transform.position - camera.transform.position).normalized;
				transform.position += normalized * CameraOffset;
				sgtProminencePlane.Save(camera);
			}
		}
	}

	private void CameraPreRender(Camera camera)
	{
		if (Planes == null)
		{
			return;
		}
		for (int num = Planes.Count - 1; num >= 0; num--)
		{
			SgtProminencePlane sgtProminencePlane = Planes[num];
			if (sgtProminencePlane != null)
			{
				sgtProminencePlane.Restore(camera);
			}
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (Planes == null)
		{
			return;
		}
		for (int num = Planes.Count - 1; num >= 0; num--)
		{
			SgtProminencePlane sgtProminencePlane = Planes[num];
			if (sgtProminencePlane != null)
			{
				sgtProminencePlane.Revert();
			}
		}
	}

	private SgtProminencePlane GetOrAddPlane(int index)
	{
		SgtProminencePlane sgtProminencePlane = null;
		if (Planes == null)
		{
			Planes = new List<SgtProminencePlane>();
		}
		if (index < Planes.Count)
		{
			sgtProminencePlane = Planes[index];
		}
		else
		{
			Planes.Add(sgtProminencePlane);
		}
		if (sgtProminencePlane == null)
		{
			SgtProminencePlane sgtProminencePlane2 = SgtProminencePlane.Create(this);
			Planes[index] = sgtProminencePlane2;
			sgtProminencePlane = sgtProminencePlane2;
			sgtProminencePlane.SetMaterial(Material);
			sgtProminencePlane.SetMesh(Mesh);
		}
		return sgtProminencePlane;
	}

	private void CheckUpdateCalls()
	{
		if (!updateMaterialCalled)
		{
			UpdateMaterial();
		}
		if (!updateMeshCalled)
		{
			UpdateMesh();
		}
		if (!updatePlanesCalled)
		{
			UpdatePlanes();
		}
	}
}
