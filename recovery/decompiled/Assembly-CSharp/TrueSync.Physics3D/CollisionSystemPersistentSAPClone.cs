using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CollisionSystemPersistentSAPClone
{
	public static ResourcePoolSweetPointClone poolSweetPointClone = new ResourcePoolSweetPointClone();

	public List<IBroadphaseEntity> bodyList = new List<IBroadphaseEntity>();

	public List<SweetPointClone> axis1 = new List<SweetPointClone>();

	public List<SweetPointClone> axis2 = new List<SweetPointClone>();

	public List<SweetPointClone> axis3 = new List<SweetPointClone>();

	public List<OverlapPair> fullOverlaps = new List<OverlapPair>();

	public List<IBroadphaseEntity> activeList = new List<IBroadphaseEntity>();

	public bool swapOrder;

	private int index;

	private int length;

	public void Reset()
	{
		index = 0;
		for (length = axis1.Count; index < length; index++)
		{
			poolSweetPointClone.GiveBack(axis1[index]);
		}
		index = 0;
		for (length = axis2.Count; index < length; index++)
		{
			poolSweetPointClone.GiveBack(axis2[index]);
		}
		index = 0;
		for (length = axis3.Count; index < length; index++)
		{
			poolSweetPointClone.GiveBack(axis3[index]);
		}
	}

	public void Clone(CollisionSystemPersistentSAP cs)
	{
		bodyList.Clear();
		index = 0;
		for (length = cs.bodyList.Count; index < length; index++)
		{
			bodyList.Add(cs.bodyList[index]);
		}
		axis1.Clear();
		index = 0;
		for (length = cs.axis1.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone = poolSweetPointClone.GetNew();
			sweetPointClone.Clone(cs.axis1[index]);
			axis1.Add(sweetPointClone);
		}
		axis2.Clear();
		index = 0;
		for (length = cs.axis2.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone2 = poolSweetPointClone.GetNew();
			sweetPointClone2.Clone(cs.axis2[index]);
			axis2.Add(sweetPointClone2);
		}
		axis3.Clear();
		index = 0;
		for (length = cs.axis3.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone3 = poolSweetPointClone.GetNew();
			sweetPointClone3.Clone(cs.axis3[index]);
			axis3.Add(sweetPointClone3);
		}
		fullOverlaps.Clear();
		index = 0;
		for (length = cs.fullOverlaps.Count; index < length; index++)
		{
			fullOverlaps.Add(cs.fullOverlaps[index]);
		}
		activeList.Clear();
		index = 0;
		for (length = cs.activeList.Count; index < length; index++)
		{
			activeList.Add(cs.activeList[index]);
		}
		swapOrder = cs.swapOrder;
	}

	public void Restore(CollisionSystemPersistentSAP cs)
	{
		cs.bodyList.Clear();
		cs.bodyList.AddRange(bodyList);
		cs.axis1.Clear();
		index = 0;
		for (length = axis1.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone = axis1[index];
			SweepPoint sweepPoint = new SweepPoint(null, begin: false, 0);
			sweetPointClone.Restore(sweepPoint);
			cs.axis1.Add(sweepPoint);
		}
		cs.axis2.Clear();
		index = 0;
		for (length = axis2.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone2 = axis2[index];
			SweepPoint sweepPoint2 = new SweepPoint(null, begin: false, 0);
			sweetPointClone2.Restore(sweepPoint2);
			cs.axis2.Add(sweepPoint2);
		}
		cs.axis3.Clear();
		index = 0;
		for (length = axis3.Count; index < length; index++)
		{
			SweetPointClone sweetPointClone3 = axis3[index];
			SweepPoint sweepPoint3 = new SweepPoint(null, begin: false, 0);
			sweetPointClone3.Restore(sweepPoint3);
			cs.axis3.Add(sweepPoint3);
		}
		cs.fullOverlaps.Clear();
		cs.fullOverlaps.AddRange(fullOverlaps);
		cs.activeList.Clear();
		cs.activeList.AddRange(activeList);
		cs.swapOrder = swapOrder;
	}
}
