using System;
using System.Collections.Generic;
using System.Linq;
using Apex.DataStructures;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Services;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Path", 1026)]
[ApexComponent("Steering")]
public class SteerForPathComponent : ArrivalBase, IMovable, INeedPath
{
	private class InternalPathRequest : BasicPathRequest
	{
		internal enum Type
		{
			Normal,
			Waypoint
		}

		internal Type pathType { get; set; }

		internal static InternalPathRequest Internalize(IPathRequest request)
		{
			InternalPathRequest internalPathRequest = request as InternalPathRequest;
			if (internalPathRequest == null)
			{
				internalPathRequest = new InternalPathRequest();
				Utils.CopyProps(request, internalPathRequest);
			}
			internalPathRequest.pathType = Type.Normal;
			return internalPathRequest;
		}
	}

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public int pathingPriority;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool usePathSmoothing = true;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool allowCornerCutting;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool preventOffGridNavigation;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool preventDiagonalMoves;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool navigateToNearestIfBlocked;

	[MinCheck(0)]
	[Obsolete("Set on the PathFinderOptions component instead.")]
	public int maxEscapeCellDistanceIfOriginBlocked = 3;

	[MinCheck(0.1f)]
	[Obsolete("Set on the PathFinderOptions component instead.")]
	public float nextNodeDistance = 1f;

	[MinCheck(0f)]
	[Obsolete("Set on the PathFinderOptions component instead.")]
	public float requestNextWaypointDistance = 2f;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public bool announceAllNodes;

	[Obsolete("Set on the PathFinderOptions component instead.")]
	public ReplanMode replanMode = ReplanMode.Dynamic;

	[MinCheck(0.1f)]
	[Obsolete("Set on the PathFinderOptions component instead.")]
	public float replanInterval = 0.5f;

	[Tooltip("Controls how the unit follows the path.\nIf set to true, the unit will insist on getting within Next Node Distance of the next node on the path as specified on the Path Options Component; otherwise it will proceed once it passes by its current next node.")]
	public bool strictPathFollowing = true;

	private readonly object _syncLock = new object();

	private IUnitFacade _unit;

	private IPathNavigationOptions _pathSettings;

	private IPathRequest _pendingPathRequest;

	private PathResult _pendingResult;

	private Path _currentPath;

	private Vector3 _endOfResolvedPath;

	private ReplanCallback _manualReplan;

	private IPositioned _currentDestination;

	private float _remainingPathDistance;

	private float _remainingTotalDistance;

	private IGrid _currentGrid;

	private Transform _transform;

	private Vector3 _curPlannedDirection;

	private bool _stop;

	private bool _blockMoveOrders;

	private bool _stopped;

	private bool _isPortaling;

	private float _lastPathRequestTime;

	private UnitNavigationEventMessage _navMessage;

	private UnitNavigationEventMessage.Event _arrivalEvent;

	private UnitNavigationEventMessage.Event _lastNavigationEvent;

	private IProcessPathResults[] _resultProcessors;

	private WaypointList _wayPoints;

	public UnitNavigationEventMessage.Event lastNavigationEvent => _lastNavigationEvent;

	public Path currentPath => _currentPath;

	public IIterable<Vector3> currentWaypoints => _wayPoints;

	public Vector3? finalDestination
	{
		get
		{
			if (_wayPoints.count > 0)
			{
				return _wayPoints.last;
			}
			if (_currentPath != null && _currentPath.count > 0)
			{
				return _currentPath.Last().position;
			}
			if (_currentDestination != null)
			{
				return _currentDestination.position;
			}
			return null;
		}
	}

