using System;
using System.Collections;
using Apex.DataStructures;
using Apex.Services;
using Apex.Steering;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class StandardCellHeightSettingsProvider : HeightSettingsProviderBase
{
	private class InnerProvider
	{
		private float _maxWalkableSlopeAngle;

		private float _maxClimbHeight;

		private float _maxDropHeight;

		private float _granularity;

		private CellMatrix _matrix;

		private ISampleHeightsSimple _heightSampler;

		private Vector3[] _offsets;

		public InnerProvider(CellMatrix matrix, StandardCellHeightSettingsProvider parent)
		{
			_matrix = matrix;
			_maxWalkableSlopeAngle = parent._maxWalkableSlopeAngle;
			_maxClimbHeight = parent._maxClimbHeight;
			_maxDropHeight = parent._maxDropHeight;
			_granularity = parent._granularity;
			_offsets = new Vector3[3];
			_heightSampler = ((GameServices.heightStrategy.heightMode != HeightSamplingMode.HeightMap) ? ((ISampleHeightsSimple)GameServices.heightStrategy.heightSampler) : ((ISampleHeightsSimple)matrix));
		}

		public IEnumerator AssignHeightSettings(MatrixBounds bounds)
		{
			if (_maxWalkableSlopeAngle >= 90f)
			{
				yield break;
			}
			float ledgeThreshold = Mathf.Tan(GameServices.heightStrategy.ledgeThreshold * ((float)Math.PI / 180f)) * _matrix.granularity;
			float maxHeightDiff = Mathf.Tan(_maxWalkableSlopeAngle * ((float)Math.PI / 180f)) * _granularity;
			float maxHeightDiffDiag = maxHeightDiff * 1.414214f;
			int maxColumn = bounds.maxColumn;
			int maxRow = bounds.maxRow;
			bool isPartial = maxColumn - bounds.minColumn < _matrix.columns - 1 || maxRow - bounds.minRow < _matrix.rows - 1;
			Cell[,] raw_matrix = _matrix.rawMatrix;
			for (int x = bounds.minColumn; x <= maxColumn; x++)
			{
				for (int z = bounds.minRow; z <= maxRow; z++)
				{
					StandardCell c = raw_matrix[x, z] as StandardCell;
					if (c.isPermanentlyBlocked)
					{
						continue;
					}
					for (int dz = -1; dz <= 1; dz++)
					{
						for (int dx = -1; dx <= 1; dx++)
						{
							if ((dx == 0 && dz == 0) || !(_matrix[x + dx, z + dz] is StandardCell { isPermanentlyBlocked: false } n))
							{
								continue;
							}
							NeighbourPosition pos = n.GetRelativePositionTo(c);
							if (isPartial || (c.heightIntializedFrom & pos) <= NeighbourPosition.None)
							{
								if (isPartial)
								{
									c.heightBlockedFrom &= ~pos;
									n.heightBlockedFrom &= ~c.GetRelativePositionTo(n);
								}
								if (pos == NeighbourPosition.Bottom || pos == NeighbourPosition.Left || pos == NeighbourPosition.Right || pos == NeighbourPosition.Top)
								{
									UpdateCellHeightData(c, n, pos, maxHeightDiff, ledgeThreshold, testForPermanentBlock: true);
								}
								else
								{
									UpdateCellHeightData(c, n, pos, maxHeightDiffDiag, ledgeThreshold, testForPermanentBlock: false);
								}
								yield return null;
							}
						}
					}
				}
			}
			if (isPartial)
			{
				StandardCell bll = _matrix[bounds.minColumn - 1, bounds.minRow] as StandardCell;
				StandardCell blb = _matrix[bounds.minColumn, bounds.minRow - 1] as StandardCell;
				StandardCell tll = _matrix[bounds.minColumn - 1, bounds.maxRow] as StandardCell;
				StandardCell tlt = _matrix[bounds.minColumn, bounds.maxRow + 1] as StandardCell;
				StandardCell brr = _matrix[bounds.maxColumn + 1, bounds.minRow] as StandardCell;
				StandardCell brb = _matrix[bounds.maxColumn, bounds.minRow - 1] as StandardCell;
				StandardCell trr = _matrix[bounds.maxColumn + 1, bounds.maxRow] as StandardCell;
				StandardCell trt = _matrix[bounds.maxColumn, bounds.maxRow + 1] as StandardCell;
				if (bll != null && blb != null)
				{
					bll.heightBlockedFrom &= ~NeighbourPosition.BottomRight;
					blb.heightBlockedFrom &= ~NeighbourPosition.TopLeft;
					UpdateCellHeightData(bll, blb, NeighbourPosition.BottomRight, maxHeightDiffDiag, ledgeThreshold, testForPermanentBlock: false);
					yield return null;
				}
				if (tll != null && tlt != null)
				{
					tll.heightBlockedFrom &= ~NeighbourPosition.TopRight;
					tlt.heightBlockedFrom &= ~NeighbourPosition.BottomLeft;
					UpdateCellHeightData(tll, tlt, NeighbourPosition.TopRight, maxHeightDiffDiag, ledgeThreshold, testForPermanentBlock: false);
					yield return null;
				}
				if (brr != null && brb != null)
				{
					brr.heightBlockedFrom &= ~NeighbourPosition.BottomLeft;
					brb.heightBlockedFrom &= ~NeighbourPosition.TopRight;
					UpdateCellHeightData(brr, brb, NeighbourPosition.BottomLeft, maxHeightDiffDiag, ledgeThreshold, testForPermanentBlock: false);
					yield return null;
				}
				if (trr != null && trt != null)
				{
					trr.heightBlockedFrom &= ~NeighbourPosition.TopLeft;
					trt.heightBlockedFrom &= ~NeighbourPosition.BottomRight;
					UpdateCellHeightData(trr, trt, NeighbourPosition.TopLeft, maxHeightDiffDiag, ledgeThreshold, testForPermanentBlock: false);
					yield return null;
				}
			}
		}

		private void UpdateCellHeightData(StandardCell reference, StandardCell neighbour, NeighbourPosition neighbourPos, float maxHeightDiff, float ledgeThreshold, bool testForPermanentBlock)
		{
			VectorXZ directionTo = reference.GetDirectionTo(neighbour);
			GetPerpendicularOffsets(directionTo.x, directionTo.z);
			float granularity = _matrix.granularity;
			float num = _matrix.cellSize / granularity;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < 3; i++)
			{
				Vector3 position = reference.position + _offsets[i];
				float num4 = _heightSampler.SampleHeight(position, _matrix);
				float num5 = 0f;
				float num6 = 0f;
				for (int j = 0; (float)j < num; j++)
				{
					position.x += (float)directionTo.x * granularity;
					position.z += (float)directionTo.z * granularity;
					float num7 = _heightSampler.SampleHeight(position, _matrix);
					float num8 = num7 - num4;
					float num9 = Mathf.Abs(num8);
					bool flag = num9 < ledgeThreshold;
					if (num9 <= maxHeightDiff || flag)
					{
						if (num5 > num2)
						{
							num2 = num5;
						}
						else if (num6 > num3)
						{
							num3 = num6;
						}
						num5 = 0f;
						num6 = 0f;
					}
					else if (num8 > 0f)
					{
						num5 += num8;
						if (num6 > num3)
						{
							num3 = num6;
						}
						num6 = 0f;
					}
					else if (num8 < 0f)
					{
						num6 += num9;
						if (num5 > num2)
						{
							num2 = num5;
						}
						num5 = 0f;
					}
					num4 = num7;
				}
				num2 = Mathf.Max(num5, num2);
				num3 = Mathf.Max(num6, num3);
			}
			NeighbourPosition relativePositionTo = reference.GetRelativePositionTo(neighbour);
			if (num2 > _maxClimbHeight || num3 > _maxDropHeight)
			{
				neighbour.heightBlockedFrom |= relativePositionTo;
			}
			if (num2 > _maxDropHeight || num3 > _maxClimbHeight)
			{
				reference.heightBlockedFrom |= neighbourPos;
			}
			reference.heightIntializedFrom |= neighbourPos;
			neighbour.heightIntializedFrom |= relativePositionTo;
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

	private float _maxWalkableSlopeAngle;

	private float _maxClimbHeight;

	private float _maxDropHeight;

	private float _granularity;

	public StandardCellHeightSettingsProvider()
	{
		HeightNavigationCapabilities unitsHeightNavigationCapability = GameServices.heightStrategy.unitsHeightNavigationCapability;
		_maxWalkableSlopeAngle = unitsHeightNavigationCapability.maxSlopeAngle;
		_maxClimbHeight = unitsHeightNavigationCapability.maxClimbHeight;
		_maxDropHeight = unitsHeightNavigationCapability.maxDropHeight;
		_granularity = GameServices.heightStrategy.sampleGranularity;
	}

	public override IEnumerator AssignHeightSettings(CellMatrix matrix, MatrixBounds bounds)
	{
		InnerProvider innerProvider = new InnerProvider(matrix, this);
		return innerProvider.AssignHeightSettings(bounds);
	}
}
