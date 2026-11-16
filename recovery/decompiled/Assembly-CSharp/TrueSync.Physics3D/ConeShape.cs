namespace TrueSync.Physics3D;

public class ConeShape : Shape
{
	internal FP height;

	internal FP radius;

	internal FP sina = FP.Zero;

	public FP Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
			UpdateShape();
		}
	}

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

	public ConeShape(FP height, FP radius)
	{
		this.height = height;
		this.radius = radius;
		UpdateShape();
	}

	public override void UpdateShape()
	{
		sina = radius / FP.Sqrt(radius * radius + height * height);
		base.UpdateShape();
	}

	public override void CalculateMassInertia()
	{
		mass = FP.One / (3 * FP.One) * TSMath.Pi * radius * radius * height;
		inertia = TSMatrix.Identity;
		inertia.M11 = 3 * FP.EN1 / 8 * mass * (radius * radius + 4 * height * height);
		inertia.M22 = 3 * FP.EN1 * mass * radius * radius;
		inertia.M33 = 3 * FP.EN1 / 8 * mass * (radius * radius + 4 * height * height);
		geomCen = TSVector.zero;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		FP fP = FP.Sqrt(direction.x * direction.x + direction.z * direction.z);
		if (direction.y > direction.magnitude * sina)
		{
			result.x = FP.Zero;
			result.y = 2 * FP.One / 3 * height;
			result.z = FP.Zero;
		}
		else if (fP > FP.Zero)
		{
			result.x = radius * direction.x / fP;
			result.y = -(FP.One / 3) * height;
			result.z = radius * direction.z / fP;
		}
		else
		{
			result.x = FP.Zero;
			result.y = -(FP.One / 3) * height;
			result.z = FP.Zero;
		}
	}
}
