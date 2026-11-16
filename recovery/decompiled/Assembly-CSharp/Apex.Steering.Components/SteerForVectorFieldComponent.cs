using System;
using Apex.DataStructures;
using Apex.PathFinding;
using Apex.Services;
using Apex.Steering.VectorFields;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Vector Field", 1031)]
[ApexComponent("Steering")]
public class SteerForVectorFieldComponent : ArrivalBase
{
	[MinCheck(0.1f, label = "Arrival Radius Margin", tooltip = "This radius margin is added to the unit's radius and used to evaluate whether hasArrivedAtDestination is true, i.e. when the distance to the destination is less than the radius plus this margin.")]
	public float arrivalRadiusMargin = 2f;

	private Vector3 _endOfResolvedPath;

	private float _remainingSquaredDistance;

	private Vector3 _nextPoint;

	private Path _currentPath;

	private bool _stopped;

	private bool _isPortalling;

	private DefaultSteeringTransientUnitGroup _nextGroup;

	private int _lastPathPortalIndex;

	private DynamicArray<Cell> _neighbours;

	private bool _requestedPath;

	private bool _enabledSoloPath;

	private SteeringController _steeringController;

	private DistanceComparer<Cell> _cellComparer = new DistanceComparer<Cell>(ascending: true);

	protected override void Start()
	{
		base.Start();
		_neighbours = new DynamicArray<Cell>(8);
		_lastPathPortalIndex = -1;
		IUnitFacade unitFacade = this.GetUnitFacade();
		if (!unitFacade.isMovable)
		{
			Debug.LogWarning(gameObject.name + " does not have a component that implements IMovable, which means that if it gets stuck, it cannot request a path to get to the destination");
		}
		_steeringController = GetComponent<SteeringController>();
		if (_steeringController == null)
		{
			Debug.LogWarning(gameObject.name + " does not have a SteeringController component, which means that it cannot disable formations while solo pathing");
		}
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (_isPortalling)
		{
			output.pause = true;
			return;
		}
		if (_requestedPath)
		{
			output.pause = true;
			return;
		}
		IUnitFacade unit = input.unit;
		if (!(unit.transientGroup is DefaultSteeringTransientUnitGroup { count: not 0 } defaultSteeringTransientUnitGroup))
		{
			return;
		}
		IGrid grid = input.grid;
		if (grid == null)
		{
			return;
		}
		IVectorField vectorField = defaultSteeringTransientUnitGroup.vectorField;
		if (vectorField == null)
		{
			return;
		}
		Path currentPath = vectorField.currentPath;
		if (currentPath == null || currentPath.count == 0)
		{
			return;
		}
		Vector3 position = unit.position;
		Cell cell = grid.GetCell(position);
		if (cell == null)
		{
			return;
		}
		if (!object.ReferenceEquals(currentPath, _currentPath))
		{
			_remainingSquaredDistance = currentPath.CalculateSquaredLength();
			_endOfResolvedPath = currentPath.Last().position;
			_stopped = false;
			_lastPathPortalIndex = -1;
			_currentPath = currentPath;
			unit.hasArrivedAtDestination = false;
			if (_enabledSoloPath)
			{
				EndSoloPath();
			}
		}
		if (_stopped || unit.hasArrivedAtDestination)
		{
			return;
		}
		VectorFieldCell fieldCellAtPos = vectorField.GetFieldCellAtPos(cell);
		Vector3 vector = fieldCellAtPos.direction;
		if (vector.sqrMagnitude == 0f)
		{
			if (!_enabledSoloPath)
			{
				HandleMissingVectorFromField(cell, vectorField, input, output);
			}
			else
			{
				output.maxAllowedSpeed = unit.maximumSpeed;
			}
			return;
		}
		int pathPortalIndex = fieldCellAtPos.pathPortalIndex;
		if (pathPortalIndex != -1 && pathPortalIndex != _lastPathPortalIndex)
		{
			_lastPathPortalIndex = pathPortalIndex;
			HandlePortal(unit, defaultSteeringTransientUnitGroup, vectorField, pathPortalIndex, input.grid);
			return;
		}
		Vector3 position2 = vectorField.nextNodePosition.position;
		if (_nextPoint != position2)
		{
			_remainingSquaredDistance -= (_nextPoint - position2).sqrMagnitude;
			_stopped = false;
			_nextPoint = position2;
		}
		if (_enabledSoloPath)
		{
			EndSoloPath();
		}
		IUnitFacade modelUnit = defaultSteeringTransientUnitGroup.modelUnit;
		Vector3 zero = Vector3.zero;
		Vector3 to = position + vector;
		if (object.ReferenceEquals(unit, modelUnit))
		{
			float num = slowingDistance * slowingDistance;
			if (position.DirToXZ(_endOfResolvedPath).sqrMagnitude < num && (_remainingSquaredDistance < num || vectorField.isOnFinalApproach))
			{
				zero = Arrive(position2, input);
				if (hasArrived)
				{
					defaultSteeringTransientUnitGroup.hasArrived = true;
					_stopped = true;
					unit.hasArrivedAtDestination = true;
				}
			}
			else
			{
				zero = SeekWithVectorFieldVector(to, input);
			}
		}
		else
		{
			float num2 = unit.radius + arrivalDistance + arrivalRadiusMargin;
			IPositioned finalFormationPosition = defaultSteeringTransientUnitGroup.GetFinalFormationPosition(unit.formationIndex);
			Vector3 vector2 = ((finalFormationPosition == null || unit.formationPos == null) ? vectorField.destination.position : finalFormationPosition.position);
			if ((vector2 - position).sqrMagnitude < num2 * num2)
			{
				unit.hasArrivedAtDestination = true;
				return;
			}
			zero = SeekWithVectorFieldVector(to, input);
		}
		if (zero.sqrMagnitude != 0f)
		{
			output.desiredAcceleration = zero;
		}
	}

