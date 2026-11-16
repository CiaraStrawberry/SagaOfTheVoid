using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Common;

public class GridEvaluatorClosestMatch
{
	private struct CellItem
	{
		internal Cell cell;

		internal float priority;
	}

	private class ItemComparerMin : IComparer<CellItem>
	{
		public static readonly IComparer<CellItem> instance = new ItemComparerMin();

		public int Compare(CellItem x, CellItem y)
		{
			return y.priority.CompareTo(x.priority);
		}
	}

	private BinaryHeap<CellItem> _queue;

	private HashSet<Cell> _set;

	private Cell[] _buffer;

	public GridEvaluatorClosestMatch(int bufferInitialSize)
	{
		_queue = new BinaryHeap<CellItem>(bufferInitialSize, ItemComparerMin.instance);
		_set = new HashSet<Cell>();
		_buffer = new Cell[8];
	}

	public Cell ClosestMatch(IGrid grid, Vector3 start, Func<Cell, bool> match, Func<Cell, bool> discard)
	{
		Cell cell = grid.GetCell(start, adjustToBounds: true);
		_set.Add(cell);
		Vector3 vector = start;
		Cell cell2 = cell;
		CellItem cellItem = new CellItem
		{
			cell = cell,
			priority = 0f
		};
		while (true)
		{
			if (match(cell2))
			{
				Reset();
				return cell2;
			}
			int neighbours = cell2.GetNeighbours(_buffer);
			for (int i = 0; i < neighbours; i++)
			{
				Cell cell3 = _buffer[i];
				if (_set.Add(cell3) && !discard(cell3))
				{
					CellItem item = new CellItem
					{
						cell = cell3,
						priority = cellItem.priority + (cell3.position - vector).sqrMagnitude
					};
					_queue.Add(item);
				}
			}
			if (_queue.count == 0)
			{
				break;
			}
			cellItem = _queue.Remove();
			cell2 = cellItem.cell;
			vector = cell2.position;
		}
		Reset();
		return null;
	}

	private void Reset()
	{
		_set.Clear();
		_queue.Clear();
		Array.Clear(_buffer, 0, _buffer.Length);
	}
}
