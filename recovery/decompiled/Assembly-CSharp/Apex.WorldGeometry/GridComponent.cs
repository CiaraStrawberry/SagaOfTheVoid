using System;
using System.Linq;
using Apex.DataStructures;
using Apex.Messages;
using Apex.Services;
using Apex.Utilities;
using UnityEngine;

namespace Apex.WorldGeometry;

[ExecuteInEditMode]
[AddComponentMenu("Apex/Game World/Grid", 1033)]
[ApexComponent("Game World")]
public class GridComponent : MonoBehaviour
{
	private class RuntimeGridComponent : GridComponent
	{
		private bool _configured;

		internal void Configure(GridConfig cfg)
		{
			automaticInitialization = false;
			linkOriginToTransform = false;
			origin = cfg.origin;
			sizeX = cfg.sizeX;
			sizeZ = cfg.sizeZ;
			cellSize = cfg.cellSize;
			connectorPortalWidth = cfg.connectorPortalWidth;
			generateHeightmap = cfg.generateHeightmap;
			heightLookupType = cfg.heightLookupType;
			heightLookupMaxDepth = cfg.heightLookupMaxDepth;
			lowerBoundary = cfg.lowerBoundary;
			upperBoundary = cfg.upperBoundary;
			obstacleSensitivityRange = cfg.obstacleSensitivityRange;
			obstacleAndGroundDetection = cfg.obstacleAndGroundDetection;
			subSectionsCellOverlap = cfg.subSectionsCellOverlap;
			subSectionsX = cfg.subSectionsX;
			subSectionsZ = cfg.subSectionsZ;
			friendlyName = cfg.friendlyName;
			_automaticConnections = cfg.automaticConnections;
			_obstacleAndGroundDetector = cfg.obstacleAndGroundDetector;
			Init();
			enabled = false;
			_configured = true;
		}

		internal void ConfigureAndInitialize(GridConfig cfg, int maxMillisecondsUsedPerFrame, Action<GridComponent> callback)
		{
			Configure(cfg);
			Initialize(maxMillisecondsUsedPerFrame, callback);
		}

		protected override void Awake()
		{
		}

		protected override void OnEnable()
		{
			if (_configured)
			{
				base.OnEnable();
			}
		}
	}

	[HideInInspector]
	public float heightGranularity = 0.1f;

	[HideInInspector]
	public float maxWalkableSlopeAngle = 30f;

	[HideInInspector]
	public float maxScaleHeight = 0.5f;

	[MinCheck(1, tooltip = "Number of cells along the x-axis.")]
	public int sizeX = 10;

	[MinCheck(1, tooltip = "Number of cells along the z-axis.")]
	public int sizeZ = 10;

	[MinCheck(1, label = "Subsections X", tooltip = "Number of subsections along the x-axis.")]
	public int subSectionsX = 2;

	[MinCheck(1, label = "Subsections Z", tooltip = "Number of subsections along the z-axis.")]
	public int subSectionsZ = 2;

	[MinCheck(0, label = "Subsections Cell Overlap", tooltip = "The number of cells by which subsections overlap neighbouring subsections.")]
	public int subSectionsCellOverlap = 2;

	[MinCheck(0.1f, tooltip = "The size of each grid cell, expressed as the length of one side.")]
	public float cellSize = 2f;

	[Label("Generate Height Map", "Controls whether the grid generates a height map to allow height sensitive navigation.")]
	public bool generateHeightmap = true;

	[Label("Lookup Type", "Dictionaries are fast but dense. Quad Trees are sparse but slower and are very dependent on height distributions. Use Quad trees on maps with large same height areas. Don't use Quad trees if you do dynamic grid updating.")]
	public HeightLookupType heightLookupType = HeightLookupType.QuadTree;

	[MinCheck(1, label = "Tree Depth", tooltip = "The higher the allowed depth, the more sparse the tree will be but it will also get slower.")]
	public int heightLookupMaxDepth = 5;

	[MinCheck(0f, tooltip = "How far below the grid's plane does the grid have influence.")]
	public float lowerBoundary = 1f;

