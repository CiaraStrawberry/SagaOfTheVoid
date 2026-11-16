using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CollisionIslandClone
{
	public List<RigidBody> bodies = new List<RigidBody>();

	public List<ArbiterClone> arbiters = new List<ArbiterClone>();

	public List<Constraint> constraints = new List<Constraint>();

	private int index;

	private int length;

	public void Reset()
	{
		index = 0;
		for (length = arbiters.Count; index < length; index++)
		{
			ArbiterClone arbiterClone = arbiters[index];
			arbiterClone.Reset();
			WorldClone.poolArbiterClone.GiveBack(arbiterClone);
		}
	}

	public void Clone(CollisionIsland ci)
	{
		bodies.Clear();
		index = 0;
		for (length = ci.bodies.Count; index < length; index++)
		{
			bodies.Add(ci.bodies[index]);
		}
		arbiters.Clear();
		index = 0;
		for (length = ci.arbiter.Count; index < length; index++)
		{
			ArbiterClone arbiterClone = WorldClone.poolArbiterClone.GetNew();
			arbiterClone.Clone(ci.arbiter[index]);
			arbiters.Add(arbiterClone);
		}
		constraints.Clear();
		index = 0;
		for (length = ci.constraints.Count; index < length; index++)
		{
			constraints.Add(ci.constraints[index]);
		}
	}

	public void Restore(CollisionIsland ci, World world)
	{
		ci.ClearLists();
		ci.islandManager = world.islands;
		index = 0;
		for (length = bodies.Count; index < length; index++)
		{
			RigidBody rigidBody = bodies[index];
			rigidBody.island = ci;
			ci.bodies.Add(rigidBody);
		}
		index = 0;
		for (length = arbiters.Count; index < length; index++)
		{
			ArbiterClone arbiterClone = arbiters[index];
			Arbiter arbiter = null;
			world.ArbiterMap.LookUpArbiter(arbiterClone.body1, arbiterClone.body2, out arbiter);
			ci.arbiter.Add(arbiter);
		}
		index = 0;
		for (length = constraints.Count; index < length; index++)
		{
			Constraint item = constraints[index];
			ci.constraints.Add(item);
		}
	}
}
