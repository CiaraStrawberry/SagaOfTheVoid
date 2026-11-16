using System;
using System.Collections.Generic;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.DataStructures;

public class HeightQuadTree : IHeightLookup
{
	private class UpdatingHeightTree : IHeightLookup
	{
		private HeightQuadTree _target;

		private HeightQuadTree _parent;

		private int _parentIdx;

		public MatrixBounds bounds => _target._bounds;

		public bool hasHeights => true;

		public int heightCount => 0;

		public UpdatingHeightTree(HeightQuadTree target, HeightQuadTree parent, int parentIdx)
		{
			_target = target;
			_parent = parent;
			_parentIdx = parentIdx;
		}

		internal void Inject()
		{
			_target.Cleanup();
			_parent._subTrees[_parentIdx] = _target;
		}

		bool IHeightLookup.Add(int x, int z, float height)
		{
			return _target.Add(x, z, height);
		}

		bool IHeightLookup.TryGetHeight(int x, int z, out float height)
		{
			throw new InvalidOperationException("This is a temporary construct used for dynamic updates and is not meant for lookups.");
		}

		void IHeightLookup.Cleanup()
		{
			_target.Cleanup();
		}

		void IHeightLookup.Render(Vector3 position, float pointGranularity, Color drawColor)
		{
			throw new InvalidOperationException("This is a temporary construct used for dynamic updates and is not meant for rendering.");
		}

		IHeightLookup IHeightLookup.PrepareForUpdate(MatrixBounds suggestedBounds, out MatrixBounds requiredBounds)
		{
			throw new InvalidOperationException("This is a temporary construct used for dynamic updates only.");
		}

		void IHeightLookup.FinishUpdate(IHeightLookup updatedHeights)
		{
			throw new InvalidOperationException("This is a temporary construct used for dynamic updates only.");
		}
	}

	private int _depth;

	private int _maxDepth;

	private float _height;

	private DynamicArray<VectorXZ> _indexes;

	private Dictionary<int, float> _lookup;

	private MatrixBounds _bounds;

	private HeightQuadTree[] _subTrees;

	public bool hasHeights => _subTrees != null || _depth > 0;

	public int heightCount => _bounds.columns * _bounds.rows;

	private bool canSplit
	{
		get
		{
			if (_depth >= _maxDepth)
			{
				return false;
			}
			return _bounds.rows > 1 && _bounds.columns > 1;
		}
	}

	public HeightQuadTree(int sizeX, int sizeZ, int maxDepth, float originY)
		: this(0, 0, sizeX, sizeZ, 0, maxDepth)
	{
		_height = originY;
	}

	private HeightQuadTree(int startX, int startZ, int sizeX, int sizeZ, int depth, int maxDepth)
	{
		_bounds = new MatrixBounds(startX, startZ, startX + (sizeX - 1), startZ + (sizeZ - 1));
		_depth = depth;
		_maxDepth = maxDepth;
		_indexes = new DynamicArray<VectorXZ>(10);
	}

	private HeightQuadTree(MatrixBounds bounds, int depth, int maxDepth)
	{
		_bounds = bounds;
		_depth = depth;
		_maxDepth = maxDepth;
		_indexes = new DynamicArray<VectorXZ>(10);
	}

	public IHeightLookup PrepareForUpdate(MatrixBounds suggestedBounds, out MatrixBounds requiredBounds)
	{
		if (!_bounds.Contains(suggestedBounds))
		{
			throw new ArgumentException("Cannot update a quadtree of a smaller size than requested bounds.", "suggestedBounds");
		}
		UpdatingHeightTree updateTree = GetUpdateTree(null, 0, suggestedBounds);
		if (updateTree != null)
		{
			requiredBounds = updateTree.bounds;
			return updateTree;
		}
		requiredBounds = _bounds;
		_lookup = null;
		_subTrees = null;
		_indexes = new DynamicArray<VectorXZ>(10);
		return this;
	}

	public void FinishUpdate(IHeightLookup updatedHeights)
	{
		if (updatedHeights is UpdatingHeightTree updatingHeightTree)
		{
			updatingHeightTree.Inject();
		}
	}

