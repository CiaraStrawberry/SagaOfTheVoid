using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Ridged Simplex")]
public class SgtTerrainRidgedSimplex : SgtTerrainModifier
{
	[Tooltip("The density/frequency/tiling of the displacement")]
	public float Density = 10f;

	[Tooltip("The +- strength of the displacement")]
	public float Strength = 0.5f;

	[Tooltip("The detail of the simplex noise")]
	[Range(1f, 20f)]
	public int Octaves = 5;

	[Tooltip("Invert the ridges?")]
	public bool Invert;

	[Tooltip("The random seed used for the simplex noise")]
	[SgtSeed]
	public int Seed;

	[NonSerialized]
	private SgtSimplex[] generators;

	[NonSerialized]
	private float scale = 1f;

	public void UpdateNoise()
	{
		if (generators == null || generators.Length != Octaves)
		{
			generators = new SgtSimplex[Octaves];
		}
		float num = 1f;
		float num2 = 0f;
		for (int i = 0; i < Octaves; i++)
		{
			SgtSimplex sgtSimplex = generators[i];
			if (sgtSimplex == null)
			{
				sgtSimplex = (generators[i] = new SgtSimplex());
			}
			sgtSimplex.SetSeed(Seed + i * 999);
			num2 += num;
			num *= 0.5f;
		}
		if (num2 > 0f)
		{
			scale = 1f / num2;
		}
		else
		{
			scale = 1f;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateHeight = (SgtTerrain.CalculateHeightDelegate)Delegate.Combine(sgtTerrain.OnCalculateHeight, new SgtTerrain.CalculateHeightDelegate(CalculateHeight));
		UpdateNoise();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateHeight = (SgtTerrain.CalculateHeightDelegate)Delegate.Remove(sgtTerrain.OnCalculateHeight, new SgtTerrain.CalculateHeightDelegate(CalculateHeight));
	}

	private void CalculateHeight(SgtVector3D localPosition, ref float height)
	{
		localPosition /= localPosition.magnitude;
		localPosition *= (double)Density;
		float num = 1f;
		float num2 = 0f;
		for (int i = 0; i < Octaves; i++)
		{
			num2 += generators[i].Generate((float)localPosition.x, (float)localPosition.y, (float)localPosition.z) * num;
			num *= 0.5f;
			localPosition *= 2.0;
		}
		num2 *= scale * 2f;
		num2 = Math.Abs(num2);
		if (Invert)
		{
			num2 = 2f - num2;
		}
		num2--;
		height += num2 * Strength;
	}
}
