using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Jovian Scattering")]
public class SgtJovianScattering : MonoBehaviour
{
	[Tooltip("The jovian this texture will be applied to")]
	public SgtJovian Jovian;

	[Tooltip("The resolution of the day/sunset/night color transition in pixels")]
	public int Width = 64;

	[Tooltip("The resolution of the scattering transition in pixels")]
	public int Height = 512;

	[Tooltip("The format of the generated texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The sharpness of the forward scattered light")]
	public float Mie = 150f;

	[Tooltip("The brightness of the front and back scattered light")]
	public float Rayleigh = 0.1f;

	[Tooltip("The transition style between the day and night")]
	public SgtEase.Type SunsetEase = SgtEase.Type.Smoothstep;

	[Tooltip("The start point of the sunset (0 = dark side, 1 = light side)")]
	[Range(0f, 1f)]
	public float SunsetStart = 0.4f;

	[Tooltip("The end point of the sunset (0 = dark side, 1 = light side)")]
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
	[HideInInspector]
	private bool startCalled;

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Texture")]
	public void UpdateTexture()
	{
		if (Width <= 0 || Height <= 0)
		{
			return;
		}
		if (generatedTexture != null && (generatedTexture.width != Width || generatedTexture.height != Height || generatedTexture.format != Format))
		{
			generatedTexture = SgtHelper.Destroy(generatedTexture);
		}
		if (generatedTexture == null)
		{
			generatedTexture = SgtHelper.CreateTempTexture2D("Jovian Scattering (Generated)", Width, Height, Format);
			generatedTexture.wrapMode = TextureWrapMode.Clamp;
			UpdateApply();
		}
		float num = 1f / (float)(Width - 1);
		float num2 = 1f / (float)(Height - 1);
		for (int i = 0; i < Height; i++)
		{
			float v = (float)i * num2;
			for (int j = 0; j < Width; j++)
			{
				float u = (float)j * num;
				WriteTexture(u, v, j, i);
			}
		}
		generatedTexture.Apply();
	}

	private void WriteTexture(float u, float v, int x, int y)
	{
		float num = Mathf.Abs(v * 2f - 1f);
		num = Rayleigh * num * num;
		float num2 = Mathf.Pow(v, Mie);
		float num3 = num + num2 * (1f - num);
		float f = Mathf.InverseLerp(SunsetEnd, SunsetStart, u);
		Color color = default;
		color.r = 1f - SgtEase.Evaluate(SunsetEase, Mathf.Pow(f, SunsetPowerR));
		color.g = 1f - SgtEase.Evaluate(SunsetEase, Mathf.Pow(f, SunsetPowerG));
		color.b = 1f - SgtEase.Evaluate(SunsetEase, Mathf.Pow(f, SunsetPowerB));
		color.a = (color.r + color.g + color.b) / 3f;
		generatedTexture.SetPixel(x, y, color * num3);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Jovian != null)
		{
			Jovian.ScatteringTex = generatedTexture;
			Jovian.UpdateScatteringTex();
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
			if (Jovian == null)
			{
				Jovian = GetComponent<SgtJovian>();
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
