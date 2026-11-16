namespace TrueSync.Physics3D;

public struct DynamicTreeNode<T>
{
	public TSBBox AABB;

	public FP MinorRandomExtension;

	public int Child1;

	public int Child2;

	public int LeafCount;

	public int ParentOrNext;

	public T UserData;

	public bool IsLeaf()
	{
		return Child1 == -1;
	}
}
