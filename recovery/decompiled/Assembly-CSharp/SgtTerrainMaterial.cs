using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Material")]
public class SgtTerrainMaterial : SgtTerrainModifier
{
	[Tooltip("The material that will be assigned")]
	public Material Material;

	[Tooltip("Apply this material to all sides?")]
	public bool AllSides = true;

	[Tooltip("The side this material will be applied to")]
	public CubemapFace RequiredSide;

	[Tooltip("The minimum LOD level this material will be applied to")]
	public int LevelMin = 5;

	[Tooltip("The maximum LOD level this material will be applied to")]
	public int LevelMax = 5;

	protected override void OnEnable()
	{
		base.OnEnable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateMaterial = (SgtTerrain.CalculateMaterialDelegate)Delegate.Combine(sgtTerrain.OnCalculateMaterial, new SgtTerrain.CalculateMaterialDelegate(CalculateMaterial));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SgtTerrain sgtTerrain = terrain;
		sgtTerrain.OnCalculateMaterial = (SgtTerrain.CalculateMaterialDelegate)Delegate.Remove(sgtTerrain.OnCalculateMaterial, new SgtTerrain.CalculateMaterialDelegate(CalculateMaterial));
	}

	private void CalculateMaterial(SgtTerrainLevel level, ref Material material)
	{
		if (level.Index >= LevelMin && level.Index <= LevelMax && (AllSides || level.Face.Side == RequiredSide))
		{
			material = Material;
		}
	}
}
