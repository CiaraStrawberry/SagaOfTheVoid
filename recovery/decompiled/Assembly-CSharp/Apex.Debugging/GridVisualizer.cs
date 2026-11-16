using System;
using Apex.Common;
using Apex.DataStructures;
using Apex.Steering;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Debugging;

[ExecuteInEditMode]
[AddComponentMenu("Apex/Game World/Debugging/Grid Visualizer", 1013)]
[ApexComponent("Debugging")]
public class GridVisualizer : Visualizer
{
	public enum GridMode
	{
		Layout,
		Accessibility,
		HeightOverlay
	}

	private class ModelProps : IUnitProperties, IPositioned, IHaveAttributes
	{
		private AttributeMask _attribs = AttributeMask.None;

		public float radius => 0.5f;

		public float fieldOfView => 200f;

		public float groundOffset => 0f;

		public float baseToPositionOffset => 0f;

		public float height => 1f;

		public Vector3 basePosition => Vector3.zero;

		public HeightNavigationCapabilities heightNavigationCapability { get; set; }

		public Vector3 position => Vector3.zero;

		public AttributeMask attributes
		{
			get
			{
				return _attribs;
			}
			set
			{
				_attribs = value;
			}
		}

		public bool isSelectable => false;

		public bool isSelected { get; set; }

		public int determination
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		public void RecalculateBasePosition()
		{
		}

		public void MarkSelectPending(bool pending)
		{
		}
	}

	[Tooltip("Controls how the grid visualization is drawn.")]
	public GridMode drawMode;

	public HeightNavigationCapabilities modelHeightNavigationCapabilities = new HeightNavigationCapabilities
	{
		maxClimbHeight = 0.5f,
		maxDropHeight = 1f,
		maxSlopeAngle = 30f
	};

	[AttributeProperty("Model Attributes", "The attributes of the model unit for which to show the visualization")]
	public int modelAttributes;

	[Tooltip("Draw a crude representation of the grid's sub sections.")]
	public bool drawSubSections;

	[Tooltip("Draw all grids in the scene, i.e. only one visualizer needed.")]
	public bool drawAllGrids;

	[Tooltip("The delay between updates after changes have been made to the scene.")]
	public int editorRefreshDelay = 100;

	[Tooltip("How zoomed out can you be and still see the grid visuals.")]
	public float drawDistanceThreshold = 175f;

	[Tooltip("The color of the grid lines.")]
	public Color gridLinesColor = new Color(135f / 255f, 135f / 255f, 135f / 255f);

	[Tooltip("The color of accessibility lines where only descent is possible.")]
	public Color descentOnlyColor = new Color(175f / 255f, 24.215687f, 60f / 255f);

	[Tooltip("The color of accessibility lines where only ascent is possible.")]
	public Color ascentOnlyColor = new Color(60f / 255f, 60f / 255f, 175f / 255f);

	[Tooltip("The color of obstructed cells.")]
	public Color obstacleColor = new Color(226f / 255f, 41f / 255f, 32f / 255f, 150f / 255f);

	[Tooltip("The color of the sub sections visualization.")]
	public Color subSectionsColor = new Color(0f, 150f / 255f, 211f / 255f);

	[Tooltip("The color of the bounds frame.")]
	public Color boundsColor = Color.grey;

	private DateTime? _nextRefresh;

	private ModelProps _modelProps = new ModelProps();