	private Vector3 SeekWithVectorFieldVector(Vector3 to, SteeringInput input)
	{
		if (autoCalculateSlowingDistance)
		{
			CalculateRequiredSlowingDistance(input);
		}
		return Seek(to, input);
	}

	private void HandlePortal(IUnitFacade unitData, DefaultSteeringTransientUnitGroup group, IVectorField vectorField, int pathPortalIndex, IGrid grid)
	{
		if (!(_currentPath[pathPortalIndex] is IPortalNode portalNode) || object.ReferenceEquals(unitData, group.modelUnit))
		{
			return;
		}
		int groupCount = group.count;
		IPositioned to = _currentPath[pathPortalIndex + 1];
		float num = grid.cellSize * 1.414214f + 0.1f;
		bool isFarPortal = (portalNode.position - portalNode.partner.position).sqrMagnitude > num * num;
		if (isFarPortal)
		{
			if (group.nextGroup == null)
			{
				IGroupingStrategy<IUnitFacade> groupingStrategy = GroupingManager.GetGroupingStrategy<IUnitFacade>();
				if (groupingStrategy == null)
				{
					Debug.Log("No Grouping Strategy has been registered for IUnitFacade");
					return;
				}
				if (!(groupingStrategy.CreateGroup(groupCount) is DefaultSteeringTransientUnitGroup nextGroup))
				{
					return;
				}
				group.nextGroup = nextGroup;
				_nextGroup = nextGroup;
			}
			else
			{
				_nextGroup = group.nextGroup;
			}
			group.Remove(unitData);
		}
		_isPortalling = true;
		portalNode.Execute(unitData.transform, to, () =>
		{
			_isPortalling = false;
			if (isFarPortal)
			{
				_nextGroup.Add(unitData);
				unitData.transientGroup = _nextGroup;
				if (_nextGroup.count == 1 && vectorField.destination != null)
				{
					int count = _currentPath.count;
					Path path = new Path(count - (pathPortalIndex + 2));
					for (int num2 = count - 1; num2 >= pathPortalIndex + 2; num2--)
					{
						path.Push(_currentPath[num2]);
					}
					Vector3 position = vectorField.destination.position;
					if ((to.position - position).sqrMagnitude > 1f)
					{
						_nextGroup.MoveAlong(path);
					}
					if (group.currentWaypoints.count > 0)
					{
						_nextGroup.SetWaypoints(group.currentWaypoints);
					}
					if (group.currentFormation != null)
					{
						_nextGroup.TransferFormation(group.currentFormation, groupCount, group);
					}
				}
			}
		});
	}

