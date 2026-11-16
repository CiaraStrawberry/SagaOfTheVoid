using UnityEngine;

namespace Apex.Steering;

public sealed class SteeringOutput
{
	private bool _hasOutput;

	private bool _pause;

	private Vector3 _desiredAcceleration;

	private float _verticalForce;

	public float maxAllowedSpeed;

	public bool overrideHeightNavigation;

	public Vector3 desiredAcceleration
	{
		get
		{
			return _desiredAcceleration;
		}
		set
		{
			_desiredAcceleration = value;
			_hasOutput = true;
		}
	}

	public float verticalForce
	{
		get
		{
			return _verticalForce;
		}
		set
		{
			_verticalForce = value;
			_hasOutput = true;
		}
	}

	public bool pause
	{
		get
		{
			return _pause;
		}
		set
		{
			_pause = value;
			_hasOutput = true;
		}
	}

	public bool hasOutput => _hasOutput;

	public void Clear()
	{
		_desiredAcceleration = Vector3.zero;
		_verticalForce = 0f;
		_hasOutput = false;
		_pause = false;
		overrideHeightNavigation = false;
		maxAllowedSpeed = 0f;
	}
}
