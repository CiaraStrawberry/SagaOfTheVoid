using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class IslandManager : List<CollisionIsland>
{
	public static ResourcePool<CollisionIsland> Pool = new ResourcePool<CollisionIsland>();

	public List<CollisionIsland> islands;

	public Stack<Arbiter> rmStackArb = new Stack<Arbiter>();

	public Stack<Constraint> rmStackCstr = new Stack<Constraint>();

	private Queue<RigidBody> leftSearchQueue = new Queue<RigidBody>();

	private Queue<RigidBody> rightSearchQueue = new Queue<RigidBody>();

	public List<RigidBody> visitedBodiesLeft = new List<RigidBody>();

	public List<RigidBody> visitedBodiesRight = new List<RigidBody>();

	public IslandManager()
		: base((IEnumerable<CollisionIsland>)new List<CollisionIsland>())
	{
		islands = this;
	}

	public void ArbiterCreated(Arbiter arbiter)
	{
		AddConnection(arbiter.body1, arbiter.body2);
		arbiter.body1.arbiters.Add(arbiter);
		arbiter.body2.arbiters.Add(arbiter);
		if (arbiter.body1.island != null)
		{
			arbiter.body1.island.arbiter.Add(arbiter);
		}
		else if (arbiter.body2.island != null)
		{
			arbiter.body2.island.arbiter.Add(arbiter);
		}
	}

	public void ArbiterRemoved(Arbiter arbiter)
	{
		arbiter.body1.arbiters.Remove(arbiter);
		arbiter.body2.arbiters.Remove(arbiter);
		if (arbiter.body1.island != null)
		{
			arbiter.body1.island.arbiter.Remove(arbiter);
		}
		else if (arbiter.body2.island != null)
		{
			arbiter.body2.island.arbiter.Remove(arbiter);
		}
		RemoveConnection(arbiter.body1, arbiter.body2);
	}

	public void ConstraintCreated(Constraint constraint)
	{
		AddConnection(constraint.body1, constraint.body2);
		constraint.body1.constraints.Add(constraint);
		if (constraint.body2 != null)
		{
			constraint.body2.constraints.Add(constraint);
		}
		if (constraint.body1.island != null)
		{
			constraint.body1.island.constraints.Add(constraint);
		}
		else if (constraint.body2 != null && constraint.body2.island != null)
		{
			constraint.body2.island.constraints.Add(constraint);
		}
	}

	public void ConstraintRemoved(Constraint constraint)
	{
		constraint.body1.constraints.Remove(constraint);
		if (constraint.body2 != null)
		{
			constraint.body2.constraints.Remove(constraint);
		}
		if (constraint.body1.island != null)
		{
			constraint.body1.island.constraints.Remove(constraint);
		}
		else if (constraint.body2 != null && constraint.body2.island != null)
		{
			constraint.body2.island.constraints.Remove(constraint);
		}
		RemoveConnection(constraint.body1, constraint.body2);
	}

	public void MakeBodyStatic(RigidBody body)
	{
		body.connections.Clear();
		if (body.island != null)
		{
			body.island.bodies.Remove(body);
			if (body.island.bodies.Count == 0)
			{
				body.island.ClearLists();
				islands.Remove(body.island);
				Pool.GiveBack(body.island);
			}
		}
		body.island = null;
	}

	public void RemoveBody(RigidBody body)
	{
		foreach (Arbiter arbiter in body.arbiters)
		{
			rmStackArb.Push(arbiter);
		}
		while (rmStackArb.Count > 0)
		{
			ArbiterRemoved(rmStackArb.Pop());
		}
		foreach (Constraint constraint in body.constraints)
		{
			rmStackCstr.Push(constraint);
		}
		while (rmStackCstr.Count > 0)
		{
			ConstraintRemoved(rmStackCstr.Pop());
		}
		body.arbiters.Clear();
		body.constraints.Clear();
		if (body.island != null)
		{
			body.island.ClearLists();
			Pool.GiveBack(body.island);
			islands.Remove(body.island);
			body.island = null;
		}
	}

	public void RemoveAll()
	{
		foreach (CollisionIsland island in islands)
		{
			foreach (RigidBody body in island.bodies)
			{
				body.arbiters.Clear();
				body.constraints.Clear();
				body.connections.Clear();
				body.island = null;
			}
			island.ClearLists();
		}
		islands.Clear();
	}

	private void AddConnection(RigidBody body1, RigidBody body2)
	{
		if (body1 == null || body1.IsStaticNonKinematic)
		{
			if (body2 != null && body2.island == null)
			{
				CollisionIsland collisionIsland = Pool.GetNew();
				collisionIsland.islandManager = this;
				body2.island = collisionIsland;
				body2.island.bodies.Add(body2);
				islands.Add(collisionIsland);
			}
		}
		else if (body2 == null || body2.IsStaticNonKinematic)
		{
			if (body1 != null && body1.island == null)
			{
				CollisionIsland collisionIsland2 = Pool.GetNew();
				collisionIsland2.islandManager = this;
				body1.island = collisionIsland2;
				body1.island.bodies.Add(body1);
				islands.Add(collisionIsland2);
			}
		}
		else if (body1 != null && body2 != null)
		{
			MergeIslands(body1, body2);
			body1.connections.Add(body2);
			body2.connections.Add(body1);
		}
	}

	private void RemoveConnection(RigidBody body1, RigidBody body2)
	{
		if (body1.IsStaticNonKinematic)
		{
			body2.connections.Remove(body1);
			return;
		}
		if (body2 == null || body2.IsStaticNonKinematic)
		{
			body1.connections.Remove(body2);
			return;
		}
		body1.connections.Remove(body2);
		body2.connections.Remove(body1);
		SplitIslands(body1, body2);
	}

	private void SplitIslands(RigidBody body0, RigidBody body1)
	{
		leftSearchQueue.Enqueue(body0);
		rightSearchQueue.Enqueue(body1);
		visitedBodiesLeft.Add(body0);
		visitedBodiesRight.Add(body1);
		body0.marker = 1;
		body1.marker = 2;
		while (true)
		{
			if (leftSearchQueue.Count > 0 && rightSearchQueue.Count > 0)
			{
				RigidBody rigidBody = leftSearchQueue.Dequeue();
				if (!rigidBody.IsStaticNonKinematic)
				{
					for (int i = 0; i < rigidBody.connections.Count; i++)
					{
						RigidBody rigidBody2 = rigidBody.connections[i];
						if (rigidBody2.marker == 0)
						{
							leftSearchQueue.Enqueue(rigidBody2);
							visitedBodiesLeft.Add(rigidBody2);
							rigidBody2.marker = 1;
							continue;
						}
						if (rigidBody2.marker != 2)
						{
							continue;
						}
						goto IL_00a9;
					}
				}
				rigidBody = rightSearchQueue.Dequeue();
				if (rigidBody.IsStaticNonKinematic)
				{
					continue;
				}
				for (int j = 0; j < rigidBody.connections.Count; j++)
				{
					RigidBody rigidBody3 = rigidBody.connections[j];
					if (rigidBody3.marker == 0)
					{
						rightSearchQueue.Enqueue(rigidBody3);
						visitedBodiesRight.Add(rigidBody3);
						rigidBody3.marker = 2;
						continue;
					}
					if (rigidBody3.marker != 1)
					{
						continue;
					}
					goto IL_0145;
				}
				continue;
			}
			CollisionIsland collisionIsland = Pool.GetNew();
			collisionIsland.islandManager = this;
			islands.Add(collisionIsland);
			if (leftSearchQueue.Count == 0)
			{
				for (int k = 0; k < visitedBodiesLeft.Count; k++)
				{
					RigidBody rigidBody4 = visitedBodiesLeft[k];
					body1.island.bodies.Remove(rigidBody4);
					collisionIsland.bodies.Add(rigidBody4);
					rigidBody4.island = collisionIsland;
					foreach (Arbiter arbiter in rigidBody4.arbiters)
					{
						body1.island.arbiter.Remove(arbiter);
						collisionIsland.arbiter.Add(arbiter);
					}
					foreach (Constraint constraint in rigidBody4.constraints)
					{
						body1.island.constraints.Remove(constraint);
						collisionIsland.constraints.Add(constraint);
					}
				}
				rightSearchQueue.Clear();
			}
			else
			{
				if (rightSearchQueue.Count != 0)
				{
					break;
				}
				for (int l = 0; l < visitedBodiesRight.Count; l++)
				{
					RigidBody rigidBody5 = visitedBodiesRight[l];
					body0.island.bodies.Remove(rigidBody5);
					collisionIsland.bodies.Add(rigidBody5);
					rigidBody5.island = collisionIsland;
					foreach (Arbiter arbiter2 in rigidBody5.arbiters)
					{
						body0.island.arbiter.Remove(arbiter2);
						collisionIsland.arbiter.Add(arbiter2);
					}
					foreach (Constraint constraint2 in rigidBody5.constraints)
					{
						body0.island.constraints.Remove(constraint2);
						collisionIsland.constraints.Add(constraint2);
					}
				}
				leftSearchQueue.Clear();
			}
			break;
			IL_0145:
			leftSearchQueue.Clear();
			rightSearchQueue.Clear();
			break;
			IL_00a9:
			leftSearchQueue.Clear();
			rightSearchQueue.Clear();
			break;
		}
		for (int m = 0; m < visitedBodiesLeft.Count; m++)
		{
			visitedBodiesLeft[m].marker = 0;
		}
		for (int n = 0; n < visitedBodiesRight.Count; n++)
		{
			visitedBodiesRight[n].marker = 0;
		}
		visitedBodiesLeft.Clear();
		visitedBodiesRight.Clear();
	}

	private void MergeIslands(RigidBody body0, RigidBody body1)
	{
		if (body0.island != body1.island)
		{
			if (body0.island == null)
			{
				body0.island = body1.island;
				body0.island.bodies.Add(body0);
				return;
			}
			if (body1.island == null)
			{
				body1.island = body0.island;
				body1.island.bodies.Add(body1);
				return;
			}
			RigidBody rigidBody;
			RigidBody rigidBody2;
			if (body0.island.bodies.Count > body1.island.bodies.Count)
			{
				rigidBody = body1;
				rigidBody2 = body0;
			}
			else
			{
				rigidBody = body0;
				rigidBody2 = body1;
			}
			CollisionIsland island = rigidBody.island;
			Pool.GiveBack(island);
			islands.Remove(island);
			foreach (RigidBody body2 in island.bodies)
			{
				body2.island = rigidBody2.island;
				rigidBody2.island.bodies.Add(body2);
			}
			foreach (Arbiter item in island.arbiter)
			{
				rigidBody2.island.arbiter.Add(item);
			}
			foreach (Constraint constraint in island.constraints)
			{
				rigidBody2.island.constraints.Add(constraint);
			}
			island.ClearLists();
		}
		else if (body0.island == null)
		{
			CollisionIsland collisionIsland = Pool.GetNew();
			collisionIsland.islandManager = this;
			body0.island = (body1.island = collisionIsland);
			body0.island.bodies.Add(body0);
			body0.island.bodies.Add(body1);
			islands.Add(collisionIsland);
		}
	}
}
