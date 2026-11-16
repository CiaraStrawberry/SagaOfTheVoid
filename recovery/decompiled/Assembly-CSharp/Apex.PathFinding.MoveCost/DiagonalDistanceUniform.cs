using System;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public class DiagonalDistanceUniform : MoveCostBase
{
	public DiagonalDistanceUniform(int cellMoveCost)
		: base(cellMoveCost)
	{
	}

	public override int GetHeuristic(IPositioned current, IPositioned goal)
	{
		float val = Math.Abs(current.position.x - goal.position.x);
		float val2 = Math.Abs(current.position.z - goal.position.z);
		float num = Math.Abs(current.position.y - goal.position.y);
		return Mathf.RoundToInt((float)baseMoveCost * (Math.Max(val, val2) + num));
	}
}
