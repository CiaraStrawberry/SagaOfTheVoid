namespace TrueSync.Physics3D;

public interface ISupportMappable
{
	void SupportMapping(ref TSVector direction, out TSVector result);

	void SupportCenter(out TSVector center);
}
