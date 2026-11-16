using System;
using Apex.Steering;
using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public class RichCell : Cell
{
	[Serializable]
	public struct HeightData(float slope, float climbHeight, float dropHeight)
	{
		public float slope = slope;

		public float climbHeight = climbHeight;

		public float dropHeight = dropHeight;
	}

	private class RichCellFactory : ICellFactory
	{
		public Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		{
			return new RichCell(parent, position, matrixPosX, matrixPosZ, blocked);
		}
	}

	public static readonly ICellFactory factory = new RichCellFactory();

	private HeightData[] _heightData;

	private HeightData _worstCase;

	public RichCell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
		: base(parent, position, matrixPosX, matrixPosZ, blocked)
	{
		_heightData = new HeightData[9];
		for (int i = 0; i < 9; i++)
		{
			_heightData[i].slope = 100f;
		}
		_worstCase = default;
	}

	public override bool IsWalkableFromAllDirections(IUnitProperties unitProps)
	{
		if (!IsWalkableWithClearance(unitProps))
		{
			return false;
		}
		HeightNavigationCapabilities heightNavigationCapability = unitProps.heightNavigationCapability;
		return (heightNavigationCapability.maxClimbHeight >= _worstCase.dropHeight && heightNavigationCapability.maxDropHeight >= _worstCase.climbHeight) || heightNavigationCapability.maxSlopeAngle >= _worstCase.slope;
	}

	public override bool IsWalkableFrom(IGridCell neighbour, IUnitProperties unitProps)
	{
		if (!IsWalkable(unitProps.attributes))
		{
			return false;
		}
		int num = Mathf.Clamp(neighbour.matrixPosX - matrixPosX, -1, 1);
		int num2 = Mathf.Clamp(neighbour.matrixPosZ - matrixPosZ, -1, 1);
		int num3 = num + 3 * num2 + 4;
		HeightNavigationCapabilities heightNavigationCapability = unitProps.heightNavigationCapability;
		HeightData heightData = _heightData[num3];
		return (heightNavigationCapability.maxClimbHeight >= heightData.dropHeight && heightNavigationCapability.maxDropHeight >= heightData.climbHeight) || heightNavigationCapability.maxSlopeAngle >= heightData.slope;
	}

	public override bool IsWalkableFromWithClearance(IGridCell neighbour, IUnitProperties unitProps)
	{
		if (!IsWalkableWithClearance(unitProps))
		{
			return false;
		}
		int num = Mathf.Clamp(neighbour.matrixPosX - matrixPosX, -1, 1);
		int num2 = Mathf.Clamp(neighbour.matrixPosZ - matrixPosZ, -1, 1);
		int num3 = num + 3 * num2 + 4;
		HeightNavigationCapabilities heightNavigationCapability = unitProps.heightNavigationCapability;
		HeightData heightData = _heightData[num3];
		return (heightNavigationCapability.maxClimbHeight >= heightData.dropHeight && heightNavigationCapability.maxDropHeight >= heightData.climbHeight) || heightNavigationCapability.maxSlopeAngle >= heightData.slope;
	}

	internal bool HasInitializedHeightData(int dx, int dz)
	{
		int num = dx + 3 * dz + 4;
		return _heightData[num].slope < 100f;
	}

	internal void SetHeightData(int dx, int dz, HeightData data)
	{
		int num = dx + 3 * dz + 4;
		_heightData[num] = data;
	}

	internal void SetHeightData(int idx, HeightData data)
	{
		_heightData[idx] = data;
	}

	internal HeightData[] GetHeightData()
	{
		return _heightData;
	}

	internal void CalculateWorst()
	{
		for (int i = 0; i < 9; i++)
		{
			if (i == 4)
			{
				i++;
			}
			HeightData heightData = _heightData[i];
			if (_worstCase.dropHeight < heightData.dropHeight)
			{
				_worstCase.dropHeight = heightData.dropHeight;
			}
			if (_worstCase.climbHeight < heightData.climbHeight)
			{
				_worstCase.climbHeight = heightData.climbHeight;
			}
			if (_worstCase.slope < heightData.slope)
			{
				_worstCase.slope = heightData.slope;
			}
		}
	}

	internal override void EnsureDataForEditor()
	{
		InjectDataFrom(0, 1);
		InjectDataFrom(1, 1);
		InjectDataFrom(1, 0);
		InjectDataFrom(1, -1);
	}

	internal HeightData GetHeightData(IGridCell neighbour)
	{
		int num = Mathf.Clamp(neighbour.matrixPosX - matrixPosX, -1, 1);
		int num2 = Mathf.Clamp(neighbour.matrixPosZ - matrixPosZ, -1, 1);
		int num3 = num + 3 * num2 + 4;
		return _heightData[num3];
	}

	private void InjectDataFrom(int dx, int dz)
	{
		if (parent[matrixPosX + dx, matrixPosZ + dz] is RichCell richCell)
		{
			int num = dx + 3 * dz + 4;
			int num2 = -dx + 3 * -dz + 4;
			ref HeightData reference = ref _heightData[num];
			reference = richCell._heightData[num2];
		}
	}
}
