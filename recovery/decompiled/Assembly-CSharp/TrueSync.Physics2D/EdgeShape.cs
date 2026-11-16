namespace TrueSync.Physics2D;

public class EdgeShape : Shape
{
	internal TSVector2 _vertex1;

	internal TSVector2 _vertex2;

	public override int ChildCount => 1;

	public bool HasVertex0 { get; set; }

	public bool HasVertex3 { get; set; }

	public TSVector2 Vertex0 { get; set; }

	public TSVector2 Vertex3 { get; set; }

	public TSVector2 Vertex1
	{
		get
		{
			return _vertex1;
		}
		set
		{
			_vertex1 = value;
			ComputeProperties();
		}
	}

	public TSVector2 Vertex2
	{
		get
		{
			return _vertex2;
		}
		set
		{
			_vertex2 = value;
			ComputeProperties();
		}
	}

	internal EdgeShape()
		: base(0)
	{
		ShapeType = ShapeType.Edge;
		_radius = Settings.PolygonRadius;
	}

	public EdgeShape(TSVector2 start, TSVector2 end)
		: base(0)
	{
		ShapeType = ShapeType.Edge;
		_radius = Settings.PolygonRadius;
		Set(start, end);
	}

	public void Set(TSVector2 start, TSVector2 end)
	{
		_vertex1 = start;
		_vertex2 = end;
		HasVertex0 = false;
		HasVertex3 = false;
		ComputeProperties();
	}

	public override bool TestPoint(ref Transform transform, ref TSVector2 point)
	{
		return false;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default;
		TSVector2 tSVector = MathUtils.MulT(transform.q, input.Point1 - transform.p);
		TSVector2 tSVector2 = MathUtils.MulT(transform.q, input.Point2 - transform.p);
		TSVector2 tSVector3 = tSVector2 - tSVector;
		TSVector2 vertex = _vertex1;
		TSVector2 vertex2 = _vertex2;
		TSVector2 tSVector4 = vertex2 - vertex;
		TSVector2 tSVector5 = new TSVector2(tSVector4.y, -tSVector4.x);
		tSVector5.Normalize();
		FP fP = TSVector2.Dot(tSVector5, vertex - tSVector);
		FP fP2 = TSVector2.Dot(tSVector5, tSVector3);
		if (fP2 == 0f)
		{
			return false;
		}
		FP fP3 = fP / fP2;
		if (fP3 < 0f || input.MaxFraction < fP3)
		{
			return false;
		}
		TSVector2 tSVector6 = tSVector + fP3 * tSVector3;
		TSVector2 tSVector7 = vertex2 - vertex;
		FP fP4 = TSVector2.Dot(tSVector7, tSVector7);
		if (fP4 == 0f)
		{
			return false;
		}
		FP fP5 = TSVector2.Dot(tSVector6 - vertex, tSVector7) / fP4;
		if (fP5 < 0f || 1f < fP5)
		{
			return false;
		}
		output.Fraction = fP3;
		if (fP > 0f)
		{
			output.Normal = -tSVector5;
		}
		else
		{
			output.Normal = tSVector5;
		}
		return true;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		TSVector2 value = MathUtils.Mul(ref transform, _vertex1);
		TSVector2 value2 = MathUtils.Mul(ref transform, _vertex2);
		TSVector2 tSVector = TSVector2.Min(value, value2);
		TSVector2 tSVector2 = TSVector2.Max(value, value2);
		TSVector2 tSVector3 = new TSVector2(Radius, Radius);
		aabb.LowerBound = tSVector - tSVector3;
		aabb.UpperBound = tSVector2 + tSVector3;
	}

	protected override void ComputeProperties()
	{
		MassData.Centroid = 0.5f * (_vertex1 + _vertex2);
	}

	public override FP ComputeSubmergedArea(ref TSVector2 normal, FP offset, ref Transform xf, out TSVector2 sc)
	{
		sc = TSVector2.zero;
		return 0;
	}

	public bool CompareTo(EdgeShape shape)
	{
		return HasVertex0 == shape.HasVertex0 && HasVertex3 == shape.HasVertex3 && Vertex0 == shape.Vertex0 && Vertex1 == shape.Vertex1 && Vertex2 == shape.Vertex2 && Vertex3 == shape.Vertex3;
	}

	public override Shape Clone()
	{
		EdgeShape edgeShape = new EdgeShape();
		edgeShape.ShapeType = ShapeType;
		edgeShape._radius = _radius;
		edgeShape._density = _density;
		edgeShape.HasVertex0 = HasVertex0;
		edgeShape.HasVertex3 = HasVertex3;
		edgeShape.Vertex0 = Vertex0;
		edgeShape._vertex1 = _vertex1;
		edgeShape._vertex2 = _vertex2;
		edgeShape.Vertex3 = Vertex3;
		edgeShape.MassData = MassData;
		return edgeShape;
	}
}
