using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class WorldClone : IWorldClone
{
	public static ResourcePoolRigidBodyClone poolRigidBodyClone = new ResourcePoolRigidBodyClone();

	public static ResourcePoolArbiterClone poolArbiterClone = new ResourcePoolArbiterClone();

	public static ResourcePoolCollisionIslandClone poolCollisionIslandClone = new ResourcePoolCollisionIslandClone();

	public Dictionary<int, RigidBodyClone> clonedPhysics = new Dictionary<int, RigidBodyClone>();

	public List<CollisionIslandClone> collisionIslands = new List<CollisionIslandClone>();

	public CollisionSystemPersistentSAPClone cloneCollision = new CollisionSystemPersistentSAPClone();

	public List<ArbiterClone> clonedArbiters = new List<ArbiterClone>();

	public List<OverlapPairContact> clonedInitialCollisions = new List<OverlapPairContact>();

	public List<ArbiterClone> clonedArbitersTrigger = new List<ArbiterClone>();

	public List<OverlapPairContact> clonedInitialTriggers = new List<OverlapPairContact>();

	public int rigidBodyInstanceCount;

	public int constraintInstanceCount;

	public List<Contact> contactsToGiveBack = new List<Contact>();

	public List<Arbiter> arbiterToGiveBack = new List<Arbiter>();

	public List<CollisionIsland> collisionIslandToGiveBack = new List<CollisionIsland>();

	private int index;

	private int length;

	public string checksum { get; private set; }

	public void Reset()
	{
		if (clonedPhysics != null)
		{
			foreach (RigidBodyClone value in clonedPhysics.Values)
			{
				value.Reset();
				poolRigidBodyClone.GiveBack(value);
			}
		}
		if (collisionIslands != null)
		{
			index = 0;
			for (length = collisionIslands.Count; index < length; index++)
			{
				CollisionIslandClone collisionIslandClone = collisionIslands[index];
				collisionIslandClone.Reset();
				poolCollisionIslandClone.GiveBack(collisionIslandClone);
			}
		}
		if (cloneCollision != null)
		{
			cloneCollision.Reset();
		}
		if (clonedArbiters != null)
		{
			index = 0;
			for (length = clonedArbiters.Count; index < length; index++)
			{
				ArbiterClone arbiterClone = clonedArbiters[index];
				arbiterClone.Reset();
				poolArbiterClone.GiveBack(arbiterClone);
			}
		}
		if (clonedArbitersTrigger != null)
		{
			index = 0;
			for (length = clonedArbitersTrigger.Count; index < length; index++)
			{
				ArbiterClone arbiterClone2 = clonedArbitersTrigger[index];
				arbiterClone2.Reset();
				poolArbiterClone.GiveBack(arbiterClone2);
			}
		}
	}

	public void Clone(IWorld iWorld)
	{
		Clone(iWorld, doChecksum: false);
	}

	public void Clone(IWorld iWorld, bool doChecksum)
	{
		World world = (World)iWorld;
		Reset();
		if (doChecksum)
		{
			checksum = ChecksumExtractor.GetEncodedChecksum();
		}
		clonedPhysics.Clear();
		foreach (RigidBody rigidBody in world.RigidBodies)
		{
			RigidBodyClone rigidBodyClone = poolRigidBodyClone.GetNew();
			rigidBodyClone.Clone(rigidBody);
			clonedPhysics.Add(rigidBody.GetInstance(), rigidBodyClone);
		}
		clonedArbiters.Clear();
		foreach (Arbiter arbiter in world.ArbiterMap.Arbiters)
		{
			ArbiterClone arbiterClone = poolArbiterClone.GetNew();
			arbiterClone.Clone(arbiter);
			clonedArbiters.Add(arbiterClone);
		}
		clonedArbitersTrigger.Clear();
		foreach (Arbiter arbiter2 in world.ArbiterTriggerMap.Arbiters)
		{
			ArbiterClone arbiterClone2 = poolArbiterClone.GetNew();
			arbiterClone2.Clone(arbiter2);
			clonedArbitersTrigger.Add(arbiterClone2);
		}
		collisionIslands.Clear();
		index = 0;
		for (length = world.islands.Count; index < length; index++)
		{
			CollisionIsland ci = world.islands[index];
			CollisionIslandClone collisionIslandClone = poolCollisionIslandClone.GetNew();
			collisionIslandClone.Clone(ci);
			collisionIslands.Add(collisionIslandClone);
		}
		cloneCollision.Clone((CollisionSystemPersistentSAP)world.CollisionSystem);
		clonedInitialCollisions.Clear();
		clonedInitialCollisions.AddRange(world.initialCollisions);
		clonedInitialTriggers.Clear();
		clonedInitialTriggers.AddRange(world.initialTriggers);
		rigidBodyInstanceCount = RigidBody.instanceCount;
		constraintInstanceCount = Constraint.instanceCount;
	}

	public void Restore(IWorld iWorld)
	{
		World world = (World)iWorld;
		List<RigidBody> list = new List<RigidBody>();
		foreach (RigidBody rigidBody3 in world.RigidBodies)
		{
			if (!clonedPhysics.ContainsKey(rigidBody3.GetInstance()))
			{
				list.Add(rigidBody3);
			}
		}
		index = 0;
		for (length = list.Count; index < length; index++)
		{
			RigidBody body = list[index];
			world.RemoveBody(body);
		}
		foreach (RigidBody rigidBody4 in world.RigidBodies)
		{
			if (clonedPhysics.ContainsKey(rigidBody4.GetInstance()))
			{
				RigidBodyClone rigidBodyClone = clonedPhysics[rigidBody4.GetInstance()];
				rigidBodyClone.Restore(world, rigidBody4);
				rigidBody4.island = null;
				rigidBody4.arbiters.Clear();
				rigidBody4.arbitersTrigger.Clear();
			}
		}
		foreach (Arbiter arbiter3 in world.ArbiterMap.Arbiters)
		{
			index = 0;
			for (length = arbiter3.contactList.Count; index < length; index++)
			{
				Contact item = arbiter3.contactList[index];
				contactsToGiveBack.Add(item);
			}
			arbiterToGiveBack.Add(arbiter3);
		}
		world.ArbiterMap.Clear();
		foreach (Arbiter arbiter4 in world.ArbiterTriggerMap.Arbiters)
		{
			foreach (Contact contact in arbiter4.contactList)
			{
				contactsToGiveBack.Add(contact);
			}
			arbiterToGiveBack.Add(arbiter4);
		}
		world.ArbiterTriggerMap.Clear();
		index = 0;
		for (length = world.islands.islands.Count; index < length; index++)
		{
			CollisionIsland item2 = world.islands.islands[index];
			collisionIslandToGiveBack.Add(item2);
		}
		index = 0;
		for (length = clonedArbiters.Count; index < length; index++)
		{
			ArbiterClone arbiterClone = clonedArbiters[index];
			Arbiter arbiter = Arbiter.Pool.GetNew();
			arbiterClone.Restore(arbiter);
			arbiter.body1.arbiters.Add(arbiter);
			arbiter.body2.arbiters.Add(arbiter);
			world.ArbiterMap.Add(new ArbiterKey(arbiter.body1, arbiter.body2), arbiter);
		}
		index = 0;
		for (length = clonedArbitersTrigger.Count; index < length; index++)
		{
			ArbiterClone arbiterClone2 = clonedArbitersTrigger[index];
			Arbiter arbiter2 = Arbiter.Pool.GetNew();
			arbiterClone2.Restore(arbiter2);
			arbiter2.body1.arbitersTrigger.Add(arbiter2);
			arbiter2.body2.arbitersTrigger.Add(arbiter2);
			world.ArbiterTriggerMap.Add(new ArbiterKey(arbiter2.body1, arbiter2.body2), arbiter2);
		}
		world.islands.islands.Clear();
		index = 0;
		for (length = collisionIslands.Count; index < length; index++)
		{
			CollisionIslandClone collisionIslandClone = collisionIslands[index];
			CollisionIsland collisionIsland = IslandManager.Pool.GetNew();
			collisionIslandClone.Restore(collisionIsland, world);
			world.islands.islands.Add(collisionIsland);
		}
		cloneCollision.Restore((CollisionSystemPersistentSAP)world.CollisionSystem);
		world.initialCollisions.Clear();
		world.initialCollisions.AddRange(clonedInitialCollisions);
		world.initialTriggers.Clear();
		world.initialTriggers.AddRange(clonedInitialTriggers);
		RigidBody.instanceCount = rigidBodyInstanceCount;
		Constraint.instanceCount = constraintInstanceCount;
		index = 0;
		for (length = contactsToGiveBack.Count; index < length; index++)
		{
			Contact obj = contactsToGiveBack[index];
			Contact.Pool.GiveBack(obj);
		}
		contactsToGiveBack.Clear();
		index = 0;
		for (length = arbiterToGiveBack.Count; index < length; index++)
		{
			Arbiter obj2 = arbiterToGiveBack[index];
			Arbiter.Pool.GiveBack(obj2);
		}
		arbiterToGiveBack.Clear();
		index = 0;
		for (length = collisionIslandToGiveBack.Count; index < length; index++)
		{
			CollisionIsland obj3 = collisionIslandToGiveBack[index];
			IslandManager.Pool.GiveBack(obj3);
		}
		collisionIslandToGiveBack.Clear();
	}
}
