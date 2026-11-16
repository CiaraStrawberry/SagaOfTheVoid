using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Space Graphics Toolkit/SGT Displaced Mesh")]
public class SgtDisplacedMesh : MonoBehaviour
{
	[Tooltip("The original mesh we want to displace")]
	public Mesh OriginalMesh;

	[Tooltip("The height map texture used to displace the mesh (Height must be stored in alpha channel)")]
	public Texture2D HeightTex;

	[Tooltip("The mesh radius represented by a 0 alpha value")]
	public float InnerRadius = 0.9f;

	[Tooltip("The mesh radius represented by a 255 alpha value")]
	public float OuterRadius = 1.1f;

	[NonSerialized]
	private Mesh displacedMesh;

	[NonSerialized]
	private MeshFilter meshFilter;

	public float GetSurfaceHeightLocal(Vector3 localPosition)
	{
		if (HeightTex != null)
		{
			Vector2 uv = SgtHelper.CartesianToPolarUV(localPosition);
			float heightTexAlphaBilinear = GetHeightTexAlphaBilinear(uv);
			return Mathf.Lerp(InnerRadius, OuterRadius, heightTexAlphaBilinear);
		}
		return 1f;
	}

	[ContextMenu("Rebuild Mesh")]
	public void RebuildMesh()
	{
		displacedMesh = SgtHelper.Destroy(displacedMesh);
		if (OriginalMesh != null)
		{
			displacedMesh = UnityEngine.Object.Instantiate(OriginalMesh);
			displacedMesh.name = OriginalMesh.name + " (Displaced)";
			Vector3[] vertices = OriginalMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 normalized = vertices[i].normalized;
				ref Vector3 reference = ref vertices[i];
				reference = normalized * GetSurfaceHeightLocal(normalized);
			}
			displacedMesh.vertices = vertices;
			displacedMesh.RecalculateBounds();
		}
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		meshFilter.sharedMesh = displacedMesh;
	}

	protected virtual void OnEnable()
	{
		if (displacedMesh != null)
		{
			meshFilter.sharedMesh = displacedMesh;
		}
	}

	protected virtual void Start()
	{
		if (OriginalMesh == null)
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			OriginalMesh = meshFilter.sharedMesh;
		}
		if (OriginalMesh != null)
		{
			RebuildMesh();
		}
	}

	protected virtual void OnDisable()
	{
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		meshFilter.sharedMesh = OriginalMesh;
	}

	private float GetHeightTexAlphaBilinear(Vector2 uv)
	{
		return HeightTex.GetPixelBilinear(uv.x, uv.y).a;
	}

	private float GetHeightTexAlphaCubic(Vector2 uv)
	{
		FracInt(uv.x * (float)HeightTex.width, out var i, out var f);
		FracInt(uv.y * (float)HeightTex.height, out var i2, out var f2);
		float pixel = GetPixel(i - 1, i2 - 1);
		float pixel2 = GetPixel(i, i2 - 1);
		float pixel3 = GetPixel(i + 1, i2 - 1);
		float pixel4 = GetPixel(i + 2, i2 - 1);
		float pixel5 = GetPixel(i - 1, i2);
		float pixel6 = GetPixel(i, i2);
		float pixel7 = GetPixel(i + 1, i2);
		float pixel8 = GetPixel(i + 2, i2);
		float pixel9 = GetPixel(i - 1, i2 + 1);
		float pixel10 = GetPixel(i, i2 + 1);
		float pixel11 = GetPixel(i + 1, i2 + 1);
		float pixel12 = GetPixel(i + 2, i2 + 1);
		float pixel13 = GetPixel(i - 1, i2 + 2);
		float pixel14 = GetPixel(i, i2 + 2);
		float pixel15 = GetPixel(i + 1, i2 + 2);
		float pixel16 = GetPixel(i + 2, i2 + 2);
		float a = Cubic(pixel, pixel2, pixel3, pixel4, f);
		float b = Cubic(pixel5, pixel6, pixel7, pixel8, f);
		float c = Cubic(pixel9, pixel10, pixel11, pixel12, f);
		float d = Cubic(pixel13, pixel14, pixel15, pixel16, f);
		return Cubic(a, b, c, d, f2);
	}

	private float GetHeightTexAlphaCos(Vector2 uv)
	{
		FracInt(uv.x * (float)HeightTex.width, out var i, out var f);
		FracInt(uv.y * (float)HeightTex.height, out var i2, out var f2);
		int x = RepeatX(i);
		int x2 = RepeatX(i + 1);
		int y = ClampY(i2);
		int y2 = ClampY(i2 + 1);
		float a = HeightTex.GetPixel(x, y).a;
		float a2 = HeightTex.GetPixel(x2, y).a;
		float a3 = HeightTex.GetPixel(x, y2).a;
		float a4 = HeightTex.GetPixel(x2, y2).a;
		float a5 = Coserp(a, a2, f);
		float b = Coserp(a3, a4, f);
		return Coserp(a5, b, f2);
	}

	private int RepeatX(int x)
	{
		if (x < 0)
		{
			return HeightTex.width + x;
		}
		if (x >= HeightTex.width)
		{
			return x - HeightTex.width;
		}
		return x;
	}

	private int ClampY(int y)
	{
		if (y < 0)
		{
			return 0;
		}
		if (y >= HeightTex.height)
		{
			return HeightTex.height - 1;
		}
		return y;
	}

	private float Coserp(float a, float b, float t)
	{
		t = (1f - Mathf.Cos(t * (float)Math.PI)) * 0.5f;
		return a + (b - a) * t;
	}

	public static float Cubic(float a, float b, float c, float d, float t)
	{
		float num = t * t;
		d = d - c - (a - b);
		return d * (num * t) + (a - b - d) * num + (c - a) * t + b;
	}

	private void FracInt(float v, out int i, out float f)
	{
		i = (int)v;
		f = v - (float)i;
	}

	private float GetPixel(int x, int y)
	{
		return HeightTex.GetPixel(RepeatX(x), ClampY(y)).a;
	}
}
