using Apex.Common;
using Apex.DataStructures;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Steering;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public class UnitFacade : IUnitFacade, IUnitProperties, IMovable, IMovingObject, IDefineSpeed, IGameObjectComponent, IGroupable<IUnitFacade>, IPositioned, IHaveAttributes
{
	private class MovableDummy : IMovable
	{
		private string _gameObjectName;

		public Path currentPath
		{
			get
			{
				ThrowException();
				return null;
			}
		}

		public IIterable<Vector3> currentWaypoints
		{
			get
			{
				ThrowException();
				return null;
			}
		}

		public Vector3? finalDestination
		{
			get
			{
				ThrowException();
				return null;
			}
		}

		public Vector3? nextNodePosition
		{
			get
			{
				ThrowException();
				return null;
			}
		}

		public UnitNavigationEventMessage.Event lastNavigationEvent
		{
			get
			{
				ThrowException();
				return UnitNavigationEventMessage.Event.None;
			}
		}

		public MovableDummy(string gameObjectName)
		{
			_gameObjectName = gameObjectName;
		}

		public void MoveTo(Vector3 position, bool append)
		{
			ThrowException();
		}

		public void MoveAlong(Path path)
		{
			ThrowException();
		}

		public void MoveAlong(Path path, ReplanCallback onReplan)
		{
			ThrowException();
		}

		public void Wait(float? seconds)
		{
			ThrowException();
		}

		public void Resume()
		{
			ThrowException();
		}

		public void EnableMovementOrders()
		{
			ThrowException();
		}

		public void DisableMovementOrders()
		{
			ThrowException();
		}

		private void ThrowException()
		{
			throw new MissingComponentException($"Game object {_gameObjectName} does not have a component of type IMovable, which is required to call IMovable methods and properties.");
		}
	}

	private int _formationIndex = -1;

	private IUnitProperties _props;

	private IMovable _movable;

	private IMovingObject _moving;

	private IDefineSpeed _speeder;

	private IPathFinderOptions _pathFinderOptions;

	private IPathNavigationOptions _pathNavOptions;

	public AttributeMask attributes => _props.attributes;

	public float groundOffset => _props.groundOffset;

	public float baseToPositionOffset => _props.baseToPositionOffset;

	public Collider collider { get; private set; }

	public Path currentPath => _movable.currentPath;

	public IIterable<Vector3> currentWaypoints => _movable.currentWaypoints;

	public float fieldOfView => _props.fieldOfView;

	public Vector3? finalDestination => _movable.finalDestination;

	public Vector3 forward => transform.forward;

	public Vector3? lookAt { get; set; }

	public GameObject gameObject { get; private set; }

	public float height => _props.height;

	public HeightNavigationCapabilities heightNavigationCapability => _props.heightNavigationCapability;

	public bool isGrounded => _moving.isGrounded;

	public bool isMovable { get; private set; }

	public bool isAlive { get; set; }

	public bool isSelectable => _props.isSelectable;

	public bool isSelected
	{
		get
		{
			return _props.isSelected;
		}
		set
		{
			_props.isSelected = value;
		}
	}

	public UnitNavigationEventMessage.Event lastNavigationEvent => _movable.lastNavigationEvent;

	public float maxAcceleration => _speeder.maxAcceleration;

	public float maxAngularAcceleration => _speeder.maxAngularAcceleration;

	public float maxDeceleration => _speeder.maxDeceleration;

	public float maximumAngularSpeed => _speeder.maximumAngularSpeed;

	public float maximumSpeed => _speeder.maximumSpeed;

	public float minimumSpeed => _speeder.minimumSpeed;

	public Vector3? nextNodePosition => _movable.nextNodePosition;

	public IPathFinderOptions pathFinderOptions => _pathFinderOptions;

	public IPathNavigationOptions pathNavigationOptions => _pathNavOptions;

	public Vector3 position => transform.position;

	public float radius => _props.radius;

	public Vector3 basePosition
	{
		get
		{
			if (_props != null)
			{
				return _props.basePosition;
			}
			return new Vector3(0f, 0f, 0f);
		}
	}

	public Transform transform { get; private set; }

	public TransientGroup<IUnitFacade> transientGroup { get; set; }

	public Vector3 velocity => _moving.velocity;

	public Vector3 actualVelocity => _moving.actualVelocity;

	public int formationIndex
	{
		get
		{
			return _formationIndex;
		}
		set
		{
			_formationIndex = value;
		}
	}

	public IPositioned formationPos { get; set; }

	public bool hasArrivedAtDestination { get; set; }

	public int determination
	{
		get
		{
			return _props.determination;
		}
		set
		{
			_props.determination = value;
		}
	}

	public void DisableMovementOrders()
	{
		_movable.DisableMovementOrders();
	}

	public void EnableMovementOrders()
	{
		_movable.EnableMovementOrders();
	}

	public void SetPreferredSpeed(float speed)
	{
		_speeder.SetPreferredSpeed(speed);
	}

	public float GetPreferredSpeed(Vector3 currentMovementDirection)
	{
		return _speeder.GetPreferredSpeed(currentMovementDirection);
	}

	public void MarkSelectPending(bool pending)
	{
		_props.MarkSelectPending(pending);
	}

	public void MoveAlong(Path path)
	{
		_movable.MoveAlong(path);
	}

	public void MoveAlong(Path path, ReplanCallback onReplan)
	{
		_movable.MoveAlong(path, onReplan);
	}

	public void MoveTo(Vector3 position, bool append)
	{
		_movable.MoveTo(position, append);
	}

	public void RecalculateBasePosition()
	{
		_props.RecalculateBasePosition();
	}

	public void Resume()
	{
		_moving.Resume();
	}

	public void SignalStop()
	{
		_speeder.SignalStop();
	}

	public void Stop()
	{
		_moving.Stop();
	}

	public void Wait(float? seconds)
	{
		_moving.Wait(seconds);
	}

	void IDefineSpeed.CloneFrom(IDefineSpeed speedComponent)
	{
		_speeder.CloneFrom(speedComponent);
	}

	public virtual void Initialize(GameObject unitObject)
	{
		if (unitObject.GetComponent<IUnitProperties>() != null)
		{
			_props = unitObject.As<IUnitProperties>(searchParent: false, required: true);
			_movable = unitObject.As<IMovable>();
			_moving = unitObject.As<IMovingObject>(searchParent: false, required: true);
			_speeder = unitObject.As<IDefineSpeed>(searchParent: false, required: true);
			_pathFinderOptions = unitObject.As<IPathFinderOptions>() ?? new PathFinderOptions();
			_pathNavOptions = unitObject.As<IPathNavigationOptions>() ?? new PathNavigationOptions();
		}
		else
		{
			Debug.Log(unitObject);
		}
		isMovable = _movable != null;
		if (!isMovable)
		{
			_movable = new MovableDummy(unitObject.name);
		}
		gameObject = unitObject;
		transform = unitObject.transform;
		collider = unitObject.GetComponent<Collider>();
		isAlive = true;
		hasArrivedAtDestination = true;
	}
}
