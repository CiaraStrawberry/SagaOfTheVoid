using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Sphere Shadow")]
public class SgtSphereShadow : SgtShadow
{
	[Tooltip("The resolution of the surface/space optical thickness transition in pixels")]
	public int Width = 256;

	[Tooltip("The format of this texture")]
	public TextureFormat Format = TextureFormat.ARGB32;

	[Tooltip("The power of the sunset red channel transition")]
	public float PowerR = 2f;

	[Tooltip("The power of the sunset green channel transition")]
	public float PowerG = 2f;

	[Tooltip("The power of the sunset blue channel transition")]
	public float PowerB = 2f;

	[Tooltip("The opacity shadow")]
	public float Opacity = 1f;

	[Tooltip("The inner radius of the sphere in local coordinates")]
	[FormerlySerializedAs("InnerRadius")]
	public float RadiusMin = 1f;

	[Tooltip("The outer radius of the sphere in local coordinates")]
	[FormerlySerializedAs("OuterRadius")]
	public float RadiusMax = 2f;

	[NonSerialized]
	private Texture2D generatedTexture;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	public Texture2D GeneratedTexture => generatedTexture;

	public override Texture GetTexture()
	{
		if (!generatedTexture)
		{
			UpdateTexture();
		}
		return generatedTexture;
	}

	[ContextMenu("Update Textures")]
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
				generatedTexture = SgtHelper.CreateTempTexture2D("Sphere Shadow (Generated)", Width, 1, Format);
				generatedTexture.wrapMode = TextureWrapMode.Clamp;
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
		Color color = new Color
		{
			r = 1f - Mathf.Pow(1f - u, PowerR) * Opacity,
			g = 1f - Mathf.Pow(1f - u, PowerG) * Opacity,
			b = 1f - Mathf.Pow(1f - u, PowerB) * Opacity,
			a = 1f
		};
		generatedTexture.SetPixel(x, 0, color);
	}

	public override bool CalculateShadow(ref Matrix4x4 matrix, ref float ratio)
	{
		if (base.CalculateShadow(ref matrix, ref ratio))
		{
			Vector3 direction = default;
			Vector3 position = default;
			Color color = default;
			SgtHelper.CalculateLight(Light, transform.position, null, null, ref position, ref direction, ref color);
			float num = Vector3.Dot(direction, transform.up);
			float num2 = (transform.lossyScale.x + transform.lossyScale.z) * 0.5f * RadiusMax;
			float a = transform.lossyScale.y * RadiusMax;
			float radius = GetRadius(a, num2, num * (float)Math.PI * 0.5f);
			Quaternion quaternion = Quaternion.FromToRotation(direction, Vector3.back);
			Vector3 vector = quaternion * transform.up;
			Quaternion quaternion2 = Quaternion.LookRotation(Vector3.forward, new Vector2(0f - vector.x, vector.y));
			Vector3 xyz = SgtHelper.Reciprocal3(new Vector3(num2, radius, 1f));
			Matrix4x4 matrix4x = SgtHelper.Translation(-transform.position);
			Matrix4x4 matrix4x2 = SgtHelper.Rotation(quaternion2 * quaternion);
			Matrix4x4 matrix4x3 = SgtHelper.Scaling(xyz);
			matrix = matrix4x3 * matrix4x2 * matrix4x;
			ratio = SgtHelper.Divide(RadiusMax, RadiusMax - RadiusMin);
			return true;
		}
		return false;
	}

	private float GetRadius(float a, float b, float theta)
	{
		float num = Mathf.Sin(theta);
		float num2 = Mathf.Cos(theta);
		float num3 = Mathf.Sqrt(a * a * (num * num) + b * b * (num2 * num2));
		if (num3 != 0f)
		{
			return a * b / num3;
		}
		return a;
	}

	protected virtual void OnEnable()
	{
		if (startCalled)
		{
			CheckUpdateCalls();
		}
	}

	protected override void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
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
	}
}
