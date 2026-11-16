namespace TrueSync.Physics3D;

public class ResourcePoolCollisionIslandClone : ResourcePool<CollisionIslandClone>
{
	protected override CollisionIslandClone NewInstance()
	{
		return new CollisionIslandClone();
	}
}
