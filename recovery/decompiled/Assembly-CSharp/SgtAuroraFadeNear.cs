using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Aurora Fade Near")]
public class SgtAuroraFadeNear : MonoBehaviour
{
	[Tooltip("The aurora the generated texture gets applied to")]
	public SgtAurora Aurora;

	[Tooltip("The resolution of the fade transition")]
	public int Width = 256;

	[Tooltip("The format of the generated texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The transition style")]
	public SgtEase.Type Ease = SgtEase.Type.Smoothstep;

	[Tooltip("The sharpness of the transition")]
	public float Power = 2f;

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
				generatedTexture = SgtHelper.CreateTempTexture2D("Aurora Fade Near (Generated)", Width, 1, Format);
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
		float num = 1f - SgtEase.Evaluate(Ease, 1f - SgtHelper.Pow(u, Power));
		Color color = new Color(num, num, num, num);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Aurora != null)
		{
			Aurora.FadeNearTex = generatedTexture;
			Aurora.UpdateFadeNearTex();
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
