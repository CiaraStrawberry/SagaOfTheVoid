using System;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Space Graphics Toolkit/SGT Depth Texture Mode")]
public class SgtDepthTextureMode : MonoBehaviour
{
	[Tooltip("The depth mode that will be applied to the camera")]
	public DepthTextureMode DepthMode;

	[NonSerialized]
	private Camera cachedCamera;

	public void UpdateDepthMode()
	{
		if (cachedCamera == null)
		{
			cachedCamera = GetComponent<Camera>();
		}
		cachedCamera.depthTextureMode = DepthMode;
	}

	protected virtual void Update()
	{
		UpdateDepthMode();
	}
}
