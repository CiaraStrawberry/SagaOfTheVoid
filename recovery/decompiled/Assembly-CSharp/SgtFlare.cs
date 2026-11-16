using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Flare")]
public class SgtFlare : MonoBehaviour
{
	[Tooltip("The mesh used to render this flare")]
	public Mesh Mesh;

	[Tooltip("The material used to render this flare")]
	public Material Material;

	[Tooltip("The thickness required between the camera and this flare for the flare to be fully hidden in world space")]
	public float HideThickness = 10f;

	[Tooltip("If you want this flare to hide behind solid objects then set this")]
	public SgtDepth Depth;

	[Tooltip("Should the flare automatically snap to cameras ")]
	public bool FollowCameras;

	[Tooltip("The distance from the camera this flare will be placed in world space")]
	public float FollowDistance = 100f;

	[Tooltip("The distance this flare is moved toward the current camera when rendering in world space")]
	public float CameraOffset;

	public SgtFlareModel Model;

	private static bool busy;

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		if (Model != null)
		{
			Model.SetMesh(Mesh);
		}
	}

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		if (Model != null)
		{
			Model.SetMaterial(Material);
		}
	}

	[ContextMenu("Update Model")]
	public void UpdateModel()
	{
		if (Model == null)
		{
			Model = SgtFlareModel.Create(this);
			Model.SetMesh(Mesh);
			Model.SetMaterial(Material);
		}
	}

	public static SgtFlare CreateFlare(int layer = 0, Transform parent = null)
	{
		return CreateFlare(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtFlare CreateFlare(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Flare", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtFlare>();
	}

	protected virtual void OnEnable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Model != null)
		{
			Model.gameObject.SetActive(value: true);
		}
	}

	protected virtual void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
		if (Model != null)
		{
			Model.gameObject.SetActive(value: false);
		}
	}

	protected virtual void Start()
	{
		if (Model == null)
		{
			UpdateModel();
		}
	}

	protected virtual void OnDestroy()
	{
		SgtFlareModel.MarkForDestruction(Model);
	}

	private void CameraPreCull(Camera camera)
	{
		if (!busy && Model != null)
		{
			Model.Revert();
			if (FollowCameras)
			{
				Model.transform.position = camera.transform.position - Model.transform.forward * FollowDistance;
			}
			if (SgtHelper.Enabled(Depth))
			{
				busy = true;
				Model.transform.localScale *= 1f - Depth.Calculate(camera.transform.position, Model.transform.position);
				busy = false;
			}
			Vector3 normalized = (Model.transform.position - camera.transform.position).normalized;
			Model.transform.rotation = camera.transform.rotation;
			Model.transform.position += normalized * CameraOffset;
			Model.Save(camera);
		}
	}

	private void CameraPreRender(Camera camera)
	{
		if (!busy && Model != null)
		{
			Model.Restore(camera);
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (!busy && Model != null)
		{
			Model.Revert();
		}
	}
}
