using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public abstract class Shape : ISupportMappable
{
	private struct ClipTriangle
	{
		public TSVector n1;

		public TSVector n2;

		public TSVector n3;

		public int generation;
	}

	internal TSMatrix inertia = TSMatrix.Identity;

	internal FP mass = FP.One;

	internal TSBBox boundingBox = TSBBox.LargeBox;

	internal TSVector geomCen = TSVector.zero;

	public TSMatrix Inertia
	{
		get
		{
			return inertia;
		}
		protected set
		{
			inertia = value;
		}
	}

	public FP Mass
	{
		get
		{
			return mass;
		}
		protected set
		{
			mass = value;
		}
	}

	public TSBBox BoundingBox => boundingBox;

	public object Tag { get; set; }

	public event ShapeUpdatedHandler ShapeUpdated;

	public Shape()
	{
	}

	protected void RaiseShapeUpdated()
	{
		if (ShapeUpdated != null)
		{
			ShapeUpdated();
		}
	}

	public virtual void MakeHull(ref List<TSVector> triangleList, int generationThreshold)
	{
		FP zero = FP.Zero;
		if (generationThreshold < 0)
		{
			generationThreshold = 4;
		}
		Stack<ClipTriangle> stack = new Stack<ClipTriangle>();
		TSVector[] array = new TSVector[6]
		{
			new TSVector(-1, 0, 0),
			new TSVector(1, 0, 0),
			new TSVector(0, -1, 0),
			new TSVector(0, 1, 0),
			new TSVector(0, 0, -1),
			new TSVector(0, 0, 1)
		};
		int[,] array2 = new int[8, 3]
		{
			{ 5, 1, 3 },
			{ 4, 3, 1 },
			{ 3, 4, 0 },
			{ 0, 5, 3 },
			{ 5, 2, 1 },
			{ 4, 1, 2 },
			{ 2, 0, 4 },
			{ 0, 2, 5 }
		};
		for (int i = 0; i < 8; i++)
		{
			stack.Push(new ClipTriangle
			{
				n1 = array[array2[i, 0]],
				n2 = array[array2[i, 1]],
				n3 = array[array2[i, 2]],
				generation = 0
			});
		}
		while (stack.Count > 0)
		{
			ClipTriangle clipTriangle = stack.Pop();
			SupportMapping(ref clipTriangle.n1, out var result);
			SupportMapping(ref clipTriangle.n2, out var result2);
			SupportMapping(ref clipTriangle.n3, out var result3);
			FP sqrMagnitude = (result2 - result).sqrMagnitude;
			FP sqrMagnitude2 = (result3 - result2).sqrMagnitude;
			FP sqrMagnitude3 = (result - result3).sqrMagnitude;
			if (TSMath.Max(TSMath.Max(sqrMagnitude, sqrMagnitude2), sqrMagnitude3) > zero && clipTriangle.generation < generationThreshold)
			{
				ClipTriangle t = default;
				ClipTriangle t2 = default;
				ClipTriangle t3 = default;
				ClipTriangle t4 = default;
				t.generation = clipTriangle.generation + 1;
				t2.generation = clipTriangle.generation + 1;
				t3.generation = clipTriangle.generation + 1;
				t4.generation = clipTriangle.generation + 1;
				t.n1 = clipTriangle.n1;
				t2.n2 = clipTriangle.n2;
				t3.n3 = clipTriangle.n3;
				TSVector tSVector = FP.Half * (clipTriangle.n1 + clipTriangle.n2);
				tSVector.Normalize();
				t.n2 = tSVector;
				t2.n1 = tSVector;
				t4.n3 = tSVector;
				tSVector = FP.Half * (clipTriangle.n2 + clipTriangle.n3);
				tSVector.Normalize();
				t2.n3 = tSVector;
				t3.n2 = tSVector;
				t4.n1 = tSVector;
				tSVector = FP.Half * (clipTriangle.n3 + clipTriangle.n1);
				tSVector.Normalize();
				t.n3 = tSVector;
				t3.n1 = tSVector;
				t4.n2 = tSVector;
				stack.Push(t);
				stack.Push(t2);
				stack.Push(t3);
				stack.Push(t4);
			}
			else if (((result3 - result) % (result2 - result)).sqrMagnitude > TSMath.Epsilon)
			{
				triangleList.Add(result);
				triangleList.Add(result2);
				triangleList.Add(result3);
			}
		}
	}

	public virtual void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		TSVector direction = TSVector.zero;
		direction.Set(orientation.M11, orientation.M21, orientation.M31);
		SupportMapping(ref direction, out direction);
		box.max.x = orientation.M11 * direction.x + orientation.M21 * direction.y + orientation.M31 * direction.z;
		direction.Set(orientation.M12, orientation.M22, orientation.M32);
		SupportMapping(ref direction, out direction);
		box.max.y = orientation.M12 * direction.x + orientation.M22 * direction.y + orientation.M32 * direction.z;
		direction.Set(orientation.M13, orientation.M23, orientation.M33);
		SupportMapping(ref direction, out direction);
		box.max.z = orientation.M13 * direction.x + orientation.M23 * direction.y + orientation.M33 * direction.z;
		direction.Set(-orientation.M11, -orientation.M21, -orientation.M31);
		SupportMapping(ref direction, out direction);
		box.min.x = orientation.M11 * direction.x + orientation.M21 * direction.y + orientation.M31 * direction.z;
		direction.Set(-orientation.M12, -orientation.M22, -orientation.M32);
		SupportMapping(ref direction, out direction);
		box.min.y = orientation.M12 * direction.x + orientation.M22 * direction.y + orientation.M32 * direction.z;
		direction.Set(-orientation.M13, -orientation.M23, -orientation.M33);
		SupportMapping(ref direction, out direction);
		box.min.z = orientation.M13 * direction.x + orientation.M23 * direction.y + orientation.M33 * direction.z;
	}

	public virtual void UpdateShape()
	{
		GetBoundingBox(ref TSMatrix.InternalIdentity, out boundingBox);
		CalculateMassInertia();
		RaiseShapeUpdated();
	}

	public static FP CalculateMassInertia(Shape shape, out TSVector centerOfMass, out TSMatrix inertia)
	{
		FP zero = FP.Zero;
		centerOfMass = TSVector.zero;
		inertia = TSMatrix.Zero;
		if (shape is Multishape)
		{
			throw new ArgumentException("Can't calculate inertia of multishapes.", "shape");
		}
		List<TSVector> triangleList = new List<TSVector>();
		shape.MakeHull(ref triangleList, 3);
		FP fP = FP.One / (60 * FP.One);
		FP fP2 = FP.One / (120 * FP.One);
		TSMatrix tSMatrix = new TSMatrix(fP, fP2, fP2, fP2, fP, fP2, fP2, fP2, fP);
		for (int i = 0; i < triangleList.Count; i += 3)
		{
			TSVector tSVector = triangleList[i];
			TSVector tSVector2 = triangleList[i + 1];
			TSVector tSVector3 = triangleList[i + 2];
			TSMatrix tSMatrix2 = new TSMatrix(tSVector.x, tSVector2.x, tSVector3.x, tSVector.y, tSVector2.y, tSVector3.y, tSVector.z, tSVector2.z, tSVector3.z);
			FP fP3 = tSMatrix2.Determinant();
			TSMatrix tSMatrix3 = TSMatrix.Multiply(tSMatrix2 * tSMatrix * TSMatrix.Transpose(tSMatrix2), fP3);
			TSVector tSVector4 = FP.One / (4 * FP.One) * (triangleList[i] + triangleList[i + 1] + triangleList[i + 2]);
			FP fP4 = FP.One / (6 * FP.One) * fP3;
			inertia += tSMatrix3;
			centerOfMass += fP4 * tSVector4;
			zero += fP4;
		}
		inertia = TSMatrix.Multiply(TSMatrix.Identity, inertia.Trace()) - inertia;
		centerOfMass *= FP.One / zero;
		FP x = centerOfMass.x;
		FP y = centerOfMass.y;
		FP z = centerOfMass.z;
		TSMatrix matrix = new TSMatrix(-zero * (y * y + z * z), zero * x * y, zero * x * z, zero * y * x, -zero * (z * z + x * x), zero * y * z, zero * z * x, zero * z * y, -zero * (x * x + y * y));
		TSMatrix.Add(ref inertia, ref matrix, out inertia);
		return zero;
	}

	public virtual void CalculateMassInertia()
	{
		mass = CalculateMassInertia(this, out geomCen, out inertia);
	}

	public abstract void SupportMapping(ref TSVector direction, out TSVector result);

	public void SupportCenter(out TSVector geomCenter)
	{
		geomCenter = geomCen;
	}
}
