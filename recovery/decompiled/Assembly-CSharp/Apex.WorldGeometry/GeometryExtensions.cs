using System;
using System.Collections.Generic;
using System.Linq;
using Apex.Common;
using Apex.DataStructures;
using Apex.Services;
using Apex.Units;
using Apex.Utilities;
using UnityEngine;

namespace Apex.WorldGeometry;

public static class GeometryExtensions
{
	public static Cell GetNearestWalkableCell(this IGrid grid, Vector3 position, Vector3 inRelationTo, bool requireWalkableFromPosition, int maxCellDistance, IUnitProperties unitProps)
	{
		Cell cell = grid.GetCell(position);
		if (cell == null)
		{
			return null;
		}
		if (cell.IsWalkableWithClearance(unitProps))
		{
			return cell;
		}
		int num = 1;
		List<Cell> list = new List<Cell>();
		while (list.Count == 0 && num <= maxCellDistance)
		{
			foreach (Cell concentricNeighbour in grid.GetConcentricNeighbours(cell, num++))
			{
				if (requireWalkableFromPosition)
				{
					if (concentricNeighbour.IsWalkableFromWithClearance(cell, unitProps))
					{
						list.Add(concentricNeighbour);
					}
				}
				else if (concentricNeighbour.IsWalkableWithClearance(unitProps))
				{
					list.Add(concentricNeighbour);
				}
			}
		}
		Cell result = null;
		float num2 = float.MaxValue;
		for (int i = 0; i < list.Count; i++)
		{
			float sqrMagnitude = (list[i].position - inRelationTo).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				result = list[i];
				num2 = sqrMagnitude;
			}
		}
		return result;
	}

	public static Cell GetNearestWalkableCellFromRelation(this IGrid grid, Vector3 position, Vector3 inRelationTo, bool requireWalkableFromPosition, int maxCellDistance, IUnitProperties unitProps)
	{
		Cell cell = grid.GetCell(position);
		if (cell == null)
		{
			return null;
		}
		if (cell.IsWalkableWithClearance(unitProps))
		{
			return cell;
		}
		Cell cell2 = grid.GetCell(inRelationTo);
		if (cell2 == null)
		{
			return null;
		}
		int num = 1;
		List<Cell> list = new List<Cell>();
		while (list.Count == 0 && num <= maxCellDistance)
		{
			foreach (Cell concentricNeighbour in grid.GetConcentricNeighbours(cell, num++))
			{
				if (requireWalkableFromPosition)
				{
					if (concentricNeighbour.IsWalkableFromWithClearance(cell2, unitProps))
					{
						list.Add(concentricNeighbour);
					}
				}
				else if (concentricNeighbour.IsWalkableWithClearance(unitProps))
				{
					list.Add(concentricNeighbour);
				}
			}
		}
		Cell result = null;
		float num2 = float.MaxValue;
		for (int i = 0; i < list.Count; i++)
		{
			float sqrMagnitude = (list[i].position - inRelationTo).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				result = list[i];
				num2 = sqrMagnitude;
			}
		}
		return result;
	}

	public static IEnumerable<Cell> GetWalkableNeighbours(this IGrid grid, IGridCell c, IUnitProperties unitProps, bool excludeCornerCutting)
	{
		bool uw = grid.TryGetWalkableNeighbour(c, 0, 1, unitProps, out var n);
		if (uw)
		{
			yield return n;
		}
		bool dw = grid.TryGetWalkableNeighbour(c, 0, -1, unitProps, out n);
		if (dw)
		{
			yield return n;
		}
		bool rw = grid.TryGetWalkableNeighbour(c, 1, 0, unitProps, out n);
		if (rw)
		{
			yield return n;
		}
		bool lw = grid.TryGetWalkableNeighbour(c, -1, 0, unitProps, out n);
		if (lw)
		{
			yield return n;
		}
		bool urw;
		bool drw;
		bool dlw;
		bool ulw;
		if (excludeCornerCutting)
		{
			urw = uw && rw;
			drw = dw && rw;
			dlw = dw && lw;
			ulw = uw && lw;
		}
		else
		{
			urw = uw || rw;
			drw = dw || rw;
			dlw = dw || lw;
			ulw = uw || lw;
		}
		if (urw && grid.TryGetWalkableNeighbour(c, 1, 1, unitProps, out n))
		{
			yield return n;
		}
		if (drw && grid.TryGetWalkableNeighbour(c, 1, -1, unitProps, out n))
		{
			yield return n;
		}
		if (dlw && grid.TryGetWalkableNeighbour(c, -1, -1, unitProps, out n))
		{
			yield return n;
		}
		if (ulw && grid.TryGetWalkableNeighbour(c, -1, 1, unitProps, out n))
		{
			yield return n;
		}
	}

	public static bool IsWalkableToAll(this IGridCell cell)
	{
		return cell.IsWalkable(AttributeMask.None);
	}

	public static bool IsWalkableToAny(this IGridCell cell)
	{
		return cell.IsWalkable(AttributeMask.All);
	}

	public static float SampleHeight(this IGrid grid, Vector3 position)
	{
		Ensure.ArgumentNotNull(grid, "grid");
		return GameServices.heightStrategy.heightSampler.SampleHeight(position, grid.cellMatrix);
	}

	public static bool TrySampleHeight(this IGrid grid, Vector3 position, out float height)
	{
		Ensure.ArgumentNotNull(grid, "grid");
		return GameServices.heightStrategy.heightSampler.TrySampleHeight(position, grid.cellMatrix, out height);
	}

	public static void Update(this Bounds extent, int maxMillisecondsUsedPerFrame, bool blockWhileUpdating, Action callback)
	{
		GridManager.instance.Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating, callback);
	}

	public static GridComponent GetNeighbourGrid(this IGrid grid, DirectionVector direction)
	{
		Bounds bounds = grid.bounds;
		Vector3 pos = bounds.center + (bounds.extents + Vector3.one) * direction;
		return GridManager.instance.GetGridComponent(pos);
	}

	public static GridComponent GetNeighbourGrid(this GridComponent grid, DirectionVector direction)
	{
		Bounds bounds = grid.bounds;
		Vector3 pos = bounds.center + (bounds.extents + Vector3.one) * direction;
		return GridManager.instance.GetGridComponent(pos);
	}

	public static GridComponent GetNeighbourGrid(this IGridManager mgr, Vector3 position, DirectionVector direction)
	{
		GridComponent gridComponent = mgr.GetGridComponent(position);
		Bounds bounds = gridComponent.bounds;
		Vector3 pos = bounds.center + (bounds.extents + Vector3.one) * direction;
		return mgr.GetGridComponent(pos);
	}

	public static IList<GridPortalComponent> Connect(this GridComponent grid, NeighbourPosition pos)
	{
		if (grid.grid == null)
		{
			throw new ArgumentException("Can only connect a grid component that has been initialized.");
		}
		float connectorPortalWidth = grid.connectorPortalWidth;
		NeighbourPosition pos2;
		Vector3 vector;
		Bounds b;
		switch (pos)
		{
		case NeighbourPosition.Bottom:
			pos2 = NeighbourPosition.Top;
			vector = new Vector3(0f, 0f, connectorPortalWidth);
			b = grid.grid.GetEdge(NeighbourPosition.Bottom, connectorPortalWidth).Translate(-vector);
			break;
		case NeighbourPosition.Top:
			pos2 = NeighbourPosition.Bottom;
			vector = new Vector3(0f, 0f, 0f - connectorPortalWidth);
			b = grid.grid.GetEdge(NeighbourPosition.Top, connectorPortalWidth).Translate(-vector);
			break;
		case NeighbourPosition.Left:
			pos2 = NeighbourPosition.Right;
			vector = new Vector3(connectorPortalWidth, 0f, 0f);
			b = grid.grid.GetEdge(NeighbourPosition.Left, connectorPortalWidth).Translate(-vector);
			break;
		case NeighbourPosition.Right:
			pos2 = NeighbourPosition.Left;
			vector = new Vector3(0f - connectorPortalWidth, 0f, 0f);
			b = grid.grid.GetEdge(NeighbourPosition.Right, connectorPortalWidth).Translate(-vector);
			break;
		default:
			throw new ArgumentException("Only direct neighbours, i.e. Left, Right, Top or Bottom are supported.");
		}
		b = b.DeltaSize(-0.05f, 0f, -0.05f);
		GridPortalComponent[] array = (from p in GridManager.instance.GetAssociatedPortals(grid)
			where p.type == PortalType.Connector
			select p).ToArray();
		List<GridPortalComponent> list = new List<GridPortalComponent>();
		IEnumerable<IGrid> grids = GridManager.instance.GetGrids(b);
		foreach (IGrid item2 in grids)
		{
			bool flag = true;
			for (int num = 0; num < array.Length; num++)
			{
				if (array[num].Connects(grid.grid, item2))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Bounds a = item2.GetEdge(pos2, grid.connectorPortalWidth).DeltaSize(-0.05f, 0f, -0.05f);
				a = a.Intersection(b);
				Bounds portalTwo = a.Translate(vector);
				GridPortalComponent item = GridPortalComponent.CreateConnector(grid.gameObject, a, portalTwo);
				list.Add(item);
			}
		}
		return list;
	}

	public static Bounds GetEdge(this IGrid grid, NeighbourPosition pos, float width)
	{
		Bounds result = default;
		Vector3 min = grid.bounds.min;
		Vector3 max = grid.bounds.max;
		min.y = grid.origin.y;
		max.y = Mathf.Min(0.1f, max.y);
		switch (pos)
		{
		case NeighbourPosition.Bottom:
			max.z = min.z + width;
			result.SetMinMax(min, max);
			return result;
		case NeighbourPosition.Top:
			min.z = max.z - width;
			result.SetMinMax(min, max);
			return result;
		case NeighbourPosition.Left:
			max.x = min.x + width;
			result.SetMinMax(min, max);
			return result;
		case NeighbourPosition.Right:
			min.x = max.x - width;
			result.SetMinMax(min, max);
			return result;
		default:
			throw new ArgumentException("Only direct neighbours, i.e. Left, Right, Top or Bottom are supported.");
		}
	}
}
