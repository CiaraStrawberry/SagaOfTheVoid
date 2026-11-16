using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public class CardinalDistance : MoveCostDiagonalBase
{
	public CardinalDistance(int cellMoveCost)
		: base(cellMoveCost)
	{
	}

	public override int GetHeuristic(IPositioned current, IPositioned goal)
	{
		float val = Math.Abs(current.position.x - goal.position.x);
		float val2 = Math.Abs(current.position.z - goal.position.z);
		float num = Math.Abs(current.position.y - goal.position.y);
		return Mathf.RoundToInt(Math.Min(val, val2) * 1.414214f + Math.Max(val, val2) - Math.Min(val, val2) + num);
	}
}
