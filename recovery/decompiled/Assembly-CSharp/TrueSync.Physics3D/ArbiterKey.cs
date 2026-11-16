using System;

namespace TrueSync.Physics3D;

public struct ArbiterKey(RigidBody body1, RigidBody body2) : IComparable
{
	internal RigidBody body1 = body1;

	internal RigidBody body2 = body2;

	internal void SetBodies(RigidBody body1, RigidBody body2)
	{
		this.body1 = body1;
		this.body2 = body2;
	}

	public override bool Equals(object obj)
	{
		ArbiterKey arbiterKey = (ArbiterKey)obj;
		return (arbiterKey.body1.Equals(body1) && arbiterKey.body2.Equals(body2)) || (arbiterKey.body1.Equals(body2) && arbiterKey.body2.Equals(body1));
	}

	public override int GetHashCode()
	{
		return body1.GetHashCode() + body2.GetHashCode();
	}

	public int CompareTo(object obj)
	{
		if (obj is ArbiterKey arbiterKey)
		{
			long num = arbiterKey.GetHashCode();
			long num2 = GetHashCode();
			long num3 = num - num2;
			if (num3 < 0)
			{
				return 1;
			}
			if (num3 > 0)
			{
				return -1;
			}
		}
		return 0;
	}
}
