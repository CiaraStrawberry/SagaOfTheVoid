using System;
using System.Collections.Generic;
using Apex.Common;
using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public interface IGridManager
{
	IEnumerable<IGrid> grids { get; }

	IIndexable<GridPortal> portals { get; }

	IGrid GetGrid(Vector3 pos);

	IEnumerable<IGrid> GetGrids(Bounds b);

	IEnumerable<IGridCell> GetCells(Bounds b);

	void RegisterGrid(IGrid grid);

	void UnregisterGrid(IGrid grid);

	GridComponent GetGridComponent(Vector3 pos);

	IEnumerable<GridComponent> GetGridComponents(Bounds b);

	void RegisterGridComponent(GridComponent grid);

	void UnregisterGridComponent(GridComponent grid);

	bool PortalExists(IGrid first, IGrid second, AttributeMask requesterAttributes);

	GridPortal GetPortal(string name);

	string RegisterPortal(string name, GridPortal portal);

	void UnregisterPortal(string name);

	void RegisterPortalComponent(GridPortalComponent portal);

	void UnregisterPortalComponent(GridPortalComponent portal);

	IEnumerable<GridPortalComponent> GetAssociatedPortals(GridComponent grid);

	IEnumerable<GridPortalComponent> GetAssociatedPortals(IGrid grid);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame, Action callback);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame, bool blockWhileUpdating, Action callback);
}
