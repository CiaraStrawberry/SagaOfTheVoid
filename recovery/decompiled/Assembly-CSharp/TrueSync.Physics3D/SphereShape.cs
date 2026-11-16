namespace TrueSync.Physics3D;

public class SphereShape : Shape
{
	internal FP radius = FP.One;

	public FP Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = value;
			UpdateShape();
		}
	}

	public SphereShape(FP radius)
	{
		this.radius = radius;
		UpdateShape();
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		result = direction;
		result.Normalize();
		TSVector.Multiply(ref result, radius, out result);
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		box.min.x = -radius;
		box.min.y = -radius;
		box.min.z = -radius;
		box.max.x = radius;
		box.max.y = radius;
		box.max.z = radius;
	}

	public override void CalculateMassInertia()
	{
		mass = 4 * FP.One / (3 * FP.One) * TSMath.Pi * radius * radius * radius;
		inertia = TSMatrix.Identity;
		inertia.M11 = 4 * FP.EN1 * mass * radius * radius;
		inertia.M22 = 4 * FP.EN1 * mass * radius * radius;
		inertia.M33 = 4 * FP.EN1 * mass * radius * radius;
	}
}
