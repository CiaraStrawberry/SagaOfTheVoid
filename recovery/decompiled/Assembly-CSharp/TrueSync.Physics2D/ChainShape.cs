namespace TrueSync.Physics2D;

public class ChainShape : Shape
{
	public Vertices Vertices;

	private TSVector2 _prevVertex;

	private TSVector2 _nextVertex;

	private bool _hasPrevVertex;

	private bool _hasNextVertex;

	private static EdgeShape _edgeShape = new EdgeShape();

	public override int ChildCount => Vertices.Count - 1;

	public TSVector2 PrevVertex
	{
		get
		{
			return _prevVertex;
		}
		set
		{
			_prevVertex = value;
			_hasPrevVertex = true;
		}
	}

	public TSVector2 NextVertex
	{
		get
		{
			return _nextVertex;
		}
		set
		{
			_nextVertex = value;
			_hasNextVertex = true;
		}
	}

	public ChainShape()
		: base(0)
	{
		ShapeType = ShapeType.Chain;
		_radius = Settings.PolygonRadius;
	}

	public ChainShape(Vertices vertices, bool createLoop = false)
		: base(0)
	{
		ShapeType = ShapeType.Chain;
		_radius = Settings.PolygonRadius;
		for (int i = 1; i < vertices.Count; i++)
		{
			TSVector2 tSVector = vertices[i - 1];
			TSVector2 tSVector2 = vertices[i];
		}
		Vertices = new Vertices(vertices);
		if (createLoop)
		{
			Vertices.Add(vertices[0]);
			PrevVertex = Vertices[Vertices.Count - 2];
			NextVertex = Vertices[1];
		}
	}

	internal void GetChildEdge(EdgeShape edge, int index)
	{
		edge.ShapeType = ShapeType.Edge;
		edge._radius = _radius;
		edge.Vertex1 = Vertices[index];
		edge.Vertex2 = Vertices[index + 1];
		if (index > 0)
		{
			edge.Vertex0 = Vertices[index - 1];
			edge.HasVertex0 = true;
		}
		else
		{
			edge.Vertex0 = _prevVertex;
			edge.HasVertex0 = _hasPrevVertex;
		}
		if (index < Vertices.Count - 2)
		{
			edge.Vertex3 = Vertices[index + 2];
			edge.HasVertex3 = true;
		}
		else
		{
			edge.Vertex3 = _nextVertex;
			edge.HasVertex3 = _hasNextVertex;
		}
	}

	public EdgeShape GetChildEdge(int index)
	{
		EdgeShape edgeShape = new EdgeShape();
		GetChildEdge(edgeShape, index);
		return edgeShape;
	}

	public override bool TestPoint(ref Transform transform, ref TSVector2 point)
	{
		return false;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		int num = childIndex + 1;
		if (num == Vertices.Count)
		{
			num = 0;
		}
		_edgeShape.Vertex1 = Vertices[childIndex];
		_edgeShape.Vertex2 = Vertices[num];
		return _edgeShape.RayCast(out output, ref input, ref transform, 0);
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		int num = childIndex + 1;
		if (num == Vertices.Count)
		{
			num = 0;
		}
		TSVector2 value = MathUtils.Mul(ref transform, Vertices[childIndex]);
		TSVector2 value2 = MathUtils.Mul(ref transform, Vertices[num]);
		aabb.LowerBound = TSVector2.Min(value, value2);
		aabb.UpperBound = TSVector2.Max(value, value2);
	}

	protected override void ComputeProperties()
	{
	}

	public override FP ComputeSubmergedArea(ref TSVector2 normal, FP offset, ref Transform xf, out TSVector2 sc)
	{
		sc = TSVector2.zero;
		return 0;
	}

	public bool CompareTo(ChainShape shape)
	{
		if (Vertices.Count != shape.Vertices.Count)
		{
			return false;
		}
		for (int i = 0; i < Vertices.Count; i++)
		{
			if (Vertices[i] != shape.Vertices[i])
			{
				return false;
			}
		}
		return PrevVertex == shape.PrevVertex && NextVertex == shape.NextVertex;
	}

	public override Shape Clone()
	{
		ChainShape chainShape = new ChainShape();
		chainShape.ShapeType = ShapeType;
		chainShape._density = _density;
		chainShape._radius = _radius;
		chainShape.PrevVertex = _prevVertex;
		chainShape.NextVertex = _nextVertex;
		chainShape._hasNextVertex = _hasNextVertex;
		chainShape._hasPrevVertex = _hasPrevVertex;
		chainShape.Vertices = new Vertices(Vertices);
		chainShape.MassData = MassData;
		return chainShape;
	}
}
