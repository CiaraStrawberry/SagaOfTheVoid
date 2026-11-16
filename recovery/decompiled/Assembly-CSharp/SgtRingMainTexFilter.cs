using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring Main Tex Filter")]
public class SgtRingMainTexFilter : MonoBehaviour
{
	[Tooltip("The ring this texture will be applied to")]
	public SgtRing Ring;

	[Tooltip("The source ring texture that will be filtered")]
	public Texture2D Source;

	[Tooltip("The format of the generated texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The sharpness of the light/dark transition")]
	public float Power = 0.5f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	public Texture2D GeneratedTexture => generatedTexture;

	[ContextMenu("Update Texture")]
	public void UpdateTexture()
	{
		if (Source != null)
		{
			if (generatedTexture != null && (generatedTexture.width != Source.width || generatedTexture.height != 1 || generatedTexture.format != Format))
			{
				generatedTexture = SgtHelper.Destroy(generatedTexture);
			}
			if (generatedTexture == null)
			{
				generatedTexture = SgtHelper.CreateTempTexture2D("Ring MainTex (Generated)", Source.width, 1, Format);
				generatedTexture.wrapMode = TextureWrapMode.Clamp;
				UpdateApply();
			}
			for (int num = Source.width - 1; num >= 0; num--)
			{
				WriteTexture(num);
			}
			generatedTexture.Apply();
		}
	}

	private void WriteTexture(int x)
	{
		Color pixel = Source.GetPixel(x, 0);
		float num = 0f;
		if (pixel.r > num)
		{
			num = pixel.r;
		}
		if (pixel.g > num)
		{
			num = pixel.g;
		}
		if (pixel.b > num)
		{
			num = pixel.b;
		}
		if (num > 0f)
		{
			num = 1f - Mathf.Pow(1f - num, Power);
			pixel.a = num;
		}
		else
		{
			pixel.a = 0f;
		}
		generatedTexture.SetPixel(x, 0, pixel);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Ring != null)
		{
			Ring.MainTex = generatedTexture;
			Ring.UpdateMainTex();
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
			UpdateTexture();
		}
		UpdateApply();
	}
}
