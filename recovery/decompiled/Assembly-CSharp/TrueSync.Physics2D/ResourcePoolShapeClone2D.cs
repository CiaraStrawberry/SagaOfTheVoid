namespace TrueSync.Physics2D;

internal class ResourcePoolShapeClone2D : ResourcePool<GenericShapeClone2D>
{
	protected override GenericShapeClone2D NewInstance()
	{
		return new GenericShapeClone2D();
	}
}
