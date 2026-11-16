namespace TrueSync.Physics3D;

public class ResourcePoolGenericShapeClone : ResourcePool<GenericShapeClone>
{
	protected override GenericShapeClone NewInstance()
	{
		return new GenericShapeClone();
	}
}
