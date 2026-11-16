using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class Octree
{
	[Flags]
	private enum EChild
	{
		XP = 1,
		YP = 2,
		ZP = 4,
		PPP = XP | YP | ZP,
		PPM = 3,
		PMP = 5,
		PMM = XP,
		MPP = 6,
		MPM = YP,
		MMP = ZP,
		MMM = 0
	}

	private struct Node
	{
		public ushort[] nodeIndices;

		public int[] triIndices;

		public TSBBox box;
	}

	private class BuildNode
	{
		public int childType;

		public List<int> nodeIndices = new List<int>();

		public List<int> triIndices = new List<int>();

		public TSBBox box;
	}

	private TSVector[] positions;

	private TSBBox[] triBoxes;

	private Node[] nodes;

	internal TriangleVertexIndices[] tris;

	internal TSBBox rootNodeBox;

	private ArrayResourcePool<ushort> nodeStackPool;

	public TSBBox RootNodeBox => rootNodeBox;

	public int NumTriangles => tris.Length;

	public Octree(List<TSVector> positions, List<TriangleVertexIndices> tris)
	{
		SetTriangles(positions, tris);
		BuildOctree();
	}

	public void Clear()
	{
		positions = null;
		triBoxes = null;
		tris = null;
		nodes = null;
		nodeStackPool.ResetResourcePool();
	}

	public void SetTriangles(List<TSVector> positions, List<TriangleVertexIndices> tris)
	{
		this.positions = new TSVector[positions.Count];
		positions.CopyTo(this.positions);
		this.tris = new TriangleVertexIndices[tris.Count];
		tris.CopyTo(this.tris);
	}

	public void BuildOctree()
	{
		triBoxes = new TSBBox[tris.Length];
		rootNodeBox = new TSBBox(new TSVector(FP.PositiveInfinity, FP.PositiveInfinity, FP.PositiveInfinity), new TSVector(FP.NegativeInfinity, FP.NegativeInfinity, FP.NegativeInfinity));
		for (int i = 0; i < tris.Length; i++)
		{
			TSVector.Min(ref positions[tris[i].I1], ref positions[tris[i].I2], out triBoxes[i].min);
			TSVector.Min(ref positions[tris[i].I0], ref triBoxes[i].min, out triBoxes[i].min);
			TSVector.Max(ref positions[tris[i].I1], ref positions[tris[i].I2], out triBoxes[i].max);
			TSVector.Max(ref positions[tris[i].I0], ref triBoxes[i].max, out triBoxes[i].max);
			TSVector.Min(ref rootNodeBox.min, ref triBoxes[i].min, out rootNodeBox.min);
			TSVector.Max(ref rootNodeBox.max, ref triBoxes[i].max, out rootNodeBox.max);
		}
		List<BuildNode> list = new List<BuildNode>();
		list.Add(new BuildNode());
		list[0].box = rootNodeBox;
		TSBBox[] array = new TSBBox[8];
		for (int j = 0; j < tris.Length; j++)
		{
			int num = 0;
			TSBBox aabb = rootNodeBox;
			while (aabb.Contains(ref triBoxes[j]) == TSBBox.ContainmentType.Contains)
			{
				int num2 = -1;
				for (int k = 0; k < 8; k++)
				{
					CreateAABox(ref aabb, (EChild)k, out array[k]);
					if (array[k].Contains(ref triBoxes[j]) == TSBBox.ContainmentType.Contains)
					{
						num2 = k;
						break;
					}
				}
				if (num2 == -1)
				{
					list[num].triIndices.Add(j);
					break;
				}
				int num3 = -1;
				for (int l = 0; l < list[num].nodeIndices.Count; l++)
				{
					if (list[list[num].nodeIndices[l]].childType == num2)
					{
						num3 = l;
						break;
					}
				}
				if (num3 == -1)
				{
					BuildNode buildNode = list[num];
					BuildNode buildNode2 = new BuildNode();
					buildNode2.childType = num2;
					buildNode2.box = array[num2];
					list.Add(buildNode2);
					num = list.Count - 1;
					aabb = array[num2];
					buildNode.nodeIndices.Add(num);
				}
				else
				{
					num = list[num].nodeIndices[num3];
					aabb = array[num2];
				}
			}
		}
		nodes = new Node[list.Count];
		nodeStackPool = new ArrayResourcePool<ushort>(list.Count);
		for (int m = 0; m < nodes.Length; m++)
		{
			nodes[m].nodeIndices = new ushort[list[m].nodeIndices.Count];
			for (int n = 0; n < nodes[m].nodeIndices.Length; n++)
			{
				nodes[m].nodeIndices[n] = (ushort)list[m].nodeIndices[n];
			}
			nodes[m].triIndices = new int[list[m].triIndices.Count];
			list[m].triIndices.CopyTo(nodes[m].triIndices);
			nodes[m].box = list[m].box;
		}
		list.Clear();
		list = null;
	}

	private void CreateAABox(ref TSBBox aabb, EChild child, out TSBBox result)
	{
		TSVector.Subtract(ref aabb.max, ref aabb.min, out var result2);
		TSVector.Multiply(ref result2, FP.Half, out result2);
		TSVector tSVector = TSVector.zero;
		switch (child)
		{
		case EChild.PPP:
			tSVector = new TSVector(1, 1, 1);
			break;
		case EChild.PPM:
			tSVector = new TSVector(1, 1, 0);
			break;
		case EChild.PMP:
			tSVector = new TSVector(1, 0, 1);
			break;
		case EChild.XP:
			tSVector = new TSVector(1, 0, 0);
			break;
		case EChild.MPP:
			tSVector = new TSVector(0, 1, 1);
			break;
		case EChild.YP:
			tSVector = new TSVector(0, 1, 0);
			break;
		case EChild.ZP:
			tSVector = new TSVector(0, 0, 1);
			break;
		case EChild.MMM:
			tSVector = new TSVector(0, 0, 0);
			break;
		}
		result = default;
		result.min = new TSVector(tSVector.x * result2.x, tSVector.y * result2.y, tSVector.z * result2.z);
		TSVector.Add(ref result.min, ref aabb.min, out result.min);
		TSVector.Add(ref result.min, ref result2, out result.max);
		FP eN = FP.EN5;
		TSVector.Multiply(ref result2, eN, out var result3);
		TSVector.Subtract(ref result.min, ref result3, out result.min);
		TSVector.Add(ref result.max, ref result3, out result.max);
	}

	private void GatherTriangles(int nodeIndex, ref List<int> tris)
	{
		tris.AddRange(nodes[nodeIndex].triIndices);
		int num = nodes[nodeIndex].nodeIndices.Length;
		for (int i = 0; i < num; i++)
		{
			int nodeIndex2 = nodes[nodeIndex].nodeIndices[i];
			GatherTriangles(nodeIndex2, ref tris);
		}
	}

	public int GetTrianglesIntersectingtAABox(List<int> triangles, ref TSBBox testBox)
	{
		if (nodes.Length == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 1;
		ushort[] array = nodeStackPool.GetNew();
		array[0] = 0;
		int num3 = 0;
		while (num < num2)
		{
			ushort num4 = array[num];
			num++;
			if (nodes[num4].box.Contains(ref testBox) == TSBBox.ContainmentType.Disjoint)
			{
				continue;
			}
			for (int i = 0; i < nodes[num4].triIndices.Length; i++)
			{
				if (triBoxes[nodes[num4].triIndices[i]].Contains(ref testBox) != TSBBox.ContainmentType.Disjoint)
				{
					triangles.Add(nodes[num4].triIndices[i]);
					num3++;
				}
			}
			int num5 = nodes[num4].nodeIndices.Length;
			for (int j = 0; j < num5; j++)
			{
				array[num2++] = nodes[num4].nodeIndices[j];
			}
		}
		nodeStackPool.GiveBack(array);
		return num3;
	}

	public int GetTrianglesIntersectingRay(List<int> triangles, TSVector rayOrigin, TSVector rayDelta)
	{
		if (nodes.Length == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 1;
		ushort[] array = nodeStackPool.GetNew();
		array[0] = 0;
		int num3 = 0;
		while (num < num2)
		{
			ushort num4 = array[num];
			num++;
			if (!nodes[num4].box.SegmentIntersect(ref rayOrigin, ref rayDelta))
			{
				continue;
			}
			for (int i = 0; i < nodes[num4].triIndices.Length; i++)
			{
				if (triBoxes[nodes[num4].triIndices[i]].SegmentIntersect(ref rayOrigin, ref rayDelta))
				{
					triangles.Add(nodes[num4].triIndices[i]);
					num3++;
				}
			}
			int num5 = nodes[num4].nodeIndices.Length;
			for (int j = 0; j < num5; j++)
			{
				array[num2++] = nodes[num4].nodeIndices[j];
			}
		}
		nodeStackPool.GiveBack(array);
		return num3;
	}

	public TriangleVertexIndices GetTriangleVertexIndex(int index)
	{
		return tris[index];
	}

	public TSVector GetVertex(int vertex)
	{
		return positions[vertex];
	}

	public void GetVertex(int vertex, out TSVector result)
	{
		result = positions[vertex];
	}
}
