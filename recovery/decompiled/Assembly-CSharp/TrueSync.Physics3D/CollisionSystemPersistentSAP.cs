using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CollisionSystemPersistentSAP : CollisionSystem
{
	private const int AddedObjectsBruteForceIsUsed = 250;

	public List<IBroadphaseEntity> bodyList = new List<IBroadphaseEntity>();

	public List<SweepPoint> axis1 = new List<SweepPoint>();

	public List<SweepPoint> axis2 = new List<SweepPoint>();

	public List<SweepPoint> axis3 = new List<SweepPoint>();

	public HashList<OverlapPair> fullOverlaps = new HashList<OverlapPair>();

	private Action<object> sortCallback;

	public List<IBroadphaseEntity> activeList = new List<IBroadphaseEntity>();

	public int addCounter;

	public Stack<OverlapPair> depricated = new Stack<OverlapPair>();

	public bool swapOrder;

	public CollisionSystemPersistentSAP()
	{
		sortCallback = SortCallback;
	}

	private int QuickSort(SweepPoint sweepPoint1, SweepPoint sweepPoint2)
	{
		FP value = sweepPoint1.Value;
		FP value2 = sweepPoint2.Value;
		if (value > value2)
		{
			return 1;
		}
		if (value2 > value)
		{
			return -1;
		}
		return 0;
	}

	private void DirtySortAxis(List<SweepPoint> axis)
	{
		axis.Sort(QuickSort);
		activeList.Clear();
		for (int i = 0; i < axis.Count; i++)
		{
			SweepPoint sweepPoint = axis[i];
			if (sweepPoint.Begin)
			{
				foreach (IBroadphaseEntity active in activeList)
				{
					if (CheckBoundingBoxes(active, sweepPoint.Body))
					{
						fullOverlaps.Add(new OverlapPair(active, sweepPoint.Body));
					}
				}
				activeList.Add(sweepPoint.Body);
			}
			else
			{
				activeList.Remove(sweepPoint.Body);
			}
		}
	}

	private void SortAxis(List<SweepPoint> axis)
	{
		for (int i = 1; i < axis.Count; i++)
		{
			SweepPoint sweepPoint = axis[i];
			FP value = sweepPoint.Value;
			int num = i - 1;
			while (num >= 0 && axis[num].Value > value)
			{
				SweepPoint sweepPoint2 = axis[num];
				if (sweepPoint.Begin && !sweepPoint2.Begin && CheckBoundingBoxes(sweepPoint2.Body, sweepPoint.Body))
				{
					lock (fullOverlaps)
					{
						fullOverlaps.Add(new OverlapPair(sweepPoint2.Body, sweepPoint.Body));
					}
				}
				if (!sweepPoint.Begin && sweepPoint2.Begin)
				{
					lock (fullOverlaps)
					{
						fullOverlaps.Remove(new OverlapPair(sweepPoint2.Body, sweepPoint.Body));
					}
				}
				axis[num + 1] = sweepPoint2;
				num--;
			}
			axis[num + 1] = sweepPoint;
		}
	}

	public override void AddEntity(IBroadphaseEntity body)
	{
		bodyList.Add(body);
		axis1.Add(new SweepPoint(body, begin: true, 0));
		axis1.Add(new SweepPoint(body, begin: false, 0));
		axis2.Add(new SweepPoint(body, begin: true, 1));
		axis2.Add(new SweepPoint(body, begin: false, 1));
		axis3.Add(new SweepPoint(body, begin: true, 2));
		axis3.Add(new SweepPoint(body, begin: false, 2));
		addCounter++;
	}

	public override bool RemoveEntity(IBroadphaseEntity body)
	{
		int num = 0;
		for (int i = 0; i < axis1.Count; i++)
		{
			if (axis1[i].Body == body)
			{
				num++;
				axis1.RemoveAt(i);
				if (num == 2)
				{
					break;
				}
				i--;
			}
		}
		num = 0;
		for (int j = 0; j < axis2.Count; j++)
		{
			if (axis2[j].Body == body)
			{
				num++;
				axis2.RemoveAt(j);
				if (num == 2)
				{
					break;
				}
				j--;
			}
		}
		num = 0;
		for (int k = 0; k < axis3.Count; k++)
		{
			if (axis3[k].Body == body)
			{
				num++;
				axis3.RemoveAt(k);
				if (num == 2)
				{
					break;
				}
				k--;
			}
		}
		foreach (OverlapPair fullOverlap in fullOverlaps)
		{
			if (fullOverlap.Entity1 == body || fullOverlap.Entity2 == body)
			{
				depricated.Push(fullOverlap);
			}
		}
		while (depricated.Count > 0)
		{
			fullOverlaps.Remove(depricated.Pop());
		}
		bodyList.Remove(body);
		return true;
	}

	public override void Detect()
	{
		if (addCounter > 250)
		{
			fullOverlaps.Clear();
			DirtySortAxis(axis1);
			DirtySortAxis(axis2);
			DirtySortAxis(axis3);
		}
		else
		{
			sortCallback(axis1);
			sortCallback(axis2);
			sortCallback(axis3);
		}
		addCounter = 0;
		foreach (OverlapPair fullOverlap in fullOverlaps)
		{
			if (!CheckBothStaticNonKinematic(fullOverlap.Entity1, fullOverlap.Entity2) && RaisePassedBroadphase(fullOverlap.Entity1, fullOverlap.Entity2))
			{
				if (swapOrder)
				{
					Detect(fullOverlap.Entity1, fullOverlap.Entity2);
				}
				else
				{
					Detect(fullOverlap.Entity2, fullOverlap.Entity1);
				}
				swapOrder = !swapOrder;
			}
		}
	}

	private void SortCallback(object obj)
	{
		SortAxis(obj as List<SweepPoint>);
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
