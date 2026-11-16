using System;
using System.Collections.Generic;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.Services;
using Apex.Steering.HeightNavigation;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

[AddComponentMenu("Apex/Unit/Navigation/Steerable Unit", 1031)]
[ApexComponent("Steering")]
public class SteerableUnitComponent : ExtendedMonoBehaviour, ILoadBalanced, IMovingObject
{
	private class DefaultMover : IMoveUnits
	{
		private Transform _transform;

		public DefaultMover(Transform transform)
		{
			_transform = transform;
		}

		public void Move(Vector3 velocity, float deltaTime)
		{
			_transform.position += velocity * deltaTime;
		}

		public void Rotate(Vector3 targetOrientation, float angularSpeed, float deltaTime)
		{
			Quaternion to = Quaternion.LookRotation(targetOrientation);
			_transform.rotation = Quaternion.RotateTowards(_transform.rotation, to, angularSpeed * 57.29578f * deltaTime);
		}

		public void Stop()
		{
		}
	}

	private class RigidBodyMover : IMoveUnits
	{
		private Rigidbody _rigidBody;

		public RigidBodyMover(Rigidbody rigidBody)
		{
			_rigidBody = rigidBody;
		}

		public void Move(Vector3 velocity, float deltaTime)
		{
			if (!_rigidBody.isKinematic)
			{
				Rigidbody rigidBody = _rigidBody;
				Vector3 zero = Vector3.zero;
				_rigidBody.angularVelocity = zero;
				rigidBody.velocity = zero;
			}
			_rigidBody.MovePosition(_rigidBody.position + velocity * deltaTime);
		}

		public void Rotate(Vector3 targetOrientation, float angularSpeed, float deltaTime)
		{
			if (targetOrientation.sqrMagnitude != 0f)
			{
				Quaternion to = Quaternion.LookRotation(targetOrientation);
				Quaternion rot = Quaternion.RotateTowards(_rigidBody.rotation, to, angularSpeed * 57.29578f * deltaTime);
				_rigidBody.MoveRotation(rot);
			}
		}

		public void Stop()
		{
			if (!_rigidBody.isKinematic)
			{
				_rigidBody.velocity = Vector3.zero;
				_rigidBody.Sleep();
			}
		}
	}

	private class CharacterControllerMover : IMoveUnits
	{
		private Transform _transform;

		private CharacterController _controller;

		public CharacterControllerMover(CharacterController controller)
		{
			_transform = controller.transform;
			_controller = controller;
		}

		public void Move(Vector3 velocity, float deltaTime)
		{
			_controller.Move(velocity * deltaTime);
		}

		public void Rotate(Vector3 targetOrientation, float angularSpeed, float deltaTime)
		{
			Quaternion to = Quaternion.LookRotation(targetOrientation);
			_transform.rotation = Quaternion.RotateTowards(_transform.rotation, to, angularSpeed * 57.29578f * deltaTime);
		}

		public void Stop()
		{
		}
	}

	private const float AngularSpeedLowerLimit = 0.0001f;

	[Tooltip("The time over which to stop when waiting or when no steering components have any output (as permitted by deceleration capabilities).")]
	public float stopTimeFrame = 0.2f;

	[Tooltip("The amount of seconds after which the unit will stop if it is stuck.")]
	public float stopIfStuckForSeconds = 3f;

	[Tooltip("Determines how the actual velocity is calculated. If set to true, the calculation will be carried out in the load balanced update, which will average the actual velocity over several frames. If set to false, it will calculate it each frame.")]
	public bool averageActualVelocity;

	[Obsolete("This is handled by the height navigator component on the unit.")]
	public float groundStickynessFactor = 1f;

	[Obsolete("This is handled by the height navigator component on the unit.")]
	public float gravity = -9.81f;

	[Obsolete("This is handled by the height navigator component on the unit.")]
	public float terminalVelocity = 50f;

	private Vector3 _currentVelocity;

	private Vector3 _currentPlanarVelocity;

	private Vector3 _currentSpatialVelocity;

	private float _currentAngularSpeed;

	private bool _waiting;

	private bool _stopped;

	private float _stuckCheckLastMove;

	private Vector3 _lastFramePosition;

	private Vector3 _actualVelocity;

	private Transform _transform;

	private IUnitFacade _unit;

	private IMoveUnits _mover;

	private IHeightNavigator _heights;

	private List<ISteeringBehaviour> _steeringComponents;

	private List<IOrientationBehaviour> _orientationComponents;

	private SteeringOutput _steering;

	private OrientationOutput _orientation;

	private SteeringInput _steeringInput;

	bool ILoadBalanced.repeat => true;

	public float speed => _currentVelocity.magnitude;

	public Vector3 velocity => _currentVelocity;

	public Vector3 actualVelocity => _actualVelocity;

	public bool isGrounded { get; private set; }

