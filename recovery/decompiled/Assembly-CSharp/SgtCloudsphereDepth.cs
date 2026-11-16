using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Cloudsphere Depth")]
public class SgtCloudsphereDepth : MonoBehaviour
{
	[Tooltip("The cloudsphere this texture will be applied to")]
	public SgtCloudsphere Cloudsphere;

	[Tooltip("The resolution of the surface/space optical thickness transition in pixels")]
	public int Width = 256;

	[Tooltip("The format of this texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The rim transition style")]
	public SgtEase.Type RimEase = SgtEase.Type.Exponential;

	[Tooltip("The rim color")]
	public Color RimColor = new Color(1f, 0f, 0f, 0.25f);

	[Tooltip("The rim transition sharpness")]
	public float RimPower = 5f;

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
				generatedTexture = SgtHelper.CreateTempTexture2D("Cloudsphere Depth (Generated)", Width, 1, Format);
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
		float num = SgtEase.Evaluate(RimEase, Mathf.Pow(1f - u, RimPower));
		Color color = Color.Lerp(Color.white, RimColor, num * RimColor.a);
		color.a = 1f - Mathf.Pow(1f - Mathf.Pow(u, AlphaFade), AlphaDensity);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Cloudsphere != null && generatedTexture != null && Cloudsphere.DepthTex != generatedTexture)
		{
			Cloudsphere.DepthTex = generatedTexture;
			Cloudsphere.UpdateDeptchTex();
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
			if (Cloudsphere == null)
			{
				Cloudsphere = GetComponent<SgtCloudsphere>();
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