	private void StartSoloPath()
	{
		_steeringController.StartSoloPath();
		_enabledSoloPath = true;
	}

	private void EndSoloPath()
	{
		_steeringController.EndSoloPath();
		_enabledSoloPath = false;
	}

	private void HandleMissingVectorFromField(Cell selfCell, IVectorField vectorField, SteeringInput input, SteeringOutput output)
	{
		IUnitFacade unit = input.unit;
		Vector3 position = unit.position;
		_neighbours.Clear();
		input.grid.GetConcentricNeighbours(selfCell, 1, _neighbours);
		_cellComparer.compareTo = position;
		_neighbours.Sort(_cellComparer);
		int count = _neighbours.count;
		for (int i = 0; i < count; i++)
		{
			Cell cell = _neighbours[i];
			Vector3 position2 = cell.position;
			if (cell.IsWalkableWithClearance(unit) && vectorField.GetFieldCellAtPos(cell).direction.sqrMagnitude != 0f && position2.y <= position.y)
			{
				output.maxAllowedSpeed = unit.maximumSpeed;
				output.desiredAcceleration = Seek(position2, input);
				return;
			}
		}
		if (unit.isMovable && vectorField.destination != null)
		{
			RequestPath(vectorField.destination.position, input);
		}
	}

	private void RequestPath(Vector3 destination, SteeringInput input)
	{
		IUnitFacade unit = input.unit;
		Action<PathResult> callback = (PathResult result) =>
		{
			if (result.status == PathingStatus.Complete)
			{
				FollowPath(result.path, unit);
			}
			else
			{
				PathRequestCallback(result, unit, unit.position, destination, input.grid);
			}
		};
		QueuePathRequest(unit.position, destination, unit, callback);
	}

	private void PathRequestCallback(PathResult result, IUnitFacade unit, Vector3 unitPos, Vector3 destination, IGrid grid)
	{
		Action<PathResult> callback = (PathResult secondResult) =>
		{
			if (secondResult.status == PathingStatus.Complete)
			{
				Path path = result.path;
				path.Push(unit);
				FollowPath(path, unit);
			}
			else
			{
				_stopped = true;
			}
		};
		Cell nearestWalkableCell = grid.GetNearestWalkableCell(unitPos, unitPos, requireWalkableFromPosition: true, 1, unit);
		if (nearestWalkableCell != null)
		{
			QueuePathRequest(nearestWalkableCell.position, destination, unit, callback);
			return;
		}
		int count = _neighbours.count;
		for (int num = 0; num < count; num++)
		{
			Cell cell = _neighbours[num];
			if (cell.IsWalkableWithClearance(unit))
			{
				QueuePathRequest(cell.position, destination, unit, callback);
				break;
			}
		}
	}

	private void QueuePathRequest(Vector3 fromPos, Vector3 toPos, IUnitFacade unitData, Action<PathResult> callback)
	{
		_requestedPath = true;
		CallbackPathRequest callbackPathRequest = new CallbackPathRequest(callback);
		callbackPathRequest.from = fromPos;
		callbackPathRequest.to = toPos;
		callbackPathRequest.requesterProperties = unitData;
		callbackPathRequest.pathFinderOptions = unitData.pathFinderOptions;
		CallbackPathRequest request = callbackPathRequest;
		GameServices.pathService.QueueRequest(request, unitData.pathFinderOptions.pathingPriority);
	}

	private void FollowPath(Path path, IUnitFacade unit)
	{
		unit.MoveAlong(path);
		_requestedPath = false;
		StartSoloPath();
	}

	public void CloneFrom(SteerForVectorFieldComponent steerForVector)
	{
		priority = steerForVector.priority;
		weight = steerForVector.weight;
		slowingAlgorithm = steerForVector.slowingAlgorithm;
		slowingDistance = steerForVector.slowingDistance;
		arrivalDistance = steerForVector.arrivalDistance;
		autoCalculateSlowingDistance = steerForVector.autoCalculateSlowingDistance;
		arrivalRadiusMargin = steerForVector.arrivalRadiusMargin;
	}

	public override void Stop()
	{
		_stopped = true;
	}
}
