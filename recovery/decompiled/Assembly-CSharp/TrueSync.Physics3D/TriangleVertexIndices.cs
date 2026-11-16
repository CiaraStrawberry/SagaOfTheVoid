namespace TrueSync.Physics3D;

public struct TriangleVertexIndices(int i0, int i1, int i2)
{
	public int I0 = i0;

	public int I1 = i1;

	public int I2 = i2;

	public void Set(int i0, int i1, int i2)
	{
		I0 = i0;
		I1 = i1;
		I2 = i2;
	}
}
