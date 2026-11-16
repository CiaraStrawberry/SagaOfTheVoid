namespace TrueSync;

public class TSRay
{
	public TSVector direction;

	public TSVector origin;

	public TSRay(TSVector origin, TSVector direction)
	{
		this.origin = origin;
		this.direction = direction;
	}
}
