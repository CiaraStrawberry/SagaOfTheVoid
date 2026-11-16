using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apex.PathFinding;
using Apex.PathFinding.MoveCost;
using Apex.Services;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Debugging;

[AddComponentMenu("Apex/Game World/Debugging/Path Finder Visualizer", 1014)]
public class PathFinderVisualizer : MonoBehaviour, INeedPath
{
	public enum CostDiplay
	{
		None,
		MoveCost,
		Heuristics,
		Total
	}

	private interface IVisualizedPathEngine : IPathingEngine
	{
		List<IPathNode> expandedSet { get; }

		IPathNode currentNode { get; }

		void Reset();
	}

	private class Styles
	{
		public GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 14,
			fontStyle = FontStyle.Bold
		};
	}

	private class VisualizedAStar : PathingAStar, IVisualizedPathEngine, IPathingEngine
	{
		public List<IPathNode> expandedSet => _expandedSet;

		public IPathNode currentNode => _current;

		public VisualizedAStar(int heapInitialSize, IMoveCost moveCostProvider, ICellCostStrategy cellCostStrategy, ISmoothPaths pathSmoother, IRequestPreProcessor[] requestPreprocessors)
			: base(heapInitialSize, moveCostProvider, cellCostStrategy, pathSmoother, requestPreprocessors)
		{
		}

		public void Reset()
		{
			_expandedSet.Apply((IPathNode c) =>
			{
				c.g = 0;
			});
			_expandedSet.Clear();
			_current = null;
		}
	}

	private class VisualizedJPS : PathingJumpPointSearch, IVisualizedPathEngine, IPathingEngine
	{
		public List<IPathNode> expandedSet => _expandedSet;

		public IPathNode currentNode => _current;

		public VisualizedJPS(int heapInitialSize, IMoveCost moveCostProvider, ICellCostStrategy cellCostStrategy, ISmoothPaths pathSmoother, IRequestPreProcessor[] requestPreprocessors)
			: base(heapInitialSize, moveCostProvider, cellCostStrategy, pathSmoother, requestPreprocessors)
		{
		}

		public void Reset()
		{
			_expandedSet.Apply((IPathNode c) =>
			{
				c.g = 0;
			});
			_expandedSet.Clear();
			_current = null;
		}
	}

	[Tooltip("The pathing engine to use.")]
	public PathingEngineType engineType;

	[Tooltip("The algorithm used for calculating move costs.")]
	public CostProviderType moveCost;

	[MinCheck(50, tooltip = "Memory allocation optimization. You only need to change this if you get warnings that ask you to do so.")]
	public int initialHeapSize = 100;

	[Tooltip("The unit used for navigation settings.")]
	public GameObject unit;

	[Tooltip("What cost info to display for each cell.")]
	public CostDiplay costInfo;

	[Tooltip("Controls whether the visualization is done step-by-step or in full.")]
	public bool stepByStep = true;

	[Tooltip("Controls whether an instructions box is displayed.")]
	public bool showInstructions = true;

	private Styles _styles;

	private IVisualizedPathEngine _engine;

	private IEnumerator _pathEnumerator;

	private BasicPathRequest _req;

	private IUnitFacade _unit;

	private float _cellSize;

	private Vector3 _gizmoSize;

	private void Awake()
	{
		Init();
	}

	public void Reset()
	{
		ResetEnumerator();
		Init();
	}

	private void Init()
	{
		if (unit == null)
		{
			Debug.LogWarning("The Path Finder Visualizer must have a unit set.");
			return;
		}
		_unit = unit.GetUnitFacade();
		IGrid grid = GridManager.instance.GetGrid(_unit.position);
		_cellSize = grid.cellSize;
		_gizmoSize = new Vector3(_cellSize, 0.2f, _cellSize);
		IMoveCost moveCostProvider;
		if (moveCost != CostProviderType.Custom)
		{
			moveCostProvider = moveCost switch
			{
				CostProviderType.Euclidean => (IMoveCost)new EuclideanDistance(10), 
				CostProviderType.Cardinal => new CardinalDistance(10), 
				CostProviderType.Manhattan => new ManhattanDistance(10), 
				_ => new DiagonalDistance(10), 
			};
		}
		else
		{
			IMoveCostFactory moveCostFactory = this.As<IMoveCostFactory>();
			if (moveCostFactory == null)
			{
				moveCostProvider = new DiagonalDistance(10);
				Debug.LogWarning("Path Service Component: Cost Provider set to Custom but no Cost Provider Factory found!.");
			}
			else
			{
				moveCostProvider = moveCostFactory.CreateMoveCostProvider();
			}
		}
		IRequestPreProcessor[] requestPreprocessors = (from IRequestPreProcessor p in GetComponents(typeof(IRequestPreProcessor))
			orderby p.priority descending
			select p).ToArray();
		if (engineType == PathingEngineType.Astar)
		{
			_engine = new VisualizedAStar(initialHeapSize, moveCostProvider, GameServices.cellCostStrategy, new PathSmoother(), requestPreprocessors);
		}
		else
		{
			_engine = new VisualizedJPS(initialHeapSize, moveCostProvider, GameServices.cellCostStrategy, new PathSmoother(), requestPreprocessors);
		}
		BasicPathRequest basicPathRequest = new BasicPathRequest();
		basicPathRequest.requester = this;
		basicPathRequest.requesterProperties = _unit;
		basicPathRequest.pathFinderOptions = _unit.pathFinderOptions;
		BasicPathRequest basicPathRequest2 = basicPathRequest;
		if (_req != null)
		{
			basicPathRequest2.from = _req.from;
			basicPathRequest2.to = _req.to;
		}
		_req = basicPathRequest2;
	}

	private void Update()
	{
		RaycastHit hit;
		if (UnityEngine.Input.GetMouseButtonUp(0))
		{
			ResetEnumerator();
			UnityServices.mainCamera.ScreenToLayerHit(UnityEngine.Input.mousePosition, Layers.terrain, 1000f, out hit);
			_req.from = hit.point;
		}
		else if (UnityEngine.Input.GetMouseButtonUp(1))
		{
			ResetEnumerator();
			UnityServices.mainCamera.ScreenToLayerHit(UnityEngine.Input.mousePosition, Layers.terrain, 1000f, out hit);
			_req.to = hit.point;
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.R))
		{
			ResetEnumerator();
		}
		else
		{
			if (!UnityEngine.Input.GetKeyUp(KeyCode.Space) || _req == null)
			{
				return;
			}
			if (_pathEnumerator == null)
			{
				_pathEnumerator = _engine.ProcessRequestCoroutine(_req);
			}
			if (!_pathEnumerator.MoveNext())
			{
				ResetEnumerator();
			}
			else if (!stepByStep)
			{
				while (_pathEnumerator.MoveNext())
				{
				}
			}
		}
	}

	private void ResetEnumerator()
	{
		if (_pathEnumerator != null)
		{
			while (_pathEnumerator.MoveNext())
			{
			}
			_pathEnumerator = null;
			_engine.Reset();
		}
	}

	private Vector3 GetPos(IPathNode n)
	{
		if (n is PortalCell)
		{
			return n.predecessor.position;
		}
		return n.position;
	}

	private void OnDrawGizmos()
	{
		if (_engine == null)
		{
			return;
		}
		List<IPathNode> expandedSet = _engine.expandedSet;
		for (int i = 0; i < expandedSet.Count; i++)
		{
			IPathNode pathNode = expandedSet[i];
			Vector3 pos = GetPos(pathNode);
			Gizmos.color = Color.green;
			if (pathNode.isClosed)
			{
				if (pathNode is PortalCell)
				{
					Gizmos.color = Color.magenta;
				}
				else
				{
					Gizmos.color = Color.yellow;
				}
			}
			else if (pathNode is PortalCell)
			{
				continue;
			}
			Gizmos.DrawCube(pos, _gizmoSize);
		}
		Gizmos.color = Color.blue;
		IPathNode pathNode2 = _engine.currentNode;
		if (pathNode2 != null)
		{
			if (!(pathNode2 is PortalCell))
			{
				Gizmos.DrawCube(pathNode2.position, _gizmoSize);
			}
			while (pathNode2.predecessor != null)
			{
				Vector3 pos2 = GetPos(pathNode2);
				Gizmos.DrawLine(pos2, GetPos(pathNode2.predecessor));
				pathNode2 = pathNode2.predecessor;
			}
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(_req.from, 0.25f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_req.to, 0.25f);
	}

	private void OnGUI()
	{
		if (_styles == null)
		{
			_styles = new Styles();
		}
		if (costInfo != CostDiplay.None)
		{
			if (Camera.main == null)
			{
				return;
			}
			List<IPathNode> expandedSet = _engine.expandedSet;
			for (int i = 0; i < expandedSet.Count; i++)
			{
				IPathNode pathNode = expandedSet[i];
				if (pathNode != null && !(pathNode is PortalCell))
				{
					int num = costInfo switch
					{
						CostDiplay.MoveCost => pathNode.g, 
						CostDiplay.Heuristics => pathNode.h, 
						_ => pathNode.f, 
					};
					Vector3 vector = Camera.main.WorldToScreenPoint(pathNode.position);
					vector.y = (float)Screen.height - vector.y;
					GUI.color = Color.black;
					GUI.Label(new Rect(vector.x - 10f, vector.y - 6f, 50f, 20f), num.ToString());
				}
			}
		}
		if (showInstructions)
		{
			float width = 300f;
			float num2 = (float)Screen.height * 0.9f;
			GUILayout.BeginArea(new Rect(5f, (float)Screen.height / 2f - num2 / 2f, width, num2), GUI.skin.window);
			GUILayout.BeginVertical();
			GUILayout.Label("Path Finder Visualization", _styles.headerStyle);
			GUILayout.Label("Blue Squares represent the node being processed.");
			GUILayout.Label("Yellow Squares represent the nodes that have been processed (closed).");
			GUILayout.Label("Green Squares represent expanded nodes yet to be processed.");
			GUILayout.Label("Purple Squares represent nodes from which a portal is crossed.");
			GUILayout.Label("\nLeft click to set the Starting Point.\n\nRight click to set the End Point.\n\nPress 'Space' to visualize.\n\nPress 'R' to reset.");
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	public void ConsumePathResult(PathResult result)
	{
		Debug.Log("Pathing complete, result: " + result.status);
		if (result.status == PathingStatus.Failed)
		{
			Debug.Log(result.errorInfo);
		}
		else
		{
			Debug.Log("Path length: " + result.path.length);
		}
	}
}
