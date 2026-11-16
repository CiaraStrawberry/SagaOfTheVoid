namespace TrueSync.Physics3D;

public enum TSRigidBodyConstraints
{
	None = 0,
	FreezePositionX = 2,
	FreezePositionY = 4,
	FreezePositionZ = 8,
	FreezePosition = 14,
	FreezeRotationX = 16,
	FreezeRotationY = 32,
	FreezeRotationZ = 64,
	FreezeRotation = 112,
	FreezeAll = 126
}
