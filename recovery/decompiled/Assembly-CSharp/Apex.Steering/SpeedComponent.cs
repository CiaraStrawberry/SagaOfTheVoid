using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering;

[AddComponentMenu("Apex/Unit/Navigation/Basic Speed", 1025)]
[ApexComponent("Steering")]
public class SpeedComponent : MonoBehaviour, IDefineSpeed
{
	[SerializeField]
	[MinCheck(0f, tooltip = "The maximum acceleration rate of the unit (m / s^2), i.e. how fast can the unit reach its desired speed.")]
	private float _maxAcceleration = 20f;

	[SerializeField]
	[MinCheck(0f, tooltip = "The maximum deceleration rate of the unit (m / s^2), i.e. how fast can the unit slow down.")]
	private float _maxDeceleration = 30f;

	[SerializeField]
	[MinCheck(0f, tooltip = "The maximum angular acceleration rate of the unit (rads / s^2), i.e. how fast can the unit reach its desired turn speed.")]
	private float _maxAngularAcceleration = 40f;

	[SerializeField]
	[MinCheck(0f, tooltip = "The maximum turn speed of the unit (rads / s), i.e. how fast can the unit turn.")]
	private float _maxAngularSpeed = 6f;

	[SerializeField]
	[MinCheck(0.01f, tooltip = "The minimum speed ever, regardless of movement form. Any speed below this will mean a stop.")]
	private float _minimumSpeed = 0.01f;

	[SerializeField]
	[MinCheck(0.01f, tooltip = "The maximum speed, regardless of movement form.")]
	private float _maximumSpeed = 1f;

	protected float _preferredSpeed;

	public virtual float minimumSpeed
	{
		get
		{
			return _minimumSpeed;
		}
		set
		{
			_minimumSpeed = value;
		}
	}

	public virtual float maximumSpeed
	{
		get
		{
			return _maximumSpeed;
		}
		set
		{
			_maximumSpeed = value;
		}
	}

	public float maxAcceleration
	{
		get
		{
			return _maxAcceleration;
		}
		set
		{
			_maxAcceleration = value;
		}
	}

	public float maxDeceleration
	{
		get
		{
			return _maxDeceleration;
		}
		set
		{
			_maxDeceleration = value;
		}
	}

	public float maxAngularAcceleration
	{
		get
		{
			return _maxAngularAcceleration;
		}
		set
		{
			_maxAngularAcceleration = value;
		}
	}

	public float maximumAngularSpeed
	{
		get
		{
			return _maxAngularSpeed;
		}
		set
		{
			_maxAngularSpeed = value;
		}
	}

	public virtual void SetPreferredSpeed(float speed)
	{
		_preferredSpeed = Mathf.Min(speed, maximumSpeed);
	}

	public virtual float GetPreferredSpeed(Vector3 currentMovementDirection)
	{
		return _preferredSpeed;
	}

	public virtual void SignalStop()
	{
	}

	public virtual void CloneFrom(IDefineSpeed speedComponent)
	{
		maxAngularAcceleration = speedComponent.maxAngularAcceleration;
		maximumAngularSpeed = speedComponent.maximumAngularSpeed;
		maxAcceleration = speedComponent.maxAcceleration;
		maxDeceleration = speedComponent.maxDeceleration;
		minimumSpeed = speedComponent.minimumSpeed;
		maximumSpeed = speedComponent.maximumSpeed;
		SpeedComponent speedComponent2 = speedComponent as SpeedComponent;
		if (speedComponent2 != null)
		{
			_preferredSpeed = speedComponent2._preferredSpeed;
		}
	}
}
