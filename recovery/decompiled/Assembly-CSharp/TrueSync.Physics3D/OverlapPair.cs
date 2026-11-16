using System;

namespace TrueSync.Physics3D;

public class OverlapPair : IComparable
{
	public IBroadphaseEntity Entity1;

	public IBroadphaseEntity Entity2;

	public OverlapPair(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		Entity1 = entity1;
		Entity2 = entity2;
	}

	internal void SetBodies(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		Entity1 = entity1;
		Entity2 = entity2;
	}

	public override bool Equals(object obj)
	{
		OverlapPair overlapPair = (OverlapPair)obj;
		return (overlapPair.Entity1.Equals(Entity1) && overlapPair.Entity2.Equals(Entity2)) || (overlapPair.Entity1.Equals(Entity2) && overlapPair.Entity2.Equals(Entity1));
	}

	public override int GetHashCode()
	{
		return Entity1.GetHashCode() + Entity2.GetHashCode();
	}

	public int CompareTo(object obj)
	{
		if (obj is OverlapPair)
		{
			long num = ((OverlapPair)obj).GetHashCode();
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
