namespace TrueSync.Physics2D;

internal class ResourcePoolBodyClone2D : ResourcePool<BodyClone2D>
{
	protected override BodyClone2D NewInstance()
	{
		return new BodyClone2D();
	}
}
