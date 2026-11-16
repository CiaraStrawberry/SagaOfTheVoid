using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public class ManhattanDistance : MoveCostBase
{
	public ManhattanDistance(int cellMoveCost)
		: base(cellMoveCost)
	{
	}

	public override int GetMoveCost(IPositioned current, IPositioned other)
	{
		float num = Math.Abs(current.position.x - other.position.x);
		float num2 = Math.Abs(current.position.z - other.position.z);
		float num3 = Math.Abs(current.position.y - other.position.y);
		return Mathf.RoundToInt((float)baseMoveCost * (num + num2 + num3));
	}

	public override int GetHeuristic(IPositioned current, IPositioned goal)
	{
		float num = Math.Abs(current.position.x - goal.position.x);
		float num2 = Math.Abs(current.position.z - goal.position.z);
		float num3 = Math.Abs(current.position.y - goal.position.y);
		return Mathf.RoundToInt((float)baseMoveCost * (num + num2 + num3));
	}
}
