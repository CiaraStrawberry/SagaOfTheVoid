using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Heightmap")]
public class SgtTerrainHeightmap : SgtTerrainModifier
{
	public enum EncodingType
	{
		Alpha,
		RedGreen
	}

	[Tooltip("The heightmap texture using a cylindrical (equirectangular) projection")]
	public Texture2D Heightmap;

	[Tooltip("The way the height data is stored in the texture")]
	public EncodingType Encoding;

	[Tooltip("The height displacement represented by alpha = 0")]
	public float DisplacementMin;

	[Tooltip("The height displacement represented by alpha = 255")]
	public float DisplacementMax = 0.1f;

	protected override void OnEnable()
	{
		base.OnEnable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateHeight = (SgtTerrain.CalculateHeightDelegate)Delegate.Combine(sgtTerrain.OnCalculateHeight, new SgtTerrain.CalculateHeightDelegate(CalculateHeight));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateHeight = (SgtTerrain.CalculateHeightDelegate)Delegate.Remove(sgtTerrain.OnCalculateHeight, new SgtTerrain.CalculateHeightDelegate(CalculateHeight));
	}

	private void CalculateHeight(SgtVector3D localPosition, ref float height)
	{
		if (Heightmap != null)
		{
			Vector2 uv = SgtHelper.CartesianToPolarUV((Vector3)localPosition);
			Color color = SampleBilinear(uv);
			switch (Encoding)
			{
			case EncodingType.Alpha:
				height += Mathf.Lerp(DisplacementMin, DisplacementMax, color.a);
				break;
			case EncodingType.RedGreen:
				height += Mathf.Lerp(DisplacementMin, DisplacementMax, (color.r * 255f + color.g) / 256f);
				break;
			}
		}
	}

	private Color SampleBilinear(Vector2 uv)
	{
		return Heightmap.GetPixelBilinear(uv.x, uv.y);
	}
}
