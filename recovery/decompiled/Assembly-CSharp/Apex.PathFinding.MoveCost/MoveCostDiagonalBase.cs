using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public abstract class MoveCostDiagonalBase : IMoveCost
{
	private readonly int _cellMoveCost;

	private readonly int _cellDiagonalMoveCost;

	public int baseMoveCost => _cellMoveCost;

	protected int diagonalMoveCost => _cellDiagonalMoveCost;

	protected MoveCostDiagonalBase(int cellMoveCost)
	{
		_cellMoveCost = cellMoveCost;
		_cellDiagonalMoveCost = Mathf.FloorToInt(1.414214f * (float)cellMoveCost);
	}

	public virtual int GetMoveCost(IPositioned current, IPositioned other)
	{
		float num = Math.Abs(current.position.x - other.position.x);
		float num2 = Math.Abs(current.position.z - other.position.z);
		float num3 = Math.Abs(current.position.y - other.position.y);
		if (num > 0f && num2 > 0f)
		{
			return Mathf.RoundToInt(num * (float)_cellDiagonalMoveCost + num3 * (float)_cellMoveCost);
		}
		return Mathf.RoundToInt((Math.Max(num, num2) + num3) * (float)_cellMoveCost);
	}

	public abstract int GetHeuristic(IPositioned current, IPositioned goal);
}
