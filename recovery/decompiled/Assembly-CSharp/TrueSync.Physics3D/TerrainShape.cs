using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class TerrainShape : Multishape
{
	private FP[,] heights;

	private FP scaleX;

	private FP scaleZ;

	private int heightsLength0;

	private int heightsLength1;

	private int minX;

	private int maxX;

	private int minZ;

	private int maxZ;

	private int numX;

	private int numZ;

	private TSBBox boundings;

	private FP sphericalExpansion = 5 * FP.EN2;

	private TSVector[] points = new TSVector[3];

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

	public TerrainShape(FP[,] heights, FP scaleX, FP scaleZ)
	{
		heightsLength0 = heights.GetLength(0);
		heightsLength1 = heights.GetLength(1);
		boundings = TSBBox.SmallBox;
		for (int i = 0; i < heightsLength0; i++)
		{
			for (int j = 0; j < heightsLength1; j++)
			{
				if (heights[i, j] > boundings.max.y)
				{
					boundings.max.y = heights[i, j];
				}
				else if (heights[i, j] < boundings.min.y)
				{
					boundings.min.y = heights[i, j];
				}
			}
		}
		boundings.min.x = FP.Zero;
		boundings.min.z = FP.Zero;
		boundings.max.x = heightsLength0 * scaleX;
		boundings.max.z = heightsLength1 * scaleZ;
		this.heights = heights;
		this.scaleX = scaleX;
		this.scaleZ = scaleZ;
		UpdateShape();
	}

	internal TerrainShape()
	{
	}

	protected override Multishape CreateWorkingClone()
	{
		TerrainShape terrainShape = new TerrainShape();
		terrainShape.heights = heights;
		terrainShape.scaleX = scaleX;
		terrainShape.scaleZ = scaleZ;
		terrainShape.boundings = boundings;
		terrainShape.heightsLength0 = heightsLength0;
		terrainShape.heightsLength1 = heightsLength1;
		terrainShape.sphericalExpansion = sphericalExpansion;
		return terrainShape;
	}

	public override void SetCurrentShape(int index)
	{
		bool flag = false;
		if (index >= numX * numZ)
		{
			flag = true;
			index -= numX * numZ;
		}
		int num = index % numX;
		int num2 = index / numX;
		if (flag)
		{
			points[0].Set((minX + num) * scaleX, heights[minX + num, minZ + num2], (minZ + num2) * scaleZ);
			points[1].Set((minX + num + 1) * scaleX, heights[minX + num + 1, minZ + num2], (minZ + num2) * scaleZ);
			points[2].Set((minX + num) * scaleX, heights[minX + num, minZ + num2 + 1], (minZ + num2 + 1) * scaleZ);
		}
		else
		{
			points[0].Set((minX + num + 1) * scaleX, heights[minX + num + 1, minZ + num2], (minZ + num2) * scaleZ);
			points[1].Set((minX + num + 1) * scaleX, heights[minX + num + 1, minZ + num2 + 1], (minZ + num2 + 1) * scaleZ);
			points[2].Set((minX + num) * scaleX, heights[minX + num, minZ + num2 + 1], (minZ + num2 + 1) * scaleZ);
		}
		TSVector value = points[0];
		TSVector.Add(ref value, ref points[1], out value);
		TSVector.Add(ref value, ref points[2], out value);
		TSVector.Multiply(ref value, FP.One / (3 * FP.One), out value);
		geomCen = value;
		TSVector.Subtract(ref points[1], ref points[0], out value);
		TSVector.Subtract(ref points[2], ref points[0], out normal);
		TSVector.Cross(ref value, ref normal, out normal);
	}

	public void CollisionNormal(out TSVector normal)
	{
		normal = this.normal;
	}

	public override int Prepare(ref TSBBox box)
	{
		if (box.min.x < boundings.min.x)
		{
			minX = 0;
		}
		else
		{
			minX = (int)(long)FP.Floor((box.min.x - sphericalExpansion) / scaleX);
			minX = (int)(long)TSMath.Max(minX, 0);
		}
		if (box.max.x > boundings.max.x)
		{
			maxX = heightsLength0 - 1;
		}
		else
		{
			maxX = (int)(long)FP.Ceiling((box.max.x + sphericalExpansion) / scaleX);
			maxX = (int)(long)TSMath.Min(maxX, heightsLength0 - 1);
		}
		if (box.min.z < boundings.min.z)
		{
			minZ = 0;
		}
		else
		{
			minZ = (int)(long)FP.Floor((box.min.z - sphericalExpansion) / scaleZ);
			minZ = (int)(long)TSMath.Max(minZ, 0);
		}
		if (box.max.z > boundings.max.z)
		{
			maxZ = heightsLength1 - 1;
		}
		else
		{
			maxZ = (int)(long)FP.Ceiling((box.max.z + sphericalExpansion) / scaleZ);
			maxZ = (int)(long)TSMath.Min(maxZ, heightsLength1 - 1);
		}
		numX = maxX - minX;
		numZ = maxZ - minZ;
		return numX * numZ * 2;
	}

	public override void CalculateMassInertia()
	{
		inertia = TSMatrix.Identity;
		Mass = FP.One;
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		box = boundings;
		box.min.x -= sphericalExpansion;
		box.min.y -= sphericalExpansion;
		box.min.z -= sphericalExpansion;
		box.max.x += sphericalExpansion;
		box.max.y += sphericalExpansion;
		box.max.z += sphericalExpansion;
		box.Transform(ref orientation);
	}

	public override void MakeHull(ref List<TSVector> triangleList, int generationThreshold)
	{
		for (int i = 0; i < (heightsLength0 - 1) * (heightsLength1 - 1); i++)
		{
			int num = i % (heightsLength0 - 1);
			int num2 = i / (heightsLength0 - 1);
			triangleList.Add(new TSVector(num * scaleX, heights[num, num2], num2 * scaleZ));
			triangleList.Add(new TSVector((num + 1) * scaleX, heights[num + 1, num2], num2 * scaleZ));
			triangleList.Add(new TSVector(num * scaleX, heights[num, num2 + 1], (num2 + 1) * scaleZ));
			triangleList.Add(new TSVector((num + 1) * scaleX, heights[num + 1, num2], num2 * scaleZ));
			triangleList.Add(new TSVector((num + 1) * scaleX, heights[num + 1, num2 + 1], (num2 + 1) * scaleZ));
			triangleList.Add(new TSVector(num * scaleX, heights[num, num2 + 1], (num2 + 1) * scaleZ));
		}
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		TSVector.Normalize(ref direction, out var result2);
		TSVector.Multiply(ref result2, sphericalExpansion, out result2);
		int num = 0;
		FP fP = TSVector.Dot(ref points[0], ref direction);
		FP fP2 = TSVector.Dot(ref points[1], ref direction);
		if (fP2 > fP)
		{
			fP = fP2;
			num = 1;
		}
		fP2 = TSVector.Dot(ref points[2], ref direction);
		if (fP2 > fP)
		{
			fP = fP2;
			num = 2;
		}
		TSVector.Add(ref points[num], ref result2, out result);
	}

	public override int Prepare(ref TSVector rayOrigin, ref TSVector rayDelta)
	{
		TSBBox box = TSBBox.SmallBox;
		TSVector.Normalize(ref rayDelta, out var result);
		result = rayOrigin + rayDelta + result * sphericalExpansion;
		box.AddPoint(ref rayOrigin);
		box.AddPoint(ref result);
		return Prepare(ref box);
	}
}
