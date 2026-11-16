namespace TrueSync.Physics2D;

internal struct ContactEdgeCloneKey(ContactCloneKey contactKey, int bodyId)
{
	public ContactCloneKey contactKey = contactKey;

	public int bodyId = bodyId;

	public void Set(ContactCloneKey contactKey, int bodyId)
	{
		this.contactKey = contactKey;
		this.bodyId = bodyId;
	}
}
