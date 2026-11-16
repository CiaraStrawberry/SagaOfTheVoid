namespace TrueSync.Physics3D;

public class BoxShape : Shape
{
	internal TSVector size = TSVector.zero;

	internal TSVector halfSize = TSVector.zero;

	public TSVector Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			UpdateShape();
		}
	}

	public BoxShape(TSVector size)
	{
		this.size = size;
		UpdateShape();
	}

	public BoxShape(FP length, FP height, FP width)
	{
		size.x = length;
		size.y = height;
		size.z = width;
		UpdateShape();
	}

	public override void UpdateShape()
	{
		halfSize = size * FP.Half;
		base.UpdateShape();
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		TSMath.Absolute(ref orientation, out var result);
		TSVector.Transform(ref halfSize, ref result, out var result2);
		box.max = result2;
		TSVector.Negate(ref result2, out box.min);
	}

	public override void CalculateMassInertia()
	{
		mass = size.x * size.y * size.z;
		inertia = TSMatrix.Identity;
		inertia.M11 = FP.One / (12 * FP.One) * mass * (size.y * size.y + size.z * size.z);
		inertia.M22 = FP.One / (12 * FP.One) * mass * (size.x * size.x + size.z * size.z);
		inertia.M33 = FP.One / (12 * FP.One) * mass * (size.x * size.x + size.y * size.y);
		geomCen = TSVector.zero;
	}

	public override void SupportMapping(ref TSVector direction, out TSVector result)
	{
		result.x = FP.Sign(direction.x) * halfSize.x;
		result.y = FP.Sign(direction.y) * halfSize.y;
		result.z = FP.Sign(direction.z) * halfSize.z;
	}
}
