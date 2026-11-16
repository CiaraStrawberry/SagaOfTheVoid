using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public abstract class MoveCostBase : IMoveCost
{
	private readonly int _cellMoveCost;

	public int baseMoveCost => _cellMoveCost;

	protected MoveCostBase(int cellMoveCost)
	{
		_cellMoveCost = cellMoveCost;
	}

	public virtual int GetMoveCost(IPositioned current, IPositioned other)
	{
		float val = Math.Abs(current.position.x - other.position.x);
		float val2 = Math.Abs(current.position.z - other.position.z);
		float num = Math.Abs(current.position.y - other.position.y);
		return Mathf.RoundToInt((float)_cellMoveCost * (Math.Max(val, val2) + num));
	}

	public abstract int GetHeuristic(IPositioned current, IPositioned goal);
}
