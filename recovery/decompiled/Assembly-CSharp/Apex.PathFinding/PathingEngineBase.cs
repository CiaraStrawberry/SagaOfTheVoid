using System;
using System.Collections;
using Apex.DataStructures;
using Apex.PathFinding.MoveCost;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public abstract class PathingEngineBase : IPathingEngine
{
	private class SegmentRequest : IPathRequest
	{
		private IPathRequest _actualRequest;

		private Vector3 _actualFrom;

		private Vector3 _actualTo;

		private int _viaIdx;

		public Vector3 from
		{
			get
			{
				return _actualFrom;
			}
			set
			{
				_actualFrom = value;
			}
		}

		public Vector3 to
		{
			get
			{
				return _actualTo;
			}
			set
			{
				_actualTo = value;
			}
		}

		public Vector3[] via
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public bool isRunning => _actualRequest != null;

		public INeedPath requester
		{
			get
			{
				return _actualRequest.requester;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public IUnitProperties requesterProperties
		{
			get
			{
				return _actualRequest.requesterProperties;
			}
			set
			{
				_actualRequest.requesterProperties = value;
			}
		}

		public IPathFinderOptions pathFinderOptions
		{
			get
			{
				return _actualRequest.pathFinderOptions;
			}
			set
			{
				_actualRequest.pathFinderOptions = value;
			}
		}

		public RequestType type
		{
			get
			{
				return _actualRequest.type;
			}
			set
			{
				_actualRequest.type = value;
			}
		}

		public float timeStamp { get; set; }

		public bool isValid => _actualRequest.isValid;

		public bool hasDecayed
		{
			get
			{
				return _actualRequest.hasDecayed;
			}
			set
			{
				_actualRequest.hasDecayed = value;
			}
		}

		public object customData
		{
			get
			{
				return _actualRequest.customData;
			}
			set
			{
				_actualRequest.customData = value;
			}
		}

		public void Complete(PathResult result)
		{
			throw new InvalidOperationException();
		}

		public void Start(IPathRequest req)
		{
			_actualRequest = req;
			_actualFrom = req.from;
			if (req.via != null && req.via.Length > 0)
			{
				_viaIdx = 0;
				_actualTo = req.via[0];
			}
			else
			{
				_viaIdx = -1;
				_actualTo = req.to;
			}
		}

		public bool MoveNext()
		{
			if (_viaIdx < 0)
			{
				_actualRequest.to = _actualTo;
				return false;
			}
			Vector3[] array = _actualRequest.via;
			ref Vector3 reference = ref array[_viaIdx++];
			reference = _actualTo;
			_actualFrom = _actualTo;
			if (_viaIdx == array.Length)
			{
				_actualTo = _actualRequest.to;
				_viaIdx = -1;
			}
			else
			{
				_actualTo = array[_viaIdx];
			}
			return true;
		}

		public Vector3[] GetPendingWaypoints()
		{
			if (_viaIdx < 0)
			{
				return new Vector3[1] { _actualRequest.to };
			}
			Vector3[] array = _actualRequest.via;
			int num = array.Length - _viaIdx;
			Vector3[] array2 = new Vector3[num + 1];
			Array.Copy(array, _viaIdx, array2, 0, num);
			ref Vector3 reference = ref array2[num];
			reference = _actualRequest.to;
			return array2;
		}

		public void Clear()
		{
			_actualRequest = null;
		}
	}

	private class SafeIterator : IEnumerator
	{
		private PathingEngineBase _engine;

		private IEnumerator _innerIter;

		private IPathRequest _request;

		public object Current => _innerIter.Current;

		public SafeIterator(PathingEngineBase engine)
		{
			_engine = engine;
		}

		public bool MoveNext()
		{
			try
			{
				return _innerIter.MoveNext();
			}
			catch (Exception e)
			{
				_engine.FailRequest(_request, e);
			}
			return false;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public void Init(IPathRequest request)
		{
			_request = request;
			_innerIter = _engine.ProcessRequestCoroutineInternal(request);
		}
	}

	private readonly ICellCostStrategy _cellCostStrategy;

	private readonly IMoveCost _costProvider;

	private readonly ISmoothPaths _smoother;

	private IPathNode _goal;

	private PathResult _currentResult;

	private SafeIterator _coroutineIter;

	private SegmentRequest _segmentRequest;

	private DynamicArray<Path> _segments;

	private IRequestPreProcessor[] _requestPreprocessors;

	protected IMoveCost costProvider => _costProvider;

	protected ICellCostStrategy cellCostStrategy => _cellCostStrategy;

	protected ISmoothPaths pathSmoother => _smoother;

	protected IPathRequest currentRequest => _segmentRequest;

	protected IPathNode goal => _goal;

	protected PathingEngineBase(IMoveCost moveCostProvider, ICellCostStrategy cellCostStrategy, ISmoothPaths smoother, IRequestPreProcessor[] requestPreprocessors)
	{
		Ensure.ArgumentNotNull(moveCostProvider, "moveCostProvider");
		Ensure.ArgumentNotNull(cellCostStrategy, "cellCostStrategy");
		Ensure.ArgumentNotNull(smoother, "smoother");
		_costProvider = moveCostProvider;
		_smoother = smoother;
		_cellCostStrategy = cellCostStrategy;
		_coroutineIter = new SafeIterator(this);
		_segmentRequest = new SegmentRequest();
		_segments = new DynamicArray<Path>(10);
		_requestPreprocessors = requestPreprocessors;
	}

	public void ProcessRequest(IPathRequest request)
	{
		try
		{
			ValidateRequest(request);
			_currentResult = new PathResult(PathingStatus.Complete, null, 0, request);
			PathingStatus pathingStatus = PathingStatus.Complete;
			_segmentRequest.Start(request);
			do
			{
				pathingStatus = StartPathSegment();
				if (pathingStatus != PathingStatus.Running)
				{
					break;
				}
				while (pathingStatus == PathingStatus.Running)
				{
					pathingStatus = ProcessNext();
				}
			}
			while (CompletePathSegment(pathingStatus) && _segmentRequest.MoveNext());
			if (pathingStatus != PathingStatus.Complete)
			{
				if (_segments.count > 0)
				{
					_currentResult.RegisterPartialResult(pathingStatus, _segmentRequest.GetPendingWaypoints());
				}
				else
				{
					_currentResult.status = pathingStatus;
				}
			}
			_currentResult.path = Path.FromSegments(_segments);
			request.Complete(_currentResult);
		}
		catch (Exception e)
		{
			FailRequest(request, e);
		}
		finally
		{
			_segments.Clear();
			_segmentRequest.Clear();
			_currentResult = null;
		}
	}

	public IEnumerator ProcessRequestCoroutine(IPathRequest request)
	{
		_coroutineIter.Init(request);
		return _coroutineIter;
	}

	protected void UpdateGoal(IPathNode newGoal)
	{
		_goal = newGoal;
		_segmentRequest.to = newGoal.position;
	}

	protected virtual void ValidateRequest(IPathRequest request)
	{
		Ensure.ArgumentNotNull(request, "request");
		if (!request.isValid)
		{
			throw new ArgumentException("The request is invalid.", "request");
		}
		if (_segmentRequest.isRunning)
		{
			throw new InvalidOperationException("A new request cannot be started while another request is being processed.");
		}
	}

	private IEnumerator ProcessRequestCoroutineInternal(IPathRequest request)
	{
		ValidateRequest(request);
		_currentResult = new PathResult(PathingStatus.Complete, null, 0, request);
		PathingStatus status = PathingStatus.Complete;
		_segmentRequest.Start(request);
		do
		{
			status = StartPathSegment();
			if (status != PathingStatus.Running)
			{
				break;
			}
			while (status == PathingStatus.Running)
			{
				status = ProcessNext();
				yield return null;
			}
		}
		while (CompletePathSegment(status) && _segmentRequest.MoveNext());
		if (status != PathingStatus.Complete)
		{
			if (_segments.count > 0)
			{
				_currentResult.RegisterPartialResult(status, _segmentRequest.GetPendingWaypoints());
			}
			else
			{
				_currentResult.status = status;
			}
		}
		_currentResult.path = Path.FromSegments(_segments);
		request.Complete(_currentResult);
		_segments.Clear();
		_segmentRequest.Clear();
		_currentResult = null;
	}

	private PathingStatus StartPathSegment()
	{
		if (_requestPreprocessors != null)
		{
			for (int i = 0; i < _requestPreprocessors.Length && !_requestPreprocessors[i].PreProcess(_segmentRequest); i++)
			{
			}
		}
		IGrid grid = GridManager.instance.GetGrid(_segmentRequest.from);
		IGrid grid2 = GridManager.instance.GetGrid(_segmentRequest.to);
		Position position = new Position(_segmentRequest.from);
		Position position2 = new Position(_segmentRequest.to);
		if (grid == null && grid2 == null)
		{
			Path path = new Path(2);
			path.Push(position2);
			path.Push(position);
			_segments.Add(path);
			return PathingStatus.Complete;
		}
		if (grid == null)
		{
			return PathingStatus.StartOutsideGrid;
		}
		IPathFinderOptions pathFinderOptions = _segmentRequest.pathFinderOptions;
		if (pathFinderOptions.optimizeUnobstructedPaths && pathFinderOptions.usePathSmoothing && grid == grid2 && !pathFinderOptions.preventDiagonalMoves && PathSmoother.CanReducePath(position, position2, _segmentRequest.requesterProperties, grid.cellMatrix, _cellCostStrategy))
		{
			Path path2 = new Path(2);
			path2.Push(position2);
			path2.Push(position);
			_segments.Add(path2);
			return PathingStatus.Complete;
		}
		if (grid2 == null)
		{
			grid2 = grid;
		}
		IPathNode pathNode = grid.GetCell(_segmentRequest.from, adjustToBounds: false);
		_goal = grid2.GetCell(_segmentRequest.to, adjustToBounds: false);
		if (_goal == null)
		{
			if (!pathFinderOptions.navigateToNearestIfBlocked)
			{
				return PathingStatus.EndOutsideGrid;
			}
			_goal = grid2.GetCell(_segmentRequest.to, adjustToBounds: true);
			_segmentRequest.to = _goal.position;
		}
		if (!_goal.IsWalkableWithClearance(_segmentRequest.requesterProperties) && !pathFinderOptions.navigateToNearestIfBlocked)
		{
			return PathingStatus.DestinationBlocked;
		}
		IPathNode actualStart = null;
		if (!pathNode.IsWalkableWithClearance(_segmentRequest.requesterProperties))
		{
			actualStart = pathNode;
			pathNode = grid.GetNearestWalkableCell(pathNode.position, _goal.position, requireWalkableFromPosition: true, pathFinderOptions.maxEscapeCellDistanceIfOriginBlocked, _segmentRequest.requesterProperties);
			if (pathNode == null)
			{
				return PathingStatus.NoRouteExists;
			}
		}
		OnStart(pathNode, actualStart);
		return PathingStatus.Running;
	}

	private bool CompletePathSegment(PathingStatus status)
	{
		if (status != PathingStatus.Complete)
		{
			return false;
		}
		_currentResult.pathCost += _goal.g;
		FixupGoal(_segmentRequest);
		if (_segmentRequest.type == RequestType.IntelOnly)
		{
			return true;
		}
		int num = Mathf.CeilToInt((float)_goal.g / (_goal.parent.cellSize * (float)_costProvider.baseMoveCost));
		Path path;
		if (_segmentRequest.pathFinderOptions.usePathSmoothing)
		{
			path = _smoother.Smooth(_goal, num, _segmentRequest, _cellCostStrategy);
		}
		else
		{
			path = new Path(num);
			path.Push(new Position(_segmentRequest.to));
			for (IPathNode predecessor = _goal.predecessor; predecessor != null; predecessor = predecessor.predecessor)
			{
				path.Push(predecessor);
			}
			if (path.count > 1)
			{
				path.Pop();
			}
			path.Push(new Position(_segmentRequest.from));
		}
		_segments.Add(path);
		return true;
	}

	private void FailRequest(IPathRequest request, Exception e)
	{
		_segments.Clear();
		_segmentRequest.Clear();
		_currentResult = null;
		PathResult pathResult = new PathResult(PathingStatus.Failed, null, 0, request);
		pathResult.errorInfo = e.Message + Environment.NewLine + e.StackTrace;
		PathResult result = pathResult;
		request.Complete(result);
	}

	private void FixupGoal(IPathRequest request)
	{
		IUnitProperties requesterProperties = request.requesterProperties;
		float radius = request.requesterProperties.radius;
		Vector3 to = request.to;
		float num = _goal.parent.cellSize * 0.5f;
		float num2 = to.x - _goal.position.x;
		float num3 = to.z - _goal.position.z;
		float num4 = radius - num2 - num;
		float num5 = num2 + radius - num;
		float num6 = num3 + radius - num;
		float num7 = radius - num3 - num;
		float num8 = 0f;
		float num9 = 0f;
		if (num4 > 0f && !ValidateGoalNeighbour(requesterProperties, -1, 0))
		{
			num8 = 0f - num4;
		}
		else if (num5 > 0f && !ValidateGoalNeighbour(requesterProperties, 1, 0))
		{
			num8 = num5;
		}
		if (num6 > 0f && !ValidateGoalNeighbour(requesterProperties, 0, 1))
		{
			num9 = num6;
		}
		else if (num7 > 0f && !ValidateGoalNeighbour(requesterProperties, 0, -1))
		{
			num9 = 0f - num7;
		}
		if (num8 == 0f && num9 == 0f)
		{
			if (num4 > 0f && num6 > 0f && !ValidateGoalNeighbour(requesterProperties, -1, 1))
			{
				num8 = 0f - num4;
				num9 = num6;
			}
			else if (num4 > 0f && num7 > 0f && !ValidateGoalNeighbour(requesterProperties, -1, -1))
			{
				num8 = 0f - num4;
				num9 = 0f - num7;
			}
			else if (num5 > 0f && num6 > 0f && !ValidateGoalNeighbour(requesterProperties, 1, 1))
			{
				num8 = num5;
				num9 = num6;
			}
			else if (num5 > 0f && num7 > 0f && !ValidateGoalNeighbour(requesterProperties, 1, -1))
			{
				num8 = num5;
				num9 = 0f - num7;
			}
		}
		if (num8 != 0f || num9 != 0f)
		{
			request.to = new Vector3(to.x - num8, to.y, to.z - num9);
		}
	}

	private bool ValidateGoalNeighbour(IUnitProperties unitProps, int dx, int dz)
	{
		return _goal.GetNeighbour(dx, dz)?.IsWalkableFrom(_goal, unitProps) ?? true;
	}

	protected abstract PathingStatus ProcessNext();

	protected virtual void OnStart(IPathNode start, IPathNode actualStart)
	{
	}
}
