using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring Fade")]
public class SgtRingFade : MonoBehaviour
{
	[Tooltip("The ring this fade texture gets applied to")]
	public SgtRing Ring;

	[Tooltip("The resolution of the ring fade transition")]
	public int Width = 256;

	[Tooltip("The texture format of the textures")]
	public TextureFormat Format = TextureFormat.Alpha8;

	public SgtEase.Type Ease = SgtEase.Type.Smoothstep;

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
				generatedTexture = SgtHelper.CreateTempTexture2D("Ring Fade (Generated)", Width, 1, Format);
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
		float a = 1f - SgtEase.Evaluate(Ease, 1f - Mathf.Pow(u, Power));
		Color color = new Color(1f, 1f, 1f, a);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Ring != null && generatedTexture != null && Ring.FadeTex != generatedTexture)
		{
			Ring.FadeTex = generatedTexture;
			Ring.UpdateFadeTex();
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
			if (Ring == null)
			{
				Ring = GetComponent<SgtRing>();
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
