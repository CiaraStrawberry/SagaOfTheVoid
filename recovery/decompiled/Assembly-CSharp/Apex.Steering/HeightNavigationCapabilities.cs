using System;
using Apex.Utilities;

namespace Apex.Steering;

[Serializable]
public struct HeightNavigationCapabilities
{
	[MinCheck(0f)]
	public float maxSlopeAngle;

	[MinCheck(0f)]
	public float maxClimbHeight;

	[MinCheck(0f)]
	public float maxDropHeight;

	public static bool operator ==(HeightNavigationCapabilities lhs, HeightNavigationCapabilities rhs)
	{
		return lhs.maxSlopeAngle.Equals(rhs.maxSlopeAngle) && lhs.maxClimbHeight.Equals(rhs.maxClimbHeight) && lhs.maxClimbHeight.Equals(rhs.maxClimbHeight);
	}

	public static bool operator !=(HeightNavigationCapabilities lhs, HeightNavigationCapabilities rhs)
	{
		return !lhs.maxSlopeAngle.Equals(rhs.maxSlopeAngle) || !lhs.maxClimbHeight.Equals(rhs.maxClimbHeight) || !lhs.maxClimbHeight.Equals(rhs.maxClimbHeight);
	}

	public override int GetHashCode()
	{
		return maxSlopeAngle.GetHashCode() ^ (maxClimbHeight.GetHashCode() << 2) ^ (maxDropHeight.GetHashCode() >> 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is HeightNavigationCapabilities heightNavigationCapabilities))
		{
			return false;
		}
		return maxSlopeAngle.Equals(heightNavigationCapabilities.maxSlopeAngle) && maxClimbHeight.Equals(heightNavigationCapabilities.maxClimbHeight) && maxClimbHeight.Equals(heightNavigationCapabilities.maxClimbHeight);
	}
}
