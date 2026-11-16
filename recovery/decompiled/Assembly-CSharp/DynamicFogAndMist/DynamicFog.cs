using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicFogAndMist;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[HelpURL("http://kronnect.com/taptapgo")]
[ImageEffectAllowedInSceneView]
public class DynamicFog : MonoBehaviour
{
	public FOG_TYPE effectType = FOG_TYPE.DesktopFogPlusWithSkyHaze;

	public FOG_PRESET preset = FOG_PRESET.Mist;

	public bool useFogVolumes;

	public bool enableDithering;

	[Range(0f, 1f)]
	public float alpha = 1f;

	[Range(0f, 1f)]
	public float noiseStrength = 0.5f;

	[Range(0f, 0.999f)]
	public float distance = 0.1f;

	[Range(0.0001f, 2f)]
	public float distanceFallOff = 0.01f;

	[Range(0f, 1.2f)]
	public float maxDistance = 0.999f;

	[Range(0.0001f, 0.5f)]
	public float maxDistanceFallOff;

	[Range(0f, 500f)]
	public float height = 1f;

	[Range(0f, 1f)]
	public float heightFallOff = 0.1f;

	public float baselineHeight;

	public bool clipUnderBaseline;

	[Range(0f, 15f)]
	public float turbulence = 0.1f;

	[Range(0f, 0.2f)]
	public float speed = 0.1f;

	public Color color = Color.white;

	public Color color2 = Color.gray;

	[Range(0f, 500f)]
	public float skyHaze = 50f;

	[Range(0f, 1f)]
	public float skySpeed = 0.3f;

	[Range(0f, 1f)]
	public float skyNoiseStrength = 0.1f;

	[Range(0f, 1f)]
	public float skyAlpha = 1f;

	public GameObject sun;

	public bool fogOfWarEnabled;

	public Vector3 fogOfWarCenter;

	public Vector3 fogOfWarSize = new Vector3(1024f, 0f, 1024f);

	public int fogOfWarTextureSize = 256;

	private Material fogMatAdv;

	private Material fogMatFogSky;

	private Material fogMatOnlyFog;

	private Material fogMatVol;

	private Material fogMatSimple;

	[SerializeField]
	private Material fogMat;

	private float initialFogAlpha;

	private float targetFogAlpha;

	private float initialSkyHazeAlpha;

	private float targetSkyHazeAlpha;

	private float transitionDuration;

	private float transitionStartTime;

	private float currentFogAlpha;

	private float currentSkyHazeAlpha;

	private Camera currentCamera;

	private Texture2D fogOfWarTexture;

	private Color32[] fogOfWarColorBuffer;

	private Light sunLight;

	private Vector3 sunDirection = Vector3.zero;

	private Color sunColor = Color.white;

	private float sunIntensity = 1f;

	private static DynamicFog _fog;

	private List<string> shaderKeywords;

	public static DynamicFog instance
	{
		get
		{
			if (_fog == null)
			{
				Camera[] allCameras = Camera.allCameras;
				foreach (Camera camera in allCameras)
				{
					_fog = camera.GetComponent<DynamicFog>();
					if (_fog != null)
					{
						break;
					}
				}
			}
			return _fog;
		}
	}

	public Camera fogCamera => currentCamera;

	public string GetCurrentPresetName()
	{
		return Enum.GetName(typeof(FOG_PRESET), preset);
	}

	private void OnEnable()
	{
		Init();
		UpdateMaterialProperties();
	}

	private void Reset()
	{
		UpdateMaterialProperties();
	}

