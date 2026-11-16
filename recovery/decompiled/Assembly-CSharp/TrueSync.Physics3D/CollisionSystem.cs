using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public abstract class CollisionSystem
{
	protected class BroadphasePair
	{
		public IBroadphaseEntity Entity1;

		public IBroadphaseEntity Entity2;

		public static ResourcePool<BroadphasePair> Pool = new ResourcePool<BroadphasePair>();
	}

	public World world;

	private CollisionDetectedHandler collisionDetected;

	private bool speculativeContacts;

	internal bool useTerrainNormal = true;

	internal bool useTriangleMeshNormal = true;

	private ResourcePool<List<int>> potentialTriangleLists = new ResourcePool<List<int>>();

	public bool EnableSpeculativeContacts
	{
		get
		{
			return speculativeContacts;
		}
		set
		{
			speculativeContacts = value;
		}
	}

	public bool UseTriangleMeshNormal
	{
		get
		{
			return useTriangleMeshNormal;
		}
		set
		{
			useTriangleMeshNormal = value;
		}
	}

	public bool UseTerrainNormal
	{
		get
		{
			return useTerrainNormal;
		}
		set
		{
			useTerrainNormal = value;
		}
	}

	public event PassedBroadphaseHandler PassedBroadphase;

	public event CollisionDetectedHandler CollisionDetected
	{
		add
		{
			collisionDetected = (CollisionDetectedHandler)Delegate.Combine(collisionDetected, value);
		}
		remove
		{
			collisionDetected = (CollisionDetectedHandler)Delegate.Remove(collisionDetected, value);
		}
	}

	public CollisionSystem()
	{
	}

	public abstract bool RemoveEntity(IBroadphaseEntity body);

	public abstract void AddEntity(IBroadphaseEntity body);

	public virtual void Detect(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		RigidBody rigidBody = entity1 as RigidBody;
		RigidBody rigidBody2 = entity2 as RigidBody;
		if (rigidBody != null)
		{
			if (rigidBody2 != null)
			{
				DetectRigidRigid(rigidBody, rigidBody2);
			}
			else if (entity2 is SoftBody softBody)
			{
				DetectSoftRigid(rigidBody, softBody);
			}
			return;
		}
		SoftBody softBody2 = entity1 as SoftBody;
		if (rigidBody2 != null)
		{
			if (softBody2 != null)
			{
				DetectSoftRigid(rigidBody2, softBody2);
			}
			return;
		}
		SoftBody softBody3 = entity2 as SoftBody;
		if (softBody2 != null && softBody3 != null)
		{
			DetectSoftSoft(softBody2, softBody3);
		}
	}

	private void DetectSoftSoft(SoftBody body1, SoftBody body2)
	{
		List<int> list = potentialTriangleLists.GetNew();
		List<int> list2 = potentialTriangleLists.GetNew();
		body1.dynamicTree.Query(list2, list, body2.dynamicTree);
		for (int i = 0; i < list2.Count; i++)
		{
			SoftBody.Triangle userData = body1.dynamicTree.GetUserData(list[i]);
			SoftBody.Triangle userData2 = body2.dynamicTree.GetUserData(list2[i]);
			if (XenoCollide.Detect(userData, userData2, ref TSMatrix.InternalIdentity, ref TSMatrix.InternalIdentity, ref TSVector.InternalZero, ref TSVector.InternalZero, out var point, out var normal, out var penetration))
			{
				int index = FindNearestTrianglePoint(body1, list[i], ref point);
				int index2 = FindNearestTrianglePoint(body2, list2[i], ref point);
				RaiseCollisionDetected(body1.VertexBodies[index], body2.VertexBodies[index2], ref point, ref point, ref normal, penetration);
			}
		}
		list.Clear();
		list2.Clear();
		potentialTriangleLists.GiveBack(list);
		potentialTriangleLists.GiveBack(list2);
	}

	private void DetectRigidRigid(RigidBody body1, RigidBody body2)
	{
		bool flag = body1.Shape is Multishape;
		bool flag2 = body2.Shape is Multishape;
		bool flag3 = speculativeContacts || body1.EnableSpeculativeContacts || body2.EnableSpeculativeContacts;
		TSVector point;
		TSVector normal;
		FP penetration;
		if (!flag && !flag2)
		{
			if (XenoCollide.Detect(body1.Shape, body2.Shape, ref body1.orientation, ref body2.orientation, ref body1.position, ref body2.position, out point, out normal, out penetration))
			{
				FindSupportPoints(body1, body2, body1.Shape, body2.Shape, ref point, ref normal, out var point2, out var point3);
				RaiseCollisionDetected(body1, body2, ref point2, ref point3, ref normal, penetration);
			}
			else
			{
				if (!flag3 || !GJKCollide.ClosestPoints(body1.Shape, body2.Shape, ref body1.orientation, ref body2.orientation, ref body1.position, ref body2.position, out var point4, out var point5, out normal))
				{
					return;
				}
				TSVector tSVector = point5 - point4;
				if (tSVector.sqrMagnitude < (body1.sweptDirection - body2.sweptDirection).sqrMagnitude)
				{
					penetration = tSVector * normal;
					if (penetration < FP.Zero)
					{
						RaiseCollisionDetected(body1, body2, ref point4, ref point5, ref normal, penetration);
					}
				}
			}
			return;
		}
		if (flag && flag2)
		{
			Multishape multishape = body1.Shape as Multishape;
			Multishape multishape2 = body2.Shape as Multishape;
			multishape = multishape.RequestWorkingClone();
			multishape2 = multishape2.RequestWorkingClone();
			TSBBox box = body2.boundingBox;
			box.InverseTransform(ref body1.position, ref body1.orientation);
			int num = multishape.Prepare(ref box);
			box = body1.boundingBox;
			box.InverseTransform(ref body2.position, ref body2.orientation);
			int num2 = multishape2.Prepare(ref box);
			if (num == 0 || num2 == 0)
			{
				multishape.ReturnWorkingClone();
				multishape2.ReturnWorkingClone();
				return;
			}
			for (int i = 0; i < num; i++)
			{
				multishape.SetCurrentShape(i);
				for (int j = 0; j < num2; j++)
				{
					multishape2.SetCurrentShape(j);
					if (XenoCollide.Detect(multishape, multishape2, ref body1.orientation, ref body2.orientation, ref body1.position, ref body2.position, out point, out normal, out penetration))
					{
						FindSupportPoints(body1, body2, multishape, multishape2, ref point, ref normal, out var point6, out var point7);
						RaiseCollisionDetected(body1, body2, ref point6, ref point7, ref normal, penetration);
					}
					else
					{
						if (!flag3 || !GJKCollide.ClosestPoints(multishape, multishape2, ref body1.orientation, ref body2.orientation, ref body1.position, ref body2.position, out var point8, out var point9, out normal))
						{
							continue;
						}
						TSVector tSVector2 = point9 - point8;
						if (tSVector2.sqrMagnitude < (body1.sweptDirection - body2.sweptDirection).sqrMagnitude)
						{
							penetration = tSVector2 * normal;
							if (penetration < FP.Zero)
							{
								RaiseCollisionDetected(body1, body2, ref point8, ref point9, ref normal, penetration);
							}
						}
					}
				}
			}
			multishape.ReturnWorkingClone();
			multishape2.ReturnWorkingClone();
			return;
		}
		RigidBody rigidBody;
		RigidBody rigidBody2;
		if (body2.Shape is Multishape)
		{
			rigidBody = body2;
			rigidBody2 = body1;
		}
		else
		{
			rigidBody2 = body2;
			rigidBody = body1;
		}
		Multishape multishape3 = rigidBody.Shape as Multishape;
		multishape3 = multishape3.RequestWorkingClone();
		TSBBox box2 = rigidBody2.boundingBox;
		box2.InverseTransform(ref rigidBody.position, ref rigidBody.orientation);
		int num3 = multishape3.Prepare(ref box2);
		if (num3 == 0)
		{
			multishape3.ReturnWorkingClone();
			return;
		}
		for (int k = 0; k < num3; k++)
		{
			multishape3.SetCurrentShape(k);
			if (XenoCollide.Detect(multishape3, rigidBody2.Shape, ref rigidBody.orientation, ref rigidBody2.orientation, ref rigidBody.position, ref rigidBody2.position, out point, out normal, out penetration))
			{
				FindSupportPoints(rigidBody, rigidBody2, multishape3, rigidBody2.Shape, ref point, ref normal, out var point10, out var point11);
				if (useTerrainNormal && multishape3 is TerrainShape)
				{
					(multishape3 as TerrainShape).CollisionNormal(out normal);
					TSVector.Transform(ref normal, ref rigidBody.orientation, out normal);
				}
				else if (useTriangleMeshNormal && multishape3 is TriangleMeshShape)
				{
					(multishape3 as TriangleMeshShape).CollisionNormal(out normal);
					TSVector.Transform(ref normal, ref rigidBody.orientation, out normal);
				}
				RaiseCollisionDetected(rigidBody, rigidBody2, ref point10, ref point11, ref normal, penetration);
			}
			else
			{
				if (!flag3 || !GJKCollide.ClosestPoints(multishape3, rigidBody2.Shape, ref rigidBody.orientation, ref rigidBody2.orientation, ref rigidBody.position, ref rigidBody2.position, out var point12, out var point13, out normal))
				{
					continue;
				}
				TSVector tSVector3 = point13 - point12;
				if (tSVector3.sqrMagnitude < (body1.sweptDirection - body2.sweptDirection).sqrMagnitude)
				{
					penetration = tSVector3 * normal;
					if (penetration < FP.Zero)
					{
						RaiseCollisionDetected(rigidBody, rigidBody2, ref point12, ref point13, ref normal, penetration);
					}
				}
			}
		}
		multishape3.ReturnWorkingClone();
	}

	private void DetectSoftRigid(RigidBody rigidBody, SoftBody softBody)
	{
		if (rigidBody.Shape is Multishape)
		{
			Multishape multishape = rigidBody.Shape as Multishape;
			multishape = multishape.RequestWorkingClone();
			TSBBox box = softBody.BoundingBox;
			box.InverseTransform(ref rigidBody.position, ref rigidBody.orientation);
			int num = multishape.Prepare(ref box);
			List<int> list = potentialTriangleLists.GetNew();
			softBody.dynamicTree.Query(list, ref rigidBody.boundingBox);
			foreach (int item in list)
			{
				SoftBody.Triangle userData = softBody.dynamicTree.GetUserData(item);
				for (int i = 0; i < num; i++)
				{
					multishape.SetCurrentShape(i);
					if (XenoCollide.Detect(multishape, userData, ref rigidBody.orientation, ref TSMatrix.InternalIdentity, ref rigidBody.position, ref TSVector.InternalZero, out var point, out var normal, out var penetration))
					{
						int index = FindNearestTrianglePoint(softBody, item, ref point);
						RaiseCollisionDetected(rigidBody, softBody.VertexBodies[index], ref point, ref point, ref normal, penetration);
					}
				}
			}
			list.Clear();
			potentialTriangleLists.GiveBack(list);
			multishape.ReturnWorkingClone();
			return;
		}
		List<int> list2 = potentialTriangleLists.GetNew();
		softBody.dynamicTree.Query(list2, ref rigidBody.boundingBox);
		foreach (int item2 in list2)
		{
			SoftBody.Triangle userData2 = softBody.dynamicTree.GetUserData(item2);
			if (XenoCollide.Detect(rigidBody.Shape, userData2, ref rigidBody.orientation, ref TSMatrix.InternalIdentity, ref rigidBody.position, ref TSVector.InternalZero, out var point2, out var normal2, out var penetration2))
			{
				int index2 = FindNearestTrianglePoint(softBody, item2, ref point2);
				RaiseCollisionDetected(rigidBody, softBody.VertexBodies[index2], ref point2, ref point2, ref normal2, penetration2);
			}
		}
		list2.Clear();
		potentialTriangleLists.GiveBack(list2);
	}

	public static int FindNearestTrianglePoint(SoftBody sb, int id, ref TSVector point)
	{
		SoftBody.Triangle userData = sb.dynamicTree.GetUserData(id);
		TSVector value = sb.VertexBodies[userData.indices.I0].position;
		TSVector.Subtract(ref value, ref point, out value);
		FP sqrMagnitude = value.sqrMagnitude;
		value = sb.VertexBodies[userData.indices.I1].position;
		TSVector.Subtract(ref value, ref point, out value);
		FP sqrMagnitude2 = value.sqrMagnitude;
		value = sb.VertexBodies[userData.indices.I2].position;
		TSVector.Subtract(ref value, ref point, out value);
		FP sqrMagnitude3 = value.sqrMagnitude;
		if (sqrMagnitude < sqrMagnitude2)
		{
			if (sqrMagnitude < sqrMagnitude3)
			{
				return userData.indices.I0;
			}
			return userData.indices.I2;
		}
		if (sqrMagnitude2 < sqrMagnitude3)
		{
			return userData.indices.I1;
		}
		return userData.indices.I2;
	}

	private void FindSupportPoints(RigidBody body1, RigidBody body2, Shape shape1, Shape shape2, ref TSVector point, ref TSVector normal, out TSVector point1, out TSVector point2)
	{
		TSVector.Negate(ref normal, out var result);
		SupportMapping(body1, shape1, ref result, out var result2);
		SupportMapping(body2, shape2, ref normal, out var result3);
		TSVector.Subtract(ref result2, ref point, out result2);
		TSVector.Subtract(ref result3, ref point, out result3);
		FP scaleFactor = TSVector.Dot(ref result2, ref normal);
		FP scaleFactor2 = TSVector.Dot(ref result3, ref normal);
		TSVector.Multiply(ref normal, scaleFactor, out result2);
		TSVector.Multiply(ref normal, scaleFactor2, out result3);
		TSVector.Add(ref point, ref result2, out point1);
		TSVector.Add(ref point, ref result3, out point2);
	}

	private void SupportMapping(RigidBody body, Shape workingShape, ref TSVector direction, out TSVector result)
	{
		TSVector.Transform(ref direction, ref body.invOrientation, out result);
		workingShape.SupportMapping(ref result, out result);
		TSVector.Transform(ref result, ref body.orientation, out result);
		TSVector.Add(ref result, ref body.position, out result);
	}

	public abstract bool Raycast(TSVector rayOrigin, TSVector rayDirection, RaycastCallback raycast, out RigidBody body, out TSVector normal, out FP fraction);

	public abstract bool Raycast(RigidBody body, TSVector rayOrigin, TSVector rayDirection, out TSVector normal, out FP fraction);

	public bool CheckBothStaticOrInactive(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		return entity1.IsStaticOrInactive && entity2.IsStaticOrInactive;
	}

	public bool CheckBothStaticNonKinematic(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		return !world.CanBodiesCollide((RigidBody)entity1, (RigidBody)entity2);
	}

	public bool CheckBoundingBoxes(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		TSBBox boundingBox = entity1.BoundingBox;
		TSBBox boundingBox2 = entity2.BoundingBox;
		return boundingBox.max.z >= boundingBox2.min.z && boundingBox.min.z <= boundingBox2.max.z && boundingBox.max.y >= boundingBox2.min.y && boundingBox.min.y <= boundingBox2.max.y && boundingBox.max.x >= boundingBox2.min.x && boundingBox.min.x <= boundingBox2.max.x;
	}

	public bool RaisePassedBroadphase(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
	{
		if (PassedBroadphase != null)
		{
			return PassedBroadphase(entity1, entity2);
		}
		return true;
	}

	protected void RaiseCollisionDetected(RigidBody body1, RigidBody body2, ref TSVector point1, ref TSVector point2, ref TSVector normal, FP penetration)
	{
		if (collisionDetected != null)
		{
			collisionDetected(body1, body2, point1, point2, normal, penetration);
		}
	}

	public abstract void Detect();
}
