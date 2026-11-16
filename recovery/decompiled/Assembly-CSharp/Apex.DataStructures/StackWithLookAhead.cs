using System;
using System.Collections;
using System.Collections.Generic;

namespace Apex.DataStructures;

public class StackWithLookAhead<T> : IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>
{
	protected T[] _array;

	protected int _used;

	public int count => _used;

	public T this[int idx]
	{
		get
		{
			if (idx < 0 || idx >= _used)
			{
				throw new ArgumentOutOfRangeException("idx", "The stack does not contain an item at that index.");
			}
			idx = _used - 1 - idx;
			return _array[idx];
		}
	}

	public StackWithLookAhead()
		: this(4)
	{
	}

	public StackWithLookAhead(int capacity)
	{
		capacity = Math.Max(capacity, 4);
		_array = new T[capacity];
		_used = 0;
	}

	public void Clear()
	{
		Array.Clear(_array, 0, _used);
		_used = 0;
	}

	public IEnumerator<T> LookAhead(int steps)
	{
		steps = Math.Min(steps, _used);
		for (int i = 0; i < steps; i++)
		{
			yield return _array[i];
		}
	}

	public T Peek()
	{
		if (_used == 0)
		{
			throw new InvalidOperationException("The stack is empty.");
		}
		return _array[_used - 1];
	}

	public T PeekFront(int index)
	{
		return PeekBack(_used - 1 - index);
	}

	public T PeekBack(int index)
	{
		if (index > _used - 1 || index < 0)
		{
			throw new ArgumentOutOfRangeException("idx", "The stack does not contain an item at that index.");
		}
		return _array[index];
	}

	public T Last()
	{
		if (_used == 0)
		{
			throw new InvalidOperationException("The stack is empty.");
		}
		return _array[0];
	}

	public T Pop()
	{
		if (_used == 0)
		{
			throw new InvalidOperationException("The stack is empty.");
		}
		_used--;
		T result = _array[_used];
		_array[_used] = default;
		return result;
	}

	public void Push(T item)
	{
		if (_used == _array.Length)
		{
			T[] array = new T[2 * _array.Length];
			Array.Copy(_array, 0, array, 0, _used);
			_array = array;
		}
		_array[_used++] = item;
	}

	public void Append(params T[] items)
	{
		int num = _used + items.Length;
		T[] array = new T[num];
		Array.Copy(_array, 0, array, items.Length, _used);
		Array.Copy(items, 0, array, 0, items.Length);
		_array = array;
		_used = num;
	}

	public void Truncate(int itemsToRemove)
	{
		itemsToRemove = Math.Min(itemsToRemove, _used);
		_used -= itemsToRemove;
		Array.Clear(_array, _used, itemsToRemove);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		for (int i = _used - 1; i >= 0; i--)
		{
			yield return _array[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		for (int i = _used - 1; i >= 0; i--)
		{
			yield return _array[i];
		}
	}
}
