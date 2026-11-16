namespace TrueSync;

public class PhysicsManager
{
	public enum PhysicsType
	{
		W_2D,
		W_3D
	}

	public static IPhysicsManager instance;

	public static IPhysicsManager New(TrueSyncConfig trueSyncConfig)
	{
		if (trueSyncConfig.physics3DEnabled)
		{
			instance = new PhysicsWorldManager();
			instance.Gravity = trueSyncConfig.gravity3D;
			instance.SpeculativeContacts = trueSyncConfig.speculativeContacts3D;
		}
		else if (trueSyncConfig.physics2DEnabled)
		{
			instance = new Physics2DWorldManager();
			instance.Gravity = new TSVector(trueSyncConfig.gravity2D.x, trueSyncConfig.gravity2D.y, 0);
			instance.SpeculativeContacts = trueSyncConfig.speculativeContacts2D;
		}
		return instance;
	}

	internal static void InitTest3D()
	{
		instance = new PhysicsWorldManager();
		instance.Gravity = new TSVector(0, -10, 0);
		instance.LockedTimeStep = 0.02f;
		instance.Init();
	}

	internal static void InitTest2D()
	{
		instance = new Physics2DWorldManager();
		instance.Gravity = new TSVector(0, -10, 0);
		instance.LockedTimeStep = 0.02f;
		instance.Init();
	}
}
