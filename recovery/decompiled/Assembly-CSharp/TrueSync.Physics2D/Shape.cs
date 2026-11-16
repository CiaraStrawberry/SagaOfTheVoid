namespace TrueSync.Physics2D;

public abstract class Shape
{
	internal FP _density;

	internal FP _radius;

	internal FP _2radius;

	public MassData MassData;

	public ShapeType ShapeType { get; internal set; }

	public abstract int ChildCount { get; }

	public FP Density
	{
		get
		{
			return _density;
		}
		set
		{
			_density = value;
			ComputeProperties();
		}
	}

	public FP Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			_radius = value;
			_2radius = _radius * _radius;
			ComputeProperties();
		}
	}

	protected Shape(FP density)
	{
		_density = density;
		ShapeType = ShapeType.Unknown;
	}

	public abstract Shape Clone();

	public abstract bool TestPoint(ref Transform transform, ref TSVector2 point);

	public abstract bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex);

	public abstract void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex);

	protected abstract void ComputeProperties();

	public bool CompareTo(Shape shape)
	{
		if (shape is PolygonShape && this is PolygonShape)
		{
			return ((PolygonShape)this).CompareTo((PolygonShape)shape);
		}
		if (shape is CircleShape && this is CircleShape)
		{
			return ((CircleShape)this).CompareTo((CircleShape)shape);
		}
		if (shape is EdgeShape && this is EdgeShape)
		{
			return ((EdgeShape)this).CompareTo((EdgeShape)shape);
		}
		if (shape is ChainShape && this is ChainShape)
		{
			return ((ChainShape)this).CompareTo((ChainShape)shape);
		}
		return false;
	}

	public abstract FP ComputeSubmergedArea(ref TSVector2 normal, FP offset, ref Transform xf, out TSVector2 sc);
}
