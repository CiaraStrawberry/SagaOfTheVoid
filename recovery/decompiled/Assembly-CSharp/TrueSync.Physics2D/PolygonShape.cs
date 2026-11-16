namespace TrueSync.Physics2D;

public class PolygonShape : Shape
{
	internal Vertices _vertices;

	internal Vertices _normals;

	public Vertices Vertices
	{
		get
		{
			return _vertices;
		}
		set
		{
			_vertices = new Vertices(value);
			if (Settings.UseConvexHullPolygons)
			{
				if (_vertices.Count <= 3)
				{
					_vertices.ForceCounterClockWise();
				}
				else
				{
					_vertices = GiftWrap.GetConvexHull(_vertices);
				}
			}
			_vertices.ForceCounterClockWise();
			_normals = new Vertices(_vertices.Count);
			for (int i = 0; i < _vertices.Count; i++)
			{
				int index = ((i + 1 < _vertices.Count) ? (i + 1) : 0);
				TSVector2 tSVector = _vertices[index] - _vertices[i];
				TSVector2 item = new TSVector2(tSVector.y, -tSVector.x);
				item.Normalize();
				_normals.Add(item);
			}
			ComputeProperties();
		}
	}

	public Vertices Normals => _normals;

	public override int ChildCount => 1;

	public PolygonShape(Vertices vertices, FP density)
		: base(density)
	{
		ShapeType = ShapeType.Polygon;
		_radius = Settings.PolygonRadius;
		Vertices = vertices;
	}

	public PolygonShape(FP density)
		: base(density)
	{
		ShapeType = ShapeType.Polygon;
		_radius = Settings.PolygonRadius;
		_vertices = new Vertices(Settings.MaxPolygonVertices);
		_normals = new Vertices(Settings.MaxPolygonVertices);
	}

	internal PolygonShape()
		: base(0)
	{
		ShapeType = ShapeType.Polygon;
		_radius = Settings.PolygonRadius;
		_vertices = new Vertices(Settings.MaxPolygonVertices);
		_normals = new Vertices(Settings.MaxPolygonVertices);
	}

	protected override void ComputeProperties()
	{
		if (!(_density <= 0))
		{
			TSVector2 zero = TSVector2.zero;
			FP fP = 0f;
			FP fP2 = 0f;
			TSVector2 zero2 = TSVector2.zero;
			for (int i = 0; i < Vertices.Count; i++)
			{
				zero2 += Vertices[i];
			}
			zero2 *= (FP)(1f / (float)Vertices.Count);
			FP fP3 = 1f / 3f;
			for (int j = 0; j < Vertices.Count; j++)
			{
				TSVector2 tSVector = Vertices[j] - zero2;
				TSVector2 tSVector2 = ((j + 1 >= Vertices.Count) ? (Vertices[0] - zero2) : (Vertices[j + 1] - zero2));
				FP fP4 = MathUtils.Cross(tSVector, tSVector2);
				FP fP5 = 0.5f * fP4;
				fP += fP5;
				zero += fP5 * fP3 * (tSVector + tSVector2);
				FP x = tSVector.x;
				FP y = tSVector.y;
				FP x2 = tSVector2.x;
				FP y2 = tSVector2.y;
				FP fP6 = x * x + x2 * x + x2 * x2;
				FP fP7 = y * y + y2 * y + y2 * y2;
				fP2 += 0.25f * fP3 * fP4 * (fP6 + fP7);
			}
			MassData.Area = fP;
			MassData.Mass = _density * fP;
			zero *= 1f / fP;
			MassData.Centroid = zero + zero2;
			MassData.Inertia = _density * fP2;
			MassData.Inertia += MassData.Mass * (TSVector2.Dot(MassData.Centroid, MassData.Centroid) - TSVector2.Dot(zero, zero));
		}
	}

