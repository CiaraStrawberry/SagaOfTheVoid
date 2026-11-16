using System;
using System.Collections;
using Apex.DataStructures;
using Apex.LoadBalancing;
using Apex.Utilities;
using UnityEngine;

namespace Apex.WorldGeometry;

public class GridBuilder : ICellMatrixConfiguration
{
	public Vector3 origin { get; set; }

	public int sizeX { get; set; }

	public int sizeZ { get; set; }

	public float cellSize { get; set; }

	public float obstacleSensitivityRange { get; set; }

	public bool generateHeightmap { get; set; }

	public HeightLookupType heightLookupType { get; set; }

	public int heightLookupMaxDepth { get; set; }

	public float upperBoundary { get; set; }

	public float lowerBoundary { get; set; }

	public ColliderDetectionMode obstacleAndGroundDetection { get; set; }

	public IBlockDetector obstacleAndGroundDetector { get; set; }

	public int subSectionsX { get; set; }

	public int subSectionsZ { get; set; }

	public int subSectionsCellOverlap { get; set; }

	public Bounds bounds { get; set; }

	public IGrid Create()
	{
		CellMatrix cellMatrix = CellMatrix.Create(this);
		GridSection[] gridSections = CreateSubSections(cellMatrix.start);
		return Grid.Create(cellMatrix, gridSections);
	}

	public IGrid Create(CellMatrixData data)
	{
		Ensure.ArgumentNotNull(data, "data");
		CellMatrix cellMatrix = CellMatrix.Create(this, data);
		GridSection[] gridSections = CreateSubSections(cellMatrix.start);
		return Grid.Create(cellMatrix, gridSections);
	}

	internal IGrid CreateForEditor(CellMatrixData data)
	{
		CellMatrix cellMatrix = CellMatrix.CreateForEditor(this, data);
		GridSection[] gridSections = CreateSubSections(cellMatrix.start);
		return Grid.Create(cellMatrix, gridSections);
	}

	internal void Create(int maxMillisecondsUsedPerFrame, Action<IGrid> callback)
	{
		LoadBalancer.defaultBalancer.Add(new LongRunningAction(() => CreateIncrementally(callback), maxMillisecondsUsedPerFrame), 0.01f, delayFirstUpdate: true);
	}

	internal void Create(CellMatrixData data, int maxMillisecondsUsedPerFrame, Action<IGrid> callback)
	{
		Ensure.ArgumentNotNull(data, "data");
		LoadBalancer.defaultBalancer.Add(new LongRunningAction(() => CreateIncrementally(data, callback), maxMillisecondsUsedPerFrame), 0.01f, delayFirstUpdate: true);
	}

	private IEnumerator CreateIncrementally(CellMatrixData data, Action<IGrid> callback)
	{
		CellMatrix.MatrixIncrementalInitializer cellMatrixInitializer = CellMatrix.CreateIncrementally(this, data);
		while (cellMatrixInitializer.isInitializing)
		{
			yield return null;
		}
		CellMatrix cellMatrix = cellMatrixInitializer.matrix;
		GridSection[] subSections = CreateSubSections(cellMatrix.start);
		yield return null;
		Grid grid = Grid.Create(cellMatrix, subSections);
		callback(grid);
	}

	private IEnumerator CreateIncrementally(Action<IGrid> callback)
	{
		CellMatrix.MatrixIncrementalInitializer cellMatrixInitializer = CellMatrix.CreateIncrementally(this);
		while (cellMatrixInitializer.isInitializing)
		{
			yield return null;
		}
		CellMatrix cellMatrix = cellMatrixInitializer.matrix;
		GridSection[] subSections = CreateSubSections(cellMatrix.start);
		yield return null;
		Grid grid = Grid.Create(cellMatrix, subSections);
		callback(grid);
	}

	private GridSection[] CreateSubSections(Vector3 start)
	{
		int num = Math.Max(subSectionsX, 1);
		int num2 = Math.Max(subSectionsZ, 1);
		float num3 = (float)subSectionsCellOverlap * cellSize;
		float num4 = (float)(sizeX + (num - 1) * subSectionsCellOverlap) * cellSize / ((float)num * 1f);
		float num5 = (float)(sizeZ + (num2 - 1) * subSectionsCellOverlap) * cellSize / ((float)num2 * 1f);
		int num6 = num * num2;
		GridSection[] array = new GridSection[num6];
		int num7 = 0;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				RectangleXZ rect = new RectangleXZ(start.x + num4 * (float)i - num3 * (float)i, start.z + num5 * (float)j - num3 * (float)j, num4, num5);
				array[num7++] = new GridSection(rect);
			}
		}
		return array;
	}
}
