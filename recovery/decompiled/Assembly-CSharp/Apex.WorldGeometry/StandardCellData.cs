using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class StandardCellData : CellMatrixData
{
	[HideInInspector]
	[SerializeField]
	private NeighbourPosition[] _heightBlockStatus;

	protected override void PrepareForInitialization(CellMatrix matrix)
	{
		_heightBlockStatus = new NeighbourPosition[matrix.columns * matrix.rows];
	}

	protected override void RecordCellData(Cell c, int cellIdx)
	{
		StandardCell standardCell = c as StandardCell;
		_heightBlockStatus[cellIdx] = standardCell.heightBlockedFrom;
	}

	protected override void InjectCellData(Cell c, int cellIdx)
	{
		StandardCell standardCell = c as StandardCell;
		standardCell.heightBlockedFrom = _heightBlockStatus[cellIdx];
	}
}
