using Apex.Common;
using Apex.LoadBalancing;
using Apex.Messages;
using Apex.Services;
using Apex.Steering;
using Apex.Utilities;
using UnityEngine;

namespace Apex.WorldGeometry;

[RequireComponent(typeof(Collider))]
public abstract class DynamicObstacleBase : ExtendedMonoBehaviour, IDynamicObstacle, ILoadBalanced, IHandleMessage<GridStatusMessage>, IPositioned
{
	public enum UpdateMode
	{
		OnInterval,
		OnRequest
	}

	private class ToggleAction : ILoadBalanced
	{
		private DynamicObstacleBase _target;

		private bool _block;

		public bool repeat => false;

		public ToggleAction(DynamicObstacleBase target, bool block)
		{
			_target = target;
			_block = block;
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			_target.UpdateCells(_block);
			return null;
		}
	}

	[Tooltip("Controls when the obstacle updates its state, and thereby its associated grid.")]
	public UpdateMode updateMode;

	[MinCheck(0f, tooltip = "Determines how far in the obstacles direction of movement, that cells will be considered blocked.")]
	public float velocityPredictionFactor = 1.5f;

	[Tooltip("Controls whether the obstacle will look for a velocity source on the parent game object if one is not found on its own.")]
	public bool resolveVelocityFromParent;

	[Tooltip("Setting this to true, will stop the dynamic updates if the obstacle remains stationary for 'Stationary Threshold Seconds' seconds.")]
	public bool stopUpdatingIfStationary;

	[MinCheck(0f, tooltip = "The amount of seconds after which dynamic updates on the obstacle will stop.")]
	public float stationaryThresholdSeconds = 5f;

	[Tooltip("Controls whether the obstacle sensitivity range of the grid will be used by this obstacle.")]
	public bool useGridObstacleSensitivity = true;

	[MinCheck(0f, tooltip = "The custom obstacle sensitivity range to use.")]
	public float customSensitivity;

	[MinCheck(0f, tooltip = "Setting this to a value other than 0, will override the default update interval of the load balancer.")]
	public float customUpdateInterval;

	[Tooltip("If checked, the Dynamic Obstacle will react to grids being disabled / initialized at runtime. If you use dynamic grid initialization this should be checked.")]
	public bool supportDynamicGrids;

	[Tooltip("Controls whether the dynamic obstacle will trigger path replanning for unit's in the grid sections it influences.")]
	public bool causesReplanning = true;

	protected Transform _transform;

	private IMovingObject _mover;

	private Rigidbody _rigidBody;

	private float _hasBeenStationaryForSeconds;

	private bool _isScheduledForUpdates;

	[SerializeField]
	[AttributeProperty("Exceptions", "Units with one or more matching attributes will not consider this obstacle when path finding, e.g. useful for doors etc..")]
	private int _exceptionsMask;

	public Vector3 position => _transform.position;

	public AttributeMask exceptionsMask
	{
		get
		{
			return _exceptionsMask;
		}
		set
		{
			if (_exceptionsMask != value)
			{
				if (_transform != null)
				{
					UpdateCells(block: false);
					_exceptionsMask = value;
					UpdateCells(block: true);
				}
				else
				{
					_exceptionsMask = value;
				}
			}
		}
	}

	public bool repeat { get; private set; }

	protected bool isVelocityEnabled { get; private set; }

	private void Awake()
	{
		_transform = transform;
		if (resolveVelocityFromParent && _transform.parent != null)
		{
			_mover = _transform.parent.As<IMovingObject>();
			_rigidBody = _transform.parent.GetComponent<Rigidbody>();
		}
		if (_mover == null && _rigidBody == null)
		{
			_mover = this.As<IMovingObject>();
			_rigidBody = GetComponent<Rigidbody>();
		}
		isVelocityEnabled = _mover != null || (_rigidBody != null && !_rigidBody.isKinematic);
		OnAwake();
		if (updateMode == UpdateMode.OnRequest && isVelocityEnabled)
		{
			Debug.LogWarning("Please note that this obstacle is marked to update on request, and will not be automatically updating after moving.");
		}
	}

	public virtual void RenderVisualization()
	{
	}

	protected virtual void OnAwake()
	{
	}

	protected override void OnStartAndEnable()
	{
		if (supportDynamicGrids)
		{
			GameServices.messageBus.Subscribe(this);
			IGrid grid = GridManager.instance.GetGrid(_transform.position);
			if (grid == null)
			{
				return;
			}
		}
		EnableInternal();
	}

	protected abstract void UpdateCells(bool block);

	private void OnDisable()
	{
		if (supportDynamicGrids)
		{
			GameServices.messageBus.Unsubscribe(this);
		}
		DisableInternal();
	}

	private void EnableInternal()
	{
		bool flag = updateMode == UpdateMode.OnInterval;
		InitializeState(customUpdateInterval, flag);
	}

	private void DisableInternal()
	{
		if (_isScheduledForUpdates)
		{
			NavLoadBalancer.dynamicObstacles.Remove(this);
		}
		_isScheduledForUpdates = false;
		UpdateCells(block: false);
	}

	public void ActivateUpdates(float? interval, bool repeat)
	{
		if (_isScheduledForUpdates)
		{
			NavLoadBalancer.dynamicObstacles.Remove(this);
			_isScheduledForUpdates = false;
		}
		InitializeState(interval.GetValueOrDefault(customUpdateInterval), repeat);
	}

	public void Toggle(bool active)
	{
		NavLoadBalancer.dynamicObstacles.Add(new ToggleAction(this, active));
	}

	float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (repeat && !_transform.hasChanged)
		{
			if (stopUpdatingIfStationary)
			{
				_hasBeenStationaryForSeconds += deltaTime;
				if (_hasBeenStationaryForSeconds > stationaryThresholdSeconds)
				{
					_isScheduledForUpdates = false;
					repeat = false;
				}
			}
			return null;
		}
		_transform.hasChanged = false;
		_hasBeenStationaryForSeconds = 0f;
		_isScheduledForUpdates = repeat;
		UpdateCells(block: true);
		return null;
	}

	void IHandleMessage<GridStatusMessage>.Handle(GridStatusMessage message)
	{
		if (!message.gridBounds.Contains(_transform.position))
		{
			return;
		}
		switch (message.status)
		{
		case GridStatusMessage.StatusCode.DisableComplete:
			DisableInternal();
			break;
		case GridStatusMessage.StatusCode.InitializationComplete:
			if (enabled)
			{
				EnableInternal();
			}
			break;
		}
	}

	private void InitializeState(float updateInterval, bool repeat)
	{
		if (!_isScheduledForUpdates)
		{
			_transform.hasChanged = true;
			_isScheduledForUpdates = true;
			this.repeat = repeat;
			if (updateInterval > 0f)
			{
				NavLoadBalancer.dynamicObstacles.Add(this, updateInterval);
			}
			else
			{
				NavLoadBalancer.dynamicObstacles.Add(this);
			}
		}
	}

	protected Vector3 GetVelocity()
	{
		Vector3 vector = Vector3.zero;
		if (_mover != null)
		{
			vector = _mover.velocity;
		}
		if (_rigidBody != null)
		{
			vector += _rigidBody.velocity;
		}
		return vector * velocityPredictionFactor;
	}
}
