namespace TrueSync.Physics3D;

public class CapsuleShape : Shape
{
	internal FP length;

	internal FP radius;

	public FP Length
	{
		get
		{
			return length;
		}
		set
		{
			length = value;
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

	public CapsuleShape(FP length, FP radius)
	{
		this.length = length;
		this.radius = radius;
		UpdateShape();
	}

	public override void CalculateMassInertia()
	{
		FP fP = 3 * FP.One / (4 * FP.One) * TSMath.Pi * radius * radius * radius;
		FP fP2 = TSMath.Pi * radius * radius * length;
		mass = fP2 + fP;
		inertia.M11 = FP.One / (4 * FP.One) * fP2 * radius * radius + FP.One / (12 * FP.One) * fP2 * length * length + 2 * FP.One / (5 * FP.One) * fP * radius * radius + FP.One / (4 * FP.One) * length * length * fP;
		inertia.M22 = FP.One / (2 * FP.One) * fP2 * radius * radius + 2 * FP.One / (5 * FP.One) * fP * radius * radius;
		inertia.M33 = FP.One / (4 * FP.One) * fP2 * radius * radius + FP.One / (12 * FP.One) * fP2 * length * length + 2 * FP.One / (5 * FP.One) * fP * radius * radius + FP.One / (4 * FP.One) * length * length * fP;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		FP fP = FP.Sqrt(direction.x * direction.x + direction.z * direction.z);
		if (FP.Abs(direction.y) > FP.Zero)
		{
			TSVector.Normalize(ref direction, out var result2);
			TSVector.Multiply(ref result2, radius, out result);
			result.y += FP.Sign(direction.y) * FP.Half * length;
		}
		else if (fP > FP.Zero)
		{
			result.x = direction.x / fP * radius;
			result.y = FP.Zero;
			result.z = direction.z / fP * radius;
		}
		else
		{
			result.x = FP.Zero;
			result.y = FP.Zero;
			result.z = FP.Zero;
		}
	}
}
