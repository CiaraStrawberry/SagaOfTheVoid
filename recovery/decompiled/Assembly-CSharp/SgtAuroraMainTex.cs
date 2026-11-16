using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Aurora MainTex")]
public class SgtAuroraMainTex : MonoBehaviour
{
	public SgtAurora Aurora;

	[Tooltip("The resolution of the noise samples")]
	public int Width = 256;

	[Tooltip("The resolution of the bottom/middle/top transition")]
	public int Height = 64;

	[Tooltip("The format of this texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The strength of the noise points")]
	[Range(0f, 1f)]
	public float NoiseStrength = 0.5f;

	[Tooltip("The amount of noise points")]
	public int NoisePoints = 10;

	[Tooltip("The random seed used when generating this texture")]
	[SgtSeed]
	public int NoiseSeed;

	[Tooltip("The transition style between the top and middle")]
	public SgtEase.Type TopEase = SgtEase.Type.Smoothstep;

	[Tooltip("The transition strength between the top and middle")]
	public float TopPower = 1f;

	[Tooltip("The point separating the top from bottom")]
	[Range(0f, 1f)]
	public float MiddlePoint = 0.25f;

	[Tooltip("The base color of the aurora starting from the bottom")]
	public Color MiddleColor = Color.green;

	[Tooltip("The transition style between the bottom and top of the aurora")]
	public SgtEase.Type MiddleEase = SgtEase.Type.Exponential;

	[Tooltip("The strength of the color transition between the bottom and top")]
	public float MiddlePower = 4f;

	[Tooltip("The transition style between the bottom and middle")]
	public SgtEase.Type BottomEase = SgtEase.Type.Exponential;

	[Tooltip("The transition strength between the bottom and middle")]
	public float BottomPower = 2f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	private static List<float> noisePoints = new List<float>();

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Textures")]
	public void UpdateTextures()
	{
		if (Width <= 0 || Height <= 0 || NoisePoints <= 0)
		{
			return;
		}
		if (generatedTexture != null && (generatedTexture.width != Width || generatedTexture.height != Height || generatedTexture.format != Format))
		{
			generatedTexture = SgtHelper.Destroy(generatedTexture);
		}
		if (generatedTexture == null)
		{
			generatedTexture = SgtHelper.CreateTempTexture2D("Aurora MainTex (Generated)", Width, Height, Format);
			generatedTexture.wrapMode = TextureWrapMode.Repeat;
			UpdateApply();
		}
		SgtHelper.BeginRandomSeed(NoiseSeed);
		noisePoints.Clear();
		for (int i = 0; i < NoisePoints; i++)
		{
			noisePoints.Add(1f - UnityEngine.Random.Range(0f, NoiseStrength));
		}
		SgtHelper.EndRandomSeed();
		float num = 1f / (float)(Width - 1);
		float num2 = 1f / (float)(Height - 1);
		for (int j = 0; j < Height; j++)
		{
			float v = (float)j * num2;
			for (int k = 0; k < Width; k++)
			{
				float u = (float)k * num;
				WriteTexture(u, v, k, j);
			}
		}
		generatedTexture.Apply();
	}

	private void WriteTexture(float u, float v, int x, int y)
	{
		float num = u * (float)NoisePoints;
		int num2 = (int)num;
		float t = num % 1f;
		float a = noisePoints[num2 % NoisePoints];
		float b = noisePoints[(num2 + 1) % NoisePoints];
		float c = noisePoints[(num2 + 2) % NoisePoints];
		float d = noisePoints[(num2 + 3) % NoisePoints];
		Color middleColor = MiddleColor;
		if (v < MiddlePoint)
		{
			middleColor.a = SgtEase.Evaluate(BottomEase, SgtHelper.Pow(Mathf.InverseLerp(0f, MiddlePoint, v), BottomPower));
		}
		else
		{
			middleColor.a = SgtEase.Evaluate(TopEase, SgtHelper.Pow(Mathf.InverseLerp(1f, MiddlePoint, v), TopPower));
		}
		float num3 = SgtEase.Evaluate(MiddleEase, SgtHelper.Pow(1f - v, MiddlePower));
		middleColor.a *= SgtHelper.HermiteInterpolate(a, b, c, d, t);
		middleColor.r *= num3 * middleColor.a;
		middleColor.g *= num3 * middleColor.a;
		middleColor.b *= num3 * middleColor.a;
		middleColor.a *= 1f - num3;
		generatedTexture.SetPixel(x, y, middleColor);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Aurora != null)
		{
			Aurora.MainTex = generatedTexture;
			Aurora.UpdateMainTex();
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
			if (Aurora == null)
			{
				Aurora = GetComponent<SgtAurora>();
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
