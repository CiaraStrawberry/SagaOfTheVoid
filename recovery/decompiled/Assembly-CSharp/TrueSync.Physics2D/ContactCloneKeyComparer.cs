using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class ContactCloneKeyComparer : IEqualityComparer<ContactCloneKey>
{
	public bool Equals(ContactCloneKey x, ContactCloneKey y)
	{
		return x.keyA == y.keyA && x.keyB == y.keyB;
	}

	public int GetHashCode(ContactCloneKey obj)
	{
		return obj.keyA.GetHashCode() * 33 + obj.keyB.GetHashCode() * 17;
	}
}
