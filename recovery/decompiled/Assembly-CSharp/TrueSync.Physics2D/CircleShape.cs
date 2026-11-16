using System;

namespace TrueSync.Physics2D;

public class CircleShape : Shape
{
	internal TSVector2 _position;

	public override int ChildCount => 1;

	public TSVector2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
			ComputeProperties();
		}
	}

	public CircleShape(FP radius, FP density)
		: base(density)
	{
		ShapeType = ShapeType.Circle;
		_position = TSVector2.zero;
		Radius = radius;
	}

	internal CircleShape()
		: base(0)
	{
		ShapeType = ShapeType.Circle;
		_radius = 0f;
		_position = TSVector2.zero;
	}

	public override bool TestPoint(ref Transform transform, ref TSVector2 point)
	{
		TSVector2 tSVector = transform.p + MathUtils.Mul(transform.q, Position);
		TSVector2 tSVector2 = point - tSVector;
		return TSVector2.Dot(tSVector2, tSVector2) <= _2radius;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default;
		TSVector2 tSVector = transform.p + MathUtils.Mul(transform.q, Position);
		TSVector2 tSVector2 = input.Point1 - tSVector;
		FP fP = TSVector2.Dot(tSVector2, tSVector2) - _2radius;
		TSVector2 tSVector3 = input.Point2 - input.Point1;
		FP fP2 = TSVector2.Dot(tSVector2, tSVector3);
		FP fP3 = TSVector2.Dot(tSVector3, tSVector3);
		FP fP4 = fP2 * fP2 - fP3 * fP;
		if (fP4 < 0f || fP3 < Settings.Epsilon)
		{
			return false;
		}
		FP fP5 = -(fP2 + FP.Sqrt(fP4));
		if (0f <= fP5 && fP5 <= input.MaxFraction * fP3)
		{
			fP5 /= fP3;
			output.Fraction = fP5;
			output.Normal = tSVector2 + fP5 * tSVector3;
			output.Normal.Normalize();
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		TSVector2 tSVector = transform.p + MathUtils.Mul(transform.q, Position);
		aabb.LowerBound = new TSVector2(tSVector.x - Radius, tSVector.y - Radius);
		aabb.UpperBound = new TSVector2(tSVector.x + Radius, tSVector.y + Radius);
	}

	protected sealed override void ComputeProperties()
	{
		FP fP = Settings.Pi * _2radius;
		MassData.Area = fP;
		MassData.Mass = Density * fP;
		MassData.Centroid = Position;
		MassData.Inertia = MassData.Mass * (0.5f * _2radius + TSVector2.Dot(Position, Position));
	}

	public override FP ComputeSubmergedArea(ref TSVector2 normal, FP offset, ref Transform xf, out TSVector2 sc)
	{
		sc = TSVector2.zero;
		TSVector2 tSVector = MathUtils.Mul(ref xf, Position);
		FP fP = -(TSVector2.Dot(normal, tSVector) - offset);
		if (fP < -Radius + Settings.Epsilon)
		{
			return 0;
		}
		if (fP > Radius)
		{
			sc = tSVector;
			return Settings.Pi * _2radius;
		}
		FP fP2 = fP * fP;
		FP fP3 = _2radius * (TSMath.Asin(fP / Radius) + TSMath.PiOver2 + fP * TSMath.Sqrt(_2radius - fP2));
		FP fP4 = new FP(-2) / new FP(3) * Math.Pow((_2radius - fP2).AsFloat(), 1.5) / fP3;
		sc.x = tSVector.x + normal.x * fP4;
		sc.y = tSVector.y + normal.y * fP4;
		return fP3;
	}

	public bool CompareTo(CircleShape shape)
	{
		return Radius == shape.Radius && Position == shape.Position;
	}

	public override Shape Clone()
	{
		CircleShape circleShape = new CircleShape();
		circleShape.ShapeType = ShapeType;
		circleShape._radius = Radius;
		circleShape._2radius = _2radius;
		circleShape._density = _density;
		circleShape._position = _position;
		circleShape.MassData = MassData;
		return circleShape;
	}
}
