using System.Collections;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class StandardCellClearanceProvider : IClearanceProvider
{
	private float _heightDiffThreshold = 1f;

	public StandardCellClearanceProvider(float heightDiffThreshold)
	{
		_heightDiffThreshold = heightDiffThreshold;
	}

	public IEnumerator SetClearance(CellMatrix matrix)
	{
		int rows = matrix.rows;
		int columns = matrix.columns;
		float cellSize = matrix.cellSize;
		float halfCell = cellSize * 0.5f;
		Cell[,] rawMatrix = matrix.rawMatrix;
		for (int i = 0; i < columns; i++)
		{
			Cell cell = rawMatrix[i, 0];
			IHaveClearance haveClearance = cell as IHaveClearance;
			haveClearance.clearance = ((!cell.isPermanentlyBlocked) ? halfCell : 0f);
			cell = rawMatrix[i, rows - 1];
			haveClearance = cell as IHaveClearance;
			haveClearance.clearance = ((!cell.isPermanentlyBlocked) ? halfCell : 0f);
		}
		yield return null;
		for (int j = 1; j < rows - 1; j++)
		{
			Cell cell2 = rawMatrix[0, j];
			IHaveClearance haveClearance2 = cell2 as IHaveClearance;
			haveClearance2.clearance = ((!cell2.isPermanentlyBlocked) ? halfCell : 0f);
			cell2 = rawMatrix[columns - 1, j];
			haveClearance2 = cell2 as IHaveClearance;
			haveClearance2.clearance = ((!cell2.isPermanentlyBlocked) ? halfCell : 0f);
		}
		yield return null;
		for (int x = 1; x < columns - 1; x++)
		{
			for (int k = 1; k < rows - 1; k++)
			{
				Cell cell3 = rawMatrix[x, k];
				IHaveClearance haveClearance3 = cell3 as IHaveClearance;
				if (cell3.isPermanentlyBlocked)
				{
					haveClearance3.clearance = 0f;
					continue;
				}
				float y = cell3.position.y;
				float num = float.MaxValue;
				for (int l = -1; l < 2; l++)
				{
					for (int m = -1; m < 2; m++)
					{
						Cell cell4 = rawMatrix[x + l, k + m];
						IHaveClearance haveClearance4 = cell4 as IHaveClearance;
						if (cell4.isPermanentlyBlocked || cell4.position.y - y > _heightDiffThreshold)
						{
							num = 0f - halfCell;
							l = 2;
							m = 2;
						}
						else if (haveClearance4.clearance < float.MaxValue)
						{
							num = Mathf.Min(num, haveClearance4.clearance);
						}
					}
				}
				haveClearance3.clearance = num + cellSize;
			}
			yield return null;
		}
		for (int x2 = columns - 2; x2 >= 1; x2--)
		{
			for (int n = 1; n < rows - 1; n++)
			{
				Cell cell5 = rawMatrix[x2, n];
				IHaveClearance haveClearance5 = cell5 as IHaveClearance;
				if (cell5.isPermanentlyBlocked)
				{
					haveClearance5.clearance = 0f;
					continue;
				}
				float y2 = cell5.position.y;
				float num2 = float.MaxValue;
				for (int num3 = -1; num3 < 2; num3++)
				{
					for (int num4 = -1; num4 < 2; num4++)
					{
						Cell cell6 = rawMatrix[x2 + num3, n + num4];
						IHaveClearance haveClearance6 = cell6 as IHaveClearance;
						if (cell6.isPermanentlyBlocked || cell6.position.y - y2 > _heightDiffThreshold)
						{
							num2 = 0f - halfCell;
							num3 = 2;
							num4 = 2;
						}
						else if (haveClearance6.clearance < float.MaxValue)
						{
							num2 = Mathf.Min(num2, haveClearance6.clearance);
						}
					}
				}
				haveClearance5.clearance = num2 + cellSize;
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
