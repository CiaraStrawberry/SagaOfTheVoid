using System;
using System.Collections;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class ReadOnlyHashset<T> : IEnumerable, IEnumerable<T> where T : IComparable
{
	private HashList<T> hashset;

	public int Count => hashset.Count;

	public ReadOnlyHashset(HashList<T> hashset)
	{
		this.hashset = hashset;
	}

	public IEnumerator GetEnumerator()
	{
		return hashset.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return hashset.GetEnumerator();
	}

	public bool Contains(T item)
	{
		return hashset.Contains(item);
	}
}
