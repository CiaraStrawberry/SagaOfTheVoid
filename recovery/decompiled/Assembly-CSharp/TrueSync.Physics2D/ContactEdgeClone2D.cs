using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class ContactEdgeClone2D
{
	public ContactCloneKey contactKey;

	public Body body;

	public ContactEdgeCloneKey contactEdgeCloneKey = default;

	public ContactEdgeCloneKey nextEdge = default;

	public ContactEdgeCloneKey previousEdge = default;

	public void Clone(ContactEdge contactEdge)
	{
		contactKey = contactEdge.Contact.Key;
		body = contactEdge.Other;
		contactEdgeCloneKey.Set(contactKey, body.BodyId);
		if (contactEdge.Next != null)
		{
			nextEdge.Set(contactEdge.Next.Contact.Key, contactEdge.Next.Other.BodyId);
		}
		else
		{
			nextEdge.Set(nextEdge.contactKey, -1);
		}
		if (contactEdge.Prev != null)
		{
			previousEdge.Set(contactEdge.Prev.Contact.Key, contactEdge.Prev.Other.BodyId);
		}
		else
		{
			previousEdge.Set(previousEdge.contactKey, -1);
		}
	}

	public ContactEdge Restore(bool restoreLinks, Dictionary<ContactCloneKey, Contact> contactDic, Dictionary<ContactEdgeCloneKey, ContactEdge> contactEdgeDic)
	{
		if (restoreLinks)
		{
			ContactEdge contactEdge = contactEdgeDic[contactEdgeCloneKey];
			if (nextEdge.bodyId != -1)
			{
				contactEdge.Next = contactEdgeDic[nextEdge];
			}
			if (previousEdge.bodyId != -1)
			{
				contactEdge.Prev = contactEdgeDic[previousEdge];
			}
			return contactEdge;
		}
		if (contactEdgeDic.ContainsKey(contactEdgeCloneKey))
		{
			return contactEdgeDic[contactEdgeCloneKey];
		}
		ContactEdge contactEdge2 = WorldClone2D.poolContactEdge.GetNew();
		contactEdge2.Contact = contactDic[contactKey];
		contactEdge2.Other = body;
		contactEdgeDic[contactEdgeCloneKey] = contactEdge2;
		return contactEdge2;
	}
}
