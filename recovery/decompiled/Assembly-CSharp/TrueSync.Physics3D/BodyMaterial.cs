namespace TrueSync.Physics3D;

public class BodyMaterial
{
	internal FP kineticFriction = FP.One / 4;

	internal FP staticFriction = FP.One / 2;

	internal FP restitution = FP.Zero;

	public FP Restitution
	{
		get
		{
			return restitution;
		}
		set
		{
			restitution = value;
		}
	}

	public FP StaticFriction
	{
		get
		{
			return staticFriction;
		}
		set
		{
			staticFriction = value;
		}
	}

	public FP KineticFriction
	{
		get
		{
			return kineticFriction;
		}
		set
		{
			kineticFriction = value;
		}
	}
}
