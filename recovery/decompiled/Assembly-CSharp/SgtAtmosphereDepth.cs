using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Atmosphere Depth")]
public class SgtAtmosphereDepth : MonoBehaviour
{
	[Tooltip("The atmosphere this texture will be applied to")]
	public SgtAtmosphere Atmosphere;

	[Tooltip("The resolution of the surface/space optical thickness transition in pixels")]
	public int Width = 256;

	[Tooltip("The texture format of the textures")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The horizon color for both textures")]
	public Color HorizonColor = Color.white;

	[Tooltip("The transition style between the surface and horizon")]
	public SgtEase.Type InnerEase = SgtEase.Type.Exponential;

	[Tooltip("The base color of the inner texture")]
	public Color InnerColor = new Color(0.15f, 0.54f, 1f);

	[Tooltip("The strength of the inner texture transition")]
	public float InnerColorPower = 2f;

	[Tooltip("The strength of the inner texture transition")]
	public float InnerAlphaPower = 2f;

	[Tooltip("The transition style between the sky and horizon")]
	public SgtEase.Type OuterEase = SgtEase.Type.Quadratic;

	[Tooltip("The base color of the outer texture")]
	public Color OuterColor = new Color(0.29f, 0.73f, 1f);

	[Tooltip("The strength of the outer texture transition")]
	public float OuterColorPower = 1f;

	[Tooltip("The strength of the outer texture transition")]
	public float OuterAlphaPower = 3.7f;

	[NonSerialized]
	private Texture2D generatedInnerTexture;

	[NonSerialized]
	private Texture2D generatedOuterTexture;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	public Texture2D GeneratedInnerTexture => generatedInnerTexture;

	public Texture2D GeneratedOuterTexture => generatedOuterTexture;

	[ContextMenu("Update Textures")]
	public void UpdateTextures()
	{
		if (Width > 0)
		{
			ValidateTexture(ref generatedInnerTexture, "Inner Depth (Generated)");
			ValidateTexture(ref generatedOuterTexture, "Outer Depth (Generated)");
			Color clear = Color.clear;
			float num = 1f / (float)(Width - 1);
			for (int i = 0; i < Width; i++)
			{
				float u = (float)i * num;
				WriteTexture(generatedInnerTexture, u, i, InnerColor, InnerEase, InnerColorPower, InnerAlphaPower);
				WriteTexture(generatedOuterTexture, u, i, OuterColor, OuterEase, OuterColorPower, OuterAlphaPower);
			}
			generatedInnerTexture.Apply();
			generatedOuterTexture.Apply();
		}
	}

	private void ValidateTexture(ref Texture2D texture2D, string createName)
	{
		if (texture2D != null && (texture2D.width != Width || texture2D.height != 1 || texture2D.format != Format))
		{
			texture2D = SgtHelper.Destroy(texture2D);
		}
		if (texture2D == null)
		{
			texture2D = SgtHelper.CreateTempTexture2D(createName, Width, 1, Format);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			UpdateApply();
		}
	}

	private void WriteTexture(Texture2D texture2D, float u, int x, Color baseColor, SgtEase.Type ease, float colorPower, float alphaPower)
	{
		float t = SgtHelper.Pow(u, colorPower);
		t = SgtEase.Evaluate(ease, t);
		float t2 = SgtHelper.Pow(u, alphaPower);
		t2 = SgtEase.Evaluate(ease, t2);
		Color color = Color.Lerp(baseColor, HorizonColor, t);
		color.a = t2;
		texture2D.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Atmosphere != null)
		{
			if (generatedInnerTexture != null && Atmosphere.InnerDepthTex != generatedInnerTexture)
			{
				Atmosphere.InnerDepthTex = generatedInnerTexture;
				Atmosphere.UpdateDepthTex();
			}
			if (generatedOuterTexture != null && Atmosphere.OuterDepthTex != generatedOuterTexture)
			{
				Atmosphere.OuterDepthTex = generatedOuterTexture;
				Atmosphere.UpdateDepthTex();
			}
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
		SgtHelper.Destroy(generatedInnerTexture);
		SgtHelper.Destroy(generatedOuterTexture);
	}

	private void CheckUpdateCalls()
	{
		if (generatedInnerTexture == null || generatedOuterTexture == null)
		{
			UpdateTextures();
		}
		UpdateApply();
	}
}
