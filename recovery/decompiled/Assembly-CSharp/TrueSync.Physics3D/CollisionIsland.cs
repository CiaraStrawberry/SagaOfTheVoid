namespace TrueSync.Physics3D;

public class CollisionIsland : ResourcePoolItem
{
	internal IslandManager islandManager;

	internal HashList<RigidBody> bodies = new HashList<RigidBody>();

	internal HashList<Arbiter> arbiter = new HashList<Arbiter>();

	internal HashList<Constraint> constraints = new HashList<Constraint>();

	private ReadOnlyHashset<RigidBody> readOnlyBodies;

	private ReadOnlyHashset<Arbiter> readOnlyArbiter;

	private ReadOnlyHashset<Constraint> readOnlyConstraints;

	public ReadOnlyHashset<RigidBody> Bodies => readOnlyBodies;

	public ReadOnlyHashset<Arbiter> Arbiter => readOnlyArbiter;

	public ReadOnlyHashset<Constraint> Constraints => readOnlyConstraints;

	public CollisionIsland()
	{
		readOnlyBodies = new ReadOnlyHashset<RigidBody>(bodies);
		readOnlyArbiter = new ReadOnlyHashset<Arbiter>(arbiter);
		readOnlyConstraints = new ReadOnlyHashset<Constraint>(constraints);
	}

	public void CleanUp()
	{
		bodies.Clear();
		arbiter.Clear();
		constraints.Clear();
	}

	public bool IsActive()
	{
		return true;
	}

	public void SetStatus(bool active)
	{
		foreach (RigidBody body in bodies)
		{
			body.IsActive = active;
			if (active && !body.IsActive)
			{
				body.inactiveTime = FP.Zero;
			}
		}
	}

	internal void ClearLists()
	{
		arbiter.Clear();
		bodies.Clear();
		constraints.Clear();
	}
}
