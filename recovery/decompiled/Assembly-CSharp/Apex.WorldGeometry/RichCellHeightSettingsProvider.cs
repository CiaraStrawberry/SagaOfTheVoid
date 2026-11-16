using System;
using System.Collections;
using Apex.DataStructures;
using Apex.Services;
using Apex.Steering;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class RichCellHeightSettingsProvider : HeightSettingsProviderBase
{
	private class InnerProvider
	{
		private CellMatrix _matrix;

		private ISampleHeightsSimple _heightSampler;

		private Vector3[] _offsets;

		public InnerProvider(CellMatrix matrix)
		{
			_matrix = matrix;
			_offsets = new Vector3[3];
			_heightSampler = ((GameServices.heightStrategy.heightMode != HeightSamplingMode.HeightMap) ? ((ISampleHeightsSimple)GameServices.heightStrategy.heightSampler) : ((ISampleHeightsSimple)_matrix));
		}

		public IEnumerator AssignHeightSettings(MatrixBounds bounds)
		{
			int maxColumn = bounds.maxColumn;
			int maxRow = bounds.maxRow;
			bool isPartial = maxColumn - bounds.minColumn < _matrix.columns - 1 || maxRow - bounds.minRow < _matrix.rows - 1;
			float ledgeThreshold = Mathf.Tan(GameServices.heightStrategy.ledgeThreshold * ((float)Math.PI / 180f)) * _matrix.granularity;
			Cell[,] rawMatrix = _matrix.rawMatrix;
			for (int x = bounds.minColumn; x <= maxColumn; x++)
			{
				for (int z = bounds.minRow; z <= maxRow; z++)
				{
					RichCell c = rawMatrix[x, z] as RichCell;
					if (c.isPermanentlyBlocked)
					{
						continue;
					}
					for (int dz = -1; dz <= 1; dz++)
					{
						for (int dx = -1; dx <= 1; dx++)
						{
							if ((dx != 0 || dz != 0) && _matrix[x + dx, z + dz] is RichCell { isPermanentlyBlocked: false } n && (isPartial || !c.HasInitializedHeightData(dx, dz)))
							{
								UpdateCellHeightData(c, n, ledgeThreshold, dx, dz);
								yield return null;
							}
						}
					}
				}
			}
			if (isPartial)
			{
				RichCell bll = _matrix[bounds.minColumn - 1, bounds.minRow] as RichCell;
				RichCell blb = _matrix[bounds.minColumn, bounds.minRow - 1] as RichCell;
				RichCell tll = _matrix[bounds.minColumn - 1, bounds.maxRow] as RichCell;
				RichCell tlt = _matrix[bounds.minColumn, bounds.maxRow + 1] as RichCell;
				RichCell brr = _matrix[bounds.maxColumn + 1, bounds.minRow] as RichCell;
				RichCell brb = _matrix[bounds.maxColumn, bounds.minRow - 1] as RichCell;
				RichCell trr = _matrix[bounds.maxColumn + 1, bounds.maxRow] as RichCell;
				RichCell trt = _matrix[bounds.maxColumn, bounds.maxRow + 1] as RichCell;
				if (bll != null && blb != null)
				{
					UpdateCellHeightData(bll, blb, ledgeThreshold, 1, -1);
					yield return null;
				}
				if (tll != null && tlt != null)
				{
					UpdateCellHeightData(tll, tlt, ledgeThreshold, 1, 1);
					yield return null;
				}
				if (brr != null && brb != null)
				{
					UpdateCellHeightData(brr, brb, ledgeThreshold, -1, -1);
					yield return null;
				}
				if (trr != null && trt != null)
				{
					UpdateCellHeightData(trr, trt, ledgeThreshold, -1, 1);
					yield return null;
				}
			}
		}

		private void UpdateCellHeightData(RichCell reference, RichCell neighbour, float ledgeThreshold, int dx, int dz)
		{
			GetPerpendicularOffsets(dx, dz);
			float granularity = _matrix.granularity;
			float num = _matrix.cellSize / granularity;
			RichCell.HeightData data = default;
			for (int i = 0; i < 3; i++)
			{
				Vector3 position = reference.position + _offsets[i];
				float num2 = _heightSampler.SampleHeight(position, _matrix);
				float num3 = 0f;
				float num4 = 0f;
				for (int j = 0; (float)j < num; j++)
				{
					position.x += (float)dx * granularity;
					position.z += (float)dz * granularity;
					float num5 = _heightSampler.SampleHeight(position, _matrix);
					float num6 = num5 - num2;
					float num7 = Mathf.Abs(num6);
					if (data.slope < num7)
					{
						data.slope = num7;
					}
					if (num7 < ledgeThreshold)
					{
						if (data.dropHeight < num4)
						{
							data.dropHeight = num4;
						}
						else if (data.climbHeight < num3)
						{
							data.climbHeight = num3;
						}
						num4 = 0f;
						num3 = 0f;
					}
					if (num6 > 0f)
					{
						num3 += num6;
						if (data.dropHeight < num4)
						{
							data.dropHeight = num4;
						}
						num4 = 0f;
					}
					else if (num6 < 0f)
					{
						num4 += num7;
						if (data.climbHeight < num3)
						{
							data.climbHeight = num3;
						}
						num3 = 0f;
					}
					num2 = num5;
				}
				if (data.dropHeight < num4)
				{
					data.dropHeight = num4;
				}
				else if (data.climbHeight < num3)
				{
					data.climbHeight = num3;
				}
			}
			float num8 = ((dx == 0 || dz == 0) ? 1f : 1.414214f);
			data.slope = Mathf.Atan(data.slope / (granularity * num8)) * 57.29578f;
			reference.SetHeightData(dx, dz, data);
			reference.CalculateWorst();
			RichCell.HeightData data2 = new RichCell.HeightData(data.slope, data.dropHeight, data.climbHeight);
			neighbour.SetHeightData(-dx, -dz, data2);
			neighbour.CalculateWorst();
		}

		private void GetPerpendicularOffsets(int dx, int dz)
		{
			float num = Mathf.Min(_matrix.cellSize * 0.5f, _matrix.obstacleSensitivityRange);
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
			_offsets[1] = vector;
			ref Vector3 reference = ref _offsets[2];
			reference = vector * -1f;
		}
	}

	public override IEnumerator AssignHeightSettings(CellMatrix matrix, MatrixBounds bounds)
	{
		InnerProvider innerProvider = new InnerProvider(matrix);
		return innerProvider.AssignHeightSettings(bounds);
	}
}
