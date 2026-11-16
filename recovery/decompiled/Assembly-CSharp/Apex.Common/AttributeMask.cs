using System;

namespace Apex.Common;

public struct AttributeMask : IEquatable<AttributeMask>
{
	public static readonly AttributeMask None = 0;

	public static readonly AttributeMask All = -1;

	public int value;

	public static implicit operator int(AttributeMask mask)
	{
		return mask.value;
	}

	public static implicit operator AttributeMask(int val)
	{
		return new AttributeMask
		{
			value = val
		};
	}

	public static bool operator ==(AttributeMask me, AttributeMask other)
	{
		return me.value == other.value;
	}

	public static bool operator !=(AttributeMask me, AttributeMask other)
	{
		return me.value != other.value;
	}

	public bool Equals(AttributeMask other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AttributeMask attributeMask))
		{
			return false;
		}
		return attributeMask.value == value;
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}
}