	protected override void DrawVisualization()
	{
		_modelProps.heightNavigationCapability = modelHeightNavigationCapabilities;
		_modelProps.attributes = modelAttributes;
		bool refresh = false;
		if (_nextRefresh.HasValue)
		{
			DateTime? nextRefresh = _nextRefresh;
			if (nextRefresh.HasValue && nextRefresh.GetValueOrDefault() < DateTime.UtcNow)
			{
				_nextRefresh = null;
				refresh = true;
			}
		}
		GridComponent[] array = ((!drawAllGrids) ? GetComponents<GridComponent>() : UnityEngine.Object.FindObjectsOfType<GridComponent>());
		if (array == null)
		{
			return;
		}
		GridComponent[] array2 = array;
		foreach (GridComponent gridComponent in array2)
		{
			if (!gridComponent.enabled)
			{
				continue;
			}
			Bounds drawArea = GetDrawArea(gridComponent.origin.y, out var outlineOnly);
			if (drawArea.Overlaps(gridComponent.bounds))
			{
				drawArea = gridComponent.EnsureForEditor(refresh, drawArea);
				if (!DrawGrid(gridComponent, drawArea, outlineOnly))
				{
					gridComponent.EnsureForEditor(refresh: true, drawArea);
					DrawGrid(gridComponent, drawArea, outlineOnly);
				}
			}
		}
	}

	private Bounds GetDrawArea(float gridElevation, out bool outlineOnly)
	{
		outlineOnly = false;
		Camera current = Camera.current;
		Vector3 vector = current.ScreenToGroundPoint(Vector3.zero, gridElevation);
		Vector3 vector2 = current.ScreenToGroundPoint(new Vector3(current.pixelWidth, current.pixelHeight, 0f), gridElevation);
		if (vector == Vector3.zero || vector2 == Vector3.zero)
		{
			outlineOnly = true;
		}
		float sqrMagnitude = (vector2 - vector).sqrMagnitude;
		if (sqrMagnitude > drawDistanceThreshold * drawDistanceThreshold)
		{
			outlineOnly = true;
		}
		Vector3 vector3 = current.ScreenToGroundPoint(new Vector3(0f, current.pixelHeight, 0f), gridElevation);
		Vector3 vector4 = current.ScreenToGroundPoint(new Vector3(current.pixelWidth, 0f, 0f), gridElevation);
		Vector3 min = new Vector3(Mathf.Min(vector.x, vector3.x, vector4.x, vector2.x), 0f, Mathf.Min(vector.z, vector3.z, vector4.z, vector2.z));
		Vector3 max = new Vector3(Mathf.Max(vector.x, vector3.x, vector4.x, vector2.x), 0f, Mathf.Max(vector.z, vector3.z, vector4.z, vector2.z));
		Bounds result = default;
		result.SetMinMax(min, max);
		return result;
	}

	private bool DrawGrid(GridComponent gridComp, Bounds drawArea, bool outlineOnly)
	{
		if (gridComp.sizeX == 0 || gridComp.sizeZ == 0 || gridComp.cellSize == 0f)
		{
			return true;
		}
		Gizmos.color = boundsColor;
		Gizmos.DrawWireCube(gridComp.bounds.center, gridComp.bounds.size);
		IGrid grid = gridComp.grid;
		if (outlineOnly || grid == null)
		{
			return true;
		}
		Cell cell = grid.GetCell(drawArea.min);
		Cell cell2 = grid.GetCell(drawArea.max);
		if (cell == null || cell2 == null)
		{
			return false;
		}
		Color lineColor = gridLinesColor;
		if (!gridComp.automaticInitialization && !Application.isPlaying)
		{
			lineColor.a *= 0.5f;
		}
		if (drawMode == GridMode.Layout)
		{
			DrawLayout(grid, cell, cell2, lineColor);
		}
		else if (drawMode == GridMode.HeightOverlay)
		{
			grid.cellMatrix.RenderHeightOverlay(gridLinesColor);
		}
		else
		{
			DrawAccessibility(grid, cell, cell2, lineColor);
		}
		if (drawSubSections)
		{
			float y = grid.origin.y + 0.05f;
			Gizmos.color = subSectionsColor;
			GridSection[] gridSections = grid.gridSections;
			foreach (GridSection gridSection in gridSections)
			{
				Vector3 center = gridSection.bounds.center;
				center.y = y;
				Gizmos.DrawWireCube(center, gridSection.bounds.size);
			}
		}
		return true;
	}

