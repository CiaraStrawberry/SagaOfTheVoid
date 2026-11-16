using System;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class DynamicTree<T>
{
	internal const int NullNode = -1;

	private int _freeList;

	private int _insertionCount;

	private int _nodeCapacity;

	private int _nodeCount;

	private DynamicTreeNode<T>[] _nodes;

	private static FP SettingsAABBMultiplier = 2 * FP.One;

	private FP settingsRndExtension = FP.EN1;

	private int _root;

	private ResourcePool<Stack<int>> stackPool = new ResourcePool<Stack<int>>();

	public int Root => _root;

	public DynamicTreeNode<T>[] Nodes => _nodes;

	public DynamicTree()
		: this(FP.EN1)
	{
	}

	public DynamicTree(FP rndExtension)
	{
		settingsRndExtension = rndExtension;
		_root = -1;
		_nodeCapacity = 16;
		_nodes = new DynamicTreeNode<T>[_nodeCapacity];
		for (int i = 0; i < _nodeCapacity - 1; i++)
		{
			_nodes[i].ParentOrNext = i + 1;
		}
		_nodes[_nodeCapacity - 1].ParentOrNext = -1;
	}

	public int AddProxy(ref TSBBox aabb, T userData)
	{
		int num = AllocateNode();
		_nodes[num].MinorRandomExtension = FP.Half * settingsRndExtension;
		TSVector tSVector = new TSVector(_nodes[num].MinorRandomExtension);
		_nodes[num].AABB.min = aabb.min - tSVector;
		_nodes[num].AABB.max = aabb.max + tSVector;
		_nodes[num].UserData = userData;
		_nodes[num].LeafCount = 1;
		InsertLeaf(num);
		return num;
	}

	public void RemoveProxy(int proxyId)
	{
		RemoveLeaf(proxyId);
		FreeNode(proxyId);
	}

	public bool MoveProxy(int proxyId, ref TSBBox aabb, TSVector displacement)
	{
		if (_nodes[proxyId].AABB.Contains(ref aabb) != TSBBox.ContainmentType.Disjoint)
		{
			return false;
		}
		RemoveLeaf(proxyId);
		TSBBox aABB = aabb;
		TSVector tSVector = new TSVector(_nodes[proxyId].MinorRandomExtension);
		aABB.min -= tSVector;
		aABB.max += tSVector;
		TSVector tSVector2 = SettingsAABBMultiplier * displacement;
		if (tSVector2.x < FP.Zero)
		{
			aABB.min.x += tSVector2.x;
		}
		else
		{
			aABB.max.x += tSVector2.x;
		}
		if (tSVector2.y < FP.Zero)
		{
			aABB.min.y += tSVector2.y;
		}
		else
		{
			aABB.max.y += tSVector2.y;
		}
		if (tSVector2.z < FP.Zero)
		{
			aABB.min.z += tSVector2.z;
		}
		else
		{
			aABB.max.z += tSVector2.z;
		}
		_nodes[proxyId].AABB = aABB;
		InsertLeaf(proxyId);
		return true;
	}

	public T GetUserData(int proxyId)
	{
		return _nodes[proxyId].UserData;
	}

	public void GetFatAABB(int proxyId, out TSBBox fatAABB)
	{
		fatAABB = _nodes[proxyId].AABB;
	}

	public int ComputeHeight()
	{
		return ComputeHeight(_root);
	}

	public void Query(TSVector origin, TSVector direction, List<int> collisions)
	{
		Stack<int> stack = stackPool.GetNew();
		stack.Push(_root);
		while (stack.Count > 0)
		{
			int num = stack.Pop();
			DynamicTreeNode<T> dynamicTreeNode = _nodes[num];
			if (!dynamicTreeNode.AABB.RayIntersect(ref origin, ref direction))
			{
				continue;
			}
			if (dynamicTreeNode.IsLeaf())
			{
				collisions.Add(num);
				continue;
			}
			if (_nodes[dynamicTreeNode.Child1].AABB.RayIntersect(ref origin, ref direction))
			{
				stack.Push(dynamicTreeNode.Child1);
			}
			if (_nodes[dynamicTreeNode.Child2].AABB.RayIntersect(ref origin, ref direction))
			{
				stack.Push(dynamicTreeNode.Child2);
			}
		}
		stackPool.GiveBack(stack);
	}

	public void Query(List<int> other, List<int> my, DynamicTree<T> tree)
	{
		Stack<int> stack = stackPool.GetNew();
		Stack<int> stack2 = stackPool.GetNew();
		stack.Push(_root);
		stack2.Push(tree._root);
		while (stack.Count > 0)
		{
			int num = stack.Pop();
			int num2 = stack2.Pop();
			if (num != -1 && num2 != -1 && tree._nodes[num2].AABB.Contains(ref _nodes[num].AABB) != TSBBox.ContainmentType.Disjoint)
			{
				if (_nodes[num].IsLeaf() && tree._nodes[num2].IsLeaf())
				{
					my.Add(num);
					other.Add(num2);
					continue;
				}
				if (tree._nodes[num2].IsLeaf())
				{
					stack.Push(_nodes[num].Child1);
					stack2.Push(num2);
					stack.Push(_nodes[num].Child2);
					stack2.Push(num2);
					continue;
				}
				if (_nodes[num].IsLeaf())
				{
					stack.Push(num);
					stack2.Push(tree._nodes[num2].Child1);
					stack.Push(num);
					stack2.Push(tree._nodes[num2].Child2);
					continue;
				}
				stack.Push(_nodes[num].Child1);
				stack2.Push(tree._nodes[num2].Child1);
				stack.Push(_nodes[num].Child1);
				stack2.Push(tree._nodes[num2].Child2);
				stack.Push(_nodes[num].Child2);
				stack2.Push(tree._nodes[num2].Child1);
				stack.Push(_nodes[num].Child2);
				stack2.Push(tree._nodes[num2].Child2);
			}
		}
		stackPool.GiveBack(stack);
		stackPool.GiveBack(stack2);
	}

	public void Query(List<int> my, ref TSBBox aabb)
	{
		Stack<int> stack = stackPool.GetNew();
		stack.Push(_root);
		while (stack.Count > 0)
		{
			int num = stack.Pop();
			if (num == -1)
			{
				continue;
			}
			DynamicTreeNode<T> dynamicTreeNode = _nodes[num];
			if (aabb.Contains(ref dynamicTreeNode.AABB) != TSBBox.ContainmentType.Disjoint)
			{
				if (dynamicTreeNode.IsLeaf())
				{
					my.Add(num);
					continue;
				}
				stack.Push(dynamicTreeNode.Child1);
				stack.Push(dynamicTreeNode.Child2);
			}
		}
		stackPool.GiveBack(stack);
	}

	private int CountLeaves(int nodeId)
	{
		if (nodeId == -1)
		{
			return 0;
		}
		DynamicTreeNode<T> dynamicTreeNode = _nodes[nodeId];
		if (dynamicTreeNode.IsLeaf())
		{
			return 1;
		}
		int num = CountLeaves(dynamicTreeNode.Child1);
		int num2 = CountLeaves(dynamicTreeNode.Child2);
		return num + num2;
	}

	private void Validate()
	{
		CountLeaves(_root);
	}

	private int AllocateNode()
	{
		if (_freeList == -1)
		{
			DynamicTreeNode<T>[] nodes = _nodes;
			_nodeCapacity *= 2;
			_nodes = new DynamicTreeNode<T>[_nodeCapacity];
			Array.Copy(nodes, _nodes, _nodeCount);
			for (int i = _nodeCount; i < _nodeCapacity - 1; i++)
			{
				_nodes[i].ParentOrNext = i + 1;
			}
			_nodes[_nodeCapacity - 1].ParentOrNext = -1;
			_freeList = _nodeCount;
		}
		int freeList = _freeList;
		_freeList = _nodes[freeList].ParentOrNext;
		_nodes[freeList].ParentOrNext = -1;
		_nodes[freeList].Child1 = -1;
		_nodes[freeList].Child2 = -1;
		_nodes[freeList].LeafCount = 0;
		_nodeCount++;
		return freeList;
	}

	private void FreeNode(int nodeId)
	{
		_nodes[nodeId].ParentOrNext = _freeList;
		_freeList = nodeId;
		_nodeCount--;
	}

	private void InsertLeaf(int leaf)
	{
		_insertionCount++;
		if (_root == -1)
		{
			_root = leaf;
			_nodes[_root].ParentOrNext = -1;
			return;
		}
		TSBBox additional = _nodes[leaf].AABB;
		int num = _root;
		while (!_nodes[num].IsLeaf())
		{
			int child = _nodes[num].Child1;
			int child2 = _nodes[num].Child2;
			TSBBox.CreateMerged(ref _nodes[num].AABB, ref additional, out _nodes[num].AABB);
			_nodes[num].LeafCount++;
			FP perimeter = _nodes[num].AABB.Perimeter;
			TSBBox tSBBox = default;
			TSBBox.CreateMerged(ref _nodes[num].AABB, ref additional, out _nodes[num].AABB);
			FP perimeter2 = tSBBox.Perimeter;
			FP fP = 2 * FP.One * perimeter2;
			FP fP2 = 2 * FP.One * (perimeter2 - perimeter);
			FP fP3;
			if (_nodes[child].IsLeaf())
			{
				TSBBox result = default;
				TSBBox.CreateMerged(ref additional, ref _nodes[child].AABB, out result);
				fP3 = result.Perimeter + fP2;
			}
			else
			{
				TSBBox result2 = default;
				TSBBox.CreateMerged(ref additional, ref _nodes[child].AABB, out result2);
				FP perimeter3 = _nodes[child].AABB.Perimeter;
				FP perimeter4 = result2.Perimeter;
				fP3 = perimeter4 - perimeter3 + fP2;
			}
			FP fP4;
			if (_nodes[child2].IsLeaf())
			{
				TSBBox result3 = default;
				TSBBox.CreateMerged(ref additional, ref _nodes[child2].AABB, out result3);
				fP4 = result3.Perimeter + fP2;
			}
			else
			{
				TSBBox result4 = default;
				TSBBox.CreateMerged(ref additional, ref _nodes[child2].AABB, out result4);
				FP perimeter5 = _nodes[child2].AABB.Perimeter;
				FP perimeter6 = result4.Perimeter;
				fP4 = perimeter6 - perimeter5 + fP2;
			}
			if (fP < fP3 && fP < fP4)
			{
				break;
			}
			TSBBox.CreateMerged(ref additional, ref _nodes[num].AABB, out _nodes[num].AABB);
			num = ((!(fP3 < fP4)) ? child2 : child);
		}
		int parentOrNext = _nodes[num].ParentOrNext;
		int num2 = AllocateNode();
		_nodes[num2].ParentOrNext = parentOrNext;
		_nodes[num2].UserData = default;
		TSBBox.CreateMerged(ref additional, ref _nodes[num].AABB, out _nodes[num2].AABB);
		_nodes[num2].LeafCount = _nodes[num].LeafCount + 1;
		if (parentOrNext != -1)
		{
			if (_nodes[parentOrNext].Child1 == num)
			{
				_nodes[parentOrNext].Child1 = num2;
			}
			else
			{
				_nodes[parentOrNext].Child2 = num2;
			}
			_nodes[num2].Child1 = num;
			_nodes[num2].Child2 = leaf;
			_nodes[num].ParentOrNext = num2;
			_nodes[leaf].ParentOrNext = num2;
		}
		else
		{
			_nodes[num2].Child1 = num;
			_nodes[num2].Child2 = leaf;
			_nodes[num].ParentOrNext = num2;
			_nodes[leaf].ParentOrNext = num2;
			_root = num2;
		}
	}

	private void RemoveLeaf(int leaf)
	{
		if (leaf == _root)
		{
			_root = -1;
			return;
		}
		int parentOrNext = _nodes[leaf].ParentOrNext;
		int parentOrNext2 = _nodes[parentOrNext].ParentOrNext;
		int num = ((_nodes[parentOrNext].Child1 != leaf) ? _nodes[parentOrNext].Child1 : _nodes[parentOrNext].Child2);
		if (parentOrNext2 != -1)
		{
			if (_nodes[parentOrNext2].Child1 == parentOrNext)
			{
				_nodes[parentOrNext2].Child1 = num;
			}
			else
			{
				_nodes[parentOrNext2].Child2 = num;
			}
			_nodes[num].ParentOrNext = parentOrNext2;
			FreeNode(parentOrNext);
			for (parentOrNext = parentOrNext2; parentOrNext != -1; parentOrNext = _nodes[parentOrNext].ParentOrNext)
			{
				TSBBox.CreateMerged(ref _nodes[_nodes[parentOrNext].Child1].AABB, ref _nodes[_nodes[parentOrNext].Child2].AABB, out _nodes[parentOrNext].AABB);
				_nodes[parentOrNext].LeafCount--;
			}
		}
		else
		{
			_root = num;
			_nodes[num].ParentOrNext = -1;
			FreeNode(parentOrNext);
		}
	}

	private int ComputeHeight(int nodeId)
	{
		if (nodeId == -1)
		{
			return 0;
		}
		DynamicTreeNode<T> dynamicTreeNode = _nodes[nodeId];
		int val = ComputeHeight(dynamicTreeNode.Child1);
		int val2 = ComputeHeight(dynamicTreeNode.Child2);
		return 1 + Math.Max(val, val2);
	}
}
