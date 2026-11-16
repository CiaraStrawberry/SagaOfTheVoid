namespace TrueSync.Physics2D;

public struct Transform(ref TSVector2 position, ref Rot rotation)
{
	public TSVector2 p = position;

	public Rot q = rotation;

	public void SetIdentity()
	{
		p = TSVector2.zero;
		q.SetIdentity();
	}

	public void Set(TSVector2 position, FP angle)
	{
		p = position;
		q.Set(angle);
	}
}