	private void Awake()
	{
		this.WarnIfMultipleInstances();
		_transform = transform;
		_mover = this.As<IMoveUnits>();
		if (_mover == null)
		{
			IMoveUnitsFactory moveUnitsFactory = this.As<IMoveUnitsFactory>();
			CharacterController component = GetComponent<CharacterController>();
			Rigidbody component2 = GetComponent<Rigidbody>();
			if (moveUnitsFactory != null)
			{
				_mover = moveUnitsFactory.Create();
			}
			else if (component != null)
			{
				_mover = new CharacterControllerMover(component);
			}
			else if (component2 != null)
			{
				_mover = new RigidBodyMover(component2);
			}
			else
			{
				_mover = new DefaultMover(_transform);
			}
		}
		_heights = this.As<IHeightNavigator>();
		if (_heights == null)
		{
			_heights = NoHeightNavigator.Instance;
		}
		_steeringComponents = new List<ISteeringBehaviour>();
		_orientationComponents = new List<IOrientationBehaviour>();
		_steering = new SteeringOutput();
		_orientation = new OrientationOutput();
		_steeringInput = new SteeringInput();
	}

	protected override void Start()
	{
		base.Start();
		_unit = this.GetUnitFacade();
		_steeringInput.unit = _unit;
		_lastFramePosition = _unit.position;
		_stopped = true;
		UpdateInput();
	}

	protected override void OnStartAndEnable()
	{
		NavLoadBalancer.steering.Add(this);
	}

	private void OnDisable()
	{
		Stop();
		NavLoadBalancer.steering.Remove(this);
	}

	private void FixedUpdate()
	{
		Steer(Time.deltaTime);
	}

	public void RegisterSteeringBehavior(ISteeringBehaviour behavior)
	{
		int count = _steeringComponents.Count;
		for (int i = 0; i < count; i++)
		{
			ISteeringBehaviour steeringBehaviour = _steeringComponents[i];
			if (steeringBehaviour.priority == behavior.priority)
			{
				SteeringGroup steeringGroup = steeringBehaviour as SteeringGroup;
				if (steeringGroup == null)
				{
					steeringGroup = new SteeringGroup(steeringBehaviour.priority);
					_steeringComponents[i] = steeringGroup;
					steeringGroup.Add(steeringBehaviour);
				}
				steeringGroup.Add(behavior);
				return;
			}
		}
		_steeringComponents.Add(behavior);
		_steeringComponents.Sort((ISteeringBehaviour a, ISteeringBehaviour b) => b.priority.CompareTo(a.priority));
	}

	public void UnregisterSteeringBehavior(ISteeringBehaviour behavior)
	{
		int num = _steeringComponents.Count - 1;
		for (int num2 = num; num2 >= 0; num2--)
		{
			ISteeringBehaviour steeringBehaviour = _steeringComponents[num2];
			if (steeringBehaviour.priority == behavior.priority)
			{
				if (steeringBehaviour is SteeringGroup steeringGroup)
				{
					steeringGroup.Remove(behavior);
				}
				else
				{
					_steeringComponents.RemoveAt(num2);
				}
			}
		}
	}

	public void RegisterOrientationBehavior(IOrientationBehaviour behavior)
	{
		_orientationComponents.Add(behavior);
		_orientationComponents.Sort((IOrientationBehaviour a, IOrientationBehaviour b) => b.priority.CompareTo(a.priority));
	}

	public void UnregisterOrientationBehavior(IOrientationBehaviour behavior)
	{
		_orientationComponents.Remove(behavior);
	}

	public void Stop()
	{
		for (int i = 0; i < _steeringComponents.Count; i++)
		{
			_steeringComponents[i].Stop();
		}
		_waiting = false;
	}

	public void Wait(float? seconds)
	{
		_waiting = true;
		if (seconds.HasValue)
		{
			LoadBalancer.defaultBalancer.Add(new OneTimeAction((float ignored) =>
			{
				Resume();
			}), seconds.Value, delayFirstUpdate: true);
		}
	}

	public void Resume()
	{
		_stuckCheckLastMove = Time.time;
		_waiting = false;
	}

	private void StopMovement()
	{
		if (!_stopped)
		{
			_unit.SignalStop();
			_stopped = true;
			_currentVelocity = (_currentPlanarVelocity = (_currentSpatialVelocity = Vector3.zero));
			_steeringInput.currentFullVelocity = (_steeringInput.currentPlanarVelocity = (_steeringInput.currentSpatialVelocity = Vector3.zero));
			_steeringInput.currentAngularSpeed = (_currentAngularSpeed = 0f);
			_mover.Stop();
		}
	}

	private bool IsStuck(float deltaTime)
	{
		if (stopIfStuckForSeconds <= 0f)
		{
			return false;
		}
		float minimumSpeed = _unit.minimumSpeed;
		if (_actualVelocity.sqrMagnitude > minimumSpeed * minimumSpeed)
		{
			_stuckCheckLastMove = Time.time;
			return false;
		}
		return Time.time - _stuckCheckLastMove > stopIfStuckForSeconds;
	}

