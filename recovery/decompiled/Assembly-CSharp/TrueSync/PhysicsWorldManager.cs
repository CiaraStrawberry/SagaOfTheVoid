using System;
using System.Collections.Generic;
using TrueSync.Physics3D;
using UnityEngine;

namespace TrueSync;

public class PhysicsWorldManager : IPhysicsManager, IPhysicsManagerBase
{
	public static PhysicsWorldManager instance;

	private World world;

	private Dictionary<IBody, GameObject> gameObjectMap;

	private Dictionary<RigidBody, Dictionary<RigidBody, TSCollision>> collisionInfo;

	public TSVector Gravity { get; set; }

	public bool SpeculativeContacts { get; set; }

	public FP LockedTimeStep { get; set; }

	public void Init()
	{
		ChecksumExtractor.Init(this);
		gameObjectMap = new Dictionary<IBody, GameObject>();
		collisionInfo = new Dictionary<RigidBody, Dictionary<RigidBody, TSCollision>>();
		CollisionSystemPersistentSAP collisionSystemPersistentSAP = new CollisionSystemPersistentSAP();
		collisionSystemPersistentSAP.EnableSpeculativeContacts = SpeculativeContacts;
		world = new World(collisionSystemPersistentSAP);
		collisionSystemPersistentSAP.world = world;
		world.physicsManager = this;
		world.Gravity = Gravity;
		world.Events.BodiesBeginCollide += CollisionEnter;
		world.Events.BodiesStayCollide += CollisionStay;
		world.Events.BodiesEndCollide += CollisionExit;
		world.Events.TriggerBeginCollide += TriggerEnter;
		world.Events.TriggerStayCollide += TriggerStay;
		world.Events.TriggerEndCollide += TriggerExit;
		world.Events.RemovedRigidBody += OnRemovedRigidBody;
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
		TSCollider[] collection = UnityEngine.Object.FindObjectsOfType<TSCollider>();
		List<TSCollider> list = new List<TSCollider>(collection);
		list.Sort(UnityUtils.bodyComparer);
		for (int i = 0; i < list.Count; i++)
		{
			AddBody(list[i]);
		}
	}

	public void AddBody(ICollider iCollider)
	{
		if (!(iCollider is TSCollider))
		{
			Debug.LogError("You have a 2D object but your Physics 2D is disabled.");
			return;
		}
		TSCollider tSCollider = (TSCollider)iCollider;
		if (tSCollider._body == null)
		{
			TSRigidBody component = tSCollider.GetComponent<TSRigidBody>();
			TSRigidBodyConstraints freezeConstraints = ((component != null) ? component.constraints : TSRigidBodyConstraints.None);
			tSCollider.Initialize();
			world.AddBody(tSCollider._body);
			gameObjectMap[tSCollider._body] = tSCollider.gameObject;
			if (tSCollider.gameObject.transform.parent != null && tSCollider.gameObject.transform.parent.GetComponentInParent<TSCollider>() != null)
			{
				TSCollider componentInParent = tSCollider.gameObject.transform.parent.GetComponentInParent<TSCollider>();
				world.AddConstraint(new ConstraintHierarchy(componentInParent.Body, tSCollider._body, tSCollider.GetComponent<TSTransform>().position + tSCollider.ScaledCenter - (componentInParent.GetComponent<TSTransform>().position + componentInParent.ScaledCenter)));
			}
			tSCollider._body.FreezeConstraints = freezeConstraints;
		}
	}

	public void RemoveBody(IBody iBody)
	{
		world.RemoveBody((RigidBody)iBody);
	}

	public void OnRemoveBody(Action<IBody> OnRemoveBody)
	{
		world.Events.RemovedRigidBody += (RigidBody rb) =>
		{
			OnRemoveBody(rb);
		};
	}

	public bool Raycast(TSVector rayOrigin, TSVector rayDirection, RaycastCallback raycast, out IBody body, out TSVector normal, out FP fraction)
	{
		bool result = world.CollisionSystem.Raycast(rayOrigin, rayDirection, raycast, out var body2, out normal, out fraction);
		body = body2;
		return result;
	}

