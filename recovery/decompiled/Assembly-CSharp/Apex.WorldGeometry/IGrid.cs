using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.PathFinding;
using Apex.Units;
using UnityEngine;

namespace Apex.WorldGeometry;

public interface IGrid
{
	Vector3 origin { get; }

	int sizeX { get; }

	int sizeZ { get; }

	float cellSize { get; }

	Bounds bounds { get; }

	Perimeter left { get; }

	Perimeter right { get; }

	Perimeter top { get; }

	Perimeter bottom { get; }

	IEnumerable<Cell> cells { get; }

	CellMatrix cellMatrix { get; }

	GridSection[] gridSections { get; }

	bool Contains(Vector3 pos);

	Cell GetCell(Vector3 position);

	Cell GetCell(Vector3 position, bool adjustToBounds);

	Cell GetNearestWalkablePerimeterCell(Vector3 position, Vector3 perimeter, IUnitProperties unitProps, bool adjustPositionToPerimiter);

	Cell GetNeighbour(IGridCell cell, int dx, int dz);

	bool TryGetWalkableNeighbour(IGridCell cell, int dx, int dz, IUnitProperties unitProps, out Cell neighbour);

	IEnumerable<Cell> GetCellRange(Vector3 position, float radius);

	void GetCellRange(Vector3 position, float radius, ICollection<Cell> result);

	IEnumerable<Cell> GetCoveredCells(Bounds bounds);

	void GetCoveredCells(Bounds bounds, ICollection<Cell> result);

	void Apply(Vector3 position, float radius, Action<Cell> act);

	void Apply(Bounds bounds, Action<Cell> act);

	void GetSections(Bounds b, ICollection<GridSection> result);

	void TouchSections(Vector3 position);

	void TouchSections(Bounds b);

	void TouchSections(MatrixBounds b);

	bool HasSectionsChangedSince(Vector3 pos, float time);

	IEnumerable<Cell> GetConcentricNeighbours(IGridCell center, int cellDistance);

	void GetConcentricNeighbours(IGridCell center, int cellDistance, DynamicArray<Cell> array);

	IEnumerator<IPathNode> GetWalkableNeighboursIterator(IGridCell c, IUnitProperties unitProps, bool excludeCornerCutting);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame, Action callback);

	void Update(Bounds extent, int maxMillisecondsUsedPerFrame, bool blockWhileUpdating, Action callback);
}
