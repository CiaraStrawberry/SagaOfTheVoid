using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtTerrainFace : MonoBehaviour
{
	[Tooltip("The material applied to all renderers on this face")]
	public Material Material;

	[Tooltip("The terrain this belongs to")]
	public SgtTerrain Terrain;

	[Tooltip("The index of this face")]
	public CubemapFace Side;

	public List<SgtTerrainLevel> Levels;

	[NonSerialized]
	public SgtTerrainLevel LastLevel;

	private static SgtVector3D negativeX = new SgtVector3D(-1.0, 0.0, 0.0);

	private static SgtVector3D negativeY = new SgtVector3D(0.0, -1.0, 0.0);

	private static SgtVector3D negativeZ = new SgtVector3D(0.0, 0.0, -1.0);

	private static SgtVector3D positiveX = new SgtVector3D(1.0, 0.0, 0.0);

	private static SgtVector3D positiveY = new SgtVector3D(0.0, 1.0, 0.0);

	private static SgtVector3D positiveZ = new SgtVector3D(0.0, 0.0, 1.0);

	private static SgtTerrain.Ring ring = new SgtTerrain.Ring();

	public void Mark()
	{
		LastLevel = null;
		if (Levels == null)
		{
			Levels = new List<SgtTerrainLevel>();
		}
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel != null)
			{
				sgtTerrainLevel.Marked = true;
			}
			else
			{
				Levels.RemoveAt(num);
			}
		}
	}

	public SgtTerrainLevel GetLevel(int index)
	{
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel.Index == index)
			{
				sgtTerrainLevel.Marked = false;
				return sgtTerrainLevel;
			}
		}
		SgtTerrainLevel sgtTerrainLevel2 = SgtTerrainLevel.Create("Level " + index, gameObject.layer, transform);
		sgtTerrainLevel2.Marked = false;
		sgtTerrainLevel2.Index = index;
		sgtTerrainLevel2.Face = this;
		sgtTerrainLevel2.UpdateMaterials();
		Levels.Add(sgtTerrainLevel2);
		return sgtTerrainLevel2;
	}

	public void UpdateMaterials()
	{
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel != null)
			{
				sgtTerrainLevel.UpdateMaterials();
			}
		}
	}

	public void Dirty()
	{
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel != null)
			{
				sgtTerrainLevel.Dirty();
			}
		}
	}

	public void Sweep()
	{
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel.Marked)
			{
				if (Terrain != null && Terrain.OnCalculateLevel != null)
				{
					Terrain.OnCalculateLevel(sgtTerrainLevel, null);
				}
				SgtTerrainLevel.MarkForDestruction(sgtTerrainLevel);
				Levels.RemoveAt(num);
			}
		}
	}

	public void UpdateBounds(Bounds bounds)
	{
		for (int num = Levels.Count - 1; num >= 0; num--)
		{
			SgtTerrainLevel sgtTerrainLevel = Levels[num];
			if (sgtTerrainLevel != null && sgtTerrainLevel.Mesh != null)
			{
				sgtTerrainLevel.Mesh.bounds = bounds;
			}
		}
	}

	public void Write(SgtBoundsL bounds, SgtTerrain.Shell shell, int side, int index)
	{
		ring.Limit = shell.Limit;
		ring.Detail = shell.Detail;
		switch (side)
		{
		case 0:
			if (bounds.minX >= shell.Outer.minX && bounds.maxX <= shell.Outer.maxX)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectZY;
				ring.Inner = ((!shell.Inner.IsInsideX(bounds.minX)) ? default(SgtRectL) : shell.Inner.RectZY);
				ring.Center = negativeX;
				ring.ExtentsX = negativeZ;
				ring.ExtentsY = positiveY;
				ring.Outer.SwapX();
				ring.Inner.SwapX();
				GetLevel(index).Write(ring);
			}
			break;
		case 3:
			if (bounds.minX >= shell.Outer.minX && bounds.maxX <= shell.Outer.maxX)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectZY;
				ring.Inner = ((!shell.Inner.IsInsideX(bounds.maxX - 1)) ? default(SgtRectL) : shell.Inner.RectZY);
				ring.Center = positiveX;
				ring.ExtentsX = positiveZ;
				ring.ExtentsY = positiveY;
				GetLevel(index).Write(ring);
			}
			break;
		case 1:
			if (bounds.minY >= shell.Outer.minY && bounds.maxY <= shell.Outer.maxY)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectXZ;
				ring.Inner = ((!shell.Inner.IsInsideY(bounds.minY)) ? default(SgtRectL) : shell.Inner.RectXZ);
				ring.Center = negativeY;
				ring.ExtentsX = negativeX;
				ring.ExtentsY = positiveZ;
				ring.Outer.SwapX();
				ring.Inner.SwapX();
				GetLevel(index).Write(ring);
			}
			break;
		case 4:
			if (bounds.minY >= shell.Outer.minY && bounds.maxY <= shell.Outer.maxY)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectXZ;
				ring.Inner = ((!shell.Inner.IsInsideY(bounds.maxY - 1)) ? default(SgtRectL) : shell.Inner.RectXZ);
				ring.Center = positiveY;
				ring.ExtentsX = negativeX;
				ring.ExtentsY = negativeZ;
				ring.Outer.SwapX();
				ring.Outer.SwapY();
				ring.Inner.SwapX();
				ring.Inner.SwapY();
				GetLevel(index).Write(ring);
			}
			break;
		case 2:
			if (bounds.minZ >= shell.Outer.minZ && bounds.maxZ <= shell.Outer.maxZ)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectXY;
				ring.Inner = ((!shell.Inner.IsInsideZ(bounds.minZ)) ? default(SgtRectL) : shell.Inner.RectXY);
				ring.Center = negativeZ;
				ring.ExtentsX = positiveX;
				ring.ExtentsY = positiveY;
				GetLevel(index).Write(ring);
			}
			break;
		case 5:
			if (bounds.minZ >= shell.Outer.minZ && bounds.maxZ <= shell.Outer.maxZ)
			{
				bounds.ClampTo(shell.Outer);
				ring.Outer = bounds.RectXY;
				ring.Inner = ((!shell.Inner.IsInsideZ(bounds.maxZ - 1)) ? default(SgtRectL) : shell.Inner.RectXY);
				ring.Center = positiveZ;
				ring.ExtentsX = negativeX;
				ring.ExtentsY = positiveY;
				ring.Outer.SwapX();
				ring.Inner.SwapX();
				GetLevel(index).Write(ring);
			}
			break;
		}
	}

	public static SgtTerrainFace Create(string name, int layer, Transform parent)
	{
		return SgtComponentPool<SgtTerrainFace>.Pop(parent, name, layer);
	}

	public static SgtTerrainFace Pool(SgtTerrainFace face)
	{
		if (face != null)
		{
			face.Terrain = null;
			face.Material = null;
			SgtComponentPool<SgtTerrainFace>.Add(face);
		}
		return null;
	}

	public static SgtTerrainFace MarkForDestruction(SgtTerrainFace face)
	{
		if (face != null)
		{
			face.Terrain = null;
			face.gameObject.SetActive(value: true);
		}
		return null;
	}

	protected virtual void Update()
	{
		if (Terrain == null)
		{
			Pool(this);
		}
	}
}
