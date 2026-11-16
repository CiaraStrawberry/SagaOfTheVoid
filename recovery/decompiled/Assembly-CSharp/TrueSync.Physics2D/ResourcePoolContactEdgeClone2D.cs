namespace TrueSync.Physics2D;

internal class ResourcePoolContactEdgeClone2D : ResourcePool<ContactEdgeClone2D>
{
	protected override ContactEdgeClone2D NewInstance()
	{
		return new ContactEdgeClone2D();
	}
}
