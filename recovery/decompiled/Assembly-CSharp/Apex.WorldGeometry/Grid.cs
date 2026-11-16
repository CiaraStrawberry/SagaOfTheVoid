using System;
using System.Collections;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.LoadBalancing;
using Apex.PathFinding;
using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class Grid : IGrid
{
	[Serializable]
	private class WalkableNeighboursEnumerator : IEnumerator<IPathNode>, IEnumerator, IDisposable
	{
		private readonly Grid _g;

		private readonly IPathNode[] _neighbours;

		private int _index;

		private IPathNode _current;

		object IEnumerator.Current => _current;

		public IPathNode Current => _current;

		internal WalkableNeighboursEnumerator(Grid g)
		{
			_g = g;
			_current = null;
			_neighbours = new IPathNode[8];
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (_index == 8)
			{
				_current = null;
				return false;
			}
			_current = _neighbours[_index++];
			if (_current == null)
			{
				return false;
			}
			return true;
		}

		public void Reset()
		{
			Array.Clear(_neighbours, 0, 8);
			_current = null;
			_index = 0;
		}

		internal void Prepare(IGridCell c, IUnitProperties unitProps, bool excludeCornerCutting)
		{
			Reset();
			int num = 0;
			bool flag = _g.TryGetWalkableNeighbour(c, 0, 1, unitProps, out var neighbour);
			if (flag)
			{
				_neighbours[num++] = neighbour;
			}
			bool flag2 = _g.TryGetWalkableNeighbour(c, 0, -1, unitProps, out neighbour);
			if (flag2)
			{
				_neighbours[num++] = neighbour;
			}
			bool flag3 = _g.TryGetWalkableNeighbour(c, 1, 0, unitProps, out neighbour);
			if (flag3)
			{
				_neighbours[num++] = neighbour;
			}
			bool flag4 = _g.TryGetWalkableNeighbour(c, -1, 0, unitProps, out neighbour);
			if (flag4)
			{
				_neighbours[num++] = neighbour;
			}
			bool flag5;
			bool flag6;
			bool flag7;
			bool flag8;
			if (excludeCornerCutting)
			{
				flag5 = flag && flag3;
				flag6 = flag2 && flag3;
				flag7 = flag2 && flag4;
				flag8 = flag && flag4;
			}
			else
			{
				flag5 = flag || flag3;
				flag6 = flag2 || flag3;
				flag7 = flag2 || flag4;
				flag8 = flag || flag4;
			}
			if (flag5 && _g.TryGetWalkableNeighbour(c, 1, 1, unitProps, out neighbour))
			{
				_neighbours[num++] = neighbour;
			}
			if (flag6 && _g.TryGetWalkableNeighbour(c, 1, -1, unitProps, out neighbour))
			{
				_neighbours[num++] = neighbour;
			}
			if (flag7 && _g.TryGetWalkableNeighbour(c, -1, -1, unitProps, out neighbour))
			{
				_neighbours[num++] = neighbour;
			}
			if (flag8 && _g.TryGetWalkableNeighbour(c, -1, 1, unitProps, out neighbour))
			{
				_neighbours[num++] = neighbour;
			}
		}
	}

	private CellMatrix _cellMatrix;

	private GridSection[] _gridSections;

	private Vector3 _origin;

	private int _sizeX;

	private int _sizeZ;

	private float _cellSize;

	private Perimeter _left;

	private Perimeter _right;

	private Perimeter _top;

	private Perimeter _bottom;

	private WalkableNeighboursEnumerator _walkableIter;

	public Vector3 origin => _origin;

	public int sizeX => _sizeX;

	public int sizeZ => _sizeZ;

	public float cellSize => _cellSize;

	public Bounds bounds => _cellMatrix.bounds;

	public Perimeter left => _left;

	public Perimeter right => _right;

	public Perimeter top => _top;

	public Perimeter bottom => _bottom;

	public IEnumerable<Cell> cells => _cellMatrix.items;

	public CellMatrix cellMatrix => _cellMatrix;

	public GridSection[] gridSections => _gridSections;

	private Grid()
	{
		_walkableIter = new WalkableNeighboursEnumerator(this);
	}

	public static Grid Create(CellMatrix matrix, GridSection[] gridSections)
	{
		Grid grid = new Grid();
		grid.Initialize(matrix, gridSections);
		return grid;
	}

	public bool Contains(Vector3 pos)
	{
		return _cellMatrix.Contains(pos);
	}

	public Cell GetCell(Vector3 position)
	{
		return _cellMatrix.GetCell(position, adjustToBounds: false);
	}

	public Cell GetCell(Vector3 position, bool adjustToBounds)
	{
		return _cellMatrix.GetCell(position, adjustToBounds);
	}

	public IEnumerable<Cell> GetCellRange(Vector3 position, float radius)
	{
		return GetCellRange(position, radius, radius, requireCenterCell: true);
	}

	public void GetCellRange(Vector3 position, float radius, ICollection<Cell> result)
	{
		GetCellRange(position, radius, radius, requireCenterCell: true, result);
	}

	public void GetSections(Bounds b, ICollection<GridSection> result)
	{
		for (int i = 0; i < _gridSections.Length; i++)
		{
			if (_gridSections[i].bounds.Overlaps(b))
			{
				result.Add(_gridSections[i]);
			}
		}
	}

	public void TouchSections(Vector3 position)
	{
		for (int i = 0; i < _gridSections.Length; i++)
		{
			if (_gridSections[i].bounds.Contains(position))
			{
				_gridSections[i].Touch();
			}
		}
	}

	public void TouchSections(Bounds b)
	{
		for (int i = 0; i < _gridSections.Length; i++)
		{
			if (_gridSections[i].bounds.Overlaps(b))
			{
				_gridSections[i].Touch();
			}
		}
	}

	public void TouchSections(MatrixBounds b)
	{
		RectangleXZ other = new RectangleXZ(_cellMatrix.start.x + (float)b.minColumn * _cellSize, _cellMatrix.start.z + (float)b.minRow * _cellSize, (float)b.maxColumn * _cellSize, (float)b.minRow * _cellSize);
		for (int i = 0; i < _gridSections.Length; i++)
		{
			if (_gridSections[i].bounds.Overlaps(other))
			{
				_gridSections[i].Touch();
			}
		}
	}

	public bool HasSectionsChangedSince(Vector3 pos, float time)
	{
		pos = pos.AdjustAxis(_origin.y, Axis.Y);
		for (int i = 0; i < _gridSections.Length; i++)
		{
			GridSection gridSection = _gridSections[i];
			if (gridSection.bounds.Contains(pos) && gridSection.lastChanged >= time)
			{
				return true;
			}
		}
		return false;
	}

	public Cell GetNearestWalkablePerimeterCell(Vector3 position, Vector3 perimeter, IUnitProperties unitProps, bool adjustPositionToPerimiter)
	{
		if (adjustPositionToPerimiter)
		{
			if (perimeter.x != 0f)
			{
				position.x = GetPerimeterEdge(perimeter, mid: true);
				float edgeMid = bottom.edgeMid;
				float edgeMid2 = top.edgeMid;
				if (position.z < edgeMid)
				{
					position.z = edgeMid;
				}
				else if (position.z > edgeMid2)
				{
					position.z = edgeMid2;
				}
			}
			else if (perimeter.z != 0f)
			{
				position.z = GetPerimeterEdge(perimeter, mid: true);
				float edgeMid3 = left.edgeMid;
				float edgeMid4 = right.edgeMid;
				if (position.x < edgeMid3)
				{
					position.x = edgeMid3;
				}
				else if (position.x > edgeMid4)
				{
					position.x = edgeMid4;
				}
			}
		}
		Cell cell = GetCell(position);
		if (cell == null)
		{
			return null;
		}
		if (cell.IsWalkableWithClearance(unitProps))
		{
			return cell;
		}
		if (perimeter.x != 0f)
		{
			return TraversePerimeterX(cell, 1, unitProps);
		}
		return TraversePerimeterZ(cell, 1, unitProps);
	}

	public IEnumerable<Cell> GetCoveredCells(Bounds bounds)
	{
		return GetCellRange(bounds.center, bounds.extents.x, bounds.extents.z, requireCenterCell: false);
	}

	public void GetCoveredCells(Bounds bounds, ICollection<Cell> result)
	{
		GetCellRange(bounds.center, bounds.extents.x, bounds.extents.z, requireCenterCell: false, result);
	}

	public void Apply(Vector3 position, float radius, Action<Cell> act)
	{
		Apply(position, radius, radius, act);
	}

	public void Apply(Bounds bounds, Action<Cell> act)
	{
		Apply(bounds.center, bounds.extents.x, bounds.extents.z, act);
	}

	public IEnumerable<Cell> GetConcentricNeighbours(IGridCell center, int cellDistance)
	{
		return _cellMatrix.GetConcentricNeighbours(center.matrixPosX, center.matrixPosZ, cellDistance);
	}

	public void GetConcentricNeighbours(IGridCell center, int cellDistance, DynamicArray<Cell> array)
	{
		_cellMatrix.GetConcentricNeighbours(center.matrixPosX, center.matrixPosZ, cellDistance, array);
	}

	public Cell GetNeighbour(IGridCell cell, int dx, int dz)
	{
		int column = cell.matrixPosX + dx;
		int row = cell.matrixPosZ + dz;
		return _cellMatrix[column, row];
	}

	public bool TryGetWalkableNeighbour(IGridCell cell, int dx, int dz, IUnitProperties unitProps, out Cell neighbour)
	{
		int column = cell.matrixPosX + dx;
		int row = cell.matrixPosZ + dz;
		neighbour = _cellMatrix[column, row];
		if (neighbour == null)
		{
			return false;
		}
		return neighbour.IsWalkableFromWithClearance(cell, unitProps);
	}

	public IEnumerator<IPathNode> GetWalkableNeighboursIterator(IGridCell c, IUnitProperties unitProps, bool excludeCornerCutting)
	{
		_walkableIter.Prepare(c, unitProps, excludeCornerCutting);
		return _walkableIter;
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame)
	{
		Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating: false, null);
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame, Action callback)
	{
		Update(extent, maxMillisecondsUsedPerFrame, blockWhileUpdating: false, callback);
	}

	public void Update(Bounds extent, int maxMillisecondsUsedPerFrame, bool blockWhileUpdating, Action callback)
	{
		LoadBalancer.defaultBalancer.Add(new LongRunningAction(() => UpdateInternal(extent, blockWhileUpdating), maxMillisecondsUsedPerFrame, callback), 0.01f, delayFirstUpdate: true);
	}

	private IEnumerator UpdateInternal(Bounds extent, bool blockWhileUpdating)
	{
		ManualDynamicObstacle mo = null;
		if (blockWhileUpdating)
		{
			mo = new ManualDynamicObstacle(extent);
			mo.Toggle(active: true);
		}
		IEnumerator iter = _cellMatrix.Update(extent);
		if (iter != null)
		{
			while (iter.MoveNext())
			{
				yield return null;
			}
		}
		if (blockWhileUpdating)
		{
			mo.Toggle(active: false);
			yield break;
		}
		for (int i = 0; i < _gridSections.Length; i++)
		{
			if (_gridSections[i].bounds.Overlaps(extent))
			{
				_gridSections[i].Touch();
			}
		}
	}

	private void Initialize(CellMatrix matrix, GridSection[] gridSections)
	{
		_cellMatrix = matrix;
		_gridSections = gridSections;
		_origin = _cellMatrix.origin;
		_cellSize = _cellMatrix.cellSize;
		_sizeX = _cellMatrix.columns;
		_sizeZ = _cellMatrix.rows;
		_left = new Perimeter(Vector3.left, GetPerimeterEdge(Vector3.left, mid: false), GetPerimeterEdge(Vector3.left, mid: true));
		_right = new Perimeter(Vector3.right, GetPerimeterEdge(Vector3.right, mid: false), GetPerimeterEdge(Vector3.right, mid: true));
		_top = new Perimeter(Vector3.forward, GetPerimeterEdge(Vector3.forward, mid: false), GetPerimeterEdge(Vector3.forward, mid: true));
		_bottom = new Perimeter(Vector3.back, GetPerimeterEdge(Vector3.back, mid: false), GetPerimeterEdge(Vector3.back, mid: true));
		_left.SetPerpendiculars(_top, _bottom, _cellSize);
		_right.SetPerpendiculars(_bottom, _top, _cellSize);
		_top.SetPerpendiculars(_right, _left, _cellSize);
		_bottom.SetPerpendiculars(_left, _right, _cellSize);
	}

	private IEnumerable<Cell> GetCellRange(Vector3 position, float radiusX, float radiusZ, bool requireCenterCell)
	{
		float minOverlapToInclude = 0f;
		if (requireCenterCell)
		{
			minOverlapToInclude = _cellSize * 0.5f;
		}
		MatrixBounds matrixBounds = _cellMatrix.GetMatrixBounds(position, radiusX, radiusZ, minOverlapToInclude, adjustToBounds: false);
		return _cellMatrix.GetRange(matrixBounds);
	}

	private void GetCellRange(Vector3 position, float radiusX, float radiusZ, bool requireCenterCell, ICollection<Cell> result)
	{
		float minOverlapToInclude = 0f;
		if (requireCenterCell)
		{
			minOverlapToInclude = _cellSize * 0.5f;
		}
		MatrixBounds matrixBounds = _cellMatrix.GetMatrixBounds(position, radiusX, radiusZ, minOverlapToInclude, adjustToBounds: false);
		_cellMatrix.GetRange(matrixBounds, result);
	}

	private void Apply(Vector3 position, float radiusX, float radiusZ, Action<Cell> act)
	{
		float minOverlapToInclude = _cellSize * 0.5f;
		MatrixBounds matrixBounds = _cellMatrix.GetMatrixBounds(position, radiusX, radiusZ, minOverlapToInclude, adjustToBounds: false);
		_cellMatrix.Apply(matrixBounds, act);
	}

	private float GetPerimeterEdge(Vector3 perimeter, bool mid)
	{
		int num = (mid ? 1 : 0);
		if (perimeter.x != 0f)
		{
			return _origin.x + perimeter.x * (float)(_sizeX - num) * (_cellSize / 2f);
		}
		if (perimeter.z != 0f)
		{
			return _origin.z + perimeter.z * (float)(_sizeZ - num) * (_cellSize / 2f);
		}
		return 0f;
	}

	private Cell TraversePerimeterX(Cell c, int step, IUnitProperties unitProps)
	{
		int num = c.matrixPosZ + step;
		bool flag = num < _sizeZ;
		if (flag)
		{
			Cell cell = _cellMatrix.rawMatrix[c.matrixPosX, num];
			if (cell.IsWalkableWithClearance(unitProps))
			{
				return cell;
			}
		}
		int num2 = c.matrixPosZ - step;
		bool flag2 = num2 >= 0;
		if (flag2)
		{
			Cell cell2 = _cellMatrix.rawMatrix[c.matrixPosX, num2];
			if (cell2.IsWalkableWithClearance(unitProps))
			{
				return cell2;
			}
		}
		if (flag || flag2)
		{
			return TraversePerimeterX(c, step + 1, unitProps);
		}
		return null;
	}

	private Cell TraversePerimeterZ(Cell c, int step, IUnitProperties unitProps)
	{
		int num = c.matrixPosX + step;
		bool flag = num < _sizeX;
		if (flag)
		{
			Cell cell = _cellMatrix.rawMatrix[num, c.matrixPosZ];
			if (cell.IsWalkableWithClearance(unitProps))
			{
				return cell;
			}
		}
		int num2 = c.matrixPosX - step;
		bool flag2 = num2 >= 0;
		if (flag2)
		{
			Cell cell2 = _cellMatrix.rawMatrix[num2, c.matrixPosZ];
			if (cell2.IsWalkableWithClearance(unitProps))
			{
				return cell2;
			}
		}
		if (flag || flag2)
		{
			return TraversePerimeterZ(c, step + 1, unitProps);
		}
		return null;
	}
}
