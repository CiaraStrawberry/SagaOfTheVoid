using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public class DiagonalDistance : MoveCostDiagonalBase
{
	public DiagonalDistance(int cellMoveCost)
		: base(cellMoveCost)
	{
	}

	public override int GetHeuristic(IPositioned current, IPositioned goal)
	{
		float num = Math.Abs(current.position.x - goal.position.x);
		float num2 = Math.Abs(current.position.z - goal.position.z);
		float num3 = Math.Abs(current.position.y - goal.position.y);
		return Mathf.RoundToInt((float)baseMoveCost * (num + num2 + num3) + ((float)diagonalMoveCost - 2f * (float)baseMoveCost) * Math.Min(num, num2));
	}
}