	[MinCheck(0f, tooltip = "How far above the grid's plane does the grid have influence.")]
	public float upperBoundary = 10f;

	[MinCheck(0f, tooltip = "How close to the center of a cell must an obstacle be to block the cell.")]
	public float obstacleSensitivityRange = 0.5f;

	[Tooltip("Controls the method used to detect terrain and obstacles. Choose whichever works for your type of terrain.\nTo use custom mode, you must first add a component that implements IBlockDetectorFactory (can be on parent)")]
	public ColliderDetectionMode obstacleAndGroundDetection = ColliderDetectionMode.Mixed;

	[SerializeField]
	[HideInInspector]
	public CellMatrixData bakedData;

	[SerializeField]
	[Tooltip("The center of the grid.")]
	private Vector3 _origin;

	[SerializeField]
	[Label("Offset", "The offset in relation to the linked transform.")]
	private Vector3 _originOffset;

	[SerializeField]
	[Tooltip("An optional friendly name for the grid that will be used in messages and such.")]
	private string _friendlyName;

	[SerializeField]
	[Tooltip("Link the center of the grid to the position of the game object.")]
	private bool _linkOriginToTransform = true;

	[SerializeField]
	[Label("Store Grid data as asset", "Store baked data in a separate asset file instead of storing to the scene, this enables prefab'ing.")]
	private bool _storeBakedDataAsAsset;

	[SerializeField]
	[Tooltip("Controls whether the grid is automatically initialized when enabled. If unchecked the grid must be manually initialized.")]
	private bool _automaticInitialization = true;

	[SerializeField]
	[Tooltip("Controls whether the grid will automatically attempt to connect itself with adjacent grids once it has initialized.\nWill only connect to other initialized and enabled grids.")]
	private bool _automaticConnections;

	[SerializeField]
	[MinCheck(0f, tooltip = "The width of the connector portals.\nThis is used when auto-connecting grids.\n\nIf left at 0 it will default to the cellSize, but if larger units need to be able to traverse a connector the width must be set accordingly.")]
	private float _connectorPortalWidth;

	protected IBlockDetector _obstacleAndGroundDetector;

	private Bounds _bounds;

	public string friendlyName
	{
		get
		{
			return _friendlyName;
		}
		set
		{
			_friendlyName = value;
		}
	}

	public bool linkOriginToTransform
	{
		get
		{
			return _linkOriginToTransform;
		}
		set
		{
			_linkOriginToTransform = value;
		}
	}

	public float connectorPortalWidth
	{
		get
		{
			if (_connectorPortalWidth <= 0f)
			{
				return cellSize;
			}
			return _connectorPortalWidth;
		}
		set
		{
			_connectorPortalWidth = value;
		}
	}

	public bool storeBakedDataAsAsset
	{
		get
		{
			return _storeBakedDataAsAsset;
		}
		set
		{
			_storeBakedDataAsAsset = value;
		}
	}

	public bool automaticInitialization
	{
		get
		{
			return _automaticInitialization;
		}
		set
		{
			_automaticInitialization = value;
		}
	}

	public Vector3 origin
	{
		get
		{
			if (_linkOriginToTransform)
			{
				return transform.position + _originOffset;
			}
			return _origin;
		}
		set
		{
			if (!_linkOriginToTransform)
			{
				_origin = value;
			}
		}
	}

	public Vector3 originOffset
	{
		get
		{
			return _originOffset;
		}
		set
		{
			_originOffset = value;
		}
	}

	public IGrid grid { get; private set; }

	public Bounds bounds
	{
		get
		{
			if (grid == null)
			{
				CalculateBounds();
			}
			return _bounds;
		}
	}

	public static GridComponent Create(GameObject host, GridConfig cfg)
	{
		Ensure.ArgumentNotNull(host, "host");
		Ensure.ArgumentNotNull(cfg, "cfg");
		RuntimeGridComponent runtimeGridComponent = host.AddComponent<RuntimeGridComponent>();
		runtimeGridComponent.Configure(cfg);
		return runtimeGridComponent;
	}

