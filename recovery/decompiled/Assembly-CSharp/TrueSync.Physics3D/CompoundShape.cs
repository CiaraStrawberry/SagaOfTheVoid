using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class CompoundShape : Multishape
{
	public struct TransformedShape
	{
		private Shape shape;

		internal TSVector position;

		internal TSMatrix orientation;

		internal TSMatrix invOrientation;

		internal TSBBox boundingBox;

		public Shape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
			}
		}

		public TSVector Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public TSBBox BoundingBox => boundingBox;

		public TSMatrix InverseOrientation => invOrientation;

		public TSMatrix Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				TSMatrix.Transpose(ref orientation, out invOrientation);
			}
		}

		public TransformedShape(Shape shape, TSMatrix orientation, TSVector position)
		{
			this.position = position;
			this.orientation = orientation;
			TSMatrix.Transpose(ref orientation, out invOrientation);
			this.shape = shape;
			boundingBox = default;
			UpdateBoundingBox();
		}

		public void UpdateBoundingBox()
		{
			Shape.GetBoundingBox(ref orientation, out boundingBox);
			boundingBox.min += position;
			boundingBox.max += position;
		}
	}

	private TransformedShape[] shapes;

	private TSVector shifted;

	private TSBBox mInternalBBox;

	private int currentShape;

	private List<int> currentSubShapes = new List<int>();

	public TransformedShape[] Shapes => shapes;

	public TSVector Shift => -FP.One * shifted;

	public CompoundShape(List<TransformedShape> shapes)
	{
		this.shapes = new TransformedShape[shapes.Count];
		shapes.CopyTo(this.shapes);
		if (!TestValidity())
		{
			throw new ArgumentException("Multishapes are not supported!");
		}
		UpdateShape();
	}

	public CompoundShape(TransformedShape[] shapes)
	{
		this.shapes = new TransformedShape[shapes.Length];
		Array.Copy(shapes, this.shapes, shapes.Length);
		if (!TestValidity())
		{
			throw new ArgumentException("Multishapes are not supported!");
		}
		UpdateShape();
	}

	internal CompoundShape()
	{
	}

	private bool TestValidity()
	{
		for (int i = 0; i < shapes.Length; i++)
		{
			if (shapes[i].Shape is Multishape)
			{
				return false;
			}
		}
		return true;
	}

	public override void MakeHull(ref List<TSVector> triangleList, int generationThreshold)
	{
		List<TSVector> triangleList2 = new List<TSVector>();
		for (int i = 0; i < shapes.Length; i++)
		{
			shapes[i].Shape.MakeHull(ref triangleList2, 4);
			for (int j = 0; j < triangleList2.Count; j++)
			{
				TSVector position = triangleList2[j];
				TSVector.Transform(ref position, ref shapes[i].orientation, out position);
				TSVector.Add(ref position, ref shapes[i].position, out position);
				triangleList.Add(position);
			}
			triangleList2.Clear();
		}
	}

	private void DoShifting()
	{
		for (int i = 0; i < Shapes.Length; i++)
		{
			shifted += Shapes[i].position;
		}
		shifted *= FP.One / shapes.Length;
		for (int j = 0; j < Shapes.Length; j++)
		{
			Shapes[j].position -= shifted;
		}
	}

	public override void CalculateMassInertia()
	{
		inertia = TSMatrix.Zero;
		mass = FP.Zero;
		for (int i = 0; i < Shapes.Length; i++)
		{
			TSMatrix tSMatrix = Shapes[i].InverseOrientation * Shapes[i].Shape.Inertia * Shapes[i].Orientation;
			TSVector tSVector = Shapes[i].Position * -FP.One;
			FP fP = Shapes[i].Shape.Mass;
			tSMatrix.M11 += fP * (tSVector.y * tSVector.y + tSVector.z * tSVector.z);
			tSMatrix.M22 += fP * (tSVector.x * tSVector.x + tSVector.z * tSVector.z);
			tSMatrix.M33 += fP * (tSVector.x * tSVector.x + tSVector.y * tSVector.y);
			tSMatrix.M12 += -tSVector.x * tSVector.y * fP;
			tSMatrix.M21 += -tSVector.x * tSVector.y * fP;
			tSMatrix.M31 += -tSVector.x * tSVector.z * fP;
			tSMatrix.M13 += -tSVector.x * tSVector.z * fP;
			tSMatrix.M32 += -tSVector.y * tSVector.z * fP;
			tSMatrix.M23 += -tSVector.y * tSVector.z * fP;
			inertia += tSMatrix;
			mass += fP;
		}
	}

	protected override Multishape CreateWorkingClone()
	{
		CompoundShape compoundShape = new CompoundShape();
		compoundShape.shapes = shapes;
		return compoundShape;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		TSVector.Transform(ref direction, ref shapes[currentShape].invOrientation, out result);
		shapes[currentShape].Shape.SupportMapping(ref direction, out result);
		TSVector.Transform(ref result, ref shapes[currentShape].orientation, out result);
		TSVector.Add(ref result, ref shapes[currentShape].position, out result);
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		box.min = mInternalBBox.min;
		box.max = mInternalBBox.max;
		TSVector position = FP.Half * (box.max - box.min);
		TSVector position2 = FP.Half * (box.max + box.min);
		TSVector.Transform(ref position2, ref orientation, out var result);
		TSMath.Absolute(ref orientation, out var result2);
		TSVector.Transform(ref position, ref result2, out var result3);
		box.max = result + result3;
		box.min = result - result3;
	}

	public override void SetCurrentShape(int index)
	{
		currentShape = currentSubShapes[index];
		shapes[currentShape].Shape.SupportCenter(out geomCen);
		geomCen += shapes[currentShape].Position;
	}

	public override int Prepare(ref TSBBox box)
	{
		currentSubShapes.Clear();
		for (int i = 0; i < shapes.Length; i++)
		{
			if (shapes[i].boundingBox.Contains(ref box) != TSBBox.ContainmentType.Disjoint)
			{
				currentSubShapes.Add(i);
			}
		}
		return currentSubShapes.Count;
	}

	public override int Prepare(ref TSVector rayOrigin, ref TSVector rayEnd)
	{
		TSBBox box = TSBBox.SmallBox;
		box.AddPoint(ref rayOrigin);
		box.AddPoint(ref rayEnd);
		return Prepare(ref box);
	}

	public override void UpdateShape()
	{
		DoShifting();
		UpdateInternalBoundingBox();
		base.UpdateShape();
	}

	protected void UpdateInternalBoundingBox()
	{
		mInternalBBox.min = new TSVector(FP.MaxValue);
		mInternalBBox.max = new TSVector(FP.MinValue);
		for (int i = 0; i < shapes.Length; i++)
		{
			shapes[i].UpdateBoundingBox();
			TSBBox.CreateMerged(ref mInternalBBox, ref shapes[i].boundingBox, out mInternalBBox);
		}
	}
}
