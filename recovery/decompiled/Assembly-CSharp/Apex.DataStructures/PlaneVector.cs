using UnityEngine;

namespace Apex.DataStructures;

public struct PlaneVector
{
	public float x;

	public float z;

	public static PlaneVector zero => new PlaneVector(0f, 0f);

	public float magnitude => Magnitude(this);

	public float sqrMagnitude => x * x + z * z;

	public PlaneVector normalized => Normalize(this);

	public PlaneVector(float x, float z)
	{
		this.x = x;
		this.z = z;
	}

	public PlaneVector(Vector3 vector)
	{
		x = vector.x;
		z = vector.z;
	}

	public static PlaneVector operator +(PlaneVector a, PlaneVector b)
	{
		return new PlaneVector(a.x + b.x, a.z + b.z);
	}

	public static PlaneVector operator -(PlaneVector a, PlaneVector b)
	{
		return new PlaneVector(a.x - b.x, a.z - b.z);
	}

	public static PlaneVector operator /(PlaneVector a, float d)
	{
		return new PlaneVector(a.x / d, a.z / d);
	}

	public static implicit operator Vector3(PlaneVector vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	public static implicit operator PlaneVector(Vector3 vector)
	{
		return new PlaneVector(vector.x, vector.z);
	}

	public static bool operator ==(PlaneVector lhs, PlaneVector rhs)
	{
		return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
	}

	public static bool operator !=(PlaneVector lhs, PlaneVector rhs)
	{
		return SqrMagnitude(lhs - rhs) >= 9.9999994E-11f;
	}

	public static PlaneVector Normalize(PlaneVector value)
	{
		float num = Magnitude(value);
		if (num <= 1E-05f)
		{
			return zero;
		}
		return value / num;
	}

	public static float Magnitude(PlaneVector a)
	{
		return Mathf.Sqrt(a.x * a.x + a.z * a.z);
	}

	public static float SqrMagnitude(PlaneVector a)
	{
		return a.x * a.x + a.z * a.z;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is PlaneVector))
		{
			return false;
		}
		return SqrMagnitude((PlaneVector)obj - this) < 9.9999994E-11f;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (z.GetHashCode() << 2);
	}
}