	private bool DrawLayout(IGrid grid, Cell start, Cell end, Color lineColor)
	{
		Gizmos.color = lineColor;
		float cellSize = grid.cellSize;
		float num = cellSize / 2f;
		float y = grid.origin.y + 0.05f;
		float num2 = start.position.x - num;
		float num3 = end.position.x + num;
		float num4 = start.position.z - num;
		float num5 = end.position.z + num;
		for (float num6 = num2; num6 <= num3; num6 += cellSize)
		{
			Gizmos.DrawLine(new Vector3(num6, y, num4), new Vector3(num6, y, num5));
		}
		for (float num7 = num4; num7 <= num5; num7 += cellSize)
		{
			Gizmos.DrawLine(new Vector3(num2, y, num7), new Vector3(num3, y, num7));
		}
		CellMatrix cellMatrix = grid.cellMatrix;
		for (int i = start.matrixPosX; i <= end.matrixPosX; i++)
		{
			for (int j = start.matrixPosZ; j <= end.matrixPosZ; j++)
			{
				Cell cell = cellMatrix[i, j];
				if (cell == null)
				{
					return false;
				}
				bool flag = cell.IsWalkable(AttributeMask.All);
				bool flag2 = cell.IsWalkable(AttributeMask.None);
				if (!flag)
				{
					Gizmos.color = obstacleColor;
					Gizmos.DrawCube(cell.position, new Vector3(cellSize, 0.05f, cellSize));
				}
				else if (!flag2)
				{
					Color color = obstacleColor;
					color.a *= 0.5f;
					Gizmos.color = color;
					Gizmos.DrawCube(cell.position, new Vector3(cellSize, 0.05f, cellSize));
				}
			}
		}
		return true;
	}

	private bool DrawAccessibility(IGrid grid, Cell start, Cell end, Color lineColor)
	{
		CellMatrix cellMatrix = grid.cellMatrix;
		float cellSize = grid.cellSize;
		VectorXZ[] array = new VectorXZ[4]
		{
			new VectorXZ(-1, 0),
			new VectorXZ(-1, 1),
			new VectorXZ(0, 1),
			new VectorXZ(1, 1)
		};
		Vector3 vector = new Vector3(0f, 0.05f, 0f);
		for (int i = start.matrixPosX; i <= end.matrixPosX; i++)
		{
			for (int j = start.matrixPosZ; j <= end.matrixPosZ; j++)
			{
				Cell cell = cellMatrix[i, j];
				if (cell == null)
				{
					return false;
				}
				if (!cell.IsWalkableWithClearance(_modelProps))
				{
					Gizmos.color = obstacleColor;
					Gizmos.DrawCube(cell.position, new Vector3(cellSize, 0.05f, cellSize));
					continue;
				}
				VectorXZ vectorXZ = new VectorXZ(i, j);
				for (int k = 0; k < 4; k++)
				{
					VectorXZ vectorXZ2 = vectorXZ + array[k];
					Cell cell2 = cellMatrix[vectorXZ2.x, vectorXZ2.z];
					if (cell2 == null || !cell2.IsWalkableWithClearance(_modelProps))
					{
						continue;
					}
					Cell cell3 = cell;
					Cell cell4 = cell2;
					if (cell3.position.y <= cell4.position.y)
					{
						cell3 = cell4;
						cell4 = cell;
					}
					bool flag = cell4.IsWalkableFromWithClearance(cell3, _modelProps);
					bool flag2 = cell3.IsWalkableFromWithClearance(cell4, _modelProps);
					if (flag && flag2)
					{
						Gizmos.color = lineColor;
					}
					else if (flag)
					{
						Gizmos.color = descentOnlyColor;
					}
					else
					{
						if (!flag2)
						{
							continue;
						}
						Gizmos.color = ascentOnlyColor;
					}
					Gizmos.DrawLine(cell.position + vector, cell2.position + vector);
				}
			}
		}
		return true;
	}

	private void Update()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			_nextRefresh = DateTime.UtcNow.AddMilliseconds(editorRefreshDelay);
		}
	}
}
