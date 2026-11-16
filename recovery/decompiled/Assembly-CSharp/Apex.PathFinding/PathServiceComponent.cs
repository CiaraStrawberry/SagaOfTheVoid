using System.Linq;
using Apex.PathFinding.MoveCost;
using Apex.Services;
using Apex.Utilities;
using UnityEngine;

namespace Apex.PathFinding;

[AddComponentMenu("Apex/Game World/Path Service", 1038)]
[ApexComponent("Game World")]
public sealed class PathServiceComponent : SingleInstanceComponent<PathServiceComponent>
{
	[Tooltip("The pathing engine to use.")]
	public PathingEngineType engineType;

	[Tooltip("The algorithm used for calculating move costs.")]
	public CostProviderType moveCost;

	[MinCheck(50, tooltip = "Memory allocation optimization. You only need to change this if you get warnings that ask you to do so.")]
	public int initialHeapSize = 100;

	[Tooltip("Whether the path finding will run in a separate thread.")]
	public bool runAsync = true;

	[Tooltip("Whether to use a thread pool if available instead of a dedicated thread. The recommendation is to use a dedicated thread.")]
	public bool useThreadPoolForAsyncOperations;

	[MinCheck(1, tooltip = "The maximum number of milliseconds to user for path finding per frame when the path finder runs in the main thread.")]
	public int maxMillisecondsPerFrame = 5;

	private PathService _pathService;

	protected sealed override void OnAwake()
	{
		IPathSmootherFactory pathSmootherFactory = this.As<IPathSmootherFactory>();
		ISmoothPaths pathSmoother = ((pathSmootherFactory != null) ? pathSmootherFactory.CreateSmoother() : new PathSmoother());
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
		IPathingEngine engine = ((engineType != PathingEngineType.Astar) ? new PathingJumpPointSearch(initialHeapSize, moveCostProvider, GameServices.cellCostStrategy, pathSmoother, requestPreprocessors) : new PathingAStar(initialHeapSize, moveCostProvider, GameServices.cellCostStrategy, pathSmoother, requestPreprocessors));
		_pathService = new PathService(engine, new ThreadFactory(), useThreadPoolForAsyncOperations);
		_pathService.runAsync = runAsync;
		GameServices.pathService = _pathService;
	}

	private void Start()
	{
		_pathService.OnAsyncFailed = OnAsyncFailed;
		if (!runAsync)
		{
			StartCoroutine(_pathService.ProcessRequests(maxMillisecondsPerFrame));
		}
	}

	protected sealed override void OnDestroy()
	{
		if (_pathService != null)
		{
			_pathService.OnAsyncFailed = null;
			_pathService.Dispose();
		}
		base.OnDestroy();
	}

	private void OnAsyncFailed()
	{
		runAsync = false;
		_pathService.runAsync = false;
		StartCoroutine(_pathService.ProcessRequests(maxMillisecondsPerFrame));
	}
}