	public TSRaycastHit Raycast(TSRay ray, FP maxDistance, RaycastCallback callback = null)
	{
		TSVector origin = ray.origin;
		TSVector direction = ray.direction;
		if (Raycast(origin, direction, callback, out var body, out var normal, out var fraction))
		{
			if (fraction <= maxDistance)
			{
				GameObject gameObject = PhysicsManager.instance.GetGameObject(body);
				TSRigidBody component = gameObject.GetComponent<TSRigidBody>();
				TSCollider component2 = gameObject.GetComponent<TSCollider>();
				TSTransform component3 = gameObject.GetComponent<TSTransform>();
				return new TSRaycastHit(component, component2, component3, normal, ray.origin, ray.direction, fraction);
			}
		}
		else
		{
			direction *= maxDistance;
			if (Raycast(origin, direction, callback, out body, out normal, out fraction))
			{
				GameObject gameObject2 = PhysicsManager.instance.GetGameObject(body);
				TSRigidBody component4 = gameObject2.GetComponent<TSRigidBody>();
				TSCollider component5 = gameObject2.GetComponent<TSCollider>();
				TSTransform component6 = gameObject2.GetComponent<TSTransform>();
				return new TSRaycastHit(component4, component5, component6, normal, ray.origin, direction, fraction);
			}
		}
		return null;
	}

	private void OnRemovedRigidBody(RigidBody body)
	{
		GameObject gameObject = gameObjectMap[body];
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void CollisionEnter(Contact c)
	{
		CollisionDetected(c.body1, c.body2, c, "OnSyncedCollisionEnter");
	}

	private void CollisionStay(Contact c)
	{
		CollisionDetected(c.body1, c.body2, c, "OnSyncedCollisionStay");
	}

	private void CollisionExit(RigidBody body1, RigidBody body2)
	{
		CollisionDetected(body1, body2, null, "OnSyncedCollisionExit");
	}

	private void TriggerEnter(Contact c)
	{
		CollisionDetected(c.body1, c.body2, c, "OnSyncedTriggerEnter");
	}

	private void TriggerStay(Contact c)
	{
		CollisionDetected(c.body1, c.body2, c, "OnSyncedTriggerStay");
	}

	private void TriggerExit(RigidBody body1, RigidBody body2)
	{
		CollisionDetected(body1, body2, null, "OnSyncedTriggerExit");
	}

	private void CollisionDetected(RigidBody body1, RigidBody body2, Contact c, string callbackName)
	{
		if (gameObjectMap.ContainsKey(body1) && gameObjectMap.ContainsKey(body2))
		{
			GameObject gameObject = gameObjectMap[body1];
			GameObject gameObject2 = gameObjectMap[body2];
			if (!(gameObject == null) && !(gameObject2 == null))
			{
				gameObject.SendMessage(callbackName, GetCollisionInfo(body1, body2, c), SendMessageOptions.DontRequireReceiver);
				gameObject2.SendMessage(callbackName, GetCollisionInfo(body2, body1, c), SendMessageOptions.DontRequireReceiver);
				TrueSyncManager.UpdateCoroutines();
			}
		}
	}

	private TSCollision GetCollisionInfo(RigidBody body1, RigidBody body2, Contact c)
	{
		if (!collisionInfo.ContainsKey(body1))
		{
			collisionInfo.Add(body1, new Dictionary<RigidBody, TSCollision>());
		}
		Dictionary<RigidBody, TSCollision> dictionary = collisionInfo[body1];
		TSCollision tSCollision = null;
		if (dictionary.ContainsKey(body2))
		{
			tSCollision = dictionary[body2];
		}
		else
		{
			tSCollision = new TSCollision();
			dictionary.Add(body2, tSCollision);
		}
		tSCollision.Update(gameObjectMap[body2], c);
		return tSCollision;
	}

	public GameObject GetGameObject(IBody rigidBody)
	{
		if (!gameObjectMap.ContainsKey(rigidBody))
		{
			return null;
		}
		return gameObjectMap[rigidBody];
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

	public bool IsCollisionEnabled(IBody rigidBody1, IBody rigidBody2)
	{
		return LayerCollisionMatrix.CollisionEnabled(gameObjectMap[rigidBody1], gameObjectMap[rigidBody2]);
	}

	public IWorldClone GetWorldClone()
	{
		return new WorldClone();
	}
}
