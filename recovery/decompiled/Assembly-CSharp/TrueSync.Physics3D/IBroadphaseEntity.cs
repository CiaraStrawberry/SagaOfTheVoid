namespace TrueSync.Physics3D;

public interface IBroadphaseEntity
{
	TSBBox BoundingBox { get; }

	bool IsStaticOrInactive { get; }

	bool IsStaticNonKinematic { get; }
}
