using System;
using System.Runtime.InteropServices;

namespace TrueSync.Physics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MassData : IEquatable<MassData>
{
	public FP Area { get; internal set; }

	public TSVector2 Centroid { get; internal set; }

	public FP Inertia { get; internal set; }

	public FP Mass { get; internal set; }

	public static bool operator ==(MassData left, MassData right)
	{
		return left.Area == right.Area && left.Mass == right.Mass && left.Centroid == right.Centroid && left.Inertia == right.Inertia;
	}

	public static bool operator !=(MassData left, MassData right)
	{
		return !(left == right);
	}

	public bool Equals(MassData other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(null, obj))
		{
			return false;
		}
		if (obj.GetType() != typeof(MassData))
		{
			return false;
		}
		return Equals((MassData)obj);
	}

	public override int GetHashCode()
	{
		int hashCode = Area.GetHashCode();
		hashCode = (hashCode * 397) ^ Centroid.GetHashCode();
		hashCode = (hashCode * 397) ^ Inertia.GetHashCode();
		return (hashCode * 397) ^ Mass.GetHashCode();
	}
}
