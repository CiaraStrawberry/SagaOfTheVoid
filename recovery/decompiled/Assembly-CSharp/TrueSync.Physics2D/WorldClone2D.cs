using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class WorldClone2D : IWorldClone
{
	internal static ResourcePoolBodyClone2D poolRigidBodyClone = new ResourcePoolBodyClone2D();

	internal static ResourcePoolContactClone2D poolContactClone = new ResourcePoolContactClone2D();

	internal static ResourcePoolContactEdgeClone2D poolContactEdgeClone = new ResourcePoolContactEdgeClone2D();

	internal static ResourcePoolContactEdge2D poolContactEdge = new ResourcePoolContactEdge2D();

	internal static ResourcePoolTreeFixtureProxy2D poolTreeFixtureProxy = new ResourcePoolTreeFixtureProxy2D();

	internal int bodyCounter;

	internal int fixtureCounter;

	internal Dictionary<int, BodyClone2D> clonedPhysics = new Dictionary<int, BodyClone2D>();

	internal bool _worldHasNewFixture;

	internal List<ContactClone2D> contactsClone = new List<ContactClone2D>();

	internal IslandClone2D islandClone = new IslandClone2D();

	internal TOIClone2D toiClone = new TOIClone2D();

	internal DynamicTreeBroadPhaseClone2D dynamicTreeClone = new DynamicTreeBroadPhaseClone2D();

	private List<Body> bodiesToRemove = new List<Body>();

	private Dictionary<ContactCloneKey, Contact> contactDic = new Dictionary<ContactCloneKey, Contact>(new ContactCloneKeyComparer());

	private Dictionary<ContactEdgeCloneKey, ContactEdge> contactEdgeDic = new Dictionary<ContactEdgeCloneKey, ContactEdge>(new ContactEdgeKeyComparer());

	private int index;

	private int length;

	public string checksum { get; private set; }

	public void Reset()
	{
		Dictionary<int, BodyClone2D>.Enumerator enumerator = clonedPhysics.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BodyClone2D value = enumerator.Current.Value;
			value.Reset();
			poolRigidBodyClone.GiveBack(value);
		}
		index = 0;
		for (length = contactsClone.Count; index < length; index++)
		{
			ContactClone2D obj = contactsClone[index];
			poolContactClone.GiveBack(obj);
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
		index = 0;
		for (length = world.BodyList.Count; index < length; index++)
		{
			Body body = world.BodyList[index];
			BodyClone2D bodyClone2D = poolRigidBodyClone.GetNew();
			bodyClone2D.Clone(body);
			clonedPhysics.Add(body.BodyId, bodyClone2D);
		}
		contactsClone.Clear();
		index = 0;
		for (length = world.ContactList.Count; index < length; index++)
		{
			Contact contact = world.ContactList[index];
			ContactClone2D contactClone2D = poolContactClone.GetNew();
			contactClone2D.Clone(contact);
			contactsClone.Add(contactClone2D);
		}
		islandClone.Clone(world.Island);
		toiClone.Clone(world._input);
		dynamicTreeClone.Clone((DynamicTreeBroadPhase)world.ContactManager.BroadPhase);
		_worldHasNewFixture = world._worldHasNewFixture;
		bodyCounter = Body._bodyIdCounter;
		fixtureCounter = Fixture._fixtureIdCounter;
	}

	public void Restore(IWorld iWorld)
	{
		World world = (World)iWorld;
		bodiesToRemove.Clear();
		index = 0;
		for (length = world.BodyList.Count; index < length; index++)
		{
			Body body = world.BodyList[index];
			if (!clonedPhysics.ContainsKey(body.BodyId))
			{
				bodiesToRemove.Add(body);
			}
		}
		index = 0;
		for (length = bodiesToRemove.Count; index < length; index++)
		{
			Body body2 = bodiesToRemove[index];
			world.RemoveBody(body2);
		}
		world.ProcessRemovedBodies();
		index = 0;
		for (length = world.BodyList.Count; index < length; index++)
		{
			Body body3 = world.BodyList[index];
			if (clonedPhysics.ContainsKey(body3.BodyId))
			{
				BodyClone2D bodyClone2D = clonedPhysics[body3.BodyId];
				bodyClone2D.Restore(body3);
			}
		}
		index = 0;
		for (length = world.ContactList.Count; index < length; index++)
		{
			Contact item = world.ContactList[index];
			world._contactPool.Enqueue(item);
		}
		world.ContactList.Clear();
		contactDic.Clear();
		index = 0;
		for (length = contactsClone.Count; index < length; index++)
		{
			ContactClone2D contactClone2D = contactsClone[index];
			Contact contact = null;
			contact = ((world._contactPool.Count <= 0) ? new Contact() : world._contactPool.Dequeue());
			contactClone2D.Restore(contact);
			contactDic.Add(contact.Key, contact);
			world.ContactList.Add(contact);
		}
		contactEdgeDic.Clear();
		index = 0;
		for (length = contactsClone.Count; index < length; index++)
		{
			ContactClone2D contactClone2D2 = contactsClone[index];
			contactDic[contactClone2D2.Key]._nodeA = contactClone2D2._nodeA.Restore(restoreLinks: false, contactDic, contactEdgeDic);
			contactDic[contactClone2D2.Key]._nodeB = contactClone2D2._nodeB.Restore(restoreLinks: false, contactDic, contactEdgeDic);
		}
		index = 0;
		for (length = contactsClone.Count; index < length; index++)
		{
			ContactClone2D contactClone2D3 = contactsClone[index];
			contactDic[contactClone2D3.Key]._nodeA = contactClone2D3._nodeA.Restore(restoreLinks: true, contactDic, contactEdgeDic);
			contactDic[contactClone2D3.Key]._nodeB = contactClone2D3._nodeB.Restore(restoreLinks: true, contactDic, contactEdgeDic);
		}
		index = 0;
		for (length = world.BodyList.Count; index < length; index++)
		{
			Body body4 = world.BodyList[index];
			if (clonedPhysics.ContainsKey(body4.BodyId))
			{
				BodyClone2D bodyClone2D2 = clonedPhysics[body4.BodyId];
				if (bodyClone2D2.contactEdgeClone != null)
				{
					bodyClone2D2.contactEdgeClone.Restore(restoreLinks: false, contactDic, contactEdgeDic);
				}
				else
				{
					body4.ContactList = null;
				}
			}
		}
		index = 0;
		for (length = world.BodyList.Count; index < length; index++)
		{
			Body body5 = world.BodyList[index];
			if (clonedPhysics.ContainsKey(body5.BodyId))
			{
				BodyClone2D bodyClone2D3 = clonedPhysics[body5.BodyId];
				if (bodyClone2D3.contactEdgeClone != null)
				{
					body5.ContactList = bodyClone2D3.contactEdgeClone.Restore(restoreLinks: true, contactDic, contactEdgeDic);
				}
			}
		}
		islandClone.Restore(world.Island, contactDic);
		toiClone.Restore(world._input);
		TreeNode<FixtureProxy>[] nodes = ((DynamicTreeBroadPhase)world.ContactManager.BroadPhase)._tree._nodes;
		index = 0;
		for (length = nodes.Length; index < length; index++)
		{
			TreeNode<FixtureProxy> obj = nodes[index];
			poolTreeFixtureProxy.GiveBack(obj);
		}
		dynamicTreeClone.Restore((DynamicTreeBroadPhase)world.ContactManager.BroadPhase);
		world._worldHasNewFixture = _worldHasNewFixture;
		Body._bodyIdCounter = bodyCounter;
		Fixture._fixtureIdCounter = fixtureCounter;
	}
}
