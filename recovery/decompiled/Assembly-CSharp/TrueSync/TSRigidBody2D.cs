using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[RequireComponent(typeof(TSCollider2D))]
[AddComponentMenu("TrueSync/Physics/TSRigidBody2D", 11)]
public class TSRigidBody2D : MonoBehaviour
{
	public enum InterpolateMode
	{
		None,
		Interpolate,
		Extrapolate
	}

	[FormerlySerializedAs("mass")]
	[SerializeField]
	private FP _mass = 1;

	[SerializeField]
	private bool _useGravity = true;

	[SerializeField]
	private bool _isKinematic;

	[SerializeField]
	private FP _linearDrag;

	[SerializeField]
	private FP _angularDrag = 0.05f;

	public InterpolateMode interpolation;

	public bool freezeZAxis;

	private TSCollider2D _tsCollider;

	private TSTransform2D _tsTransform;

	public FP mass
	{
		get
		{
			if (tsCollider._body != null)
			{
				return tsCollider._body.Mass;
			}
			return _mass;
		}
		set
		{
			_mass = value;
			if (tsCollider._body != null)
			{
				tsCollider._body.Mass = value;
			}
		}
	}

	public bool useGravity
	{
		get
		{
			if (tsCollider.IsBodyInitialized)
			{
				return tsCollider.Body.TSAffectedByGravity;
			}
			return _useGravity;
		}
		set
		{
			_useGravity = value;
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider.Body.TSAffectedByGravity = _useGravity;
			}
		}
	}

	public bool isKinematic
	{
		get
		{
			if (tsCollider.IsBodyInitialized)
			{
				return tsCollider.Body.TSIsKinematic;
			}
			return _isKinematic;
		}
		set
		{
			_isKinematic = value;
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider.Body.TSIsKinematic = _isKinematic;
			}
		}
	}

	public FP drag
	{
		get
		{
			if (tsCollider.IsBodyInitialized)
			{
				return tsCollider.Body.TSLinearDrag;
			}
			return _linearDrag;
		}
		set
		{
			_linearDrag = value;
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider.Body.TSLinearDrag = _linearDrag;
			}
		}
	}

	public FP angularDrag
	{
		get
		{
			if (tsCollider.IsBodyInitialized)
			{
				return tsCollider.Body.TSAngularDrag;
			}
			return _angularDrag;
		}
		set
		{
			_angularDrag = value;
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider.Body.TSAngularDrag = _angularDrag;
			}
		}
	}

	public TSCollider2D tsCollider
	{
		get
		{
			if (_tsCollider == null)
			{
				_tsCollider = GetComponent<TSCollider2D>();
			}
			return _tsCollider;
		}
	}

	private TSTransform2D tsTransform
	{
		get
		{
			if (_tsTransform == null)
			{
				_tsTransform = GetComponent<TSTransform2D>();
			}
			return _tsTransform;
		}
	}

	public TSVector2 position
	{
		get
		{
			return tsTransform.position;
		}
		set
		{
			tsTransform.position = value;
		}
	}

	public FP rotation
	{
		get
		{
			return tsTransform.rotation;
		}
		set
		{
			tsTransform.rotation = value;
		}
	}

	public TSVector2 velocity
	{
		get
		{
			return tsCollider.Body.TSLinearVelocity;
		}
		set
		{
			tsCollider.Body.TSLinearVelocity = value;
		}
	}

	public FP angularVelocity
	{
		get
		{
			return tsCollider.Body.TSAngularVelocity;
		}
		set
		{
			tsCollider.Body.TSAngularVelocity = value;
		}
	}

	public void AddForce(TSVector2 force)
	{
		AddForce(force, ForceMode.Force);
	}

	public void AddForce(TSVector2 force, ForceMode mode)
	{
		switch (mode)
		{
		case ForceMode.Force:
			tsCollider.Body.TSApplyForce(force);
			break;
		case ForceMode.Impulse:
			tsCollider.Body.TSApplyImpulse(force);
			break;
		}
	}

	public void AddForceAtPosition(TSVector2 force, TSVector2 position)
	{
		AddForceAtPosition(force, position, ForceMode.Impulse);
	}

	public void AddForceAtPosition(TSVector2 force, TSVector2 position, ForceMode mode)
	{
		switch (mode)
		{
		case ForceMode.Force:
			tsCollider.Body.TSApplyForce(force, position);
			break;
		case ForceMode.Impulse:
			tsCollider.Body.TSApplyImpulse(force, position);
			break;
		}
	}

	public TSVector2 GetPointVelocity(TSVector2 worldPoint)
	{
		TSVector vector = (position - tsCollider.Body.TSPosition).ToTSVector();
		return TSVector.Cross(new TSVector(0, 0, tsCollider.Body.TSAngularVelocity), vector).ToTSVector2() + tsCollider.Body.TSLinearVelocity;
	}

	public void AddTorque(TSVector2 torque)
	{
		tsCollider.Body.TSApplyTorque(torque);
	}

	public void MovePosition(TSVector2 position)
	{
		this.position = position;
	}

	public void MoveRotation(FP rot)
	{
		rotation = rot;
	}
}
