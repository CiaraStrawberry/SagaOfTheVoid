using Apex.Utilities;
using UnityEngine;

namespace Apex.PathFinding;

[AddComponentMenu("Apex/Unit/Navigation/Path Options", 1028)]
[ApexComponent("Unit Properties")]
public class PathOptionsComponent : MonoBehaviour, IPathFinderOptions, IPathNavigationOptions
{
	[SerializeField]
	[Tooltip("The priority with which this unit's path requests should be processed.")]
	private int _pathingPriority;

	[SerializeField]
	[Tooltip("Whether to use path smoothing to create more natural paths.")]
	private bool _usePathSmoothing = true;

	[SerializeField]
	[Tooltip("Whether to optimize by completely skipping path finding for paths where no obstruction exists between start and destination.\n\nPlease note that doing so will also skip portal evaluation inside the same grid, so e.g. short-cut portals within the same grid will not be evaluated in the case of unobstructed paths.")]
	private bool _optimizeUnobstructedPaths = true;

	[SerializeField]
	[Tooltip("Whether to allow the path to cut corners. Corner cutting has slightly better performance, but produces less natural routes.")]
	private bool _allowCornerCutting;

	[SerializeField]
	[Tooltip("Whether the unit is allowed to move to diagonal neighbours.")]
	private bool _preventDiagonalMoves;

	[SerializeField]
	[Tooltip("Whether the unit will navigate to the nearest possible position if the actual destination is blocked or otherwise inaccessible. Using this with the A* engine is highly inefficient!")]
	private bool _navigateToNearestIfBlocked;

	[SerializeField]
	[MinCheck(0, tooltip = "The maximum escape cell distance if the unit's starting position is blocked.")]
	private int _maxEscapeCellDistanceIfOriginBlocked = 3;

	[SerializeField]
	[MinCheck(0.1f, tooltip = "The distance from the current destination node on the path at which the unit will switch to the next node.")]
	private float _nextNodeDistance = 1f;

	[SerializeField]
	[MinCheck(0.2f, tooltip = "The distance from the current way point at which the next way point will be requested.")]
	private float _requestNextWaypointDistance = 2f;

	[SerializeField]
	[Tooltip("Whether to raise a navigation event for each node reached along a path.")]
	private bool _announceAllNodes;

	[SerializeField]
	[Tooltip("The replan mode.")]
	private ReplanMode _replanMode = ReplanMode.Dynamic;

	[SerializeField]
	[MinCheck(0.1f, tooltip = "The time between replans, the exact meaning depends on the replan mode.")]
	private float _replanInterval = 0.5f;

	public int pathingPriority
	{
		get
		{
			return _pathingPriority;
		}
		set
		{
			_pathingPriority = value;
		}
	}

	public int maxEscapeCellDistanceIfOriginBlocked
	{
		get
		{
			return _maxEscapeCellDistanceIfOriginBlocked;
		}
		set
		{
			_maxEscapeCellDistanceIfOriginBlocked = value;
		}
	}

	public bool usePathSmoothing
	{
		get
		{
			return _usePathSmoothing;
		}
		set
		{
			_usePathSmoothing = value;
		}
	}

	public bool optimizeUnobstructedPaths
	{
		get
		{
			return _optimizeUnobstructedPaths;
		}
		set
		{
			_optimizeUnobstructedPaths = value;
		}
	}

	public bool allowCornerCutting
	{
		get
		{
			return _allowCornerCutting;
		}
		set
		{
			_allowCornerCutting = value;
		}
	}

	public bool preventDiagonalMoves
	{
		get
		{
			return _preventDiagonalMoves;
		}
		set
		{
			_preventDiagonalMoves = value;
		}
	}

	public bool navigateToNearestIfBlocked
	{
		get
		{
			return _navigateToNearestIfBlocked;
		}
		set
		{
			_navigateToNearestIfBlocked = value;
		}
	}

	public float nextNodeDistance
	{
		get
		{
			return _nextNodeDistance;
		}
		set
		{
			_nextNodeDistance = value;
		}
	}

	public float requestNextWaypointDistance
	{
		get
		{
			return _requestNextWaypointDistance;
		}
		set
		{
			_requestNextWaypointDistance = value;
		}
	}

	public bool announceAllNodes
	{
		get
		{
			return _announceAllNodes;
		}
		set
		{
			_announceAllNodes = value;
		}
	}

	public ReplanMode replanMode
	{
		get
		{
			return _replanMode;
		}
		set
		{
			_replanMode = value;
		}
	}

	public float replanInterval
	{
		get
		{
			return _replanInterval;
		}
		set
		{
			_replanInterval = value;
		}
	}

	public void CloneFrom(PathOptionsComponent optionsComponent)
	{
		_pathingPriority = optionsComponent.pathingPriority;
		_usePathSmoothing = optionsComponent.usePathSmoothing;
		_optimizeUnobstructedPaths = optionsComponent.optimizeUnobstructedPaths;
		_allowCornerCutting = optionsComponent.allowCornerCutting;
		_preventDiagonalMoves = optionsComponent.preventDiagonalMoves;
		_navigateToNearestIfBlocked = optionsComponent.navigateToNearestIfBlocked;
		_maxEscapeCellDistanceIfOriginBlocked = optionsComponent.maxEscapeCellDistanceIfOriginBlocked;
		_nextNodeDistance = optionsComponent.nextNodeDistance;
		_requestNextWaypointDistance = optionsComponent.requestNextWaypointDistance;
		_announceAllNodes = optionsComponent.announceAllNodes;
		_replanMode = optionsComponent.replanMode;
		_replanInterval = optionsComponent.replanInterval;
	}
}
