using System.Collections.Generic;
using Apex.Common;
using UnityEngine;

namespace Apex.WorldGeometry;

public class ManualDynamicObstacle : IDynamicObstacle, IPositioned
{
	private bool _active;

	private Bounds _bounds;

	public Vector3 position => _bounds.center;

	public AttributeMask exceptionsMask { get; private set; }

	public ManualDynamicObstacle(Bounds coverage)
		: this(coverage, AttributeMask.None)
	{
	}

	public ManualDynamicObstacle(Bounds coverage, AttributeMask exceptions)
	{
		_bounds = coverage;
		exceptionsMask = exceptions;
	}

	public void Toggle(bool active)
	{
		if (active == _active)
		{
			return;
		}
		IGrid grid = GridManager.instance.GetGrid(_bounds.center);
		if (grid == null)
		{
			return;
		}
		_active = active;
		bool flag = false;
		IEnumerable<Cell> coveredCells = grid.GetCoveredCells(_bounds);
		if (_active)
		{
			foreach (Cell item in coveredCells)
			{
				flag |= item.AddDynamicObstacle(this);
			}
		}
		else
		{
			foreach (Cell item2 in coveredCells)
			{
				flag |= item2.RemoveDynamicObstacle(this);
			}
		}
		if (flag)
		{
			grid.TouchSections(_bounds);
		}
	}

	void IDynamicObstacle.ActivateUpdates(float? interval, bool repeat)
	{
	}
}
