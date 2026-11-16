namespace TrueSync.Physics3D;

public abstract class Joint3D
{
	public World World { get; private set; }

	public Joint3D(World world)
	{
		World = world;
	}

	public abstract void Activate();

	public abstract void Deactivate();
}
