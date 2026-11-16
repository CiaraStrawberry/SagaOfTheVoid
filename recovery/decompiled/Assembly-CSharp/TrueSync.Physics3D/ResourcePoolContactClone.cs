namespace TrueSync.Physics3D;

public class ResourcePoolContactClone : ResourcePool<ContactClone>
{
	protected override ContactClone NewInstance()
	{
		return new ContactClone();
	}
}
