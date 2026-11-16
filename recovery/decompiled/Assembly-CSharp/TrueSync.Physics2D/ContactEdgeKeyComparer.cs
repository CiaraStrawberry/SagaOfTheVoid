using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class ContactEdgeKeyComparer : IEqualityComparer<ContactEdgeCloneKey>
{
	public bool Equals(ContactEdgeCloneKey x, ContactEdgeCloneKey y)
	{
		return x.contactKey.Equals(y.contactKey) && x.bodyId == y.bodyId;
	}

	public int GetHashCode(ContactEdgeCloneKey obj)
	{
		return obj.contactKey.GetHashCode() * 33 + obj.bodyId.GetHashCode();
	}
}
