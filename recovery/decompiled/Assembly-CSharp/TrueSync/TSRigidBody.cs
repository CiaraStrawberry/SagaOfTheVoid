using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[RequireComponent(typeof(TSCollider))]
[AddComponentMenu("TrueSync/Physics/TSRigidBody", 11)]
public class TSRigidBody : MonoBehaviour
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

	[SerializeField]
	[HideInInspector]
	private TSRigidBodyConstraints _constraints;

	private TSCollider _tsCollider;

	private TSTransform _tsTransform;

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

	public TSRigidBodyConstraints constraints
	{
		get
		{
			if (tsCollider.IsBodyInitialized)
			{
				return tsCollider._body.FreezeConstraints;
			}
			return _constraints;
		}
		set
		{
			_constraints = value;
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider._body.FreezeConstraints = value;
			}
		}
	}

	public TSCollider tsCollider
	{
		get
		{
			if (_tsCollider == null)
			{
				_tsCollider = GetComponent<TSCollider>();
			}
			return _tsCollider;
		}
	}

	public TSTransform tsTransform
	{
		get
		{
			if (_tsTransform == null)
			{
				_tsTransform = GetComponent<TSTransform>();
			}
			return _tsTransform;
		}
	}

	public TSVector position
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

	public TSQuaternion rotation
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

	public TSVector velocity
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

	public TSVector angularVelocity
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

	public void AddForce(TSVector force)
	{
		AddForce(force, ForceMode.Force);
	}

	public void AddForce(TSVector force, ForceMode mode)
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

	public void AddForceAtPosition(TSVector force, TSVector position)
	{
		AddForceAtPosition(force, position, ForceMode.Impulse);
	}

	public void AddForceAtPosition(TSVector force, TSVector position, ForceMode mode)
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

	public TSVector GetPointVelocity(TSVector worldPoint)
	{
		TSVector vector = position - tsCollider.Body.TSPosition;
		return TSVector.Cross(tsCollider.Body.TSAngularVelocity, vector) + tsCollider.Body.TSLinearVelocity;
	}

	public void AddTorque(TSVector torque)
	{
		tsCollider.Body.TSApplyTorque(torque);
	}

	public void LookAt(TSVector target)
	{
		rotation = TSQuaternion.CreateFromMatrix(TSMatrix.CreateFromLookAt(position, target));
	}

	public void MovePosition(TSVector position)
	{
		this.position = position;
	}

	public void MoveRotation(TSQuaternion rot)
	{
		rotation = rot;
	}
}
