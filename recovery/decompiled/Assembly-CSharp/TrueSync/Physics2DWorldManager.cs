using System;
using System.Collections.Generic;
using TrueSync.Physics2D;
using UnityEngine;

namespace TrueSync;

public class Physics2DWorldManager : IPhysicsManager, IPhysicsManagerBase
{
	public static Physics2DWorldManager instance;

	private World world;

	private Dictionary<IBody, GameObject> gameObjectMap;

	private Dictionary<Body, Dictionary<Body, TSCollision2D>> collisionInfo;

	public TSVector Gravity { get; set; }

	public bool SpeculativeContacts { get; set; }

	public FP LockedTimeStep { get; set; }

	public void Init()
	{
		ChecksumExtractor.Init(this);
		Settings.ContinuousPhysics = SpeculativeContacts;
		this.world = new World(new TSVector2(Gravity.x, Gravity.y));
		ContactManager.physicsManager = this;
		World world = this.world;
		world.BodyRemoved = (BodyDelegate)Delegate.Combine(world.BodyRemoved, new BodyDelegate(OnRemovedRigidBody));
		ContactManager contactManager = this.world.ContactManager;
		contactManager.BeginContact = (BeginContactDelegate)Delegate.Combine(contactManager.BeginContact, new BeginContactDelegate(CollisionEnter));
		ContactManager contactManager2 = this.world.ContactManager;
		contactManager2.StayContact = (EndContactDelegate)Delegate.Combine(contactManager2.StayContact, new EndContactDelegate(CollisionStay));
		ContactManager contactManager3 = this.world.ContactManager;
		contactManager3.EndContact = (EndContactDelegate)Delegate.Combine(contactManager3.EndContact, new EndContactDelegate(CollisionExit));
		gameObjectMap = new Dictionary<IBody, GameObject>();
		collisionInfo = new Dictionary<Body, Dictionary<Body, TSCollision2D>>();
		instance = this;
		AddRigidBodies();
	}

	public void UpdateStep()
	{
		world.Step(LockedTimeStep);
	}

	public IWorld GetWorld()
	{
		return world;
	}

	private void AddRigidBodies()
	{
		TSCollider2D[] collection = UnityEngine.Object.FindObjectsOfType<TSCollider2D>();
		List<TSCollider2D> list = new List<TSCollider2D>(collection);
		list.Sort(UnityUtils.body2DComparer);
		for (int i = 0; i < list.Count; i++)
		{
			AddBody(list[i]);
		}
	}

	public void AddBody(ICollider iCollider)
	{
		if (!(iCollider is TSCollider2D))
		{
			Debug.LogError("You have a 3D object but your Physics 3D is disabled.");
			return;
		}
		TSCollider2D tSCollider2D = (TSCollider2D)iCollider;
		if (tSCollider2D._body == null)
		{
			tSCollider2D.Initialize(world);
			gameObjectMap[tSCollider2D._body] = tSCollider2D.gameObject;
			if (tSCollider2D.gameObject.transform.parent != null && tSCollider2D.gameObject.transform.parent.GetComponentInParent<TSCollider2D>() != null)
			{
				TSCollider2D componentInParent = tSCollider2D.gameObject.transform.parent.GetComponentInParent<TSCollider2D>();
				Body body = tSCollider2D._body;
				body.bodyConstraints.Add(new ConstraintHierarchy2D((Body)componentInParent.Body, tSCollider2D._body, tSCollider2D.GetComponent<TSTransform2D>().position + tSCollider2D.ScaledCenter - (componentInParent.GetComponent<TSTransform2D>().position + componentInParent.ScaledCenter)));
			}
			world.ProcessAddedBodies();
		}
	}

	internal string GetChecksum(bool plain)
	{
		return ChecksumExtractor.GetEncodedChecksum();
	}

	public void RemoveBody(IBody iBody)
	{
		world.RemoveBody((Body)iBody);
		world.ProcessRemovedBodies();
	}

	public void OnRemoveBody(Action<IBody> OnRemoveBody)
	{
		World world = this.world;
		world.BodyRemoved = (BodyDelegate)Delegate.Combine(world.BodyRemoved, (BodyDelegate)((Body body) =>
		{
			OnRemoveBody(body);
		}));
	}

	private void OnRemovedRigidBody(Body body)
	{
		GameObject gameObject = gameObjectMap[body];
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private bool CollisionEnter(Contact contact)
	{
		if (contact.FixtureA.IsSensor || contact.FixtureB.IsSensor)
		{
			TriggerEnter(contact);
			return true;
		}
		CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, contact, "OnSyncedCollisionEnter");
		return true;
	}

