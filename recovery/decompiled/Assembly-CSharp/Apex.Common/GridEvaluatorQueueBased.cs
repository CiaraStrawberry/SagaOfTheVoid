using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Common;

public class GridEvaluatorQueueBased
{
	private SimpleQueue<Cell> _queue;

	private HashSet<Cell> _set;

	private Cell[] _buffer;

	public GridEvaluatorQueueBased(int bufferInitialSize)
	{
		_queue = new SimpleQueue<Cell>(bufferInitialSize);
		_set = new HashSet<Cell>();
		_buffer = new Cell[8];
	}

	public Cell BestMatch(IGrid grid, Vector3 start, Func<Cell, bool> match, Func<Cell, bool> discard)
	{
		Cell cell = grid.GetCell(start, adjustToBounds: true);
		if (cell == null)
		{
			return null;
		}
		_set.Add(cell);
		_queue.Enqueue(cell);
		while (_queue.count > 0)
		{
			Cell cell2 = _queue.Dequeue();
			int neighbours = cell2.GetNeighbours(_buffer);
			for (int i = 0; i < neighbours; i++)
			{
				Cell cell3 = _buffer[i];
				if (_set.Add(cell3))
				{
					if (match(cell3))
					{
						Reset();
						return cell3;
					}
					if (!discard(cell3))
					{
						_queue.Enqueue(cell3);
					}
				}
			}
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
