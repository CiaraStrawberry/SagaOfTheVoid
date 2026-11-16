namespace TrueSync.Physics2D;

internal class AdvancingFrontNode
{
	public AdvancingFrontNode Next;

	public TriangulationPoint Point;

	public AdvancingFrontNode Prev;

	public DelaunayTriangle Triangle;

	public FP Value;

	public bool HasNext => Next != null;

	public bool HasPrev => Prev != null;

	public AdvancingFrontNode(TriangulationPoint point)
	{
		Point = point;
		Value = point.X;
	}
}
