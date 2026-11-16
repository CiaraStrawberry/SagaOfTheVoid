using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class MinkowskiSumShape : Shape
{
	private TSVector shifted;

	private List<Shape> shapes = new List<Shape>();

	public MinkowskiSumShape(IEnumerable<Shape> shapes)
	{
		AddShapes(shapes);
	}

	public void AddShapes(IEnumerable<Shape> shapes)
	{
		foreach (Shape shape in shapes)
		{
			if (shape is Multishape)
			{
				throw new Exception("Multishapes not supported by MinkowskiSumShape.");
			}
			this.shapes.Add(shape);
		}
		UpdateShape();
	}

	public void AddShape(Shape shape)
	{
		if (shape is Multishape)
		{
			throw new Exception("Multishapes not supported by MinkowskiSumShape.");
		}
		shapes.Add(shape);
		UpdateShape();
	}

	public bool Remove(Shape shape)
	{
		if (shapes.Count == 1)
		{
			throw new Exception("There must be at least one shape.");
		}
		bool result = shapes.Remove(shape);
		UpdateShape();
		return result;
	}

	public TSVector Shift()
	{
		return -1 * shifted;
	}

	public override void CalculateMassInertia()
	{
		mass = Shape.CalculateMassInertia(this, out shifted, out inertia);
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		TSVector value = TSVector.zero;
		for (int i = 0; i < shapes.Count; i++)
		{
			shapes[i].SupportMapping(ref direction, out var result2);
			TSVector.Add(ref result2, ref value, out value);
		}
		TSVector.Subtract(ref value, ref shifted, out result);
	}
}
