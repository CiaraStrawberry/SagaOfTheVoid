using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding.MoveCost;

public class EuclideanDistance : MoveCostDiagonalBase
{
	public EuclideanDistance(int cellMoveCost)
		: base(cellMoveCost)
	{
	}

	public override int GetMoveCost(IPositioned current, IPositioned other)
	{
		float num = current.position.x - other.position.x;
		float num2 = current.position.z - other.position.z;
		float num3 = current.position.y - other.position.y;
		return Mathf.RoundToInt((float)baseMoveCost * Mathf.Sqrt(num * num + num2 * num2 + num3 * num3));
	}

	public override int GetHeuristic(IPositioned current, IPositioned goal)
	{
		float num = current.position.x - goal.position.x;
		float num2 = current.position.z - goal.position.z;
		float num3 = current.position.y - goal.position.y;
		return Mathf.RoundToInt((float)baseMoveCost * Mathf.Sqrt(num * num + num2 * num2 + num3 * num3));
	}
}
