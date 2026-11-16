namespace TrueSync.Physics2D;

internal class ResourcePoolContactEdge2D : ResourcePool<ContactEdge>
{
	protected override ContactEdge NewInstance()
	{
		return new ContactEdge();
	}
}