	public Vector3? nextNodePosition
	{
		get
		{
			if (_currentDestination != null)
			{
				return _currentDestination.position;
			}
			return null;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.WarnIfMultipleInstances();
		_transform = transform;
		_wayPoints = new WaypointList();
		_navMessage = new UnitNavigationEventMessage(gameObject);
		_resultProcessors = GetComponents<SteerForPathResultProcessorComponent>();
		Array.Sort(_resultProcessors, (IProcessPathResults a, IProcessPathResults b) => a.processingOrder.CompareTo(b.processingOrder));
		_unit = this.GetUnitFacade();
		_pathSettings = _unit.pathNavigationOptions;
		_stopped = true;
	}

	public void MoveTo(Vector3 position, bool append)
	{
		if (!_blockMoveOrders)
		{
			if (append && !_stopped)
			{
				_wayPoints.AddWaypoint(position);
				return;
			}
			Vector3 vector = ((!_isPortaling) ? _transform.position : _currentDestination.position);
			StopInternal();
			_wayPoints.AddWaypoint(position, asCurrent: true);
			_lastNavigationEvent = UnitNavigationEventMessage.Event.None;
			RequestPath(vector, position, InternalPathRequest.Type.Normal);
		}
	}

	public void MoveTo(Vector3 position, IEnumerable<Vector3> via)
	{
		if (_blockMoveOrders)
		{
			return;
		}
		Vector3 vector = ((!_isPortaling) ? _transform.position : _currentDestination.position);
		StopInternal();
		Vector3 vector2 = position;
		if (via != null && via.Any())
		{
			vector2 = via.First();
			_wayPoints.AddWaypoint(vector2, asCurrent: true);
			foreach (Vector3 item in via.Skip(1))
			{
				_wayPoints.AddWaypoint(item);
			}
			_wayPoints.AddWaypoint(position);
		}
		else
		{
			_wayPoints.AddWaypoint(vector2, asCurrent: true);
		}
		_lastNavigationEvent = UnitNavigationEventMessage.Event.None;
		RequestPath(vector, vector2, InternalPathRequest.Type.Normal);
	}

	public void MoveAlong(Path path)
	{
		MoveAlong(path, null);
	}

	public void MoveAlong(Path path, ReplanCallback onReplan)
	{
		Ensure.ArgumentNotNull(path, "path");
		StopInternal();
		_lastNavigationEvent = UnitNavigationEventMessage.Event.None;
		SetManualPath(path, onReplan);
	}

	public void EnableMovementOrders()
	{
		_blockMoveOrders = false;
	}

	public void DisableMovementOrders()
	{
		_blockMoveOrders = true;
	}

	public override void Stop()
	{
		_lastNavigationEvent = UnitNavigationEventMessage.Event.StoppedByRequest;
		_stop = true;
	}

	public void ReplanPath()
	{
		if (!_stopped && _pendingPathRequest == null)
		{
			Path path = _currentPath;
			if (_manualReplan != null)
			{
				Path path2 = _manualReplan(gameObject, _currentDestination, path);
				_wayPoints.Clear();
				SetManualPath(path2, _manualReplan);
			}
			else
			{
				RequestPath(_transform.position, _wayPoints.desiredEndOfPath, InternalPathRequest.Type.Normal);
			}
		}
	}

	void INeedPath.ConsumePathResult(PathResult result)
	{
		lock (_syncLock)
		{
			if (!_stopped && result.originalRequest == _pendingPathRequest)
			{
				_pendingResult = result;
				_pendingPathRequest = null;
			}
		}
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (_isPortaling)
		{
			output.pause = true;
		}
		else
		{
			if (_stopped)
			{
				return;
			}
			if (_stop)
			{
				StopInternal();
			}
			else if (ResolveNextPoint())
			{
				if (_wayPoints.hasPendingWaypoint && _pendingPathRequest == null && _remainingTotalDistance < slowingDistance + _pathSettings.requestNextWaypointDistance)
				{
					RequestPath(_endOfResolvedPath, _wayPoints.NextWaypoint(), InternalPathRequest.Type.Waypoint);
				}
				HandlePathReplan();
				if (Arrive(_currentDestination.position, _remainingTotalDistance, input, output) && _pendingPathRequest == null)
				{
					_unit.hasArrivedAtDestination = true;
					StopAndAnnounceArrival();
				}
			}
		}
	}

	public void RequestPath(IPathRequest request)
	{
		IUnitFacade unitFacade = this.GetUnitFacade();
		request = InternalPathRequest.Internalize(request);
		request.timeStamp = Time.time;
		lock (_syncLock)
		{
			request.requester = this;
			request.requesterProperties = unitFacade;
			if (_pendingPathRequest != null)
			{
				_pendingPathRequest.hasDecayed = true;
			}
			_pendingPathRequest = request;
			_stop = false;
			_stopped = false;
		}
		GameServices.pathService.QueueRequest(_pendingPathRequest, unitFacade.pathFinderOptions.pathingPriority);
	}

	private void RequestPath(Vector3 from, Vector3 to, InternalPathRequest.Type type)
	{
		IUnitFacade unitFacade = this.GetUnitFacade();
		lock (_syncLock)
		{
			if (_pendingPathRequest != null)
			{
				_pendingPathRequest.hasDecayed = true;
			}
			_pendingPathRequest = new InternalPathRequest
			{
				from = from,
				to = to,
				pathType = type,
				requester = this,
				requesterProperties = unitFacade,
				pathFinderOptions = unitFacade.pathFinderOptions,
				timeStamp = Time.time
			};
			if (type == InternalPathRequest.Type.Normal)
			{
				_pendingPathRequest.via = _wayPoints.GetViaPoints();
			}
			_stop = false;
			_stopped = false;
		}
		GameServices.pathService.QueueRequest(_pendingPathRequest, unitFacade.pathFinderOptions.pathingPriority);
	}

	private void SetManualPath(Path path, ReplanCallback onReplan)
	{
		if (path == null || path.count == 0)
		{
			StopInternal();
			return;
		}
		_stop = false;
		_stopped = false;
		_arrivalEvent = UnitNavigationEventMessage.Event.DestinationReached;
		_manualReplan = onReplan;
		int num = path.count - 1;
		for (int i = 0; i < num; i++)
		{
			IPositioned positioned = path[i];
			if (positioned is Apex.PathFinding.Waypoint)
			{
				_wayPoints.AddWaypoint(positioned.position);
			}
		}
		_wayPoints.AddWaypoint(path.Last().position, asCurrent: true);
		SetPath(path, isWaypoint: false);
		_lastPathRequestTime = Time.time;
	}

	private void AnnounceEvent(UnitNavigationEventMessage.Event e, Vector3 destination, Vector3[] pendingWaypoints)
	{
		_lastNavigationEvent = e;
		_navMessage.isHandled = false;
		_navMessage.eventCode = e;
		_navMessage.destination = destination;
		_navMessage.pendingWaypoints = pendingWaypoints ?? Empty<Vector3>.array;
		GameServices.messageBus.Post(_navMessage);
	}

	private bool ResolveNextPoint()
	{
		if (_pendingResult != null)
		{
			ConsumeResult();
		}
		if (_currentPath == null)
		{
			return false;
		}
		Vector3 rhs = _transform.position.DirToXZ(_currentDestination.position);
		float magnitude = rhs.magnitude;
		_remainingTotalDistance = _remainingPathDistance + magnitude;
		if (_currentPath.count == 0)
		{
			return true;
		}
		if (magnitude > _pathSettings.nextNodeDistance && (strictPathFollowing || Vector3.Dot(_curPlannedDirection, rhs) > 0f))
		{
			return true;
		}
		IPositioned currentDestination = _currentDestination;
		_currentDestination = _currentPath.Pop();
		if (_currentDestination is IPortalNode portalNode)
		{
			_isPortaling = true;
			_currentDestination = _currentPath.Pop();
			_currentGrid = portalNode.Execute(_transform, _currentDestination, () =>
			{
				_isPortaling = false;
			});
		}
		if (currentDestination != null)
		{
			_remainingPathDistance -= (_currentDestination.position - currentDestination.position).magnitude;
			if (currentDestination is Apex.PathFinding.Waypoint)
			{
				_wayPoints.ConsumeViaPoint();
				AnnounceEvent(UnitNavigationEventMessage.Event.WaypointReached, currentDestination.position, null);
			}
			else if (_pathSettings.announceAllNodes)
			{
				AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, currentDestination.position, null);
			}
		}
		_curPlannedDirection = _transform.position.DirToXZ(_currentDestination.position);
		return !_isPortaling;
	}

