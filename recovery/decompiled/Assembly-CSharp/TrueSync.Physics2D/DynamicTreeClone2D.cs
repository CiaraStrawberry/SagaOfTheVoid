using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class DynamicTreeClone2D
{
	public List<int> _raycastStack = new List<int>(256);

	public List<int> _queryStack = new List<int>(256);

	public int _freeList;

	public int _nodeCapacity;

	public int _nodeCount;

	public List<TreeNode<FixtureProxy>> _nodes = new List<TreeNode<FixtureProxy>>();

	public int _root;

	private int index;

	private int length;

	public void Clone(DynamicTree<FixtureProxy> dynamicTree)
	{
		_raycastStack.Clear();
		Stack<int>.Enumerator enumerator = dynamicTree._raycastStack.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_raycastStack.Add(enumerator.Current);
		}
		_queryStack.Clear();
		Stack<int>.Enumerator enumerator2 = dynamicTree._queryStack.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			_queryStack.Add(enumerator2.Current);
		}
		_freeList = dynamicTree._freeList;
		_nodeCapacity = dynamicTree._nodeCapacity;
		_nodeCount = dynamicTree._nodeCount;
		_root = dynamicTree._root;
		index = 0;
		for (length = _nodes.Count; index < length; index++)
		{
			WorldClone2D.poolTreeFixtureProxy.GiveBack(_nodes[index]);
		}
		_nodes.Clear();
		index = 0;
		for (length = dynamicTree._nodes.Length; index < length; index++)
		{
			TreeNode<FixtureProxy> treeNode = dynamicTree._nodes[index];
			TreeNode<FixtureProxy> treeNode2 = WorldClone2D.poolTreeFixtureProxy.GetNew();
			treeNode2.AABB = treeNode.AABB;
			treeNode2.Child1 = treeNode.Child1;
			treeNode2.Child2 = treeNode.Child2;
			treeNode2.Height = treeNode.Height;
			treeNode2.ParentOrNext = treeNode.ParentOrNext;
			treeNode2.UserData = treeNode.UserData;
			_nodes.Add(treeNode2);
		}
	}

	public void Restore(DynamicTree<FixtureProxy> dynamicTree)
	{
		dynamicTree._raycastStack.Clear();
		index = 0;
		for (length = _raycastStack.Count; index < length; index++)
		{
			dynamicTree._raycastStack.Push(_raycastStack[index]);
		}
		dynamicTree._queryStack.Clear();
		index = 0;
		for (length = _queryStack.Count; index < length; index++)
		{
			dynamicTree._queryStack.Push(_queryStack[index]);
		}
		dynamicTree._freeList = _freeList;
		dynamicTree._nodeCapacity = _nodeCapacity;
		dynamicTree._nodeCount = _nodeCount;
		dynamicTree._root = _root;
		dynamicTree._nodes = new TreeNode<FixtureProxy>[_nodes.Count];
		index = 0;
		for (length = _nodes.Count; index < length; index++)
		{
			TreeNode<FixtureProxy> treeNode = _nodes[index];
			TreeNode<FixtureProxy> treeNode2 = WorldClone2D.poolTreeFixtureProxy.GetNew();
			treeNode2.AABB = treeNode.AABB;
			treeNode2.Child1 = treeNode.Child1;
			treeNode2.Child2 = treeNode.Child2;
			treeNode2.Height = treeNode.Height;
			treeNode2.ParentOrNext = treeNode.ParentOrNext;
			treeNode2.UserData = treeNode.UserData;
			dynamicTree._nodes[index] = treeNode2;
		}
	}
}
