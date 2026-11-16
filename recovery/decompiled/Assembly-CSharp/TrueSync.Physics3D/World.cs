using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class World : IWorld
{
	public delegate void WorldStep(FP timestep);

	public class WorldEvents
	{
		public event WorldStep PreStep;

		public event WorldStep PostStep;

		public event Action<RigidBody> AddedRigidBody;

		public event Action<RigidBody> RemovedRigidBody;

		public event Action<Constraint> AddedConstraint;

		public event Action<Constraint> RemovedConstraint;

		public event Action<SoftBody> AddedSoftBody;

		public event Action<SoftBody> RemovedSoftBody;

		public event Action<Contact> BodiesBeginCollide;

		public event Action<Contact> BodiesStayCollide;

		public event Action<RigidBody, RigidBody> BodiesEndCollide;

		public event Action<Contact> TriggerBeginCollide;

		public event Action<Contact> TriggerStayCollide;

		public event Action<RigidBody, RigidBody> TriggerEndCollide;

		public event Action<Contact> ContactCreated;

		public event Action<RigidBody> DeactivatedBody;

		public event Action<RigidBody> ActivatedBody;

		internal WorldEvents()
		{
		}

		internal void RaiseWorldPreStep(FP timestep)
		{
			if (PreStep != null)
			{
				PreStep(timestep);
			}
		}

		internal void RaiseWorldPostStep(FP timestep)
		{
			if (PostStep != null)
			{
				PostStep(timestep);
			}
		}

		internal void RaiseAddedRigidBody(RigidBody body)
		{
			if (AddedRigidBody != null)
			{
				AddedRigidBody(body);
			}
		}

		internal void RaiseRemovedRigidBody(RigidBody body)
		{
			if (RemovedRigidBody != null)
			{
				RemovedRigidBody(body);
			}
		}

		internal void RaiseAddedConstraint(Constraint constraint)
		{
			if (AddedConstraint != null)
			{
				AddedConstraint(constraint);
			}
		}

		internal void RaiseRemovedConstraint(Constraint constraint)
		{
			if (RemovedConstraint != null)
			{
				RemovedConstraint(constraint);
			}
		}

		internal void RaiseAddedSoftBody(SoftBody body)
		{
			if (AddedSoftBody != null)
			{
				AddedSoftBody(body);
			}
		}

		internal void RaiseRemovedSoftBody(SoftBody body)
		{
			if (RemovedSoftBody != null)
			{
				RemovedSoftBody(body);
			}
		}

		internal void RaiseBodiesBeginCollide(Contact contact)
		{
			if (BodiesBeginCollide != null)
			{
				BodiesBeginCollide(contact);
			}
		}

		internal void RaiseBodiesStayCollide(Contact contact)
		{
			if (BodiesStayCollide != null)
			{
				BodiesStayCollide(contact);
			}
		}

		internal void RaiseBodiesEndCollide(RigidBody body1, RigidBody body2)
		{
			if (BodiesEndCollide != null)
			{
				BodiesEndCollide(body1, body2);
			}
		}

		internal void RaiseTriggerBeginCollide(Contact contact)
		{
			if (TriggerBeginCollide != null)
			{
				TriggerBeginCollide(contact);
			}
		}

		internal void RaiseTriggerStayCollide(Contact contact)
		{
			if (TriggerStayCollide != null)
			{
				TriggerStayCollide(contact);
			}
		}

		internal void RaiseTriggerEndCollide(RigidBody body1, RigidBody body2)
		{
			if (TriggerEndCollide != null)
			{
				TriggerEndCollide(body1, body2);
			}
		}

		internal void RaiseActivatedBody(RigidBody body)
		{
			if (ActivatedBody != null)
			{
				ActivatedBody(body);
			}
		}

		internal void RaiseDeactivatedBody(RigidBody body)
		{
			if (DeactivatedBody != null)
			{
				DeactivatedBody(body);
			}
		}

		internal void RaiseContactCreated(Contact contact)
		{
			if (ContactCreated != null)
			{
				ContactCreated(contact);
			}
		}
	}

	private ContactSettings contactSettings = new ContactSettings();

	private FP inactiveAngularThresholdSq = FP.EN1;

	private FP inactiveLinearThresholdSq = FP.EN1;

	private FP deactivationTime = 2;

	private int contactIterations = 6;

	private int smallIterations = 3;

	private FP timestep = FP.Zero;

	public IslandManager islands = new IslandManager();

	public HashList<OverlapPairContact> initialCollisions = new HashList<OverlapPairContact>();

	public HashList<OverlapPairContact> initialTriggers = new HashList<OverlapPairContact>();

	private OverlapPairContact cacheOverPairContact = new OverlapPairContact(null, null);

	internal HashList<RigidBody> rigidBodies = new HashList<RigidBody>();

	internal HashList<Constraint> constraints = new HashList<Constraint>();

	internal HashList<SoftBody> softbodies = new HashList<SoftBody>();

	private WorldEvents events = new WorldEvents();

	private ArbiterMap arbiterMap;

	private ArbiterMap arbiterTriggerMap;

	public Queue<Arbiter> removedArbiterQueue = new Queue<Arbiter>();

	public Queue<Arbiter> addedArbiterQueue = new Queue<Arbiter>();

	private TSVector gravity = new TSVector(0, -981 * FP.EN2, 0);

	private Action<object> arbiterCallback;

	private Action<object> integrateCallback;

	private CollisionDetectedHandler collisionDetectionHandler;

	public IPhysicsManager physicsManager;

	public FP accumulatedTime = FP.Zero;

	public Stack<Arbiter> removedArbiterStack = new Stack<Arbiter>();

	public ReadOnlyHashset<RigidBody> RigidBodies { get; private set; }

	public ReadOnlyHashset<Constraint> Constraints { get; private set; }

	public ReadOnlyHashset<SoftBody> SoftBodies { get; private set; }

	public WorldEvents Events => events;

	public ArbiterMap ArbiterMap => arbiterMap;

	public ArbiterMap ArbiterTriggerMap => arbiterTriggerMap;

	public ContactSettings ContactSettings => contactSettings;

	public List<CollisionIsland> Islands => islands;

	public CollisionSystem CollisionSystem { get; set; }

	public TSVector Gravity
	{
		get
		{
			return gravity;
		}
		set
		{
			gravity = value;
		}
	}

	public bool AllowDeactivation { get; set; }

	public World(CollisionSystem collision)
	{
		if (collision == null)
		{
			throw new ArgumentNullException("The CollisionSystem can't be null.", "collision");
		}
		RigidBody.instanceCount = 0;
		Constraint.instanceCount = 0;
		arbiterCallback = ArbiterCallback;
		integrateCallback = IntegrateCallback;
		RigidBodies = new ReadOnlyHashset<RigidBody>(rigidBodies);
		Constraints = new ReadOnlyHashset<Constraint>(constraints);
		SoftBodies = new ReadOnlyHashset<SoftBody>(softbodies);
		CollisionSystem = collision;
		collisionDetectionHandler = CollisionDetected;
		CollisionSystem.CollisionDetected += collisionDetectionHandler;
		arbiterMap = new ArbiterMap();
		arbiterTriggerMap = new ArbiterMap();
		AllowDeactivation = false;
	}

	public void AddBody(SoftBody body)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body", "body can't be null.");
		}
		if (softbodies.Contains(body))
		{
			throw new ArgumentException("The body was already added to the world.", "body");
		}
		softbodies.Add(body);
		CollisionSystem.AddEntity(body);
		events.RaiseAddedSoftBody(body);
		foreach (SoftBody.Spring edgeSpring in body.EdgeSprings)
		{
			AddConstraint(edgeSpring);
		}
		foreach (SoftBody.MassPoint vertexBody in body.VertexBodies)
		{
			events.RaiseAddedRigidBody(vertexBody);
			rigidBodies.Add(vertexBody);
		}
	}

	public bool RemoveBody(SoftBody body)
	{
		if (!softbodies.Remove(body))
		{
			return false;
		}
		CollisionSystem.RemoveEntity(body);
		events.RaiseRemovedSoftBody(body);
		foreach (SoftBody.Spring edgeSpring in body.EdgeSprings)
		{
			RemoveConstraint(edgeSpring);
		}
		foreach (SoftBody.MassPoint vertexBody in body.VertexBodies)
		{
			RemoveBody(vertexBody, removeMassPoints: true);
		}
		return true;
	}

	public void ResetResourcePools()
	{
		IslandManager.Pool.ResetResourcePool();
		Arbiter.Pool.ResetResourcePool();
		Contact.Pool.ResetResourcePool();
	}

	public void Clear()
	{
		int i = 0;
		for (int count = rigidBodies.Count; i < count; i++)
		{
			RigidBody rigidBody = rigidBodies[i];
			CollisionSystem.RemoveEntity(rigidBody);
			if (rigidBody.island != null)
			{
				rigidBody.island.ClearLists();
				rigidBody.island = null;
			}
			rigidBody.connections.Clear();
			rigidBody.arbiters.Clear();
			rigidBody.constraints.Clear();
			events.RaiseRemovedRigidBody(rigidBody);
		}
		int j = 0;
		for (int count2 = softbodies.Count; j < count2; j++)
		{
			SoftBody body = softbodies[j];
			CollisionSystem.RemoveEntity(body);
		}
		rigidBodies.Clear();
		int k = 0;
		for (int count3 = constraints.Count; k < count3; k++)
		{
			Constraint constraint = constraints[k];
			events.RaiseRemovedConstraint(constraint);
		}
		constraints.Clear();
		softbodies.Clear();
		islands.RemoveAll();
		arbiterMap.Clear();
		arbiterTriggerMap.Clear();
		ResetResourcePools();
	}

	public void SetInactivityThreshold(FP angularVelocity, FP linearVelocity, FP time)
	{
		if (angularVelocity < FP.Zero)
		{
			throw new ArgumentException("Angular velocity threshold has to be larger than zero", "angularVelocity");
		}
		if (linearVelocity < FP.Zero)
		{
			throw new ArgumentException("Linear velocity threshold has to be larger than zero", "linearVelocity");
		}
		if (time < FP.Zero)
		{
			throw new ArgumentException("Deactivation time threshold has to be larger than zero", "time");
		}
		inactiveAngularThresholdSq = angularVelocity * angularVelocity;
		inactiveLinearThresholdSq = linearVelocity * linearVelocity;
		deactivationTime = time;
	}

	public void SetIterations(int iterations, int smallIterations)
	{
		if (iterations < 1)
		{
			throw new ArgumentException("The number of collision iterations has to be larger than zero", "iterations");
		}
		if (smallIterations < 1)
		{
			throw new ArgumentException("The number of collision iterations has to be larger than zero", "smallIterations");
		}
		contactIterations = iterations;
		this.smallIterations = smallIterations;
	}

	public bool RemoveBody(RigidBody body)
	{
		return RemoveBody(body, removeMassPoints: false);
	}

	private bool RemoveBody(RigidBody body, bool removeMassPoints)
	{
		if (!removeMassPoints && body.IsParticle)
		{
			return false;
		}
		if (!rigidBodies.Remove(body))
		{
			return false;
		}
		int i = 0;
		for (int count = body.arbiters.Count; i < count; i++)
		{
			Arbiter arbiter = body.arbiters[i];
			arbiterMap.Remove(arbiter);
			events.RaiseBodiesEndCollide(arbiter.body1, arbiter.body2);
			cacheOverPairContact.SetBodies(arbiter.body1, arbiter.body2);
			initialCollisions.Remove(cacheOverPairContact);
		}
		int j = 0;
		for (int count2 = body.arbitersTrigger.Count; j < count2; j++)
		{
			Arbiter arbiter2 = body.arbitersTrigger[j];
			arbiterTriggerMap.Remove(arbiter2);
			if (arbiter2.body1.isColliderOnly)
			{
				events.RaiseTriggerEndCollide(arbiter2.body1, arbiter2.body2);
			}
			else
			{
				events.RaiseTriggerEndCollide(arbiter2.body2, arbiter2.body1);
			}
			cacheOverPairContact.SetBodies(arbiter2.body1, arbiter2.body2);
			initialTriggers.Remove(cacheOverPairContact);
		}
		int k = 0;
		for (int count3 = body.constraints.Count; k < count3; k++)
		{
			Constraint constraint = body.constraints[k];
			constraints.Remove(constraint);
			events.RaiseRemovedConstraint(constraint);
		}
		CollisionSystem.RemoveEntity(body);
		islands.RemoveBody(body);
		events.RaiseRemovedRigidBody(body);
		return true;
	}

	public void AddBody(RigidBody body)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body", "body can't be null.");
		}
		if (rigidBodies.Contains(body))
		{
			throw new ArgumentException("The body was already added to the world.", "body");
		}
		events.RaiseAddedRigidBody(body);
		CollisionSystem.AddEntity(body);
		rigidBodies.Add(body);
	}

	public bool RemoveConstraint(Constraint constraint)
	{
		if (!constraints.Remove(constraint))
		{
			return false;
		}
		events.RaiseRemovedConstraint(constraint);
		islands.ConstraintRemoved(constraint);
		return true;
	}

	public void AddConstraint(Constraint constraint)
	{
		if (constraints.Contains(constraint))
		{
			throw new ArgumentException("The constraint was already added to the world.", "constraint");
		}
		constraints.Add(constraint);
		islands.ConstraintCreated(constraint);
		events.RaiseAddedConstraint(constraint);
	}

	public void Step(FP timestep)
	{
		this.timestep = timestep;
		if (timestep == FP.Zero)
		{
			return;
		}
		if (timestep < FP.Zero)
		{
			throw new ArgumentException("The timestep can't be negative.", "timestep");
		}
		events.RaiseWorldPreStep(timestep);
		UpdateContacts();
		int i = 0;
		for (int count = initialCollisions.Count; i < count; i++)
		{
			OverlapPairContact overlapPairContact = initialCollisions[i];
			events.RaiseBodiesStayCollide(overlapPairContact.contact);
		}
		int j = 0;
		for (int count2 = initialTriggers.Count; j < count2; j++)
		{
			OverlapPairContact overlapPairContact2 = initialTriggers[j];
			events.RaiseTriggerStayCollide(overlapPairContact2.contact);
		}
		while (removedArbiterQueue.Count > 0)
		{
			islands.ArbiterRemoved(removedArbiterQueue.Dequeue());
		}
		int k = 0;
		for (int count3 = softbodies.Count; k < count3; k++)
		{
			SoftBody softBody = softbodies[k];
			softBody.Update(timestep);
			softBody.DoSelfCollision(collisionDetectionHandler);
		}
		CollisionSystem.Detect();
		while (addedArbiterQueue.Count > 0)
		{
			islands.ArbiterCreated(addedArbiterQueue.Dequeue());
		}
		CheckDeactivation();
		IntegrateForces();
		HandleArbiter(contactIterations);
		Integrate();
		int l = 0;
		for (int count4 = rigidBodies.Count; l < count4; l++)
		{
			RigidBody rigidBody = rigidBodies[l];
			rigidBody.PostStep();
			int m = 0;
			for (int count5 = rigidBody.constraints.Count; m < count5; m++)
			{
				rigidBody.constraints[m].PostStep();
			}
		}
		events.RaiseWorldPostStep(timestep);
	}

	public void Step(FP totalTime, FP timestep, int maxSteps)
	{
		int num = 0;
		accumulatedTime += totalTime;
		while (accumulatedTime > timestep)
		{
			Step(timestep);
			accumulatedTime -= timestep;
			num++;
			if (num > maxSteps)
			{
				accumulatedTime = FP.Zero;
				break;
			}
		}
	}

	private void UpdateArbiterContacts(Arbiter arbiter)
	{
		if (arbiter.contactList.Count == 0)
		{
			lock (removedArbiterStack)
			{
				removedArbiterStack.Push(arbiter);
				return;
			}
		}
		for (int num = arbiter.contactList.Count - 1; num >= 0; num--)
		{
			Contact contact = arbiter.contactList[num];
			contact.UpdatePosition();
			if (contact.penetration < -contactSettings.breakThreshold)
			{
				Contact.Pool.GiveBack(contact);
				arbiter.contactList.RemoveAt(num);
			}
			else
			{
				TSVector.Subtract(ref contact.p1, ref contact.p2, out var result);
				FP fP = TSVector.Dot(ref result, ref contact.normal);
				result -= fP * contact.normal;
				fP = result.sqrMagnitude;
				if (fP > contactSettings.breakThreshold * contactSettings.breakThreshold * 100)
				{
					Contact.Pool.GiveBack(contact);
					arbiter.contactList.RemoveAt(num);
				}
			}
		}
	}

	private void UpdateContacts()
	{
		UpdateContacts(arbiterMap);
		UpdateContacts(arbiterTriggerMap);
	}

	private void UpdateContacts(ArbiterMap selectedArbiterMap)
	{
		foreach (Arbiter arbiter2 in selectedArbiterMap.Arbiters)
		{
			UpdateArbiterContacts(arbiter2);
		}
		while (removedArbiterStack.Count > 0)
		{
			Arbiter arbiter = removedArbiterStack.Pop();
			Arbiter.Pool.GiveBack(arbiter);
			selectedArbiterMap.Remove(arbiter);
			if (selectedArbiterMap == arbiterMap)
			{
				removedArbiterQueue.Enqueue(arbiter);
				events.RaiseBodiesEndCollide(arbiter.body1, arbiter.body2);
				cacheOverPairContact.SetBodies(arbiter.body1, arbiter.body2);
				initialCollisions.Remove(cacheOverPairContact);
				continue;
			}
			if (arbiter.body1.isColliderOnly)
			{
				events.RaiseTriggerEndCollide(arbiter.body1, arbiter.body2);
			}
			else
			{
				events.RaiseTriggerEndCollide(arbiter.body2, arbiter.body1);
			}
			cacheOverPairContact.SetBodies(arbiter.body1, arbiter.body2);
			initialTriggers.Remove(cacheOverPairContact);
		}
	}

	private void ArbiterCallback(object obj)
	{
		CollisionIsland collisionIsland = obj as CollisionIsland;
		int num = ((collisionIsland.Bodies.Count + collisionIsland.Constraints.Count <= 3) ? smallIterations : contactIterations);
		for (int i = -1; i < num; i++)
		{
			int j = 0;
			for (int count = collisionIsland.arbiter.Count; j < count; j++)
			{
				Arbiter arbiter = collisionIsland.arbiter[j];
				int count2 = arbiter.contactList.Count;
				for (int k = 0; k < count2; k++)
				{
					if (i == -1)
					{
						arbiter.contactList[k].PrepareForIteration(timestep);
					}
					else
					{
						arbiter.contactList[k].Iterate();
					}
				}
			}
			int l = 0;
			for (int count3 = collisionIsland.constraints.Count; l < count3; l++)
			{
				Constraint constraint = collisionIsland.constraints[l];
				if (constraint.body1 == null || constraint.body1.IsActive || constraint.body2 == null || constraint.body2.IsActive)
				{
					if (i == -1)
					{
						constraint.PrepareForIteration(timestep);
					}
					else
					{
						constraint.Iterate();
					}
				}
			}
		}
	}

	private void HandleArbiter(int iterations)
	{
		for (int i = 0; i < islands.Count; i++)
		{
			if (islands[i].IsActive())
			{
				arbiterCallback(islands[i]);
			}
		}
	}

	private void IntegrateForces()
	{
		int i = 0;
		for (int count = rigidBodies.Count; i < count; i++)
		{
			RigidBody rigidBody = rigidBodies[i];
			if (!rigidBody.isStatic && rigidBody.IsActive)
			{
				TSVector.Multiply(ref rigidBody.force, rigidBody.inverseMass * timestep, out var result);
				TSVector.Add(ref result, ref rigidBody.linearVelocity, out rigidBody.linearVelocity);
				if (!rigidBody.isParticle)
				{
					TSVector.Multiply(ref rigidBody.torque, timestep, out result);
					TSVector.Transform(ref result, ref rigidBody.invInertiaWorld, out result);
					TSVector.Add(ref result, ref rigidBody.angularVelocity, out rigidBody.angularVelocity);
				}
				if (rigidBody.affectedByGravity)
				{
					TSVector.Multiply(ref gravity, timestep, out result);
					TSVector.Add(ref rigidBody.linearVelocity, ref result, out rigidBody.linearVelocity);
				}
			}
			rigidBody.force.MakeZero();
			rigidBody.torque.MakeZero();
		}
	}

	private void IntegrateCallback(object obj)
	{
		RigidBody rigidBody = obj as RigidBody;
		TSVector.Multiply(ref rigidBody.linearVelocity, timestep, out var result);
		TSVector.Add(ref result, ref rigidBody.position, out rigidBody.position);
		if (!rigidBody.isParticle)
		{
			FP magnitude = rigidBody.angularVelocity.magnitude;
			TSVector result2;
			if (magnitude < FP.EN3)
			{
				TSVector.Multiply(ref rigidBody.angularVelocity, FP.Half * timestep - timestep * timestep * timestep * (2082 * FP.EN6) * magnitude * magnitude, out result2);
			}
			else
			{
				TSVector.Multiply(ref rigidBody.angularVelocity, FP.Sin(FP.Half * magnitude * timestep) / magnitude, out result2);
			}
			TSQuaternion quaternion = new TSQuaternion(result2.x, result2.y, result2.z, FP.Cos(magnitude * timestep * FP.Half));
			TSQuaternion.CreateFromMatrix(ref rigidBody.orientation, out var result3);
			TSQuaternion.Multiply(ref quaternion, ref result3, out quaternion);
			quaternion.Normalize();
			TSMatrix.CreateFromQuaternion(ref quaternion, out rigidBody.orientation);
		}
		rigidBody.linearVelocity *= 1 / (1 + timestep * rigidBody.linearDrag);
		rigidBody.angularVelocity *= 1 / (1 + timestep * rigidBody.angularDrag);
		rigidBody.Update();
		if (CollisionSystem.EnableSpeculativeContacts || rigidBody.EnableSpeculativeContacts)
		{
			rigidBody.SweptExpandBoundingBox(timestep);
		}
	}

	private void Integrate()
	{
		int i = 0;
		for (int count = rigidBodies.Count; i < count; i++)
		{
			RigidBody rigidBody = rigidBodies[i];
			if (!rigidBody.isStatic && rigidBody.IsActive)
			{
				integrateCallback(rigidBody);
			}
		}
	}

	internal bool CanBodiesCollide(RigidBody body1, RigidBody body2)
	{
		if (body1.disabled || body2.disabled || !physicsManager.IsCollisionEnabled(body1, body2))
		{
			return false;
		}
		if (body1.IsStaticNonKinematic && body2.IsStaticNonKinematic)
		{
			return false;
		}
		if (body1.IsColliderOnly || body2.IsColliderOnly)
		{
			if ((body1.IsColliderOnly && body1.IsStaticNonKinematic && body2.IsStaticNonKinematic) || (body2.IsColliderOnly && body2.IsStaticNonKinematic && body1.IsStaticNonKinematic))
			{
				return false;
			}
		}
		else if ((body1.isKinematic && body2.isStatic) || (body2.isKinematic && body1.isStatic))
		{
			return false;
		}
		return true;
	}

	private void CollisionDetected(RigidBody body1, RigidBody body2, TSVector point1, TSVector point2, TSVector normal, FP penetration)
	{
		bool flag = body1.IsColliderOnly || body2.IsColliderOnly;
		Arbiter arbiter = null;
		ArbiterMap arbiterMap = null;
		arbiterMap = ((!flag) ? this.arbiterMap : arbiterTriggerMap);
		bool flag2 = false;
		lock (arbiterMap)
		{
			arbiterMap.LookUpArbiter(body1, body2, out arbiter);
			if (arbiter == null)
			{
				arbiter = Arbiter.Pool.GetNew();
				arbiter.body1 = body1;
				arbiter.body2 = body2;
				arbiterMap.Add(new ArbiterKey(body1, body2), arbiter);
				flag2 = true;
			}
		}
		Contact contact = null;
		if (arbiter.body1 == body1)
		{
			TSVector.Negate(ref normal, out normal);
			contact = arbiter.AddContact(point1, point2, normal, penetration, contactSettings);
		}
		else
		{
			contact = arbiter.AddContact(point2, point1, normal, penetration, contactSettings);
		}
		if (flag2)
		{
			if (flag)
			{
				events.RaiseTriggerBeginCollide(contact);
				body1.arbitersTrigger.Add(arbiter);
				body2.arbitersTrigger.Add(arbiter);
				OverlapPairContact overlapPairContact = new OverlapPairContact(body1, body2);
				overlapPairContact.contact = contact;
				initialTriggers.Add(overlapPairContact);
			}
			else
			{
				events.RaiseBodiesBeginCollide(contact);
				addedArbiterQueue.Enqueue(arbiter);
				OverlapPairContact overlapPairContact2 = new OverlapPairContact(body1, body2);
				overlapPairContact2.contact = contact;
				initialCollisions.Add(overlapPairContact2);
			}
		}
		if (!flag && contact != null)
		{
			events.RaiseContactCreated(contact);
		}
	}

	private void CheckDeactivation()
	{
		if (!AllowDeactivation)
		{
			return;
		}
		foreach (CollisionIsland island in islands)
		{
			bool flag = true;
			if (!AllowDeactivation)
			{
				flag = false;
			}
			else
			{
				int i = 0;
				for (int count = island.bodies.Count; i < count; i++)
				{
					RigidBody rigidBody = island.bodies[i];
					if (rigidBody.AllowDeactivation && rigidBody.angularVelocity.sqrMagnitude < inactiveAngularThresholdSq && rigidBody.linearVelocity.sqrMagnitude < inactiveLinearThresholdSq)
					{
						rigidBody.inactiveTime += timestep;
						if (rigidBody.inactiveTime < deactivationTime)
						{
							flag = false;
						}
					}
					else
					{
						rigidBody.inactiveTime = FP.Zero;
						flag = false;
					}
				}
			}
			int j = 0;
			for (int count2 = island.bodies.Count; j < count2; j++)
			{
				RigidBody rigidBody2 = island.bodies[j];
				if (rigidBody2.isActive == flag)
				{
					if (rigidBody2.isActive)
					{
						rigidBody2.IsActive = false;
						events.RaiseDeactivatedBody(rigidBody2);
					}
					else
					{
						rigidBody2.IsActive = true;
						events.RaiseActivatedBody(rigidBody2);
					}
				}
			}
		}
	}

	public List<IBody> Bodies()
	{
		List<IBody> list = new List<IBody>();
		for (int i = 0; i < rigidBodies.Count; i++)
		{
			list.Add(rigidBodies[i]);
		}
		return list;
	}
}
