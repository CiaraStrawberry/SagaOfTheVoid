using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Skysphere")]
public class SgtSkysphere : MonoBehaviour
{
	public static List<SgtSkysphere> AllSkyspheres = new List<SgtSkysphere>();

	[Tooltip("The meshes used to render this")]
	public List<Mesh> Meshes;

	[Tooltip("The color tint")]
	public Color Color = Color.white;

	[Tooltip("The color brightness")]
	public float Brightness = 1f;

	[Tooltip("The render queue group")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset")]
	public int RenderQueueOffset;

	[Tooltip("The texture applied to the skysphere meshes")]
	public Texture MainTex;

	[Tooltip("Should this be placed on top of the current camera when rendering?")]
	[FormerlySerializedAs("FollowObservers")]
	public bool FollowCameras;

	[NonSerialized]
	public Material Material;

	public List<SgtSkysphereModel> Models;

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
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Skysphere (Generated)", "Hidden/SgtSkysphere");
		}
		Material.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		Material.SetTexture("_MainTex", MainTex);
		Material.SetColor("_Color", SgtHelper.Brighten(Color, Brightness));
	}

	[ContextMenu("Update Models")]
	public void UpdateModels()
	{
		updateModelsCalled = true;
		int num = ((Meshes != null) ? Meshes.Count : 0);
		for (int i = 0; i < num; i++)
		{
			Mesh mesh = Meshes[i];
			SgtSkysphereModel orAddModel = GetOrAddModel(i);
			orAddModel.SetMesh(mesh);
			orAddModel.SetMaterial(Material);
		}
		if (Models != null)
		{
			for (int num2 = Models.Count - 1; num2 >= num; num2--)
			{
				SgtSkysphereModel.Pool(Models[num2]);
				Models.RemoveAt(num2);
			}
		}
	}

	public static SgtSkysphere CreateSkysphere(int layer = 0, Transform parent = null)
	{
		return CreateSkysphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSkysphere CreateSkysphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Skysphere", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtSkysphere>();
	}

	protected virtual void OnEnable()
	{
		AllSkyspheres.Add(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtSkysphereModel sgtSkysphereModel = Models[num];
				if (sgtSkysphereModel != null)
				{
					sgtSkysphereModel.gameObject.SetActive(value: true);
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
		AllSkyspheres.Remove(this);
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtSkysphereModel sgtSkysphereModel = Models[num];
			if (sgtSkysphereModel != null)
			{
				sgtSkysphereModel.gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Models != null)
		{
			for (int num = Models.Count - 1; num >= 0; num--)
			{
				SgtSkysphereModel.MarkForDestruction(Models[num]);
			}
		}
		SgtHelper.Destroy(Material);
	}

	private void CameraPreCull(Camera camera)
	{
		if (!FollowCameras || Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtSkysphereModel sgtSkysphereModel = Models[num];
			if (sgtSkysphereModel != null)
			{
				sgtSkysphereModel.Save();
				sgtSkysphereModel.transform.position = camera.transform.position;
			}
		}
	}

	private void CameraPreRender(Camera camera)
	{
		CameraPreCull(camera);
	}

	private void CameraPostRender(Camera camera)
	{
		if (Models == null)
		{
			return;
		}
		for (int num = Models.Count - 1; num >= 0; num--)
		{
			SgtSkysphereModel sgtSkysphereModel = Models[num];
			if (sgtSkysphereModel != null)
			{
				sgtSkysphereModel.Restore();
			}
		}
	}

	private SgtSkysphereModel GetOrAddModel(int index)
	{
		SgtSkysphereModel sgtSkysphereModel = null;
		if (Models == null)
		{
			Models = new List<SgtSkysphereModel>();
		}
		if (index < Models.Count)
		{
			sgtSkysphereModel = Models[index];
			if (sgtSkysphereModel == null)
			{
				sgtSkysphereModel = SgtSkysphereModel.Create(this);
				Models[index] = sgtSkysphereModel;
			}
		}
		else
		{
			sgtSkysphereModel = SgtSkysphereModel.Create(this);
			Models.Add(sgtSkysphereModel);
		}
		return sgtSkysphereModel;
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
