using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class TriangleMeshShape : Multishape
{
	private List<int> potentialTriangles = new List<int>();

	private Octree octree;

	private FP sphericalExpansion = FP.EN2;

	private TSVector[] vecs = new TSVector[3];

	private bool flipNormal;

	private TSVector normal = TSVector.up;

	public FP SphericalExpansion
	{
		get
		{
			return sphericalExpansion;
		}
		set
		{
			sphericalExpansion = value;
		}
	}

	public bool FlipNormals
	{
		get
		{
			return flipNormal;
		}
		set
		{
			flipNormal = value;
		}
	}

	public TriangleMeshShape(Octree octree)
	{
		this.octree = octree;
		UpdateShape();
	}

	internal TriangleMeshShape()
	{
	}

	protected override Multishape CreateWorkingClone()
	{
		TriangleMeshShape triangleMeshShape = new TriangleMeshShape(octree);
		triangleMeshShape.sphericalExpansion = sphericalExpansion;
		return triangleMeshShape;
	}

	public override int Prepare(ref TSBBox box)
	{
		potentialTriangles.Clear();
		TSBBox testBox = box;
		testBox.min.x -= sphericalExpansion;
		testBox.min.y -= sphericalExpansion;
		testBox.min.z -= sphericalExpansion;
		testBox.max.x += sphericalExpansion;
		testBox.max.y += sphericalExpansion;
		testBox.max.z += sphericalExpansion;
		octree.GetTrianglesIntersectingtAABox(potentialTriangles, ref testBox);
		return potentialTriangles.Count;
	}

	public override void MakeHull(ref List<TSVector> triangleList, int generationThreshold)
	{
		TSBBox testBox = TSBBox.LargeBox;
		List<int> list = new List<int>();
		octree.GetTrianglesIntersectingtAABox(list, ref testBox);
		for (int i = 0; i < list.Count; i++)
		{
			triangleList.Add(octree.GetVertex(octree.GetTriangleVertexIndex(i).I0));
			triangleList.Add(octree.GetVertex(octree.GetTriangleVertexIndex(i).I1));
			triangleList.Add(octree.GetVertex(octree.GetTriangleVertexIndex(i).I2));
		}
	}

	public override int Prepare(ref TSVector rayOrigin, ref TSVector rayDelta)
	{
		potentialTriangles.Clear();
		TSVector.Normalize(ref rayDelta, out var result);
		result = rayDelta + result * sphericalExpansion;
		octree.GetTrianglesIntersectingRay(potentialTriangles, rayOrigin, result);
		return potentialTriangles.Count;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		TSVector.Normalize(ref direction, out var result2);
		result2 *= sphericalExpansion;
		FP fP = TSVector.Dot(ref vecs[0], ref direction);
		int num = 0;
		FP fP2 = TSVector.Dot(ref vecs[1], ref direction);
		if (fP2 > fP)
		{
			fP = fP2;
			num = 1;
		}
		fP2 = TSVector.Dot(ref vecs[2], ref direction);
		if (fP2 > fP)
		{
			fP = fP2;
			num = 2;
		}
		result = vecs[num] + result2;
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		box = octree.rootNodeBox;
		box.min.x -= sphericalExpansion;
		box.min.y -= sphericalExpansion;
		box.min.z -= sphericalExpansion;
		box.max.x += sphericalExpansion;
		box.max.y += sphericalExpansion;
		box.max.z += sphericalExpansion;
		box.Transform(ref orientation);
	}

	public override void SetCurrentShape(int index)
	{
		ref TSVector reference = ref vecs[0];
		reference = octree.GetVertex(octree.tris[potentialTriangles[index]].I0);
		ref TSVector reference2 = ref vecs[1];
		reference2 = octree.GetVertex(octree.tris[potentialTriangles[index]].I1);
		ref TSVector reference3 = ref vecs[2];
		reference3 = octree.GetVertex(octree.tris[potentialTriangles[index]].I2);
		TSVector value = vecs[0];
		TSVector.Add(ref value, ref vecs[1], out value);
		TSVector.Add(ref value, ref vecs[2], out value);
		TSVector.Multiply(ref value, FP.One / (3 * FP.One), out value);
		geomCen = value;
		TSVector.Subtract(ref vecs[1], ref vecs[0], out value);
		TSVector.Subtract(ref vecs[2], ref vecs[0], out normal);
		TSVector.Cross(ref value, ref normal, out normal);
		if (flipNormal)
		{
			normal.Negate();
		}
	}

	public void CollisionNormal(out TSVector normal)
	{
		normal = this.normal;
	}
}
