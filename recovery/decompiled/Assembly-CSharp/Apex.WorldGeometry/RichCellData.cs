using System;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class RichCellData : CellMatrixData
{
	[HideInInspector]
	[SerializeField]
	private RichCell.HeightData[] _heightData;

	[HideInInspector]
	[SerializeField]
	private int[] _heightDataIndices;

	private int _recordedEntries;

	protected override void PrepareForInitialization(CellMatrix matrix)
	{
		_heightData = new RichCell.HeightData[matrix.columns * matrix.rows * 4];
		_heightDataIndices = new int[matrix.columns * matrix.rows];
		_recordedEntries = 0;
	}

	protected override void RecordCellData(Cell c, int cellIdx)
	{
		RichCell richCell = c as RichCell;
		RichCell.HeightData[] heightData = richCell.GetHeightData();
		StoreData(heightData, 0, cellIdx, 1);
		StoreData(heightData, 1, cellIdx, 2);
		StoreData(heightData, 3, cellIdx, 4);
		StoreData(heightData, 6, cellIdx, 8);
	}

	protected override void FinalizeInitialization()
	{
		RichCell.HeightData[] array = new RichCell.HeightData[_recordedEntries];
		Array.Copy(_heightData, array, _recordedEntries);
		_heightData = array;
	}

	protected override void InjectCellData(Cell c, int cellIdx)
	{
		RichCell c2 = c as RichCell;
		int num = _heightDataIndices[cellIdx];
		int dataIdx = num >> 4;
		dataIdx = SetData(c2, 0, 8, dataIdx, (num & 1) > 0);
		dataIdx = SetData(c2, 1, 7, dataIdx, (num & 2) > 0);
		dataIdx = SetData(c2, 3, 5, dataIdx, (num & 4) > 0);
		dataIdx = SetData(c2, 6, 2, dataIdx, (num & 8) > 0);
	}

	private void StoreData(RichCell.HeightData[] source, int sourceIdx, int cellIdx, int entryMark)
	{
		RichCell.HeightData heightData = source[sourceIdx];
		if (heightData.climbHeight > 0f || heightData.dropHeight > 0f || heightData.slope > 0f)
		{
			if (_heightDataIndices[cellIdx] == 0)
			{
				_heightDataIndices[cellIdx] = _recordedEntries << 4;
			}
			_heightData[_recordedEntries++] = heightData;
			_heightDataIndices[cellIdx] |= entryMark;
		}
	}

	private int SetData(RichCell c, int selfIdx, int neighbourIdx, int dataIdx, bool dataRecorded)
	{
		RichCell.HeightData data = ((!dataRecorded) ? default(RichCell.HeightData) : _heightData[dataIdx++]);
		c.SetHeightData(selfIdx, data);
		RichCell neighbourFromIdx = GetNeighbourFromIdx(c, selfIdx);
		if (neighbourFromIdx != null)
		{
			RichCell.HeightData data2 = new RichCell.HeightData
			{
				climbHeight = data.dropHeight,
				dropHeight = data.climbHeight,
				slope = data.slope
			};
			neighbourFromIdx.SetHeightData(neighbourIdx, data2);
		}
		return dataIdx;
	}

	private RichCell GetNeighbourFromIdx(Cell c, int selfIdx)
	{
		return selfIdx switch
		{
			0 => c.GetNeighbour(-1, -1) as RichCell, 
			1 => c.GetNeighbour(0, -1) as RichCell, 
			3 => c.GetNeighbour(-1, 0) as RichCell, 
			6 => c.GetNeighbour(-1, 1) as RichCell, 
			_ => throw new InvalidOperationException("A fault in the data recovery logic has occurred, this is a fatal error."), 
		};
	}
}