	private void ConsumeResult()
	{
		PathResult pendingResult;
		lock (_syncLock)
		{
			pendingResult = _pendingResult;
			_pendingResult = null;
		}
		InternalPathRequest internalPathRequest = pendingResult.originalRequest as InternalPathRequest;
		bool flag = internalPathRequest.pathType == InternalPathRequest.Type.Waypoint;
		if (!flag)
		{
			_lastPathRequestTime = internalPathRequest.timeStamp;
		}
		if (ProcessAndValidateResult(pendingResult, flag))
		{
			SetPath(pendingResult.path, flag);
		}
	}

	private bool TrimPath(Path path)
	{
		float num = _pathSettings.nextNodeDistance;
		if (_unit.position.DirToXZ(path.Peek().position).sqrMagnitude < num * num)
		{
			return true;
		}
		int viaPointsCount = _wayPoints.viaPointsCount;
		int count = path.count;
		int num2 = 0;
		for (int num3 = count - 2; num3 >= 0; num3--)
		{
			IPositioned positioned = path[num3];
			if (positioned is Apex.PathFinding.Waypoint)
			{
				if (num2 == viaPointsCount)
				{
					path.Truncate(num3 + 1);
					break;
				}
				num2++;
			}
		}
		Bounds bounds = _currentGrid.bounds;
		CellMatrix cellMatrix = _currentGrid.cellMatrix;
		ICellCostStrategy cellCostStrategy = GameServices.cellCostStrategy;
		int num4 = -1;
		count = path.count;
		for (int i = 0; i < count; i++)
		{
			IPositioned positioned2 = path[i];
			if (positioned2 is IPortalNode || !bounds.Contains(positioned2.position))
			{
				if (num4 >= 0)
				{
					break;
				}
				continue;
			}
			if (PathSmoother.CanReducePath(positioned2, _unit, _unit, cellMatrix, cellCostStrategy))
			{
				num4 = i;
			}
			else if (num4 >= 0)
			{
				break;
			}
			if (positioned2 is Apex.PathFinding.Waypoint)
			{
				break;
			}
		}
		if (num4 < 0)
		{
			return false;
		}
		path.Truncate(num4);
		return true;
	}

