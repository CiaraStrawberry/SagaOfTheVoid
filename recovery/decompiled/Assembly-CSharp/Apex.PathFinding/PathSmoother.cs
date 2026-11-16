using System;
using System.Collections.Generic;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public class PathSmoother : ISmoothPaths
{
	private struct Tangents
	{
		public int slopeDir;

		private float _alpha;

		private float _alphaSec;

		private float _betaTanHigh;

		private float _betaTanLow;

		private float _betaSecLow;

		private float _betaSecHigh;

		public bool isVertical { get; private set; }

		public static Tangents Create(Vector3 from, Vector3 to, float circleRadius, CellMatrix matrix)
		{
			Tangents result = default;
			result.Init(from, to, circleRadius, matrix);
			return result;
		}

		public float HighTangent(float x)
		{
			return _alpha * x + _betaTanHigh;
		}

		public float LowTangent(float x)
		{
			return _alpha * x + _betaTanLow;
		}

		public bool IsOutsideSecants(Vector3 p)
		{
			return _alphaSec * p.x + _betaSecLow > p.z || _alphaSec * p.x + _betaSecHigh < p.z;
		}

		private void Init(Vector3 from, Vector3 to, float circleRadius, CellMatrix matrix)
		{
			float num = to.x - from.x;
			float num2 = to.z - from.z;
			slopeDir = Math.Sign(num2);
			if (slopeDir == 0)
			{
				slopeDir = 1;
			}
			if (Mathf.Abs(num) < 1E-05f)
			{
				isVertical = true;
				return;
			}
			_alpha = num2 / num;
			_alphaSec = ((num2 != 0f) ? ((0f - num) / num2) : (-1000000f));
			Vector3 vector = new Vector3(0f - num2, 0f, num).normalized * circleRadius;
			if (slopeDir < 0)
			{
				_betaSecLow = to.z - _alphaSec * to.x;
				_betaSecHigh = from.z - _alphaSec * from.x;
			}
			else
			{
				_betaSecLow = from.z - _alphaSec * from.x;
				_betaSecHigh = to.z - _alphaSec * to.x;
			}
			from -= matrix.start;
			Vector3 vector2 = from + vector;
			_betaTanHigh = vector2.z - _alpha * vector2.x;
			vector2 = from - vector;
			_betaTanLow = vector2.z - _alpha * vector2.x;
		}
	}

	public static bool CanReducePath(IPositioned from, IPositioned to, IUnitProperties unitProps, CellMatrix matrix, ICellCostStrategy costStrategy)
	{
		Cell cell = matrix.GetCell(from.position, adjustToBounds: false);
		Vector3 position;
		Vector3 position2;
		if (from.position.x > to.position.x)
		{
			position = to.position;
			position2 = from.position;
		}
		else
		{
			position = from.position;
			position2 = to.position;
		}
		float radius = unitProps.radius;
		Tangents tangents = Tangents.Create(position, position2, radius, matrix);
		int slopeDir = tangents.slopeDir;
		float cellSize = matrix.cellSize;
		float num = cellSize * 0.5f;
		Cell cell2 = matrix.GetCell(position, adjustToBounds: false);
		int cellCost = costStrategy.GetCellCost(cell2, unitProps);
		cell2 = matrix.GetCell(new Vector3(position.x - radius, position.y, position.z), adjustToBounds: true);
		Cell cell3 = matrix.GetCell(position2, adjustToBounds: false);
		int cellCost2 = costStrategy.GetCellCost(cell3, unitProps);
		cell3 = matrix.GetCell(new Vector3(position2.x + radius, position2.y, position2.z), adjustToBounds: true);
		float x = cell2.position.x - matrix.start.x - num;
		float num2 = cell2.position.x - matrix.start.x + num;
		int val2;
		int val4;
		if (slopeDir < 0)
		{
			float num3 = num - (cell3.position.z - position2.z);
			int val = Mathf.CeilToInt((radius - num3) / cellSize);
			val2 = Math.Max(0, cell3.matrixPosZ - Math.Max(0, val));
			float num4 = num - (position.z - cell2.position.z);
			int val3 = Mathf.CeilToInt((radius - num4) / cellSize);
			val4 = Math.Min(matrix.rows - 1, cell2.matrixPosZ + Math.Max(0, val3));
		}
		else
		{
			float num5 = num - (cell2.position.z - position.z);
			int val5 = Mathf.CeilToInt((radius - num5) / cellSize);
			val2 = Math.Max(0, cell2.matrixPosZ - Math.Max(0, val5));
			float num6 = num - (position2.z - cell3.position.z);
			int val6 = Mathf.CeilToInt((radius - num6) / cellSize);
			val4 = Math.Min(matrix.rows - 1, cell3.matrixPosZ + Math.Max(0, val6));
		}
		int matrixPosX = cell2.matrixPosX;
		int matrixPosX2 = cell3.matrixPosX;
		bool isVertical = tangents.isVertical;
		Cell[,] rawMatrix = matrix.rawMatrix;
		for (int i = matrixPosX; i <= matrixPosX2; i++)
		{
			int num7;
			int num8;
			if (isVertical)
			{
				num7 = Math.Min(cell2.matrixPosZ, cell3.matrixPosZ);
				num8 = Math.Max(cell2.matrixPosZ, cell3.matrixPosZ);
			}
			else if (slopeDir < 0)
			{
				num7 = Math.Max((int)(tangents.LowTangent(num2) / cellSize), val2);
				num8 = Math.Min((int)(tangents.HighTangent(x) / cellSize), val4);
			}
			else
			{
				num7 = Math.Max((int)(tangents.LowTangent(x) / cellSize), val2);
				num8 = Math.Min((int)(tangents.HighTangent(num2) / cellSize), val4);
			}
			for (int j = num7; j <= num8; j++)
			{
				Cell cell4 = rawMatrix[i, j];
				if (isVertical || !tangents.IsOutsideSecants(cell4.position))
				{
					int cellCost3 = costStrategy.GetCellCost(cell4, unitProps);
					if (!cell4.IsWalkableFrom(cell, unitProps) || cellCost < cellCost3 || cellCost2 < cellCost3)
					{
						return false;
					}
				}
			}
			x = num2;
			num2 += cellSize;
		}
		return true;
	}

	public Path Smooth(IPathNode goal, int maxPathLength, IPathRequest request, ICellCostStrategy costStrategy)
	{
		IUnitProperties requesterProperties = request.requesterProperties;
		List<IPositioned> list = new List<IPositioned>(maxPathLength);
		IPathNode pathNode = goal;
		IPathNode predecessor = pathNode.predecessor;
		int num = -1;
		Vector3 vector = Vector3.zero;
		while (predecessor != null)
		{
			Vector3 vector2 = predecessor.position - pathNode.position;
			if (vector2 != vector || predecessor is IPortalNode)
			{
				list.Add(pathNode);
				vector = vector2;
				num++;
			}
			pathNode = predecessor;
			predecessor = pathNode.predecessor;
		}
		if (list.Count == 0)
		{
			list.Add(new Position(request.to));
		}
		else
		{
			list[0] = new Position(request.to);
		}
		if (list.Count == 1 && num <= 0)
		{
			list.Add(goal.predecessor);
		}
		list.Add(new Position(request.from));
		int num2 = list.Count;
		IGrid grid = GridManager.instance.GetGrid(request.from);
		if (!request.pathFinderOptions.preventDiagonalMoves && grid != null)
		{
			CellMatrix matrix = grid.cellMatrix;
			int num3 = 2;
			for (int num4 = list.Count - 1; num4 >= num3; num4--)
			{
				IPositioned positioned = list[num4];
				IPositioned positioned2 = list[num4 - 1];
				IPositioned positioned3 = list[num4 - 2];
				int num5 = AdjustIfPortal(positioned, positioned2, positioned3);
				if (num5 > -1)
				{
					num4 -= num5;
					matrix = ((IPortalNode)list[num4]).partner.parent;
				}
				else
				{
					while (CanReducePath(positioned, positioned3, requesterProperties, matrix, costStrategy))
					{
						list[num4 - 1] = null;
						num2--;
						num4--;
						if (num4 < num3)
						{
							break;
						}
						positioned3 = list[num4 - 2];
						if (positioned3 is IPortalNode)
						{
							break;
						}
					}
				}
			}
		}
		Path path = new Path(num2);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			IPositioned positioned4 = list[i];
			if (positioned4 != null)
			{
				path.Push(positioned4);
			}
		}
		return path;
	}

	private static int AdjustIfPortal(params IPositioned[] nodes)
	{
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i] is IPortalNode)
			{
				return i;
			}
		}
		return -1;
	}
}
