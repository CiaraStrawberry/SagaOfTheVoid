namespace TrueSync.Physics3D;

public class ResourcePoolRigidBodyClone : ResourcePool<RigidBodyClone>
{
	protected override RigidBodyClone NewInstance()
	{
		return new RigidBodyClone();
	}
}
