using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class ContactManager
{
	public BeginContactDelegate BeginContact;

	public IBroadPhase BroadPhase;

	public CollisionFilterDelegate ContactFilter;

	public List<Contact> ContactList = new List<Contact>(128);

	public EndContactDelegate StayContact;

	public EndContactDelegate EndContact;

	public BroadphaseDelegate OnBroadphaseCollision;

	public PostSolveDelegate PostSolve;

	public PreSolveDelegate PreSolve;

	public static IPhysicsManager physicsManager;

	internal ContactManager(IBroadPhase broadPhase)
	{
		BroadPhase = broadPhase;
		OnBroadphaseCollision = AddPair;
	}

	private void AddPair(ref FixtureProxy proxyA, ref FixtureProxy proxyB)
	{
		Fixture fixture = proxyA.Fixture;
		Fixture fixture2 = proxyB.Fixture;
		int childIndex = proxyA.ChildIndex;
		int childIndex2 = proxyB.ChildIndex;
		Body body = fixture.Body;
		Body body2 = fixture2.Body;
		if (body == body2)
		{
			return;
		}
		for (ContactEdge contactEdge = body2.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
		{
			if (contactEdge.Other == body)
			{
				Fixture fixtureA = contactEdge.Contact.FixtureA;
				Fixture fixtureB = contactEdge.Contact.FixtureB;
				int childIndexA = contactEdge.Contact.ChildIndexA;
				int childIndexB = contactEdge.Contact.ChildIndexB;
				if ((fixtureA == fixture && fixtureB == fixture2 && childIndexA == childIndex && childIndexB == childIndex2) || (fixtureA == fixture2 && fixtureB == fixture && childIndexA == childIndex2 && childIndexB == childIndex))
				{
					return;
				}
			}
		}
		if (!body2.ShouldCollide(body) || !ShouldCollide(fixture, fixture2) || (ContactFilter != null && !ContactFilter(fixture, fixture2)) || (fixture.BeforeCollision != null && !fixture.BeforeCollision(fixture, fixture2)) || (fixture2.BeforeCollision != null && !fixture2.BeforeCollision(fixture2, fixture)))
		{
			return;
		}
		Body body3 = null;
		Body item = null;
		if (body.SpecialSensor != BodySpecialSensor.None)
		{
			body3 = body;
			item = body2;
		}
		else if (body2.SpecialSensor != BodySpecialSensor.None)
		{
			body3 = body2;
			item = body;
		}
		if (body3 != null)
		{
			if (!Collision.TestOverlap(fixture.Shape, childIndex, fixture2.Shape, childIndex2, ref body._xf, ref body2._xf))
			{
				return;
			}
			body3._specialSensorResults.Add(item);
			if (body3.SpecialSensor == BodySpecialSensor.ActiveOnce)
			{
				body3.disabled = true;
				return;
			}
		}
		Contact contact = Contact.Create(fixture, childIndex, fixture2, childIndex2);
		if (contact != null)
		{
			fixture = contact.FixtureA;
			fixture2 = contact.FixtureB;
			body = fixture.Body;
			body2 = fixture2.Body;
			ContactList.Add(contact);
			contact._nodeA.Contact = contact;
			contact._nodeA.Other = body2;
			contact._nodeA.Prev = null;
			contact._nodeA.Next = body.ContactList;
			if (body.ContactList != null)
			{
				body.ContactList.Prev = contact._nodeA;
			}
			body.ContactList = contact._nodeA;
			contact._nodeB.Contact = contact;
			contact._nodeB.Other = body;
			contact._nodeB.Prev = null;
			contact._nodeB.Next = body2.ContactList;
			if (body2.ContactList != null)
			{
				body2.ContactList.Prev = contact._nodeB;
			}
			body2.ContactList = contact._nodeB;
			if (!fixture.IsSensor && !fixture2.IsSensor)
			{
				body.Awake = true;
				body2.Awake = true;
			}
		}
	}

	internal void FindNewContacts()
	{
		BroadPhase.UpdatePairs(OnBroadphaseCollision);
	}

	internal void Destroy(Contact contact)
	{
		Fixture fixtureA = contact.FixtureA;
		Fixture fixtureB = contact.FixtureB;
		Body body = fixtureA.Body;
		Body body2 = fixtureB.Body;
		if (contact.IsTouching)
		{
			if (fixtureA != null && fixtureA.OnSeparation != null)
			{
				fixtureA.OnSeparation(fixtureA, fixtureB);
			}
			if (fixtureB != null && fixtureB.OnSeparation != null)
			{
				fixtureB.OnSeparation(fixtureB, fixtureA);
			}
			if (EndContact != null)
			{
				EndContact(contact);
			}
		}
		ContactList.Remove(contact);
		if (contact._nodeA.Prev != null)
		{
			contact._nodeA.Prev.Next = contact._nodeA.Next;
		}
		if (contact._nodeA.Next != null)
		{
			contact._nodeA.Next.Prev = contact._nodeA.Prev;
		}
		if (contact._nodeA == body.ContactList)
		{
			body.ContactList = contact._nodeA.Next;
		}
		if (contact._nodeB.Prev != null)
		{
			contact._nodeB.Prev.Next = contact._nodeB.Next;
		}
		if (contact._nodeB.Next != null)
		{
			contact._nodeB.Next.Prev = contact._nodeB.Prev;
		}
		if (contact._nodeB == body2.ContactList)
		{
			body2.ContactList = contact._nodeB.Next;
		}
		contact.Destroy();
	}

	internal void Collide()
	{
		for (int i = 0; i < ContactList.Count; i++)
		{
			Contact contact = ContactList[i];
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			int childIndexA = contact.ChildIndexA;
			int childIndexB = contact.ChildIndexB;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			if (!body.Enabled || !body2.Enabled)
			{
				continue;
			}
			if (contact.FilterFlag)
			{
				if (!body2.ShouldCollide(body))
				{
					Contact contact2 = contact;
					Destroy(contact2);
					continue;
				}
				if (!ShouldCollide(fixtureA, fixtureB))
				{
					Contact contact3 = contact;
					Destroy(contact3);
					continue;
				}
				if (ContactFilter != null && !ContactFilter(fixtureA, fixtureB))
				{
					Contact contact4 = contact;
					Destroy(contact4);
					continue;
				}
				contact.FilterFlag = false;
			}
			bool flag = body.Awake && body.BodyType != BodyType.Static;
			bool flag2 = body2.Awake && body2.BodyType != BodyType.Static;
			if ((flag || flag2) && fixtureA != null && fixtureA.Proxies != null && fixtureB != null && fixtureB.Proxies != null)
			{
				int proxyId = fixtureA.Proxies[childIndexA].ProxyId;
				int proxyId2 = fixtureB.Proxies[childIndexB].ProxyId;
				if (!BroadPhase.TestOverlap(proxyId, proxyId2))
				{
					Contact contact5 = contact;
					Destroy(contact5);
				}
				else
				{
					contact.Update(this);
				}
			}
		}
	}

	public static bool CheckCollisionConditions(Fixture fixtureA, Fixture fixtureB)
	{
		Body body = fixtureA.Body;
		Body body2 = fixtureB.Body;
		if (body.disabled || body2.disabled || !physicsManager.IsCollisionEnabled(body, body2))
		{
			return false;
		}
		bool flag = body._specialSensor != BodySpecialSensor.None;
		bool flag2 = body2._specialSensor != BodySpecialSensor.None;
		if (flag || flag2)
		{
			if (flag)
			{
				int num = 1 << physicsManager.GetBodyLayer(body2);
				if ((body.SpecialSensorMask & num) != num)
				{
					return false;
				}
			}
			else
			{
				int num2 = 1 << physicsManager.GetBodyLayer(body);
				if ((body2.SpecialSensorMask & num2) != num2)
				{
					return false;
				}
			}
			return true;
		}
		if (body.IsStatic && body2.IsStatic)
		{
			return false;
		}
		if (!fixtureA.IsSensor && !fixtureB.IsSensor && ((body.IsKinematic && body2.IsKinematic) || (body.IsKinematic && body2.IsStatic) || (body2.IsKinematic && body.IsStatic)))
		{
			return false;
		}
		return true;
	}

	private static bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
	{
		if (!CheckCollisionConditions(fixtureA, fixtureB))
		{
			return false;
		}
		if (Settings.UseFPECollisionCategories)
		{
			if (fixtureA.CollisionGroup == fixtureB.CollisionGroup && fixtureA.CollisionGroup != 0 && fixtureB.CollisionGroup != 0)
			{
				return false;
			}
			if (((fixtureA.CollisionCategories & fixtureB.CollidesWith) == 0) & ((fixtureB.CollisionCategories & fixtureA.CollidesWith) == 0))
			{
				return false;
			}
			if (fixtureA.IsFixtureIgnored(fixtureB) || fixtureB.IsFixtureIgnored(fixtureA))
			{
				return false;
			}
			return true;
		}
		if (fixtureA.CollisionGroup == fixtureB.CollisionGroup && fixtureA.CollisionGroup != 0)
		{
			return fixtureA.CollisionGroup > 0;
		}
		bool flag = (fixtureA.CollidesWith & fixtureB.CollisionCategories) != 0 && (fixtureA.CollisionCategories & fixtureB.CollidesWith) != 0;
		if (flag && (fixtureA.IsFixtureIgnored(fixtureB) || fixtureB.IsFixtureIgnored(fixtureA)))
		{
			return false;
		}
		return flag;
	}

	internal void UpdateContacts(ContactEdge contactEdge, bool value)
	{
	}
}
