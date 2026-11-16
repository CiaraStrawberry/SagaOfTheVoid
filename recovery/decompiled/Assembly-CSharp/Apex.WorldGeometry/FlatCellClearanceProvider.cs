using System.Collections;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class FlatCellClearanceProvider : IClearanceProvider
{
	public IEnumerator SetClearance(CellMatrix matrix)
	{
		int rows = matrix.rows;
		int columns = matrix.columns;
		float cellSize = matrix.cellSize;
		float halfCell = cellSize * 0.5f;
		Cell[,] rawMatrix = matrix.rawMatrix;
		for (int i = 0; i < columns; i++)
		{
			FlatClearanceCell flatClearanceCell = rawMatrix[i, 0] as FlatClearanceCell;
			flatClearanceCell.clearance = ((!flatClearanceCell.isPermanentlyBlocked) ? halfCell : 0f);
			flatClearanceCell = rawMatrix[i, rows - 1] as FlatClearanceCell;
			flatClearanceCell.clearance = ((!flatClearanceCell.isPermanentlyBlocked) ? halfCell : 0f);
		}
		yield return null;
		for (int j = 1; j < rows - 1; j++)
		{
			FlatClearanceCell flatClearanceCell2 = rawMatrix[0, j] as FlatClearanceCell;
			flatClearanceCell2.clearance = ((!flatClearanceCell2.isPermanentlyBlocked) ? halfCell : 0f);
			flatClearanceCell2 = rawMatrix[columns - 1, j] as FlatClearanceCell;
			flatClearanceCell2.clearance = ((!flatClearanceCell2.isPermanentlyBlocked) ? halfCell : 0f);
		}
		yield return null;
		for (int x = 1; x < columns - 1; x++)
		{
			for (int k = 1; k < rows - 1; k++)
			{
				FlatClearanceCell flatClearanceCell3 = rawMatrix[x, k] as FlatClearanceCell;
				if (flatClearanceCell3.isPermanentlyBlocked)
				{
					flatClearanceCell3.clearance = 0f;
					continue;
				}
				float num = float.MaxValue;
				for (int l = -1; l < 2; l++)
				{
					for (int m = -1; m < 2; m++)
					{
						FlatClearanceCell flatClearanceCell4 = rawMatrix[x + l, k + m] as FlatClearanceCell;
						if (flatClearanceCell4.isPermanentlyBlocked)
						{
							num = 0f - halfCell;
							l = 2;
							m = 2;
						}
						else if (flatClearanceCell4.clearance < float.MaxValue)
						{
							num = Mathf.Min(num, flatClearanceCell4.clearance);
						}
					}
				}
				flatClearanceCell3.clearance = num + cellSize;
			}
			yield return null;
		}
		for (int x2 = columns - 2; x2 >= 1; x2--)
		{
			for (int n = 1; n < rows - 1; n++)
			{
				FlatClearanceCell flatClearanceCell5 = rawMatrix[x2, n] as FlatClearanceCell;
				if (flatClearanceCell5.isPermanentlyBlocked)
				{
					flatClearanceCell5.clearance = 0f;
					continue;
				}
				float num2 = float.MaxValue;
				for (int num3 = -1; num3 < 2; num3++)
				{
					for (int num4 = -1; num4 < 2; num4++)
					{
						FlatClearanceCell flatClearanceCell6 = rawMatrix[x2 + num3, n + num4] as FlatClearanceCell;
						if (flatClearanceCell6.isPermanentlyBlocked)
						{
							num2 = 0f - halfCell;
							num3 = 2;
							num4 = 2;
						}
						else if (flatClearanceCell6.clearance < float.MaxValue)
						{
							num2 = Mathf.Min(num2, flatClearanceCell6.clearance);
						}
					}
				}
				flatClearanceCell5.clearance = num2 + cellSize;
			}
			yield return null;
		}
	}

	public IEnumerator Reset(CellMatrix matrix)
	{
		int rows = matrix.rows;
		int columns = matrix.columns;
		Cell[,] rawMatrix = matrix.rawMatrix;
		for (int x = 1; x < columns - 1; x++)
		{
			for (int i = 1; i < rows - 1; i++)
			{
				IHaveClearance haveClearance = rawMatrix[x, i] as IHaveClearance;
				haveClearance.clearance = float.MaxValue;
			}
			yield return null;
		}
	}
}
