using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Debugging;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Services;
using Apex.Steering;
using Apex.Steering.Components;
using Apex.Steering.HeightNavigation;
using Apex.Steering.VectorFields;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public class DefaultSteeringTransientUnitGroup : DefaultTransientUnitGroup, ILoadBalanced, INeedPath
{
	private struct FormationPositionWithIndex
	{
		internal IPositioned position;

		internal int index;

		internal FormationPositionWithIndex(Vector3 pos, int idx)
		{
			position = new Position(pos);
			index = idx;
		}
	}

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

	private ReplanMode _replanMode;

	private float _replanInterval;

	private float _nextNodeDistance;

	private float _requestNextWaypointDistance;

	private IPathRequest _pendingPathRequest;

	private PathResult _pendingResult;

	private Path _currentPath;

	private SimpleQueue<Vector3> _wayPoints;

	private UnitNavigationEventMessage _navMessage;

	private ReplanCallback _manualReplan;

	private float _lastPathRequestTime;

	private IGrid _currentGrid;

	private IVectorField _vectorField;

	private Vector3 _groupCog;

	private bool _blockMoveOrders;

	private bool _wait;

	private bool _stopped;

	private bool _announcedEndOnce;

	private IUnitFacade _modelUnit;

	private List<FormationPositionWithIndex> _tempFormationPositions;

	private DynamicArray<Vector3> _formationPositions;

	private DistanceComparer<IUnitFacade> _unitSortComparer;

	private IFormation _currentFormation;

	public override Vector3 centerOfGravity => _groupCog;

	public IVectorField vectorField => _vectorField;

	public override Path currentPath => _currentPath;

	internal DefaultSteeringTransientUnitGroup nextGroup { get; set; }

	public override IIterable<Vector3> currentWaypoints => _wayPoints;

	public override Vector3? finalDestination
	{
		get
		{
			if (_wayPoints != null && _wayPoints.count > 0)
			{
				return _wayPoints.Last();
			}
			if (_currentPath != null && _currentPath.count > 0)
			{
				return _currentPath.Last().position;
			}
			if (_vectorField != null && _vectorField.nextNodePosition != null)
			{
				return _vectorField.nextNodePosition.position;
			}
			return null;
		}
	}

	public override Vector3? nextNodePosition
	{
		get
		{
			if (_vectorField == null || _vectorField.nextNodePosition == null)
			{
				return null;
			}
			return _vectorField.nextNodePosition.position;
		}
	}

	public virtual bool repeat => true;

	public override IUnitFacade modelUnit
	{
		get
		{
			if (count == 0)
			{
				return null;
			}
			return _modelUnit;
		}
	}

	public override IFormation currentFormation => _currentFormation;

	public DefaultSteeringTransientUnitGroup(int capacity)
		: base(capacity)
	{
		Initialize();
	}

	public DefaultSteeringTransientUnitGroup(IUnitFacade[] members)
		: base(members)
	{
		Initialize();
	}

	public DefaultSteeringTransientUnitGroup(IEnumerable<IUnitFacade> members)
		: base(members)
	{
		Initialize();
	}

	private void Initialize()
	{
		_wayPoints = new SimpleQueue<Vector3>();
		_groupCog = GetGroupCenterOfGravity();
		_unitSortComparer = new DistanceComparer<IUnitFacade>(ascending: false);
		_stopped = true;
		float groupUpdateInterval = GameServices.navigationSettings.groupUpdateInterval;
		NavLoadBalancer.steering.Add(this, groupUpdateInterval, delayFirstUpdate: true);
	}

	private void InitializeModelUnitSettings()
	{
		IPathNavigationOptions pathNavigationOptions = ((_modelUnit == null) ? base[0].pathNavigationOptions : _modelUnit.pathNavigationOptions);
		_replanMode = pathNavigationOptions.replanMode;
		_replanInterval = pathNavigationOptions.replanInterval;
		_nextNodeDistance = pathNavigationOptions.nextNodeDistance;
		_requestNextWaypointDistance = pathNavigationOptions.requestNextWaypointDistance;
		_navMessage = new UnitNavigationEventMessage((_modelUnit == null) ? base[0].gameObject : _modelUnit.gameObject);
	}

	protected override void MoveToInternal(Vector3 position, bool append)
	{
		if (!_blockMoveOrders)
		{
			if (append && !_stopped)
			{
				_wayPoints.Enqueue(position);
				return;
			}
			StopInternal();
			RequestPath(position, InternalPathRequest.Type.Normal);
		}
	}

	protected override void MoveAlongInternal(Path path, ReplanCallback onReplan)
	{
		if (!_blockMoveOrders)
		{
			Ensure.ArgumentNotNull(path, "path");
			StopInternal();
			SetManualPath(path, onReplan);
		}
	}

	public override void Wait(float? seconds)
	{
		_wait = true;
		if (seconds.HasValue)
		{
			LoadBalancer.defaultBalancer.Add(new OneTimeAction((float ignored) =>
			{
				Resume();
			}), seconds.Value, delayFirstUpdate: true);
		}
		if (modelUnit != null)
		{
			modelUnit.Wait(null);
		}
		int num = count;
		for (int num2 = 0; num2 < num; num2++)
		{
			base[num2].Wait(null);
		}
	}

	public override void Resume()
	{
		_wait = false;
		if (modelUnit != null)
		{
			modelUnit.Resume();
		}
		int num = count;
		for (int i = 0; i < num; i++)
		{
			base[i].Resume();
		}
	}

	public override void EnableMovementOrders()
	{
		_blockMoveOrders = false;
	}

	public override void DisableMovementOrders()
	{
		_blockMoveOrders = true;
	}

	public override void Stop()
	{
		StopInternal();
		if (modelUnit != null)
		{
			modelUnit.Stop();
		}
		int num = count;
		for (int i = 0; i < num; i++)
		{
			base[i].Stop();
		}
	}

	public void RequestPath(IPathRequest request)
	{
		if (count != 0)
		{
			request = InternalPathRequest.Internalize(request);
			request.requester = this;
			request.requesterProperties = _modelUnit;
			_pendingPathRequest = request;
			_stopped = false;
			_lastPathRequestTime = Time.time;
			GameServices.pathService.QueueRequest(_pendingPathRequest, _modelUnit.pathFinderOptions.pathingPriority);
		}
	}

	private void RequestPath(Vector3 to, InternalPathRequest.Type type)
	{
		if (count == 0)
		{
			return;
		}
		Vector3 position = _modelUnit.position;
		IGrid grid = GridManager.instance.GetGrid(position);
		Cell cell = grid.GetCell(position, adjustToBounds: true);
		if (cell == null || !cell.IsWalkableWithClearance(_modelUnit))
		{
			cell = grid.GetNearestWalkableCell(position, position, requireWalkableFromPosition: false, _modelUnit.pathFinderOptions.maxEscapeCellDistanceIfOriginBlocked, _modelUnit);
			if (cell != null)
			{
				position = cell.position;
			}
		}
		_pendingPathRequest = new InternalPathRequest
		{
			from = position,
			to = to,
			pathType = type,
			requester = this,
			requesterProperties = _modelUnit,
			pathFinderOptions = _modelUnit.pathFinderOptions
		};
		_stopped = false;
		_lastPathRequestTime = Time.time;
		GameServices.pathService.QueueRequest(_pendingPathRequest, _modelUnit.pathFinderOptions.pathingPriority);
	}

	void INeedPath.ConsumePathResult(PathResult result)
	{
		LoadBalancer.marshaller.ExecuteOnMainThread(() =>
		{
			if (result.originalRequest == _pendingPathRequest)
			{
				_pendingResult = result;
				_pendingPathRequest = null;
			}
		});
	}

	private void ConsumePath()
	{
		PathResult pendingResult = _pendingResult;
		_pendingResult = null;
		InternalPathRequest internalPathRequest = pendingResult.originalRequest as InternalPathRequest;
		if (internalPathRequest.pathType == InternalPathRequest.Type.Waypoint)
		{
			AnnounceEvent(UnitNavigationEventMessage.Event.WaypointReached, _wayPoints.Dequeue(), null);
		}
		if (ProcessAndValidateResult(pendingResult))
		{
			_currentPath = pendingResult.path;
			if (_currentFormation != null)
			{
				_modelUnit.transform.LookAt(_currentPath[1].position);
				SetFormation(_currentFormation);
			}
			SetVectorFieldPath();
		}
	}

	private void SetManualPath(Path path, ReplanCallback onReplan)
	{
		if (count != 0 && _modelUnit != null)
		{
			if (path == null || path.count == 0)
			{
				StopInternal();
				return;
			}
			_manualReplan = onReplan;
			_currentPath = path;
			_lastPathRequestTime = Time.time;
			SetVectorFieldPath();
		}
	}

	public void ReplanPath()
	{
		if (_pendingResult == null && _pendingPathRequest == null && _currentPath != null && _currentPath.count != 0)
		{
			Path path = _currentPath;
			_currentPath = null;
			IPositioned positioned = path.Last();
			if (_manualReplan != null)
			{
				Path path2 = _manualReplan(_modelUnit.gameObject, positioned, path);
				SetManualPath(path2, _manualReplan);
			}
			else
			{
				RequestPath(positioned.position, InternalPathRequest.Type.Normal);
			}
		}
	}

	private void HandlePathReplan()
	{
		if (_pendingResult != null || _pendingPathRequest != null || _currentGrid == null || _replanMode == ReplanMode.NoReplan)
		{
			return;
		}
		float time = Time.time;
		if (!(time - _lastPathRequestTime < _replanInterval))
		{
			bool flag = true;
			if (_replanMode == ReplanMode.Dynamic)
			{
				flag = _currentGrid.HasSectionsChangedSince(_modelUnit.position, _lastPathRequestTime);
			}
			if (flag)
			{
				ReplanPath();
			}
		}
	}

	private void SetVectorFieldPath()
	{
		_stopped = false;
		hasArrived = false;
		_announcedEndOnce = false;
		if (_vectorField != null)
		{
			_vectorField.SetNewPath(_currentPath);
		}
		else
		{
			_vectorField = GameServices.vectorFieldManager.CreateVectorField(this, _currentPath);
		}
	}

	public override void Add(IUnitFacade member)
	{
		base.Add(member);
		if (_modelUnit == null)
		{
			CreateModelUnit();
			InitializeModelUnitSettings();
			_groupCog = member.position;
			_currentGrid = GridManager.instance.GetGrid(_groupCog);
		}
	}

	public override void Remove(IUnitFacade member)
	{
		base.Remove(member);
		if (count == 0)
		{
			CleanupGroup();
		}
		else if (_currentFormation != null && _modelUnit.isAlive)
		{
			SetFormation(_currentFormation);
		}
	}

	private bool ProcessAndValidateResult(PathResult result)
	{
		UnitNavigationEventMessage.Event obj = UnitNavigationEventMessage.Event.None;
		switch (result.status)
		{
		case PathingStatus.Complete:
		case PathingStatus.CompletePartial:
			return true;
		case PathingStatus.EndOutsideGrid:
		case PathingStatus.NoRouteExists:
			obj = UnitNavigationEventMessage.Event.StoppedNoRouteExists;
			break;
		case PathingStatus.StartOutsideGrid:
			obj = UnitNavigationEventMessage.Event.StoppedUnitOutsideGrid;
			break;
		case PathingStatus.DestinationBlocked:
			obj = UnitNavigationEventMessage.Event.StoppedDestinationBlocked;
			break;
		case PathingStatus.Decayed:
			obj = UnitNavigationEventMessage.Event.StoppedRequestDecayed;
			break;
		case PathingStatus.Failed:
			Debug.LogError("Path request failed: " + result.errorInfo);
			break;
		}
		Vector3 to = result.originalRequest.to;
		Vector3[] pendingWaypoints = ((_wayPoints.count <= 0) ? Empty<Vector3>.array : _wayPoints.ToArray());
		Stop();
		if (obj != UnitNavigationEventMessage.Event.None)
		{
			AnnounceEvent(obj, to, pendingWaypoints);
		}
		return false;
	}

	public void AnnounceEvent(UnitNavigationEventMessage.Event e, Vector3 destination, Vector3[] pendingWaypoints)
	{
		_navMessage.isHandled = false;
		_navMessage.eventCode = e;
		_navMessage.destination = destination;
		_navMessage.pendingWaypoints = pendingWaypoints ?? Empty<Vector3>.array;
		GameServices.messageBus.Post(_navMessage);
	}

	private void StopInternal()
	{
		_stopped = true;
		_wayPoints.Clear();
		_currentPath = null;
		_pendingPathRequest = null;
		_pendingResult = null;
		_manualReplan = null;
	}

	private void CleanupGroup()
	{
		_vectorField = null;
		StopInternal();
		NavLoadBalancer.steering.Remove(this);
		if (_modelUnit != null)
		{
			_modelUnit.transientGroup = null;
			Object.Destroy(_modelUnit.gameObject, 0.1f);
		}
	}

	public void SetWaypoints(IIterable<Vector3> waypoints)
	{
		int num = waypoints.count;
		for (int i = 0; i < num; i++)
		{
			_wayPoints.Enqueue(waypoints[i]);
		}
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if (_modelUnit == null && count > 0)
		{
			CreateModelUnit();
			InitializeModelUnitSettings();
		}
	}

	private void CreateModelUnit()
	{
		GameObject gameObject = base[0].gameObject;
		GameObject gameObject2 = new GameObject("Model Unit");
		gameObject2.SetActive(value: false);
		SphereCollider sphereCollider = gameObject2.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		SteerableUnitComponent component = gameObject.GetComponent<SteerableUnitComponent>();
		if (component != null && component.enabled)
		{
			SteerableUnitComponent steerableUnitComponent = gameObject2.AddComponent<SteerableUnitComponent>();
			steerableUnitComponent.CloneFrom(component);
		}
		IHeightNavigator heightNavigator = gameObject.As<IHeightNavigator>();
		if (heightNavigator != null)
		{
			IHeightNavigator heightNavigator2 = gameObject2.AddComponent(heightNavigator.GetType()) as IHeightNavigator;
			heightNavigator2.CloneFrom(heightNavigator);
		}
		IDefineSpeed defineSpeed = gameObject.As<IDefineSpeed>();
		if (defineSpeed != null)
		{
			IDefineSpeed defineSpeed2 = gameObject2.AddComponent(defineSpeed.GetType()) as IDefineSpeed;
			defineSpeed2.CloneFrom(defineSpeed);
		}
		PathOptionsComponent component2 = gameObject.GetComponent<PathOptionsComponent>();
		if (component2 != null && component2.enabled)
		{
			PathOptionsComponent pathOptionsComponent = gameObject2.AddComponent<PathOptionsComponent>();
			pathOptionsComponent.CloneFrom(component2);
		}
		SteerToAlignWithVelocity component3 = gameObject.GetComponent<SteerToAlignWithVelocity>();
		if (component3 != null && component3.enabled)
		{
			SteerToAlignWithVelocity steerToAlignWithVelocity = gameObject2.AddComponent<SteerToAlignWithVelocity>();
			steerToAlignWithVelocity.CloneFrom(component3);
		}
		PathVisualizer component4 = gameObject.GetComponent<PathVisualizer>();
		if (component4 != null && component4.enabled)
		{
			PathVisualizer pathVisualizer = gameObject2.AddComponent<PathVisualizer>();
			pathVisualizer.CloneFrom(component4);
		}
		SteerForPathComponent component5 = gameObject.GetComponent<SteerForPathComponent>();
		if (component5 != null && component5.enabled)
		{
			SteerForPathComponent steerForPathComponent = gameObject2.AddComponent<SteerForPathComponent>();
			steerForPathComponent.CloneFrom(component5);
		}
		UnitComponent component6 = gameObject.GetComponent<UnitComponent>();
		if (component6 != null && component6.enabled)
		{
			UnitComponent unitComponent = gameObject2.AddComponent<UnitComponent>();
			unitComponent.CloneFrom(component6);
			unitComponent.isSelectable = false;
			unitComponent.selectionVisual = null;
			unitComponent.enabled = true;
		}
		SteerForVectorFieldComponent component7 = gameObject.GetComponent<SteerForVectorFieldComponent>();
		if (component7 != null && component7.enabled)
		{
			SteerForVectorFieldComponent steerForVectorFieldComponent = gameObject2.AddComponent<SteerForVectorFieldComponent>();
			steerForVectorFieldComponent.CloneFrom(component7);
			steerForVectorFieldComponent.arrivalRadiusMargin = 0f;
			steerForVectorFieldComponent.arrivalDistance = 0.2f;
		}
		SteeringController component8 = gameObject.GetComponent<SteeringController>();
		if (component8 != null && component8.enabled)
		{
			gameObject2.AddComponent<SteeringController>();
		}
		ApexComponentMaster component9 = gameObject.GetComponent<ApexComponentMaster>();
		if (component9 != null && component9.enabled)
		{
			gameObject2.AddComponent<ApexComponentMaster>();
		}
		gameObject2.SetActive(value: true);
		_modelUnit = gameObject2.GetUnitFacade();
		_modelUnit.transientGroup = this;
		if (nextNodePosition.HasValue)
		{
			Vector3 position = Vector3.zero;
			float num = float.MinValue;
			for (int i = 0; i < count; i++)
			{
				Vector3 position2 = base[i].position;
				float sqrMagnitude = (position2 - nextNodePosition.Value).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					position = position2;
				}
			}
			_modelUnit.transform.position = position;
		}
		else
		{
			_modelUnit.transform.position = GetGroupCenterOfGravity();
		}
		_modelUnit.transform.forward = base[0].forward;
	}

	public void TransferFormation(IFormation formation, int expectedCount, DefaultSteeringTransientUnitGroup oldGroup)
	{
		if (count != 0 && expectedCount != 0 && formation != null && oldGroup != null)
		{
			if (_formationPositions == null)
			{
				_formationPositions = new DynamicArray<Vector3>(expectedCount);
			}
			else
			{
				_formationPositions.Clear();
			}
			_currentFormation = formation;
			for (int i = 0; i < expectedCount; i++)
			{
				_formationPositions.Add(formation.GetFormationPosition(expectedCount, i, oldGroup[i]));
			}
		}
	}

	public override void SetFormation(IFormation formation)
	{
		if (count == 0)
		{
			return;
		}
		if (_formationPositions == null)
		{
			_formationPositions = new DynamicArray<Vector3>(count);
		}
		else
		{
			_formationPositions.Clear();
		}
		if (formation == null)
		{
			_currentFormation = null;
			return;
		}
		_currentFormation = formation;
		PrepareForAction();
		for (int i = 0; i < count; i++)
		{
			base[i].hasArrivedAtDestination = false;
			_formationPositions.Add(formation.GetFormationPosition(count, i, base[i]));
		}
		SetupFormationIndices();
	}

	protected virtual void SetupFormationIndices()
	{
		if (_formationPositions == null || _formationPositions.count == 0)
		{
			return;
		}
		if (_tempFormationPositions == null)
		{
			_tempFormationPositions = new List<FormationPositionWithIndex>(count);
		}
		int num = 0;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			IPositioned formationPosition = GetFormationPosition(i);
			if (formationPosition != null)
			{
				_tempFormationPositions.Add(new FormationPositionWithIndex(formationPosition.position, num));
				num++;
				zero += formationPosition.position;
			}
		}
		zero /= (float)num;
		_unitSortComparer.compareTo = zero;
		Sort(_unitSortComparer);
		for (int j = 0; j < count; j++)
		{
			IUnitFacade unitFacade = base[j];
			if (unitFacade == null)
			{
				continue;
			}
			Vector3 position = unitFacade.position;
			float num2 = float.MaxValue;
			int index = -1;
			int formationIndex = -1;
			int num3 = _tempFormationPositions.Count;
			for (int k = 0; k < num3; k++)
			{
				IPositioned position2 = _tempFormationPositions[k].position;
				float sqrMagnitude = (position2.position - position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					index = k;
					formationIndex = _tempFormationPositions[k].index;
				}
			}
			_tempFormationPositions.RemoveAt(index);
			unitFacade.formationIndex = formationIndex;
		}
		_tempFormationPositions.Clear();
	}

	public IPositioned GetFormationPosition(int formationIndex)
	{
		if (_formationPositions == null || _formationPositions.count == 0 || _modelUnit == null || _modelUnit.transform == null)
		{
			return null;
		}
		if (formationIndex < 0 || formationIndex >= _formationPositions.count)
		{
			return null;
		}
		Vector3 direction = _formationPositions[formationIndex];
		return new Position(_modelUnit.position + _modelUnit.transform.TransformDirection(direction));
	}

	public IPositioned GetFinalFormationPosition(int formationIndex)
	{
		if (_formationPositions == null || _formationPositions.count == 0 || _modelUnit == null || _modelUnit.transform == null)
		{
			return null;
		}
		if (formationIndex < 0 || formationIndex >= _formationPositions.count)
		{
			return null;
		}
		if (_vectorField == null || _vectorField.destination == null)
		{
			return null;
		}
		Vector3 direction = _formationPositions[formationIndex];
		if (!_modelUnit.hasArrivedAtDestination)
		{
			return new Position(_vectorField.destination.position + _modelUnit.transform.TransformDirection(direction));
		}
		return new Position(_modelUnit.position + _modelUnit.transform.TransformDirection(direction));
	}

	private void HandleWaypoints()
	{
		if (_wayPoints.count == 0 || _pendingResult != null || _pendingPathRequest != null || _vectorField == null || _vectorField.destination == null)
		{
			return;
		}
		Vector3 position = _vectorField.destination.position;
		float num = _requestNextWaypointDistance * _requestNextWaypointDistance;
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			IUnitFacade unitFacade = base[i];
			if (unitFacade != null && (unitFacade.hasArrivedAtDestination || (unitFacade.position - position).sqrMagnitude < num))
			{
				flag = true;
				break;
			}
		}
		if (flag && ((_groupCog - position).sqrMagnitude < num || _modelUnit.hasArrivedAtDestination || (_modelUnit.position - position).sqrMagnitude < num))
		{
			RequestPath(_wayPoints.Peek(), InternalPathRequest.Type.Waypoint);
		}
	}

	public float? ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (count == 0)
		{
			CleanupGroup();
			return null;
		}
		if (_stopped || _wait || _modelUnit == null || !_modelUnit.isAlive)
		{
			return null;
		}
		_groupCog = GetGroupCenterOfGravity();
		_currentGrid = GridManager.instance.GetGrid(_modelUnit.position);
		HandleWaypoints();
		if (_pendingResult != null)
		{
			ConsumePath();
		}
		else if (!hasArrived)
		{
			if (_vectorField != null && _vectorField.nextNodePosition != null)
			{
				Vector3 position = _vectorField.nextNodePosition.position;
				if ((_groupCog - position).sqrMagnitude > _nextNodeDistance * _nextNodeDistance && (_modelUnit.position - position).sqrMagnitude > _nextNodeDistance * _nextNodeDistance)
				{
					HandlePathReplan();
				}
			}
		}
		else if (_pendingResult == null && _pendingPathRequest == null && _wayPoints.count == 0)
		{
			StopInternal();
			if (!_announcedEndOnce)
			{
				AnnounceEvent(UnitNavigationEventMessage.Event.DestinationReached, _groupCog, null);
				_announcedEndOnce = true;
			}
		}
		return null;
	}
}
