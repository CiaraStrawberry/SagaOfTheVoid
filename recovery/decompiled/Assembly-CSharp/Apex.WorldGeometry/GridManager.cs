using System;
using System.Collections.Generic;
using Apex.Common;
using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class GridManager : IGridManager
{
	public static readonly IGridManager instance = new GridManager();

	private List<IGrid> _grids;

	private List<GridComponent> _gridComponents;

	private DynamicArray<GridPortal> _portals;

	private DynamicArray<GridPortalComponent> _portalComponents;

	private Dictionary<string, GridPortal> _portalsLookup;

	public IEnumerable<IGrid> grids => _grids;

	public IIndexable<GridPortal> portals => _portals;

	public GridManager()
	{
		_grids = new List<IGrid>();
		_gridComponents = new List<GridComponent>();
		_portals = new DynamicArray<GridPortal>(0);
		_portalComponents = new DynamicArray<GridPortalComponent>(0);
		_portalsLookup = new Dictionary<string, GridPortal>(StringComparer.Ordinal);
	}

	public void RegisterGrid(IGrid grid)
	{
		_grids.AddUnique(grid);
	}

	public void UnregisterGrid(IGrid grid)
	{
		_grids.Remove(grid);
	}

	public IGrid GetGrid(Vector3 pos)
	{
		for (int i = 0; i < _grids.Count; i++)
		{
			IGrid grid = _grids[i];
			if (grid.Contains(pos))
			{
				return grid;
			}
		}
		return null;
	}

	public IEnumerable<IGrid> GetGrids(Bounds b)
	{
		for (int i = 0; i < _grids.Count; i++)
		{
			IGrid g = _grids[i];
			if (g.bounds.Intersects(b))
			{
				yield return g;
			}
		}
	}

	public IEnumerable<IGridCell> GetCells(Bounds b)
	{
		foreach (IGrid grid in GetGrids(b))
		{
			foreach (Cell coveredCell in grid.GetCoveredCells(b))
			{
				yield return coveredCell;
			}
		}
	}

	public GridComponent GetGridComponent(Vector3 pos)
	{
		for (int i = 0; i < _gridComponents.Count; i++)
		{
			GridComponent gridComponent = _gridComponents[i];
			if (gridComponent.Contains(pos))
			{
				return gridComponent;
			}
		}
		return null;
	}

	public IEnumerable<GridComponent> GetGridComponents(Bounds b)
	{
		for (int i = 0; i < _gridComponents.Count; i++)
		{
			GridComponent g = _gridComponents[i];
			if (g.bounds.Intersects(b))
			{
				yield return g;
			}
		}
	}

	public void RegisterGridComponent(GridComponent grid)
	{
		_gridComponents.AddUnique(grid);
	}

	public void UnregisterGridComponent(GridComponent grid)
	{
		_gridComponents.Remove(grid);
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame)
	{
		Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating: false, null);
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame, Action callback)
	{
		Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating: false, callback);
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame, bool blockWhileUpdating, Action callback)
	{
		for (int i = 0; i < _grids.Count; i++)
		{
			IGrid grid = _grids[i];
			if (grid.bounds.Intersects(extent))
			{
				grid.Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating, callback);
			}
		}
	}

	public bool PortalExists(IGrid first, IGrid second, AttributeMask requesterAttributes)
	{
		int count = _portals.count;
		for (int i = 0; i < count; i++)
		{
			GridPortal gridPortal = _portals[i];
			if (gridPortal.enabled && gridPortal.IsUsableBy(requesterAttributes) && ((gridPortal.gridOne == first && gridPortal.gridTwo == second) || (gridPortal.gridOne == second && gridPortal.gridTwo == first)))
			{
				return true;
			}
		}
		return false;
	}

	public GridPortal GetPortal(string name)
	{
		_portalsLookup.TryGetValue(name, out var value);
		return value;
	}

	public string RegisterPortal(string name, GridPortal portal)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = "Grid Portal " + (_portalsLookup.Count + 1);
		}
		int num = 1;
		while (_portalsLookup.ContainsKey(name))
		{
			name = "Grid Portal " + num++;
		}
		_portalsLookup.Add(name, portal);
		_portals.Add(portal);
		return name;
	}

	public void UnregisterPortal(string name)
	{
		_portalsLookup.Remove(name);
		_portals.Clear();
		foreach (GridPortal value in _portalsLookup.Values)
		{
			_portals.Add(value);
		}
	}

	public void RegisterPortalComponent(GridPortalComponent portal)
	{
		_portalComponents.Add(portal);
	}

	public void UnregisterPortalComponent(GridPortalComponent portal)
	{
		_portalComponents.Remove(portal);
	}

	public IEnumerable<GridPortalComponent> GetAssociatedPortals(GridComponent grid)
	{
		int count = _portalComponents.count;
		for (int i = 0; i < count; i++)
		{
			GridPortalComponent p = _portalComponents[i];
			if (p.Connects(grid))
			{
				yield return _portalComponents[i];
			}
		}
	}

	public IEnumerable<GridPortalComponent> GetAssociatedPortals(IGrid grid)
	{
		int count = _portalComponents.count;
		for (int i = 0; i < count; i++)
		{
			GridPortalComponent p = _portalComponents[i];
			if (p.Connects(grid))
			{
				yield return _portalComponents[i];
			}
		}
	}

	private static Vector3? CheckLineIntersection(IGrid g, Vector3 from, Vector3 to)
	{
		Vector3 line2p = new Vector3(g.left.edge, 0f, g.bottom.edge);
		Vector3 line2p2 = new Vector3(g.right.edge, 0f, g.top.edge);
		if (Geometry.DoLinesIntersect(from, to, line2p, line2p2, out var intersectionPoint))
		{
			return intersectionPoint;
		}
		Vector3 line2p3 = new Vector3(g.left.edge, 0f, g.top.edge);
		Vector3 line2p4 = new Vector3(g.right.edge, 0f, g.bottom.edge);
		if (Geometry.DoLinesIntersect(from, to, line2p3, line2p4, out intersectionPoint))
		{
			return intersectionPoint;
		}
		return null;
	}
}
