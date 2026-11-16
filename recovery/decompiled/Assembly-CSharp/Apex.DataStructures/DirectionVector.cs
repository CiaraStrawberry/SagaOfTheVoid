using UnityEngine;

namespace Apex.DataStructures;

public struct DirectionVector(float x, float y, float z)
{
	private float _x = Clamp(x);

	private float _y = Clamp(y);

	private float _z = Clamp(z);

	public static DirectionVector left => new DirectionVector(-1f, 0f, 0f);

	public static DirectionVector right => new DirectionVector(1f, 0f, 0f);

	public static DirectionVector forward => new DirectionVector(0f, 0f, 1f);

	public static DirectionVector back => new DirectionVector(0f, 0f, -1f);

	public static DirectionVector up => new DirectionVector(0f, 1f, 0f);

	public static DirectionVector down => new DirectionVector(0f, -1f, 0f);

	public float x => _x;

	public float y => _y;

	public float z => _z;

	public static implicit operator Vector3(DirectionVector vector)
	{
		return new Vector3(vector._x, vector._y, vector._z);
	}

	public static implicit operator DirectionVector(Vector3 vector)
	{
		return new DirectionVector(vector.x, vector.y, vector.z);
	}

	public static DirectionVector operator +(DirectionVector a, DirectionVector b)
	{
		return new DirectionVector(a._x + b._x, a._y + b._y, a._z + b._z);
	}

	public static DirectionVector operator -(DirectionVector a, DirectionVector b)
	{
		return new DirectionVector(a._x - b._x, a._y - b._y, a._z - b._z);
	}

	public static Vector3 operator *(Vector3 a, DirectionVector b)
	{
		return new Vector3(a.x * b._x, a.y * b._y, a.z * b.z);
	}

	private static float Clamp(float f)
	{
		if (f == 0f)
		{
			return f;
		}
		if (f > 0f)
		{
			return 1f;
		}
		return -1f;
	}
}
