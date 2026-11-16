using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CollisionSystemSAP : CollisionSystem
{
	private class IBroadphaseEntityXCompare : IComparer<IBroadphaseEntity>
	{
		public int Compare(IBroadphaseEntity body1, IBroadphaseEntity body2)
		{
			FP fP = body1.BoundingBox.min.x - body2.BoundingBox.min.x;
			return (fP < 0) ? (-1) : ((fP > 0) ? 1 : 0);
		}
	}

	private List<IBroadphaseEntity> bodyList = new List<IBroadphaseEntity>();

	private List<IBroadphaseEntity> active = new List<IBroadphaseEntity>();

	private IBroadphaseEntityXCompare xComparer;

	private bool swapOrder;

	public CollisionSystemSAP()
	{
		xComparer = new IBroadphaseEntityXCompare();
	}

	public override bool RemoveEntity(IBroadphaseEntity body)
	{
		return bodyList.Remove(body);
	}

	public override void AddEntity(IBroadphaseEntity body)
	{
		if (bodyList.Contains(body))
		{
			throw new ArgumentException("The body was already added to the collision system.", "body");
		}
		bodyList.Add(body);
	}

	public override void Detect()
	{
		bodyList.Sort(xComparer);
		active.Clear();
		for (int i = 0; i < bodyList.Count; i++)
		{
			AddToActive(bodyList[i], addToList: false);
		}
	}

	private void AddToActive(IBroadphaseEntity body, bool addToList)
	{
		FP x = body.BoundingBox.min.x;
		int num = active.Count;
		bool isStaticOrInactive = body.IsStaticOrInactive;
		int num2 = 0;
		while (num2 != num)
		{
			IBroadphaseEntity broadphaseEntity = active[num2];
			TSBBox boundingBox = broadphaseEntity.BoundingBox;
			if (boundingBox.max.x < x)
			{
				num--;
				active.RemoveAt(num2);
				continue;
			}
			TSBBox boundingBox2 = body.BoundingBox;
			if ((!isStaticOrInactive || !broadphaseEntity.IsStaticOrInactive) && boundingBox2.max.z >= boundingBox.min.z && boundingBox2.min.z <= boundingBox.max.z && boundingBox2.max.y >= boundingBox.min.y && boundingBox2.min.y <= boundingBox.max.y && RaisePassedBroadphase(broadphaseEntity, body))
			{
				if (swapOrder)
				{
					Detect(body, broadphaseEntity);
				}
				else
				{
					Detect(broadphaseEntity, body);
				}
				swapOrder = !swapOrder;
			}
			num2++;
		}
		active.Add(body);
	}

	private void DetectCallback(object obj)
	{
		BroadphasePair broadphasePair = obj as BroadphasePair;
		base.Detect(broadphasePair.Entity1, broadphasePair.Entity2);
		BroadphasePair.Pool.GiveBack(broadphasePair);
	}

	private int Compare(IBroadphaseEntity body1, IBroadphaseEntity body2)
	{
		FP fP = body1.BoundingBox.min.x - body2.BoundingBox.min.x;
		return (fP < 0) ? (-1) : ((fP > 0) ? 1 : 0);
	}

	public override bool Raycast(TSVector rayOrigin, TSVector rayDirection, RaycastCallback raycast, out RigidBody body, out TSVector normal, out FP fraction)
	{
		body = null;
		normal = TSVector.zero;
		fraction = FP.MaxValue;
		bool result = false;
		foreach (IBroadphaseEntity body2 in bodyList)
		{
			TSVector normal2;
			FP fraction2;
			if (body2 is SoftBody)
			{
				SoftBody softBody = body2 as SoftBody;
				foreach (SoftBody.MassPoint vertexBody in softBody.VertexBodies)
				{
					if (Raycast(vertexBody, rayOrigin, rayDirection, out normal2, out fraction2) && fraction2 < fraction && (raycast == null || raycast(vertexBody, normal2, fraction2)))
					{
						body = vertexBody;
						normal = normal2;
						fraction = fraction2;
						result = true;
					}
				}
			}
			else
			{
				RigidBody rigidBody = body2 as RigidBody;
				if (Raycast(rigidBody, rayOrigin, rayDirection, out normal2, out fraction2) && fraction2 < fraction && (raycast == null || raycast(rigidBody, normal2, fraction2)))
				{
					body = rigidBody;
					normal = normal2;
					fraction = fraction2;
					result = true;
				}
			}
		}
		return result;
	}

	public override bool Raycast(RigidBody body, TSVector rayOrigin, TSVector rayDirection, out TSVector normal, out FP fraction)
	{
		fraction = FP.MaxValue;
		normal = TSVector.zero;
		if (!body.BoundingBox.RayIntersect(ref rayOrigin, ref rayDirection))
		{
			return false;
		}
		if (body.Shape is Multishape)
		{
			Multishape multishape = (body.Shape as Multishape).RequestWorkingClone();
			bool result = false;
			TSVector.Subtract(ref rayOrigin, ref body.position, out var result2);
			TSVector.Transform(ref result2, ref body.invOrientation, out result2);
			TSVector.Transform(ref rayDirection, ref body.invOrientation, out var result3);
			int num = multishape.Prepare(ref result2, ref result3);
			for (int i = 0; i < num; i++)
			{
				multishape.SetCurrentShape(i);
				if (GJKCollide.Raycast(multishape, ref body.orientation, ref body.invOrientation, ref body.position, ref rayOrigin, ref rayDirection, out var fraction2, out var normal2) && fraction2 < fraction)
				{
					if (useTerrainNormal && multishape is TerrainShape)
					{
						(multishape as TerrainShape).CollisionNormal(out normal2);
						TSVector.Transform(ref normal2, ref body.orientation, out normal2);
						normal2.Negate();
					}
					else if (useTriangleMeshNormal && multishape is TriangleMeshShape)
					{
						(multishape as TriangleMeshShape).CollisionNormal(out normal2);
						TSVector.Transform(ref normal2, ref body.orientation, out normal2);
						normal2.Negate();
					}
					normal = normal2;
					fraction = fraction2;
					result = true;
				}
			}
			multishape.ReturnWorkingClone();
			return result;
		}
		return GJKCollide.Raycast(body.Shape, ref body.orientation, ref body.invOrientation, ref body.position, ref rayOrigin, ref rayDirection, out fraction, out normal);
	}
}
