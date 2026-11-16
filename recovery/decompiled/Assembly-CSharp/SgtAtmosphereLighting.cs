using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Atmosphere Lighting")]
public class SgtAtmosphereLighting : MonoBehaviour
{
	[Tooltip("The atmosphere this texture will be applied to")]
	public SgtAtmosphere Atmosphere;

	[Tooltip("The resolution of the day/sunset/night color transition in pixels")]
	public int Width = 256;

	[Tooltip("The texture format of the textures")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The transition style between the day and night")]
	public SgtEase.Type SunsetEase = SgtEase.Type.Smoothstep;

	[Tooltip("The start point of the day/sunset transition (0 = dark side, 1 = light side)")]
	[Range(0f, 1f)]
	public float SunsetStart = 0.4f;

	[Tooltip("The end point of the sunset/night transition (0 = dark side, 1 = light side)")]
	[Range(0f, 1f)]
	public float SunsetEnd = 0.6f;

	[Tooltip("The power of the sunset red channel transition")]
	public float SunsetPowerR = 2f;

	[Tooltip("The power of the sunset green channel transition")]
	public float SunsetPowerG = 2f;

	[Tooltip("The power of the sunset blue channel transition")]
	public float SunsetPowerB = 2f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	private bool startCalled;

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Textures")]
	public void UpdateTextures()
	{
		if (Width > 0)
		{
			if (generatedTexture != null && (generatedTexture.width != Width || generatedTexture.height != 1 || generatedTexture.format != Format))
			{
				generatedTexture = SgtHelper.Destroy(generatedTexture);
			}
			if (generatedTexture == null)
			{
				generatedTexture = SgtHelper.CreateTempTexture2D("Atmosphere Lighting (Generated)", Width, 1, Format);
				generatedTexture.wrapMode = TextureWrapMode.Clamp;
				UpdateApply();
			}
			float num = 1f / (float)(Width - 1);
			for (int i = 0; i < Width; i++)
			{
				float u = (float)i * num;
				WriteTexture(u, i);
			}
			generatedTexture.Apply();
		}
	}

	private void WriteTexture(float u, int x)
	{
		float f = Mathf.InverseLerp(SunsetEnd, SunsetStart, u);
		Color color = new Color
		{
			r = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerR)),
			g = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerG)),
			b = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerB)),
			a = 0f
		};
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Atmosphere != null && generatedTexture != null && Atmosphere.LightingTex != generatedTexture)
		{
			Atmosphere.LightingTex = generatedTexture;
			Atmosphere.UpdateLightingTex();
		}
	}

	protected virtual void OnEnable()
	{
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
			if (Atmosphere == null)
			{
				Atmosphere = GetComponent<SgtAtmosphere>();
			}
			CheckUpdateCalls();
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(generatedTexture);
	}

	private void CheckUpdateCalls()
	{
		if (generatedTexture == null)
		{
			UpdateTextures();
		}
		UpdateApply();
	}
}
