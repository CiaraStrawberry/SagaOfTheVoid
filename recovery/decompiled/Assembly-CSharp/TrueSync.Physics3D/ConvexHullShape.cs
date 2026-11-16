using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class ConvexHullShape : Shape
{
	private List<TSVector> vertices;

	private TSVector shifted;

	public TSVector Shift => -1 * shifted;

	public ConvexHullShape(List<TSVector> vertices)
	{
		this.vertices = vertices;
		UpdateShape();
	}

	public override void CalculateMassInertia()
	{
		mass = Shape.CalculateMassInertia(this, out shifted, out inertia);
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		FP fP = FP.MinValue;
		int index = 0;
		for (int i = 0; i < vertices.Count; i++)
		{
			FP fP2 = TSVector.Dot(vertices[i], direction);
			if (fP2 > fP)
			{
				fP = fP2;
				index = i;
			}
		}
		result = vertices[index] - shifted;
	}
}
