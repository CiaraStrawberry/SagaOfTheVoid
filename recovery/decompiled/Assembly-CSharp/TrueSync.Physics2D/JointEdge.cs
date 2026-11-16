namespace TrueSync.Physics2D;

public sealed class JointEdge
{
	public Joint2D Joint;

	public JointEdge Next;

	public Body Other;

	public JointEdge Prev;
}
