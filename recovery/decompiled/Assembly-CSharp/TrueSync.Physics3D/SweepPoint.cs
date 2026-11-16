namespace TrueSync.Physics3D;

public class SweepPoint
{
	public IBroadphaseEntity Body;

	public bool Begin;

	public int Axis;

	public FP Value
	{
		get
		{
			if (Begin)
			{
				if (Axis == 0)
				{
					return Body.BoundingBox.min.x;
				}
				if (Axis == 1)
				{
					return Body.BoundingBox.min.y;
				}
				return Body.BoundingBox.min.z;
			}
			if (Axis == 0)
			{
				return Body.BoundingBox.max.x;
			}
			if (Axis == 1)
			{
				return Body.BoundingBox.max.y;
			}
			return Body.BoundingBox.max.z;
		}
	}

	public SweepPoint(IBroadphaseEntity body, bool begin, int axis)
	{
		Body = body;
		Begin = begin;
		Axis = axis;
	}
}