	public static GridComponent CreateAndInitialize(GameObject host, GridConfig cfg, int maxMillisecondsUsedPerFrame, Action<GridComponent> callback)
	{
		Ensure.ArgumentNotNull(host, "host");
		Ensure.ArgumentNotNull(cfg, "cfg");
		RuntimeGridComponent runtimeGridComponent = host.AddComponent<RuntimeGridComponent>();
		runtimeGridComponent.ConfigureAndInitialize(cfg, maxMillisecondsUsedPerFrame, callback);
		return runtimeGridComponent;
	}

	public GridBuilder GetBuilder()
	{
		if (obstacleAndGroundDetection == ColliderDetectionMode.Custom && _obstacleAndGroundDetector == null)
		{
			IBlockDetectorFactory blockDetectorFactory = this.As<IBlockDetectorFactory>(searchParent: true);
			if (blockDetectorFactory == null)
			{
				obstacleAndGroundDetection = ColliderDetectionMode.Mixed;
			}
			else
			{
				_obstacleAndGroundDetector = blockDetectorFactory.Create();
				if (_obstacleAndGroundDetector == null)
				{
					obstacleAndGroundDetection = ColliderDetectionMode.Mixed;
				}
			}
		}
		GridBuilder gridBuilder = new GridBuilder();
		gridBuilder.origin = origin;
		gridBuilder.sizeX = sizeX;
		gridBuilder.sizeZ = sizeZ;
		gridBuilder.cellSize = cellSize;
		gridBuilder.obstacleSensitivityRange = obstacleSensitivityRange;
		gridBuilder.generateHeightmap = generateHeightmap;
		gridBuilder.heightLookupType = heightLookupType;
		gridBuilder.heightLookupMaxDepth = heightLookupMaxDepth;
		gridBuilder.lowerBoundary = lowerBoundary;
		gridBuilder.upperBoundary = upperBoundary;
		gridBuilder.obstacleAndGroundDetection = obstacleAndGroundDetection;
		gridBuilder.obstacleAndGroundDetector = _obstacleAndGroundDetector;
		gridBuilder.subSectionsX = subSectionsX;
		gridBuilder.subSectionsZ = subSectionsZ;
		gridBuilder.subSectionsCellOverlap = subSectionsCellOverlap;
		gridBuilder.bounds = bounds;
		return gridBuilder;
	}

	public void Initialize(int maxMillisecondsUsedPerFrame, Action<GridComponent> callback)
	{
		if (grid != null)
		{
			return;
		}
		GridBuilder builder = GetBuilder();
		Action msgCallback = () =>
		{
			if (callback != null)
			{
				callback(this);
			}
		};
		Action<IGrid> callback2 = (IGrid g) =>
		{
			grid = g;
			RegisterWithManagers();
			enabled = true;
			if (_automaticConnections)
			{
				this.Connect(NeighbourPosition.Top);
				this.Connect(NeighbourPosition.Bottom);
				this.Connect(NeighbourPosition.Left);
				this.Connect(NeighbourPosition.Right);
			}
			GridStatusMessage message = new GridStatusMessage
			{
				gridBounds = g.bounds,
				status = GridStatusMessage.StatusCode.InitializationComplete
			};
			GameServices.messageBus.PostBalanced(message, maxMillisecondsUsedPerFrame, msgCallback);
		};
		if (bakedData != null)
		{
			builder.Create(bakedData, maxMillisecondsUsedPerFrame, callback2);
		}
		else
		{
			builder.Create(maxMillisecondsUsedPerFrame, callback2);
		}
	}

