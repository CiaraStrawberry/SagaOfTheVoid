using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SgtCameraDepth : SgtDepth
{
	[Tooltip("The width/height of the camera RenderTexture in pixels")]
	public int Resolution = 8;

	[Tooltip("The width/height of the camera viewport in world space")]
	public float Size = 1f;

	private Camera cachedCamera;

	private RenderTexture renderTexture;

	private Texture2D readTexture;

	public static SgtCameraDepth CreateDepthCamera(int layer = 0, Transform parent = null)
	{
		return CreateDepthCamera(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCameraDepth CreateDepthCamera(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Camera Depth", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtCameraDepth>();
	}

	protected override float DoCalculate(Vector3 eye, Vector3 target)
	{
		float num = 0f;
		if (Resolution > 0 && Size > 0f)
		{
			if (renderTexture != null && (renderTexture.width != Resolution || renderTexture.height != Resolution))
			{
				renderTexture = SgtHelper.Destroy(renderTexture);
			}
			if (renderTexture == null)
			{
				renderTexture = new RenderTexture(Resolution, Resolution, 16);
			}
			if (readTexture != null && (readTexture.width != Resolution || readTexture.height != Resolution))
			{
				readTexture = SgtHelper.Destroy(readTexture);
			}
			if (readTexture == null)
			{
				readTexture = new Texture2D(Resolution, Resolution);
			}
			Vector3 localPosition = transform.localPosition;
			Quaternion localRotation = transform.localRotation;
			UpdateCamera();
			cachedCamera.farClipPlane = Vector3.Distance(eye, target);
			cachedCamera.transform.position = eye;
			cachedCamera.transform.LookAt(target);
			cachedCamera.targetTexture = renderTexture;
			cachedCamera.Render();
			RenderTexture.active = renderTexture;
			readTexture.ReadPixels(new Rect(0f, 0f, Resolution, Resolution), 0, 0);
			RenderTexture.active = null;
			cachedCamera.targetTexture = null;
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			for (int i = 0; i < Resolution; i++)
			{
				for (int j = 0; j < Resolution; j++)
				{
					num += Mathf.Clamp01(readTexture.GetPixel(j, i).a);
				}
			}
			num /= (float)(Resolution * Resolution);
		}
		return num;
	}

	protected virtual void OnEnable()
	{
		UpdateCamera();
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(renderTexture);
		SgtHelper.Destroy(readTexture);
	}

	private void UpdateCamera()
	{
		if (cachedCamera == null)
		{
			cachedCamera = GetComponent<Camera>();
		}
		cachedCamera.enabled = false;
		cachedCamera.aspect = 1f;
		cachedCamera.orthographic = true;
		cachedCamera.nearClipPlane = 0f;
		cachedCamera.farClipPlane = 1f;
		cachedCamera.orthographicSize = Size;
		cachedCamera.cullingMask = Layers;
		cachedCamera.clearFlags = CameraClearFlags.Color;
		cachedCamera.backgroundColor = Color.clear;
	}
}
