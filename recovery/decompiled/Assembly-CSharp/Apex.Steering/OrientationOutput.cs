using UnityEngine;

namespace Apex.Steering;

public sealed class OrientationOutput
{
	public Vector3 desiredOrientation;

	private float _desiredAngularAcceleration;

	private bool _hasOutput;

	public float desiredAngularAcceleration
	{
		get
		{
			return _desiredAngularAcceleration;
		}
		set
		{
			_desiredAngularAcceleration = value;
			_hasOutput = true;
		}
	}

	public bool hasOutput => _hasOutput;

	public void Clear()
	{
		desiredOrientation = Vector3.zero;
		desiredAngularAcceleration = 0f;
		_hasOutput = false;
	}
}