	private void SetPath(Path path, bool isWaypoint)
	{
		_endOfResolvedPath = path.Last().position;
		_currentGrid = GridManager.instance.GetGrid(path.Peek().position);
		if (isWaypoint)
		{
			_remainingPathDistance += path.CalculateLength();
			_currentPath = _currentPath.AppendSegment(path);
		}
		else
		{
			if (!TrimPath(path))
			{
				RequestPath(_transform.position, _wayPoints.desiredEndOfPath, InternalPathRequest.Type.Normal);
				_currentPath = null;
				return;
			}
			_currentPath = path;
			_remainingPathDistance = path.CalculateLength();
			_currentDestination = path.Pop();
			_curPlannedDirection = _transform.position.DirToXZ(_currentDestination.position);
		}
		_unit.hasArrivedAtDestination = false;
	}

	private bool ProcessAndValidateResult(PathResult result, bool isWaypoint)
	{
		_wayPoints.frozen = false;
		for (int i = 0; i < _resultProcessors.Length; i++)
		{
			if (_resultProcessors[i].HandleResult(result, this))
			{
				return result.status == PathingStatus.Complete;
			}
		}
		PathingStatus status = result.status;
		bool flag = status == PathingStatus.CompletePartial;
		if (flag)
		{
			status = result.innerResult.status;
		}
		switch (status)
		{
		case PathingStatus.Complete:
			_arrivalEvent = UnitNavigationEventMessage.Event.DestinationReached;
			return true;
		case PathingStatus.EndOutsideGrid:
		case PathingStatus.NoRouteExists:
			_arrivalEvent = UnitNavigationEventMessage.Event.StoppedNoRouteExists;
			break;
		case PathingStatus.StartOutsideGrid:
			_arrivalEvent = UnitNavigationEventMessage.Event.StoppedUnitOutsideGrid;
			break;
		case PathingStatus.DestinationBlocked:
			_arrivalEvent = UnitNavigationEventMessage.Event.StoppedDestinationBlocked;
			break;
		case PathingStatus.Decayed:
			_arrivalEvent = UnitNavigationEventMessage.Event.StoppedRequestDecayed;
			break;
		case PathingStatus.Failed:
			StopInternal();
			Debug.LogError("Path request failed: " + result.errorInfo);
			break;
		}
		_wayPoints.frozen = true;
		if (flag)
		{
			return true;
		}
		if (!isWaypoint)
		{
			StopAndAnnounceArrival();
		}
		return false;
	}

	private void StopAndAnnounceArrival()
	{
		Vector3 destination = Vector3.zero;
		Vector3[] array = null;
		if (_arrivalEvent == UnitNavigationEventMessage.Event.DestinationReached)
		{
			destination = _currentDestination.position;
		}
		else if (_wayPoints.count > 0)
		{
			array = _wayPoints.GetPending();
			destination = array[0];
		}
		StopInternal();
		AnnounceEvent(_arrivalEvent, destination, array);
	}

	private void HandlePathReplan()
	{
		if (_currentGrid == null || _pathSettings.replanMode == ReplanMode.NoReplan)
		{
			return;
		}
		float time = Time.time;
		if (!(time - _lastPathRequestTime < _pathSettings.replanInterval))
		{
			bool flag = true;
			if (_pathSettings.replanMode == ReplanMode.Dynamic)
			{
				flag = _currentGrid.HasSectionsChangedSince(_transform.position, _lastPathRequestTime);
			}
			if (flag)
			{
				ReplanPath();
			}
		}
	}

	private void StopInternal()
	{
		lock (_syncLock)
		{
			if (_pendingPathRequest != null)
			{
				_pendingPathRequest.hasDecayed = true;
			}
			_stopped = true;
			_wayPoints.Clear();
			_currentPath = null;
			_pendingPathRequest = null;
			_currentDestination = null;
			_pendingResult = null;
			_manualReplan = null;
		}
	}

	public void CloneFrom(SteerForPathComponent steerForPath)
	{
		priority = steerForPath.priority;
		weight = steerForPath.weight;
		slowingAlgorithm = steerForPath.slowingAlgorithm;
		autoCalculateSlowingDistance = steerForPath.autoCalculateSlowingDistance;
		slowingDistance = steerForPath.slowingDistance;
		arrivalDistance = steerForPath.arrivalDistance;
		strictPathFollowing = steerForPath.strictPathFollowing;
	}
}