	float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (averageActualVelocity)
		{
			_actualVelocity = (_transform.position - _lastFramePosition) / deltaTime;
			_lastFramePosition = _transform.position;
		}
		if (_stopped || _waiting)
		{
			return null;
		}
		if (IsStuck(deltaTime))
		{
			UnitNavigationEventMessage unitNavigationEventMessage = new UnitNavigationEventMessage(gameObject, UnitNavigationEventMessage.Event.Stuck);
			unitNavigationEventMessage.destination = _unit.finalDestination.GetValueOrDefault();
			unitNavigationEventMessage.pendingWaypoints = _unit.currentWaypoints.ToArray();
			UnitNavigationEventMessage message = unitNavigationEventMessage;
			Stop();
			StopMovement();
			GameServices.messageBus.Post(message);
			return null;
		}
		UpdateInput();
		return null;
	}

	private void Steer(float deltaTime)
	{
		if (!averageActualVelocity)
		{
			_actualVelocity = (_transform.position - _lastFramePosition) / deltaTime;
			_lastFramePosition = _transform.position;
		}
		IGrid grid = GridManager.instance.GetGrid(_transform.position);
		_steeringInput.grid = grid;
		_steeringInput.deltaTime = deltaTime;
		_steering.Clear();
		if (!_waiting)
		{
			int count = _steeringComponents.Count;
			for (int i = 0; i < count; i++)
			{
				_steeringComponents[i].GetSteering(_steeringInput, _steering);
				if (_steering.hasOutput)
				{
					break;
				}
			}
			if (_steering.pause)
			{
				return;
			}
		}
		Vector3 vector = _steering.desiredAcceleration;
		if (!_steering.hasOutput && !_stopped)
		{
			float num = Mathf.Max(stopTimeFrame, deltaTime);
			vector = Vector3.ClampMagnitude(-_currentSpatialVelocity / num, _steeringInput.maxDeceleration);
		}
		_currentSpatialVelocity += vector * deltaTime;
		float minimumSpeed = _unit.minimumSpeed;
		if (_currentSpatialVelocity.sqrMagnitude < minimumSpeed * minimumSpeed && !_steering.hasOutput)
		{
			StopMovement();
		}
		else if (_stopped)
		{
			_stuckCheckLastMove = Time.time;
			_stopped = false;
		}
		float num2 = Mathf.Max(_steering.maxAllowedSpeed, _steeringInput.desiredSpeed);
		_currentSpatialVelocity = Vector3.ClampMagnitude(_currentSpatialVelocity, num2);
		_currentPlanarVelocity = _currentSpatialVelocity.OnlyXZ();
		_steeringInput.currentSpatialVelocity = _currentSpatialVelocity;
		_steeringInput.currentPlanarVelocity = _currentPlanarVelocity;
		HeightOutput heightOutput = _heights.GetHeightOutput(_steeringInput, num2);
		isGrounded = heightOutput.isGrounded;
		if (_steering.overrideHeightNavigation)
		{
			_currentVelocity = _currentSpatialVelocity;
		}
		else
		{
			_currentVelocity = heightOutput.finalVelocity;
		}
		if (_steering.verticalForce != 0f)
		{
			_currentVelocity.y += _steering.verticalForce * deltaTime;
		}
		_steeringInput.currentFullVelocity = _currentVelocity;
		_orientation.Clear();
		int count2 = _orientationComponents.Count;
		for (int j = 0; j < count2; j++)
		{
			_orientationComponents[j].GetOrientation(_steeringInput, _orientation);
			if (_orientation.hasOutput)
			{
				break;
			}
		}
		if (Mathf.Abs(_orientation.desiredAngularAcceleration) > 0.01f)
		{
			_currentAngularSpeed += _orientation.desiredAngularAcceleration * deltaTime;
			_currentAngularSpeed = Mathf.Clamp(_currentAngularSpeed, 0f, _steeringInput.desiredAngularSpeed);
			_steeringInput.currentAngularSpeed = _currentAngularSpeed;
		}
		_mover.Move(_currentVelocity, deltaTime);
		if (_currentAngularSpeed > 0.0001f && _orientation.desiredOrientation.sqrMagnitude > 0f)
		{
			_mover.Rotate(_orientation.desiredOrientation, _currentAngularSpeed, deltaTime);
		}
	}

	private void UpdateInput()
	{
		_steeringInput.gravity = _heights.gravity;
		_steeringInput.maxAcceleration = _unit.maxAcceleration;
		_steeringInput.maxDeceleration = _unit.maxDeceleration;
		_steeringInput.maxAngularAcceleration = _unit.maxAngularAcceleration;
		_steeringInput.desiredSpeed = _unit.GetPreferredSpeed(_currentPlanarVelocity.normalized);
		_steeringInput.desiredAngularSpeed = _unit.maximumAngularSpeed;
	}

	public void CloneFrom(SteerableUnitComponent steerableUnit)
	{
		stopIfStuckForSeconds = steerableUnit.stopIfStuckForSeconds;
		stopTimeFrame = steerableUnit.stopTimeFrame;
	}
}
