using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring Lighting")]
public class SgtRingLighting : MonoBehaviour
{
	[Tooltip("The ring this texture will be applied to")]
	public SgtRing Ring;

	[Tooltip("The resolution of the light/dark transition in pixels")]
	public int Width = 256;

	[Tooltip("The format of the generated texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("How sharp the incoming light scatters forward")]
	public float FrontPower = 2f;

	[Tooltip("How sharp the incoming light scatters backward")]
	public float BackPower = 3f;

	[Tooltip("The strength of the back scattered light")]
	[Range(0f, 1f)]
	public float BackStrength = 0.5f;

	[Tooltip("The of the perpendicular scattered light")]
	[Range(0f, 1f)]
	public float BaseStrength = 0.2f;

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
				generatedTexture = SgtHelper.CreateTempTexture2D("Ring Lighting (Generated)", Width, 1, Format);
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
		float t = Mathf.Pow(u, BackPower) * BackStrength;
		float t2 = Mathf.Pow(1f - u, FrontPower);
		float baseStrength = BaseStrength;
		baseStrength = Mathf.Lerp(baseStrength, 1f, t);
		baseStrength = Mathf.Lerp(baseStrength, 1f, t2);
		Color color = new Color(baseStrength, baseStrength, baseStrength, 0f);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Ring != null && generatedTexture != null && Ring.LightingTex != generatedTexture)
		{
			Ring.LightingTex = generatedTexture;
			Ring.UpdateLightingTex();
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
