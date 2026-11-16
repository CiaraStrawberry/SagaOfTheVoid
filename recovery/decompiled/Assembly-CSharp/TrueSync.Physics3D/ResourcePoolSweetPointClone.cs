namespace TrueSync.Physics3D;

public class ResourcePoolSweetPointClone : ResourcePool<SweetPointClone>
{
	protected override SweetPointClone NewInstance()
	{
		return new SweetPointClone();
	}
}
