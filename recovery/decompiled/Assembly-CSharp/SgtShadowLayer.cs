using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Shadow Layer")]
public class SgtShadowLayer : MonoBehaviour
{
	[Tooltip("The shadows casting on this atmosphere")]
	public List<SgtShadow> Shadows;

	[Tooltip("The renderers that are used to render the inner atmosphere (surface)")]
	public List<MeshRenderer> Renderers;

	[NonSerialized]
	public Material Material;

	[ContextMenu("Apply Material")]
	public void ApplyMaterial()
	{
		if (Renderers != null)
		{
			for (int num = Renderers.Count - 1; num >= 0; num--)
			{
				SgtHelper.AddMaterial(Renderers[num], Material);
			}
		}
	}

	[ContextMenu("Remove Material")]
	public void RemoveMaterial()
	{
		if (Renderers != null)
		{
			for (int num = Renderers.Count - 1; num >= 0; num--)
			{
				SgtHelper.RemoveMaterial(Renderers[num], Material);
			}
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
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		if (Material == null)
		{
			Material = SgtHelper.CreateTempMaterial("Shadow Layer (Generated)", "Hidden/SgtShadowLayer");
		}
		if (Renderers == null)
		{
			AddRenderer(GetComponent<MeshRenderer>());
		}
		ApplyMaterial();
	}

	protected virtual void OnDisable()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		RemoveMaterial();
	}

	protected virtual void CameraPreRender(Camera camera)
	{
		if (Material != null)
		{
			SgtHelper.SetTempMaterial(Material);
			SgtHelper.WriteShadows(Shadows, 2);
		}
	}
}
