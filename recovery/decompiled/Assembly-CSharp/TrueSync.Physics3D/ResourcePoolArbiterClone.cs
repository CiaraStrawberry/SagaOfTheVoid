namespace TrueSync.Physics3D;

public class ResourcePoolArbiterClone : ResourcePool<ArbiterClone>
{
	protected override ArbiterClone NewInstance()
	{
		return new ArbiterClone();
	}
}
