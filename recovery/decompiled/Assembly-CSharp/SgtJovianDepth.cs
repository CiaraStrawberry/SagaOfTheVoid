using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Jovian Depth")]
public class SgtJovianDepth : MonoBehaviour
{
	[Tooltip("The jovian this texture will be applied to")]
	public SgtJovian Jovian;

	[Tooltip("The resolution of the surface/space optical thickness transition in pixels")]
	public int Width = 256;

	[Tooltip("The format of this texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The rim transition style")]
	public SgtEase.Type RimEase = SgtEase.Type.Exponential;

	[Tooltip("The rim transition sharpness")]
	public float RimPower = 5f;

	[Tooltip("The rim color")]
	public Color RimColor = new Color(1f, 0f, 0f, 0.25f);

	[Tooltip("The density of the atmosphere")]
	public float AlphaDensity = 50f;

	[Tooltip("The strength of the density fading in the upper atmosphere")]
	public float AlphaFade = 2f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	[HideInInspector]
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
				generatedTexture = SgtHelper.CreateTempTexture2D("Jovian Depth (Generated)", Width, 1, Format);
				generatedTexture.wrapMode = TextureWrapMode.Clamp;
				UpdateApply();
			}
			Color clear = Color.clear;
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
		float num = 1f - SgtEase.Evaluate(RimEase, 1f - Mathf.Pow(1f - u, RimPower));
		Color color = Color.Lerp(Color.white, RimColor, num * RimColor.a);
		color.a = 1f - Mathf.Pow(1f - Mathf.Pow(u, AlphaFade), AlphaDensity);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Jovian != null)
		{
			Jovian.DepthTex = generatedTexture;
			Jovian.UpdateMaterial();
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
			UpdateTextures();
		}
		UpdateApply();
	}
}
