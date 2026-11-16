using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Flare Material")]
public class SgtFlareMaterial : MonoBehaviour
{
	public enum ZTestState
	{
		LEqual = 2,
		Always = 6
	}

	[Tooltip("The flare this material will be applied to")]
	public SgtFlare Flare;

	[Tooltip("The ZTest mode of the material (Always = draw on top)")]
	public ZTestState ZTest = ZTestState.LEqual;

	[Tooltip("The render queue group for this flare")]
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	[Tooltip("The render queue offset for this flare")]
	public int RenderQueueOffset;

	[Tooltip("The format of this texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The width of this texture")]
	public int Width = 256;

	[Tooltip("The base color of the texture")]
	public Color Color = Color.white;

	[Tooltip("The color transition style")]
	public SgtEase.Type Ease = SgtEase.Type.Exponential;

	[Tooltip("The sharpness of the red transition")]
	public float PowerR = 3f;

	[Tooltip("The sharpness of the green transition")]
	public float PowerG = 2f;

	[Tooltip("The sharpness of the blue transition")]
	public float PowerB = 1f;

	[NonSerialized]
	private Material generatedMaterial;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	private bool startCalled;

	public Material GeneratedMaterial => generatedMaterial;

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Material")]
	public void UpdateMaterial()
	{
		if (generatedMaterial == null)
		{
			generatedMaterial = SgtHelper.CreateTempMaterial("Flare Material (Generated)", "Space Graphics Toolkit/SgtFlare");
			UpdateApply();
		}
		generatedMaterial.SetTexture("_MainTex", generatedTexture);
		generatedMaterial.renderQueue = (int)(RenderQueue + RenderQueueOffset);
		generatedMaterial.SetInt("_ZTest", (int)ZTest);
	}

	[ContextMenu("Update Texture")]
	public void UpdateTexture()
	{
		if (Width <= 0)
		{
			return;
		}
		if (generatedTexture != null && (generatedTexture.width != Width || generatedTexture.height != 1 || generatedTexture.format != Format))
		{
			generatedTexture = SgtHelper.Destroy(generatedTexture);
		}
		if (generatedTexture == null)
		{
			generatedTexture = SgtHelper.CreateTempTexture2D("Flare Texture (Generated)", Width, 1, Format);
			generatedTexture.wrapMode = TextureWrapMode.Clamp;
			if (generatedMaterial != null)
			{
				generatedMaterial.SetTexture("_MainTex", generatedTexture);
			}
		}
		float num = 1f / (float)(Width - 1);
		for (int i = 0; i < Width; i++)
		{
			float u = (float)i * num;
			WriteTexture(u, i);
		}
		generatedTexture.Apply();
	}

	private void WriteTexture(float u, int x)
	{
		Color color = Color;
		color.r *= 1f - SgtEase.Evaluate(Ease, 1f - Mathf.Pow(1f - u, PowerR));
		color.g *= 1f - SgtEase.Evaluate(Ease, 1f - Mathf.Pow(1f - u, PowerG));
		color.b *= 1f - SgtEase.Evaluate(Ease, 1f - Mathf.Pow(1f - u, PowerB));
		color.a = color.grayscale;
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Flare == null)
		{
			Flare = GetComponent<SgtFlare>();
		}
		if (Flare != null)
		{
			Flare.Material = generatedMaterial;
			Flare.UpdateMaterial();
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
			CheckUpdateCalls();
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(generatedMaterial);
		SgtHelper.Destroy(generatedTexture);
	}

	private void CheckUpdateCalls()
	{
		if (generatedMaterial == null)
		{
			UpdateMaterial();
		}
		if (generatedTexture == null)
		{
			UpdateTexture();
		}
	}
}
