using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Services;
using Apex.Steering;
using UnityEngine;

namespace Apex.WorldGeometry;

public abstract class CellMatrixData : ScriptableObject
{
	internal class DataAccessor
	{
		private CellMatrixData _data;

		private HashSet<int> _blockedLookup;

		private int _heightIdx;

		internal int heightEntries => _data._heightEntries;

		internal DataAccessor(CellMatrixData data)
		{
			_data = data;
			_blockedLookup = new HashSet<int>(_data._blockedIndexes);
		}

		internal float GetHeight(int idx)
		{
			int[] heightIndices = _data._heightIndices;
			if (idx >= heightIndices[_heightIdx])
			{
				_heightIdx++;
			}
			return _data._heights[_heightIdx - 1];
		}

		internal void InjectData(Cell c, int cellIdx)
		{
			_data.InjectCellData(c, cellIdx);
		}

		internal bool IsPermaBlocked(int idx)
		{
			return _blockedLookup.Contains(idx);
		}
	}

	[HideInInspector]
	[SerializeField]
	private float[] _heights;

	[HideInInspector]
	[SerializeField]
	private int[] _heightIndices;

	[HideInInspector]
	[SerializeField]
	private int _heightEntries;

	[HideInInspector]
	[SerializeField]
	private int[] _blockedIndexes;

	public static CellMatrixData Create(CellMatrix matrix)
	{
		Cell cell = matrix[0, 0];
		CellMatrixData cellMatrixData;
		if (cell is RichCell)
		{
			cellMatrixData = ScriptableObject.CreateInstance<RichCellData>();
		}
		else
		{
			cellMatrixData = ((!(cell is FlatCell)) ? ((CellMatrixData)ScriptableObject.CreateInstance<StandardCellData>()) : ((CellMatrixData)ScriptableObject.CreateInstance<FlatCellData>()));
		}
		cellMatrixData.Initialize(matrix);
		return cellMatrixData;
	}

	public void Refresh(CellMatrix matrix)
	{
		Initialize(matrix);
	}

	internal DataAccessor GetAccessor()
	{
		PrepareForAccess();
		return new DataAccessor(this);
	}

	protected abstract void PrepareForInitialization(CellMatrix matrix);

	protected abstract void RecordCellData(Cell c, int cellIdx);

	protected abstract void InjectCellData(Cell c, int cellIdx);

	protected virtual void PrepareForAccess()
	{
	}

	protected virtual void FinalizeInitialization()
	{
	}

	protected T[] Shrink<T>(T[] array, int size)
	{
		T[] array2 = new T[size];
		Array.Copy(array, array2, size);
		return array2;
	}

	private void Initialize(CellMatrix cellMatrix)
	{
		int columns = cellMatrix.columns;
		int rows = cellMatrix.rows;
		Cell[,] rawMatrix = cellMatrix.rawMatrix;
		PrepareForInitialization(cellMatrix);
		List<int> list = new List<int>();
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				int num = j * columns + i;
				Cell cell = rawMatrix[i, j];
				if (!cell.IsWalkableToAny())
				{
					list.Add(num);
				}
				RecordCellData(cell, num);
			}
		}
		FinalizeInitialization();
		_blockedIndexes = list.ToArray();
		if (GameServices.heightStrategy.heightMode != HeightSamplingMode.HeightMap)
		{
			return;
		}
		VectorXZ heightMapSize = cellMatrix.heightMapSize;
		int x = heightMapSize.x;
		int z = heightMapSize.z;
		_heightEntries = cellMatrix.heightMapEntries;
		_heights = new float[x * z + 1];
		_heightIndices = new int[x * z + 1];
		int num2 = 0;
		float num3 = float.NegativeInfinity;
		for (int k = 0; k < x; k++)
		{
			for (int l = 0; l < z; l++)
			{
				int num4 = k * z + l;
				float num5 = cellMatrix.SampleHeight(k, l);
				if (num5 != num3)
				{
					num3 = num5;
					_heights[num2] = num5;
					_heightIndices[num2++] = num4;
				}
			}
		}
		_heights[num2] = 0f;
		_heightIndices[num2++] = z * x;
		_heights = Shrink(_heights, num2);
		_heightIndices = Shrink(_heightIndices, num2);
	}
}
