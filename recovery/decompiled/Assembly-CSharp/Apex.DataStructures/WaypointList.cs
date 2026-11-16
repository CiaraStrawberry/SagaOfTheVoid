using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.DataStructures;

public class WaypointList : IIterable<Vector3>, IEnumerable<Vector3>, IEnumerable, IIndexable<Vector3>
{
	private Vector3[] _array;

	private int _used;

	private int _head;

	private int _tail;

	private int _current;

	private bool _frozen;

	public int count => _used;

	public bool hasPendingWaypoint => !_frozen && _head != _current;

	public bool frozen
	{
		get
		{
			return _frozen;
		}
		set
		{
			_frozen = value;
		}
	}

	public int viaPointsCount => _current - _tail;

	public Vector3 desiredEndOfPath => _array[_current];

	public Vector3 last
	{
		get
		{
			if (_used == 0)
			{
				throw new InvalidOperationException("The list is empty.");
			}
			return _array[_head];
		}
	}

	public Vector3 this[int idx]
	{
		get
		{
			if (idx < 0 || idx >= _used)
			{
				throw new ArgumentOutOfRangeException("idx", "The list does not contain an item at that index.");
			}
			idx = (_tail + idx) % _array.Length;
			return _array[idx];
		}
	}

	public WaypointList()
		: this(4)
	{
	}

	public WaypointList(int capacity)
	{
		capacity = Math.Max(capacity, 4);
		_array = new Vector3[capacity];
		_head = (_tail = (_current = -1));
		_used = 0;
	}

	public void Clear()
	{
		Array.Clear(_array, 0, _used);
		_used = 0;
		_head = (_tail = (_current = -1));
		_frozen = false;
	}

	public void AddWaypoint(Vector3 wp, bool asCurrent = false)
	{
		if (_used == _array.Length)
		{
			Vector3[] array = new Vector3[2 * _used];
			if (_tail < _head)
			{
				Array.Copy(_array, _tail, array, 0, _used);
			}
			else
			{
				Array.Copy(_array, _tail, array, 0, _used - _tail);
				Array.Copy(_array, 0, array, _used - _tail, _head + 1);
			}
			_array = array;
			_tail = 0;
			_current = _head;
			_head = _used - 1;
		}
		else if (_tail < 0)
		{
			_tail = 0;
		}
		_head = (_head + 1) % _array.Length;
		_used++;
		_array[_head] = wp;
		if (asCurrent)
		{
			_current = _head;
		}
	}

	public Vector3 NextWaypoint()
	{
		if (_head == _current)
		{
			throw new InvalidOperationException("There is no next waypoint.");
		}
		_current = (_current + 1) % _array.Length;
		return _array[_current];
	}

	public void ConsumeViaPoint()
	{
		if (_tail != _current)
		{
			ref Vector3 reference = ref _array[_tail];
			reference = Vector3.zero;
			_tail = (_tail + 1) % _array.Length;
			_used--;
		}
	}

	public Vector3[] GetViaPoints()
	{
		if (_tail == _current)
		{
			return null;
		}
		Vector3[] array;
		if (_tail < _current)
		{
			int num = _current - _tail;
			array = new Vector3[num];
			Array.Copy(_array, _tail, array, 0, num);
		}
		else
		{
			int num2 = _array.Length - _tail;
			array = new Vector3[num2 + _current];
			Array.Copy(_array, _tail, array, 0, num2);
			Array.Copy(_array, 0, array, num2, _current);
		}
		return array;
	}

	public Vector3[] GetPending()
	{
		Vector3[] array;
		if (_tail == _head)
		{
			array = new Vector3[1] { _array[_head] };
		}
		else if (_tail < _head)
		{
			int num = _head - _tail;
			array = new Vector3[num];
			Array.Copy(_array, _tail + 1, array, 0, num);
		}
		else
		{
			int num2 = _array.Length - (_tail + 1);
			array = new Vector3[num2 + _head + 1];
			Array.Copy(_array, _tail + 1, array, 0, num2);
			Array.Copy(_array, 0, array, num2, _head + 1);
		}
		return array;
	}

	IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator()
	{
		for (int i = 0; i < _used; i++)
		{
			int idx = (_tail + i) % _array.Length;
			yield return _array[idx];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		for (int i = 0; i < _used; i++)
		{
			int idx = (_tail + i) % _array.Length;
			yield return _array[idx];
		}
	}
}