	private void OnDestroy()
	{
		fogMat = null;
		if (fogMatVol != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMatVol);
			fogMatVol = null;
		}
		if (fogMatAdv != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMatAdv);
			fogMatAdv = null;
		}
		if (fogMatFogSky != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMatFogSky);
			fogMatFogSky = null;
		}
		if (fogMatOnlyFog != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMatOnlyFog);
			fogMatOnlyFog = null;
		}
		if (fogMatSimple != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMatSimple);
			fogMatSimple = null;
		}
		if (fogOfWarTexture != null)
		{
			UnityEngine.Object.DestroyImmediate(fogOfWarTexture);
			fogOfWarTexture = null;
		}
	}

	private void Init()
	{
		targetFogAlpha = -1f;
		targetSkyHazeAlpha = -1f;
		currentFogAlpha = alpha;
		currentSkyHazeAlpha = skyAlpha;
		currentCamera = GetComponent<Camera>();
		if (currentCamera.depthTextureMode == DepthTextureMode.None)
		{
			currentCamera.depthTextureMode = DepthTextureMode.Depth;
		}
		UpdateFogOfWarTexture();
	}

	private void Update()
	{
		if (fogMat == null)
		{
			return;
		}
		if (targetFogAlpha >= 0f)
		{
			if (targetFogAlpha != currentFogAlpha || targetSkyHazeAlpha != currentSkyHazeAlpha)
			{
				if (transitionDuration > 0f)
				{
					currentFogAlpha = Mathf.Lerp(initialFogAlpha, targetFogAlpha, (Time.time - transitionStartTime) / transitionDuration);
					currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, targetSkyHazeAlpha, (Time.time - transitionStartTime) / transitionDuration);
				}
				else
				{
					currentFogAlpha = targetFogAlpha;
					currentSkyHazeAlpha = targetSkyHazeAlpha;
				}
				fogMat.SetFloat("_FogAlpha", currentFogAlpha);
				SetSkyData();
			}
		}
		else if (currentFogAlpha != alpha || targetSkyHazeAlpha != currentSkyHazeAlpha)
		{
			if (transitionDuration > 0f)
			{
				currentFogAlpha = Mathf.Lerp(initialFogAlpha, alpha, (Time.time - transitionStartTime) / transitionDuration);
				currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, alpha, (Time.time - transitionStartTime) / transitionDuration);
			}
			else
			{
				currentFogAlpha = alpha;
				currentSkyHazeAlpha = skyAlpha;
			}
			fogMat.SetFloat("_FogAlpha", currentFogAlpha);
			SetSkyData();
		}
		if (sun != null)
		{
			bool flag = false;
			if (sun.transform.forward != sunDirection)
			{
				flag = true;
			}
			if (sunLight != null && (sunLight.color != sunColor || sunLight.intensity != sunIntensity))
			{
				flag = true;
			}
			if (flag)
			{
				UpdateFogColor();
			}
		}
	}

	public void CheckPreset()
	{
		switch (preset)
		{
		case FOG_PRESET.Clear:
			alpha = 0f;
			break;
		case FOG_PRESET.Mist:
			alpha = 0.75f;
			skySpeed = 0.11f;
			skyHaze = 15f;
			skyNoiseStrength = 1f;
			skyAlpha = 0.33f;
			distance = 0f;
			distanceFallOff = 0.07f;
			height = 4.4f;
			heightFallOff = 1f;
			turbulence = 0f;
			noiseStrength = 0.6f;
			speed = 0.01f;
			color = new Color(0.89f, 0.89f, 0.89f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		case FOG_PRESET.WindyMist:
			alpha = 0.75f;
			skySpeed = 0.3f;
			skyHaze = 35f;
			skyNoiseStrength = 0.32f;
			skyAlpha = 0.33f;
			distance = 0f;
			distanceFallOff = 0.07f;
			height = 2f;
			heightFallOff = 1f;
			turbulence = 2f;
			noiseStrength = 0.6f;
			speed = 0.06f;
			color = new Color(0.89f, 0.89f, 0.89f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		case FOG_PRESET.GroundFog:
			alpha = 1f;
			skySpeed = 0.3f;
			skyHaze = 35f;
			skyNoiseStrength = 0.32f;
			skyAlpha = 0.33f;
			distance = 0f;
			distanceFallOff = 0f;
			height = 1f;
			heightFallOff = 1f;
			turbulence = 0.4f;
			noiseStrength = 0.7f;
			speed = 0.005f;
			color = new Color(0.89f, 0.89f, 0.89f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		case FOG_PRESET.Fog:
			alpha = 0.96f;
			skySpeed = 0.3f;
			skyHaze = 155f;
			skyNoiseStrength = 0.6f;
			skyAlpha = 0.93f;
			distance = ((!effectType.isPlus()) ? 0.01f : 0.2f);
			distanceFallOff = 0.04f;
			height = 20f;
			heightFallOff = 1f;
			turbulence = 0.4f;
			noiseStrength = 0.4f;
			speed = 0.005f;
			color = new Color(0.89f, 0.89f, 0.89f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		case FOG_PRESET.HeavyFog:
			alpha = 1f;
			skySpeed = 0.05f;
			skyHaze = 350f;
			skyNoiseStrength = 0.8f;
			skyAlpha = 0.97f;
			distance = ((!effectType.isPlus()) ? 0f : 0.1f);
			distanceFallOff = 0.045f;
			height = 35f;
			heightFallOff = 0.88f;
			turbulence = 0.4f;
			noiseStrength = 0.24f;
			speed = 0.003f;
			color = new Color(0.86f, 0.847f, 0.847f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		case FOG_PRESET.SandStorm:
			alpha = 1f;
			skySpeed = 0.49f;
			skyHaze = 333f;
			skyNoiseStrength = 0.72f;
			skyAlpha = 0.97f;
			distance = ((!effectType.isPlus()) ? 0f : 0.15f);
			distanceFallOff = 0.028f;
			height = 83f;
			heightFallOff = 0f;
			turbulence = 15f;
			noiseStrength = 0.45f;
			speed = 0.2f;
			color = new Color(0.364f, 0.36f, 0.36f, 1f);
			color2 = color;
			maxDistance = 0.999f;
			maxDistanceFallOff = 0f;
			break;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (fogMat == null || alpha == 0f || currentCamera == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		fogMat.SetMatrix("_ClipToWorld", currentCamera.cameraToWorldMatrix * currentCamera.projectionMatrix.inverse);
		Graphics.Blit(source, destination, fogMat);
	}

	public void UpdateMaterialProperties()
	{
		CheckPreset();
		switch (effectType)
		{
		case FOG_TYPE.MobileFogOnlyGround:
			if (fogMatOnlyFog == null)
			{
				fogMatOnlyFog = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/DFGOnlyFog"));
				fogMatOnlyFog.hideFlags = HideFlags.DontSave;
			}
			fogMat = fogMatOnlyFog;
			break;
		case FOG_TYPE.MobileFogWithSkyHaze:
			if (fogMatFogSky == null)
			{
				fogMatFogSky = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/DFGWithSky"));
				fogMatFogSky.hideFlags = HideFlags.DontSave;
			}
			fogMat = fogMatFogSky;
			break;
		case FOG_TYPE.DesktopFogPlusWithSkyHaze:
			if (fogMatVol == null)
			{
				fogMatVol = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/DFGDesktopPlus"));
				fogMatVol.hideFlags = HideFlags.DontSave;
			}
			fogMat = fogMatVol;
			break;
		case FOG_TYPE.MobileFogSimple:
			if (fogMatSimple == null)
			{
				fogMatSimple = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/DFGSimple"));
				fogMatSimple.hideFlags = HideFlags.DontSave;
			}
			fogMat = fogMatSimple;
			break;
		default:
			if (fogMatAdv == null)
			{
				fogMatAdv = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/DFGDesktop"));
				fogMatAdv.hideFlags = HideFlags.DontSave;
			}
			fogMat = fogMatAdv;
			break;
		}
		if (fogMat == null)
		{
			return;
		}
		if (currentCamera == null)
		{
			currentCamera = GetComponent<Camera>();
		}
		fogMat.SetFloat("_FogSpeed", speed);
		Vector3 vector = new Vector3(noiseStrength, turbulence, currentCamera.farClipPlane * 15f / 1000f);
		fogMat.SetVector("_FogNoiseData", vector);
		Vector4 value = new Vector4(height + 0.001f, baselineHeight, (!clipUnderBaseline) ? (-10000f) : (-0.01f), heightFallOff);
		fogMat.SetVector("_FogHeightData", value);
		fogMat.SetFloat("_FogAlpha", currentFogAlpha);
		Vector4 value2 = new Vector4(distance, distanceFallOff, maxDistance, maxDistanceFallOff);
		if (effectType.isPlus())
		{
			value2.x = currentCamera.farClipPlane * distance;
			value2.y = distanceFallOff * value2.x + 0.0001f;
			value2.z *= currentCamera.farClipPlane;
		}
		fogMat.SetVector("_FogDistance", value2);
		UpdateFogColor();
		SetSkyData();
		if (shaderKeywords == null)
		{
			shaderKeywords = new List<string>();
		}
		else
		{
			shaderKeywords.Clear();
		}
		if (fogOfWarEnabled)
		{
			if (fogOfWarTexture == null)
			{
				UpdateFogOfWarTexture();
			}
			fogMat.SetTexture("_FogOfWar", fogOfWarTexture);
			fogMat.SetVector("_FogOfWarCenter", fogOfWarCenter);
			fogMat.SetVector("_FogOfWarSize", fogOfWarSize);
			Vector3 vector2 = fogOfWarCenter - 0.5f * fogOfWarSize;
			fogMat.SetVector("_FogOfWarCenterAdjusted", new Vector3(vector2.x / fogOfWarSize.x, 1f, vector2.z / fogOfWarSize.z));
			shaderKeywords.Add("FOG_OF_WAR_ON");
		}
		if (enableDithering)
		{
			shaderKeywords.Add("DITHER_ON");
		}
		fogMat.shaderKeywords = shaderKeywords.ToArray();
	}

	private void SetSkyData()
	{
		Vector4 value = new Vector4(skyHaze, skySpeed, skyNoiseStrength, currentSkyHazeAlpha);
		fogMat.SetVector("_FogSkyData", value);
	}

	private void UpdateFogColor()
	{
		if (fogMat == null)
		{
			return;
		}
		if (sun != null)
		{
			if (sunLight == null)
			{
				sunLight = sun.GetComponent<Light>();
			}
			if (sunLight != null && sunLight.transform != sun.transform)
			{
				sunLight = sun.GetComponent<Light>();
			}
			sunDirection = sun.transform.forward;
			if (sunLight != null)
			{
				sunColor = sunLight.color;
				sunIntensity = sunLight.intensity;
			}
		}
		float num = sunIntensity * Mathf.Clamp01(1f - sunDirection.y);
		fogMat.SetColor("_FogColor", num * color * sunColor);
		fogMat.SetColor("_FogColor2", num * color2 * sunColor);
	}

	public void SetTargetAlpha(float newFogAlpha, float newSkyHazeAlpha, float duration)
	{
		if (useFogVolumes)
		{
			initialFogAlpha = currentFogAlpha;
			initialSkyHazeAlpha = currentSkyHazeAlpha;
			targetFogAlpha = newFogAlpha;
			targetSkyHazeAlpha = newSkyHazeAlpha;
			transitionDuration = duration;
			transitionStartTime = Time.time;
		}
	}

	public void ClearTargetAlpha(float duration)
	{
		SetTargetAlpha(-1f, -1f, duration);
	}

	private void UpdateFogOfWarTexture()
	{
		if (fogOfWarEnabled)
		{
			int scaledSize = GetScaledSize(fogOfWarTextureSize, 1f);
			fogOfWarTexture = new Texture2D(scaledSize, scaledSize, TextureFormat.ARGB32, mipmap: false);
			fogOfWarTexture.hideFlags = HideFlags.DontSave;
			fogOfWarTexture.filterMode = FilterMode.Bilinear;
			fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
			ResetFogOfWar();
		}
	}

	public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha)
	{
		if (fogOfWarTexture == null)
		{
			return;
		}
		float num = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
		if (num < 0f || num > 1f)
		{
			return;
		}
		float num2 = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
		if (num2 < 0f || num2 > 1f)
		{
			return;
		}
		int width = fogOfWarTexture.width;
		int num3 = fogOfWarTexture.height;
		int num4 = (int)(num * (float)width);
		int num5 = (int)(num2 * (float)num3);
		int num6 = num5 * width + num4;
		byte b = (byte)(fogNewAlpha * 255f);
		Color32 color = fogOfWarColorBuffer[num6];
		if (b == color.a)
		{
			return;
		}
		float num7 = radius / fogOfWarSize.z;
		int num8 = Mathf.FloorToInt((float)num3 * num7);
		for (int i = num5 - num8; i <= num5 + num8; i++)
		{
			if (i <= 0 || i >= num3 - 1)
			{
				continue;
			}
			for (int j = num4 - num8; j <= num4 + num8; j++)
			{
				if (j > 0 && j < width - 1)
				{
					int num9 = Mathf.FloorToInt(Mathf.Sqrt((num5 - i) * (num5 - i) + (num4 - j) * (num4 - j)));
					if (num9 <= num8)
					{
						num6 = i * width + j;
						Color32 color2 = fogOfWarColorBuffer[num6];
						color2.a = (byte)Mathf.Lerp((int)b, (int)color2.a, (float)num9 / (float)num8);
						fogOfWarColorBuffer[num6] = color2;
						fogOfWarTexture.SetPixel(j, i, color2);
					}
				}
			}
		}
		fogOfWarTexture.Apply();
	}

	public void ResetFogOfWarAlpha(Vector3 worldPosition, float radius)
	{
		if (fogOfWarTexture == null)
		{
			return;
		}
		float num = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
		if (num < 0f || num > 1f)
		{
			return;
		}
		float num2 = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
		if (num2 < 0f || num2 > 1f)
		{
			return;
		}
		int width = fogOfWarTexture.width;
		int num3 = fogOfWarTexture.height;
		int num4 = (int)(num * (float)width);
		int num5 = (int)(num2 * (float)num3);
		int num6 = num5 * width + num4;
		float num7 = radius / fogOfWarSize.z;
		int num8 = Mathf.FloorToInt((float)num3 * num7);
		for (int i = num5 - num8; i <= num5 + num8; i++)
		{
			if (i > 0 && i < num3 - 1)
			{
				for (int j = num4 - num8; j <= num4 + num8; j++)
				{
					if (j > 0 && j < width - 1)
					{
						int num9 = Mathf.FloorToInt(Mathf.Sqrt((num5 - i) * (num5 - i) + (num4 - j) * (num4 - j)));
						if (num9 <= num8)
						{
							num6 = i * width + j;
							Color32 color = fogOfWarColorBuffer[num6];
							color.a = byte.MaxValue;
							fogOfWarColorBuffer[num6] = color;
							fogOfWarTexture.SetPixel(j, i, color);
						}
					}
				}
			}
			fogOfWarTexture.Apply();
		}
	}

	public void ResetFogOfWar()
	{
		if (!(fogOfWarTexture == null))
		{
			int num = fogOfWarTexture.height;
			int width = fogOfWarTexture.width;
			int num2 = num * width;
			if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != num2)
			{
				fogOfWarColorBuffer = new Color32[num2];
			}
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			for (int i = 0; i < num2; i++)
			{
				fogOfWarColorBuffer[i] = color;
			}
			fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
			fogOfWarTexture.Apply();
		}
	}

	private int GetScaledSize(int size, float factor)
	{
		size = (int)((float)size / factor);
		size /= 4;
		if (size < 1)
		{
			size = 1;
		}
		return size * 4;
	}
}
