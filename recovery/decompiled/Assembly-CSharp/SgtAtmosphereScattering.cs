using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Atmosphere Scattering")]
public class SgtAtmosphereScattering : MonoBehaviour
{
	[Tooltip("The atmosphere this texture will be applied to")]
	public SgtAtmosphere Atmosphere;

	[Tooltip("The resolution of the day/sunset/night transition in pixels")]
	public int Width = 64;

	[Tooltip("The format of the generated texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	public float Scale = 1f;

	[Tooltip("The sharpness of the forward scattered light")]
	public float Mie = 15f;

	[Tooltip("The brightness of the front and back scattered light")]
	public float Rayleigh = 0.1f;

	[Tooltip("The transition style between the day and night")]
	public SgtEase.Type SunsetEase = SgtEase.Type.Smoothstep;

	[Tooltip("The start point of the day/sunset transition (0 = dark side, 1 = light side)")]
	[Range(0f, 1f)]
	public float SunsetStart = 0.35f;

	[Tooltip("The end point of the sunset/night transition (0 = dark side, 1 = light side)")]
	[Range(0f, 1f)]
	public float SunsetEnd = 0.6f;

	[Tooltip("The power of the sunset red channel transition")]
	public float SunsetPowerR = 2f;

	[Tooltip("The power of the sunset green channel transition")]
	public float SunsetPowerG = 1f;

	[Tooltip("The power of the sunset blue channel transition")]
	public float SunsetPowerB = 1f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Texture")]
	public void UpdateTexture()
	{
		if (Width > 0)
		{
			if (generatedTexture != null && (generatedTexture.width != Width || generatedTexture.height != 1 || generatedTexture.format != Format))
			{
				generatedTexture = SgtHelper.Destroy(generatedTexture);
			}
			if (generatedTexture == null)
			{
				generatedTexture = SgtHelper.CreateTempTexture2D("Atmosphere Scattering (Generated)", Width, 1, Format);
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
		Color color = default;
		color.r = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerR));
		color.g = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerG));
		color.b = SgtEase.Evaluate(SunsetEase, 1f - Mathf.Pow(f, SunsetPowerB));
		color.a = (color.r + color.g + color.b) / 3f;
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Atmosphere != null && generatedTexture != null && Atmosphere.ScatteringTex != generatedTexture)
		{
			Atmosphere.ScatteringTex = generatedTexture;
			Atmosphere.UpdateScatteringTex();
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
			UpdateTexture();
		}
		UpdateApply();
	}
}