	public void Initialize()
	{
		if (grid == null)
		{
			EnsureGrid();
			RegisterWithManagers();
			enabled = true;
			if (_automaticConnections)
			{
				this.Connect(NeighbourPosition.Top);
				this.Connect(NeighbourPosition.Bottom);
				this.Connect(NeighbourPosition.Left);
				this.Connect(NeighbourPosition.Right);
			}
			GridStatusMessage gridStatusMessage = new GridStatusMessage();
			gridStatusMessage.gridBounds = grid.bounds;
			gridStatusMessage.status = GridStatusMessage.StatusCode.InitializationComplete;
			GridStatusMessage message = gridStatusMessage;
			GameServices.messageBus.Post(message);
		}
	}

	public void Disable(int maxMillisecondsUsedPerFrame)
	{
		if (grid != null)
		{
			enabled = false;
			grid = null;
			GridStatusMessage gridStatusMessage = new GridStatusMessage();
			gridStatusMessage.gridBounds = bounds;
			gridStatusMessage.status = GridStatusMessage.StatusCode.DisableComplete;
			GridStatusMessage message = gridStatusMessage;
			GameServices.messageBus.PostBalanced(message, maxMillisecondsUsedPerFrame);
		}
	}

	public void ResetGrid()
	{
		grid = null;
	}

	public bool Contains(Vector3 pos)
	{
		return bounds.Contains(pos);
	}

	internal void EnsureGrid()
	{
		if (grid != null)
		{
			return;
		}
		GridBuilder builder = GetBuilder();
		if (bakedData != null)
		{
			grid = builder.Create(bakedData);
			if (!storeBakedDataAsAsset)
			{
				bakedData = null;
			}
		}
		else
		{
			grid = builder.Create();
		}
	}

	internal Bounds EnsureForEditor(bool refresh, Bounds area)
	{
		if (!Application.isPlaying)
		{
			if (refresh && transform.hasChanged)
			{
				transform.hasChanged = false;
				grid = null;
			}
			if (grid == null)
			{
				GridBuilder builder = GetBuilder();
				grid = builder.CreateForEditor(bakedData);
			}
		}
		Bounds bounds = this.bounds;
		bounds.Expand(0f - cellSize);
		Vector3 min = new Vector3(Mathf.Max(area.min.x, bounds.min.x), 0f, Mathf.Max(area.min.z, bounds.min.z));
		Vector3 max = new Vector3(Mathf.Min(area.max.x, bounds.max.x), 0f, Mathf.Min(area.max.z, bounds.max.z));
		area.SetMinMax(min, max);
		if (refresh && !Application.isPlaying)
		{
			grid.cellMatrix.UpdateForEditor(area, bakedData);
		}
		return area;
	}

	protected void Init()
	{
		GridManager.instance.RegisterGridComponent(this);
	}

	protected virtual void Awake()
	{
		Init();
	}

	protected virtual void OnEnable()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			Init();
		}
		else if (automaticInitialization)
		{
			EnsureGrid();
			RegisterWithManagers();
			if (_automaticConnections)
			{
				this.Connect(NeighbourPosition.Top);
				this.Connect(NeighbourPosition.Bottom);
				this.Connect(NeighbourPosition.Left);
				this.Connect(NeighbourPosition.Right);
			}
		}
	}

	private void OnDisable()
	{
		if (grid != null && Application.isPlaying)
		{
			GridManager.instance.UnregisterGrid(grid);
			HeightMapManager.instance.UnregisterMap(grid.cellMatrix);
		}
	}

	private void OnDestroy()
	{
		GridManager.instance.UnregisterGridComponent(this);
		if (Application.isPlaying)
		{
			GridPortalComponent[] array = GridManager.instance.GetAssociatedPortals(this).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
			Disable(5);
		}
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			grid = null;
		}
	}

	private void CalculateBounds()
	{
		float num = (upperBoundary - lowerBoundary) * 0.5f;
		Vector3 center = origin;
		center.y += num;
		_bounds = new Bounds(center, new Vector3((float)sizeX * cellSize, upperBoundary + lowerBoundary, (float)sizeZ * cellSize));
	}

	private void RegisterWithManagers()
	{
		GridManager.instance.RegisterGrid(grid);
		CellMatrix cellMatrix = grid.cellMatrix;
		HeightMapManager.instance.RegisterMap(cellMatrix);
	}
}
