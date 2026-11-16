namespace TrueSync.Physics2D;

public class DistanceProxy
{
	internal FP Radius;

	internal Vertices Vertices = new Vertices();

	public void Set(Shape shape, int index)
	{
		switch (shape.ShapeType)
		{
		case ShapeType.Circle:
		{
			CircleShape circleShape = (CircleShape)shape;
			Vertices.Clear();
			Vertices.Add(circleShape.Position);
			Radius = circleShape.Radius;
			break;
		}
		case ShapeType.Polygon:
		{
			PolygonShape polygonShape = (PolygonShape)shape;
			Vertices.Clear();
			for (int i = 0; i < polygonShape.Vertices.Count; i++)
			{
				Vertices.Add(polygonShape.Vertices[i]);
			}
			Radius = polygonShape.Radius;
			break;
		}
		case ShapeType.Chain:
		{
			ChainShape chainShape = (ChainShape)shape;
			Vertices.Clear();
			Vertices.Add(chainShape.Vertices[index]);
			Vertices.Add((index + 1 >= chainShape.Vertices.Count) ? chainShape.Vertices[0] : chainShape.Vertices[index + 1]);
			Radius = chainShape.Radius;
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = (EdgeShape)shape;
			Vertices.Clear();
			Vertices.Add(edgeShape.Vertex1);
			Vertices.Add(edgeShape.Vertex2);
			Radius = edgeShape.Radius;
			break;
		}
		}
	}

	public int GetSupport(TSVector2 direction)
	{
		int result = 0;
		FP fP = TSVector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Count; i++)
		{
			FP fP2 = TSVector2.Dot(Vertices[i], direction);
			if (fP2 > fP)
			{
				result = i;
				fP = fP2;
			}
		}
		return result;
	}

	public TSVector2 GetSupportVertex(TSVector2 direction)
	{
		int index = 0;
		FP fP = TSVector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Count; i++)
		{
			FP fP2 = TSVector2.Dot(Vertices[i], direction);
			if (fP2 > fP)
			{
				index = i;
				fP = fP2;
			}
		}
		return Vertices[index];
	}
}
