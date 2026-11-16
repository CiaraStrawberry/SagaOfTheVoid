using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Belt Lighting")]
public class SgtBeltLighting : MonoBehaviour
{
	[Tooltip("The belt this texture will be applied to")]
	public SgtBelt Belt;

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
	public float BackStrength;

	[Tooltip("The of the perpendicular scattered light")]
	[Range(0f, 1f)]
	public float BaseStrength;

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
				generatedTexture = SgtHelper.CreateTempTexture2D("Belt Lighting (Generated)", Width, 1, Format);
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
		float t = Mathf.Pow(1f - u, BackPower) * BackStrength;
		float t2 = Mathf.Pow(u, FrontPower);
		float baseStrength = BaseStrength;
		baseStrength = Mathf.Lerp(baseStrength, 1f, t);
		baseStrength = Mathf.Lerp(baseStrength, 1f, t2);
		Color color = new Color(baseStrength, baseStrength, baseStrength, 0f);
		generatedTexture.SetPixel(x, 0, color);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Belt != null)
		{
			Belt.LightingTex = generatedTexture;
			Belt.UpdateLightingTex();
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
			if (Belt == null)
			{
				Belt = GetComponent<SgtBelt>();
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