	public override bool TestPoint(ref Transform transform, ref TSVector2 point)
	{
		TSVector2 tSVector = MathUtils.MulT(transform.q, point - transform.p);
		for (int i = 0; i < Vertices.Count; i++)
		{
			FP fP = TSVector2.Dot(Normals[i], tSVector - Vertices[i]);
			if (fP > 0f)
			{
				return false;
			}
		}
		return true;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default;
		TSVector2 tSVector = MathUtils.MulT(transform.q, input.Point1 - transform.p);
		TSVector2 tSVector2 = MathUtils.MulT(transform.q, input.Point2 - transform.p);
		TSVector2 value = tSVector2 - tSVector;
		FP fP = 0f;
		FP fP2 = input.MaxFraction;
		int num = -1;
		for (int i = 0; i < Vertices.Count; i++)
		{
			FP fP3 = TSVector2.Dot(Normals[i], Vertices[i] - tSVector);
			FP fP4 = TSVector2.Dot(Normals[i], value);
			if (fP4 == 0f)
			{
				if (fP3 < 0f)
				{
					return false;
				}
			}
			else if (fP4 < 0f && fP3 < fP * fP4)
			{
				fP = fP3 / fP4;
				num = i;
			}
			else if (fP4 > 0f && fP3 < fP2 * fP4)
			{
				fP2 = fP3 / fP4;
			}
			if (fP2 < fP)
			{
				return false;
			}
		}
		if (num >= 0)
		{
			output.Fraction = fP;
			output.Normal = MathUtils.Mul(transform.q, Normals[num]);
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		TSVector2 tSVector = MathUtils.Mul(ref transform, Vertices[0]);
		TSVector2 tSVector2 = tSVector;
		for (int i = 1; i < Vertices.Count; i++)
		{
			TSVector2 value = MathUtils.Mul(ref transform, Vertices[i]);
			tSVector = TSVector2.Min(tSVector, value);
			tSVector2 = TSVector2.Max(tSVector2, value);
		}
		TSVector2 tSVector3 = new TSVector2(Radius, Radius);
		aabb.LowerBound = tSVector - tSVector3;
		aabb.UpperBound = tSVector2 + tSVector3;
	}

	public override FP ComputeSubmergedArea(ref TSVector2 normal, FP offset, ref Transform xf, out TSVector2 sc)
	{
		sc = TSVector2.zero;
		TSVector2 value = MathUtils.MulT(xf.q, normal);
		FP fP = offset - TSVector2.Dot(normal, xf.p);
		FP[] array = new FP[Settings.MaxPolygonVertices];
		int num = 0;
		int num2 = -1;
		int num3 = -1;
		bool flag = false;
		int i;
		for (i = 0; i < Vertices.Count; i++)
		{
			ref FP reference = ref array[i];
			reference = TSVector2.Dot(value, Vertices[i]) - fP;
			bool flag2 = array[i] < -Settings.Epsilon;
			if (i > 0)
			{
				if (flag2)
				{
					if (!flag)
					{
						num2 = i - 1;
						num++;
					}
				}
				else if (flag)
				{
					num3 = i - 1;
					num++;
				}
			}
			flag = flag2;
		}
		switch (num)
		{
		case 0:
			if (flag)
			{
				sc = MathUtils.Mul(ref xf, MassData.Centroid);
				return MassData.Mass / Density;
			}
			return 0;
		case 1:
			if (num2 == -1)
			{
				num2 = Vertices.Count - 1;
			}
			else
			{
				num3 = Vertices.Count - 1;
			}
			break;
		}
		int num4 = (num2 + 1) % Vertices.Count;
		int num5 = (num3 + 1) % Vertices.Count;
		FP fP2 = (0 - array[num2]) / (array[num4] - array[num2]);
		FP fP3 = (0 - array[num3]) / (array[num5] - array[num3]);
		TSVector2 tSVector = new TSVector2(Vertices[num2].x * (1 - fP2) + Vertices[num4].x * fP2, Vertices[num2].y * (1 - fP2) + Vertices[num4].y * fP2);
		TSVector2 tSVector2 = new TSVector2(Vertices[num3].x * (1 - fP3) + Vertices[num5].x * fP3, Vertices[num3].y * (1 - fP3) + Vertices[num5].y * fP3);
		FP fP4 = 0;
		TSVector2 v = new TSVector2(0, 0);
		TSVector2 tSVector3 = Vertices[num4];
		FP fP5 = 1f / 3f;
		i = num4;
		while (i != num5)
		{
			i = (i + 1) % Vertices.Count;
			TSVector2 tSVector4 = ((i != num5) ? Vertices[i] : tSVector2);
			TSVector2 a = tSVector3 - tSVector;
			TSVector2 b = tSVector4 - tSVector;
			FP fP6 = MathUtils.Cross(a, b);
			FP fP7 = 0.5f * fP6;
			fP4 += fP7;
			v += fP7 * fP5 * (tSVector + tSVector3 + tSVector4);
			tSVector3 = tSVector4;
		}
		v *= 1f / fP4;
		sc = MathUtils.Mul(ref xf, v);
		return fP4;
	}

	public bool CompareTo(PolygonShape shape)
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
		return Radius == shape.Radius && MassData == shape.MassData;
	}

	public override Shape Clone()
	{
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.ShapeType = ShapeType;
		polygonShape._radius = _radius;
		polygonShape._density = _density;
		polygonShape._vertices = new Vertices(_vertices);
		polygonShape._normals = new Vertices(_normals);
		polygonShape.MassData = MassData;
		return polygonShape;
	}
}
