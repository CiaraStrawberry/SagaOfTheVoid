using System;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

internal class HighPointList
{
	private Vector3[] _points;

	private int _head;

	private int _used;

	private Vector3 _lastHigh;

	private float _lastDelta = -10000f;

	internal Vector3 current => _points[_head];

	internal float currentHeight => _points[_head].y;

	internal int count => _used;

	internal HighPointList(int capacity, Vector3 startHigh)
	{
		_points = new Vector3[capacity];
		_lastHigh = startHigh;
	}

	internal void MoveNext()
	{
		_used--;
		_head = (_head + 1) % _points.Length;
	}

	internal void RegisterHighpoint(Vector3 proposed)
	{
		Vector3 lastHigh = _lastHigh;
		bool flag = _lastDelta > 0f && proposed.y <= _lastHigh.y;
		_lastDelta = proposed.y - _lastHigh.y;
		_lastHigh = proposed;
		if (!flag)
		{
			return;
		}
		int num = _points.Length;
		for (int i = 0; i < _used; i++)
		{
			int num2 = (_head + i) % num;
			if (_points[num2].y <= lastHigh.y)
			{
				_points[num2] = lastHigh;
				_used = i + 1;
				return;
			}
		}
		if (_used == num)
		{
			Vector3[] array = new Vector3[2 * num];
			if (_head == 0)
			{
				Array.Copy(_points, 0, array, 0, _used);
			}
			else
			{
				Array.Copy(_points, _head, array, 0, num - _head);
				Array.Copy(_points, 0, array, num - _head, _head);
			}
			_points = array;
			_head = 0;
			num = _points.Length;
		}
		int num3 = (_head + _used) % num;
		_used++;
		_points[num3] = lastHigh;
	}
}
