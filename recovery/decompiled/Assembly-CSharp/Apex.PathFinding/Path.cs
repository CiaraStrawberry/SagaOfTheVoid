using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public class Path : StackWithLookAhead<IPositioned>, ICloneable
{
	private float _length = -1f;

	public float length
	{
		get
		{
			if (_length < 0f)
			{
				CalculateLength();
			}
			return _length;
		}
	}

	public Path()
	{
	}

	public Path(int capacity)
		: base(capacity)
	{
	}

	public Path(params Vector3[] path)
		: base(path.Length)
	{
		Ensure.ArgumentNotNull(path, "path");
		for (int num = path.Length - 1; num >= 0; num--)
		{
			Push(path[num].AsPositioned());
		}
	}

	public Path(IIndexable<Vector3> path)
		: base(path.count)
	{
		Ensure.ArgumentNotNull(path, "path");
		for (int num = path.count - 1; num >= 0; num--)
		{
			Push(path[num].AsPositioned());
		}
	}

	public Path(IIndexable<IPositioned> path)
		: base(path.count)
	{
		Ensure.ArgumentNotNull(path, "path");
		for (int num = path.count - 1; num >= 0; num--)
		{
			Push(path[num]);
		}
	}

	public static Path FromSegments(IIndexable<Path> segments)
	{
		int num = segments.count;
		switch (num)
		{
		case 0:
			return null;
		case 1:
		{
			Path path2 = segments[0];
			path2._array[0] = new Waypoint(path2._array[0].position);
			return path2;
		}
		default:
		{
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += segments[i].count;
			}
			num2 -= num - 1;
			Path path = new Path(num2);
			path._used = num2;
			IPositioned[] array = path._array;
			int num3 = 0;
			for (int num4 = num - 1; num4 >= 0; num4--)
			{
				Path path2 = segments[num4];
				IPositioned[] array2 = path2._array;
				array[num3++] = new Waypoint(array2[0].position);
				int num5 = ((num4 == 0) ? 1 : 2);
				int num6 = path2.count - num5;
				if (num6 > 0)
				{
					Array.Copy(array2, 1, array, num3, num6);
					num3 += num6;
				}
			}
			return path;
		}
		}
	}

	public Path AppendSegment(Path segment)
	{
		int used = segment._used;
		if (used == 0)
		{
			return this;
		}
		if (_used == 0)
		{
			segment.Pop();
			return segment;
		}
		int num = _used + used - 1;
		IPositioned[] array = new IPositioned[num];
		IPositioned[] array2 = segment._array;
		Array.Copy(array2, array, used - 1);
		Array.Copy(_array, 0, array, used - 1, _used);
		_used = num;
		_array = array;
		return this;
	}

	public IEnumerable<Vector3> GetPoints()
	{
		int limit = count;
		for (int i = 0; i < limit; i++)
		{
			if (!(base[i] is IPortalNode))
			{
				yield return base[i].position;
			}
		}
	}

	public float CalculateSquaredLength()
	{
		float num = 0f;
		int num2 = count - 2;
		for (int i = 0; i <= num2; i++)
		{
			IPositioned positioned = base[i];
			IPositioned positioned2 = base[i + 1];
			if (positioned2 is IPortalNode)
			{
				positioned2 = base[++i + 1];
			}
			num += (positioned.position - positioned2.position).sqrMagnitude;
		}
		return num;
	}

	public float CalculateLength()
	{
		float num = 0f;
		int num2 = count - 2;
		for (int i = 0; i <= num2; i++)
		{
			IPositioned positioned = base[i];
			IPositioned positioned2 = base[i + 1];
			if (positioned2 is IPortalNode)
			{
				positioned2 = base[++i + 1];
			}
			num += (positioned.position - positioned2.position).magnitude;
		}
		_length = num;
		return num;
	}

	public void Update(params Vector3[] path)
	{
		Clear();
		for (int num = path.Length - 1; num >= 0; num--)
		{
			Push(path[num].AsPositioned());
		}
	}

	public void Update(IIndexable<Vector3> path)
	{
		Clear();
		for (int num = path.count - 1; num >= 0; num--)
		{
			Push(path[num].AsPositioned());
		}
		_length = -1f;
	}

	public Path Clone()
	{
		return new Path(this);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}
}