	private UpdatingHeightTree GetUpdateTree(HeightQuadTree parent, int parentIdx, MatrixBounds suggestedBounds)
	{
		if (_subTrees != null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_subTrees[i]._bounds.Contains(suggestedBounds))
				{
					return _subTrees[i].GetUpdateTree(this, i, suggestedBounds);
				}
			}
		}
		if (parent != null)
		{
			HeightQuadTree target = new HeightQuadTree(_bounds, _depth, _maxDepth);
			return new UpdatingHeightTree(target, parent, parentIdx);
		}
		return null;
	}

	public bool Add(int x, int z, float height)
	{
		if (!_bounds.Contains(x, z))
		{
			return false;
		}
		if (_lookup != null)
		{
			int key = x * _bounds.rows + z;
			_lookup[key] = height;
			return true;
		}
		if (_subTrees != null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_subTrees[i].Add(x, z, height))
				{
					return true;
				}
			}
			return false;
		}
		if (_height == height)
		{
			_indexes.Add(new VectorXZ(x, z));
		}
		else if (_indexes.count == 0)
		{
			_height = height;
			_indexes.Add(new VectorXZ(x, z));
		}
		else if (canSplit)
		{
			SplitToSubTrees(x, z, height);
		}
		else
		{
			CreateLookup(x, z, height);
		}
		return true;
	}

	public bool TryGetHeight(int x, int z, out float height)
	{
		if (!_bounds.Contains(x, z))
		{
			height = 0f;
			return false;
		}
		if (_lookup != null)
		{
			int key = x * _bounds.rows + z;
			return _lookup.TryGetValue(key, out height);
		}
		if (_subTrees != null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_subTrees[i].TryGetHeight(x, z, out height))
				{
					return true;
				}
			}
		}
		height = _height;
		return true;
	}

	public void Cleanup()
	{
		_indexes = null;
		if (_subTrees != null)
		{
			for (int i = 0; i < 4; i++)
			{
				_subTrees[i].Cleanup();
			}
		}
	}

	public void Render(Vector3 position, float pointGranularity, Color drawColor)
	{
		if (_subTrees != null)
		{
			for (int i = 0; i < 4; i++)
			{
				_subTrees[i].Render(position, pointGranularity, drawColor);
			}
			return;
		}
		Bounds bounds = default;
		bounds.SetMinMax(position + new Vector3((float)_bounds.minColumn * pointGranularity, 5f, (float)_bounds.minRow * pointGranularity), position + new Vector3((float)_bounds.maxColumn * pointGranularity, 5f, (float)_bounds.maxRow * pointGranularity));
		Gizmos.color = drawColor;
		if (_lookup != null)
		{
			Gizmos.DrawCube(bounds.center, bounds.size);
		}
		else
		{
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}

	private void SplitToSubTrees(int newX, int newZ, float newHeight)
	{
		_subTrees = new HeightQuadTree[4];
		int num = _bounds.columns / 2;
		int sizeX = _bounds.columns - num;
		int num2 = _bounds.rows / 2;
		int sizeZ = _bounds.rows - num2;
		int depth = _depth + 1;
		_subTrees[0] = new HeightQuadTree(_bounds.minColumn, _bounds.minRow, num, num2, depth, _maxDepth);
		_subTrees[1] = new HeightQuadTree(_bounds.minColumn + num, _bounds.minRow, sizeX, num2, depth, _maxDepth);
		_subTrees[2] = new HeightQuadTree(_bounds.minColumn, _bounds.minRow + num2, num, sizeZ, depth, _maxDepth);
		_subTrees[3] = new HeightQuadTree(_bounds.minColumn + num, _bounds.minRow + num2, sizeX, sizeZ, depth, _maxDepth);
		int count = _indexes.count;
		for (int i = 0; i < count; i++)
		{
			VectorXZ vectorXZ = _indexes[i];
			for (int j = 0; j < 4 && !_subTrees[j].Add(vectorXZ.x, vectorXZ.z, _height); j++)
			{
			}
		}
		for (int k = 0; k < 4 && !_subTrees[k].Add(newX, newZ, newHeight); k++)
		{
		}
		_indexes = null;
		_height = 0f;
	}

	private void CreateLookup(int newX, int newZ, float newHeight)
	{
		int count = _indexes.count;
		_lookup = new Dictionary<int, float>(count + 1);
		int key = newX * _bounds.rows + newZ;
		_lookup[key] = newHeight;
		for (int i = 0; i < count; i++)
		{
			VectorXZ vectorXZ = _indexes[i];
			key = vectorXZ.x * _bounds.rows + vectorXZ.z;
			_lookup[key] = _height;
		}
		_indexes = null;
		_height = 0f;
	}
}
