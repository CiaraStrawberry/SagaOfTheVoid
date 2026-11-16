namespace TrueSync.Physics3D;

public class CylinderShape : Shape
{
	internal FP height;

	internal FP radius;

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

	public CylinderShape(FP height, FP radius)
	{
		this.height = height;
		this.radius = radius;
		UpdateShape();
	}

	public override void CalculateMassInertia()
	{
		mass = TSMath.Pi * radius * radius * height;
		inertia.M11 = FP.One / (4 * FP.One) * mass * radius * radius + FP.One / (12 * FP.One) * mass * height * height;
		inertia.M22 = FP.One / (2 * FP.One) * mass * radius * radius;
		inertia.M33 = FP.One / (4 * FP.One) * mass * radius * radius + FP.One / (12 * FP.One) * mass * height * height;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		FP fP = FP.Sqrt(direction.x * direction.x + direction.z * direction.z);
		if (fP > FP.Zero)
		{
			result.x = direction.x / fP * radius;
			result.y = FP.Sign(direction.y) * height * FP.Half;
			result.z = direction.z / fP * radius;
		}
		else
		{
			result.x = FP.Zero;
			result.y = FP.Sign(direction.y) * height * FP.Half;
			result.z = FP.Zero;
		}
	}
}
