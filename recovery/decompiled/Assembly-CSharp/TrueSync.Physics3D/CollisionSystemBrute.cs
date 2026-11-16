using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CollisionSystemBrute : CollisionSystem
{
	private List<IBroadphaseEntity> bodyList = new List<IBroadphaseEntity>();

	private bool swapOrder;

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
		int count = bodyList.Count;
		for (int i = 0; i < count; i++)
		{
			for (int j = i + 1; j < count; j++)
			{
				if (!CheckBothStaticOrInactive(bodyList[i], bodyList[j]) && CheckBoundingBoxes(bodyList[i], bodyList[j]) && RaisePassedBroadphase(bodyList[i], bodyList[j]))
				{
					if (swapOrder)
					{
						Detect(bodyList[i], bodyList[j]);
					}
					else
					{
						Detect(bodyList[j], bodyList[i]);
					}
					swapOrder = !swapOrder;
				}
			}
		}
	}

	private void DetectCallback(object obj)
	{
		BroadphasePair broadphasePair = obj as BroadphasePair;
		base.Detect(broadphasePair.Entity1, broadphasePair.Entity2);
		BroadphasePair.Pool.GiveBack(broadphasePair);
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