	private void CollisionStay(Contact contact)
	{
		if (contact.FixtureA.IsSensor || contact.FixtureB.IsSensor)
		{
			TriggerStay(contact);
		}
		else
		{
			CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, contact, "OnSyncedCollisionStay");
		}
	}

	private void CollisionExit(Contact contact)
	{
		if (contact.FixtureA.IsSensor || contact.FixtureB.IsSensor)
		{
			TriggerExit(contact);
		}
		else
		{
			CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, null, "OnSyncedCollisionExit");
		}
	}

	private void TriggerEnter(Contact contact)
	{
		CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, contact, "OnSyncedTriggerEnter");
	}

	private void TriggerStay(Contact contact)
	{
		CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, contact, "OnSyncedTriggerStay");
	}

	private void TriggerExit(Contact contact)
	{
		CollisionDetected(contact.FixtureA.Body, contact.FixtureB.Body, null, "OnSyncedTriggerExit");
	}

	private void CollisionDetected(Body body1, Body body2, Contact contact, string callbackName)
	{
		if (gameObjectMap.ContainsKey(body1) && gameObjectMap.ContainsKey(body2))
		{
			GameObject gameObject = gameObjectMap[body1];
			GameObject gameObject2 = gameObjectMap[body2];
			if (!(gameObject == null) && !(gameObject2 == null))
			{
				gameObject.SendMessage(callbackName, GetCollisionInfo(body1, body2, contact), SendMessageOptions.DontRequireReceiver);
				gameObject2.SendMessage(callbackName, GetCollisionInfo(body2, body1, contact), SendMessageOptions.DontRequireReceiver);
				TrueSyncManager.UpdateCoroutines();
			}
		}
	}

	private TSCollision2D GetCollisionInfo(Body body1, Body body2, Contact c)
	{
		if (!collisionInfo.ContainsKey(body1))
		{
			collisionInfo.Add(body1, new Dictionary<Body, TSCollision2D>());
		}
		Dictionary<Body, TSCollision2D> dictionary = collisionInfo[body1];
		TSCollision2D tSCollision2D = null;
		if (dictionary.ContainsKey(body2))
		{
			tSCollision2D = dictionary[body2];
		}
		else
		{
			tSCollision2D = new TSCollision2D();
			dictionary.Add(body2, tSCollision2D);
		}
		tSCollision2D.Update(gameObjectMap[body2], c);
		return tSCollision2D;
	}

	public GameObject GetGameObject(IBody body)
	{
		if (!gameObjectMap.ContainsKey(body))
		{
			return null;
		}
		return gameObjectMap[body];
	}

	public int GetBodyLayer(IBody body)
	{
		GameObject gameObject = GetGameObject(body);
		if (gameObject == null)
		{
			return -1;
		}
		return gameObject.layer;
	}

	public TSRaycastHit2D Raycast(TSVector2 origin, TSVector2 direction, FP distance)
	{
		TSRaycastHit2D result = null;
		Func<Fixture, TSVector2, TSVector2, FP, FP> callback = (Fixture fixture, TSVector2 point, TSVector2 normal, FP fraction) =>
		{
			result = new TSRaycastHit2D(gameObjectMap[fixture.Body].GetComponent<TSCollider2D>());
			return 0;
		};
		world.RayCast(callback, origin, origin + direction * distance);
		return result;
	}

	public TSRaycastHit2D[] RaycastAll(TSVector2 origin, TSVector2 direction, FP distance)
	{
		List<TSRaycastHit2D> result = new List<TSRaycastHit2D>();
		Func<Fixture, TSVector2, TSVector2, FP, FP> callback = (Fixture fixture, TSVector2 point, TSVector2 normal, FP fraction) =>
		{
			result.Add(new TSRaycastHit2D(gameObjectMap[fixture.Body].GetComponent<TSCollider2D>()));
			return -1;
		};
		world.RayCast(callback, origin, origin + direction * distance);
		if (result.Count == 0)
		{
			return null;
		}
		return result.ToArray();
	}

	public bool IsCollisionEnabled(IBody rigidBody1, IBody rigidBody2)
	{
		if (gameObjectMap.ContainsKey(rigidBody1) && gameObjectMap.ContainsKey(rigidBody2))
		{
			return LayerCollisionMatrix.CollisionEnabled(gameObjectMap[rigidBody1], gameObjectMap[rigidBody2]);
		}
		return true;
	}

	public IWorldClone GetWorldClone()
	{
		return new WorldClone2D();
	}
}
