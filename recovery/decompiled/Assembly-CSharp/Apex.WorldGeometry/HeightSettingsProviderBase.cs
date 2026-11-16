using System.Collections;
using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public abstract class HeightSettingsProviderBase : IHeightSettingsProvider
{
	public abstract IEnumerator AssignHeightSettings(CellMatrix matrix, MatrixBounds bounds);

	protected Vector3[] GetPerpendicularOffsets(CellMatrix matrix, int dx, int dz)
	{
		float num = Mathf.Min(matrix.cellSize / 2f, matrix.obstacleSensitivityRange);
		Vector3 vector;
		if (dx != 0 && dz != 0)
		{
			float num2 = num / 1.414214f;
			vector = new Vector3(num2 * (float)(-dx), 0f, num2 * (float)dz);
		}
		else
		{
			vector = new Vector3(num * (float)dz, 0f, num * (float)dx);
		}
		return new Vector3[3]
		{
			Vector3.zero,
			vector,
			vector * -1f
		};
	}
}
