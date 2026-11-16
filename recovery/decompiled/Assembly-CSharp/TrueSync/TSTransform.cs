using UnityEngine;

namespace TrueSync;

[ExecuteInEditMode]
public class TSTransform : MonoBehaviour
{
	private const float DELTA_TIME_FACTOR = 10f;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private TSVector _position;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private TSQuaternion _rotation;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private TSVector _scale;

	[SerializeField]
	[HideInInspector]
	private bool _serialized;

	[HideInInspector]
	public TSCollider tsCollider;

	[HideInInspector]
	public TSTransform tsParent;

	private bool initialized;

	private TSRigidBody rb;

	public _Ship shipscript;

	public TSVector position
	{
		get
		{
			if (tsCollider != null && tsCollider.Body != null)
			{
				return tsCollider.Body.TSPosition - scaledCenter;
			}
			return _position;
		}
		set
		{
			_position = value;
			if (tsCollider != null && tsCollider.Body != null)
			{
				tsCollider.Body.TSPosition = _position + scaledCenter;
			}
		}
	}

	public TSQuaternion rotation
	{
		get
		{
			if (tsCollider != null && tsCollider.Body != null)
			{
				return TSQuaternion.CreateFromMatrix(tsCollider.Body.TSOrientation);
			}
			return _rotation;
		}
		set
		{
			_rotation = value;
			if (tsCollider != null && tsCollider.Body != null)
			{
				tsCollider.Body.TSOrientation = TSMatrix.CreateFromQuaternion(_rotation);
			}
		}
	}

	public TSVector scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
		}
	}

	private TSVector scaledCenter
	{
		get
		{
			if (tsCollider != null)
			{
				return tsCollider.ScaledCenter;
			}
			return TSVector.zero;
		}
	}

	public TSVector forward => TSVector.Transform(TSVector.forward, TSMatrix.CreateFromQuaternion(rotation));

	public TSVector right => TSVector.Transform(TSVector.right, TSMatrix.CreateFromQuaternion(rotation));

	public TSVector up => TSVector.Transform(TSVector.up, TSMatrix.CreateFromQuaternion(rotation));

	public TSVector eulerAngles => rotation.eulerAngles;

	public void LookAt(TSTransform other)
	{
		LookAt(other.position);
	}

	public void LookAt(TSVector target)
	{
		rotation = TSQuaternion.CreateFromMatrix(TSMatrix.CreateFromLookAt(position, target));
	}

	public void Translate(FP x, FP y, FP z)
	{
		Translate(x, y, z, Space.Self);
	}

	public void Translate(FP x, FP y, FP z, Space relativeTo)
	{
		Translate(new TSVector(x, y, z), relativeTo);
	}

	public void Translate(FP x, FP y, FP z, TSTransform relativeTo)
	{
		Translate(new TSVector(x, y, z), relativeTo);
	}

	public void Translate(TSVector translation)
	{
		Translate(translation, Space.Self);
	}

	public void Translate(TSVector translation, Space relativeTo)
	{
		if (relativeTo == Space.Self)
		{
			Translate(translation, this);
		}
		else
		{
			position += translation;
		}
	}

	public void Translate(TSVector translation, TSTransform relativeTo)
	{
		position += TSVector.Transform(translation, TSMatrix.CreateFromQuaternion(relativeTo.rotation));
	}

	public void RotateAround(TSVector point, TSVector axis, FP angle)
	{
		TSVector tSVector = position;
		TSVector tSVector2 = tSVector - point;
		tSVector2 = TSVector.Transform(tSVector2, TSMatrix.AngleAxis(angle * FP.Deg2Rad, axis));
		tSVector = point + tSVector2;
		position = tSVector;
		Rotate(axis, angle);
	}

	public void RotateAround(TSVector axis, FP angle)
	{
		Rotate(axis, angle);
	}

	public void Rotate(FP xAngle, FP yAngle, FP zAngle)
	{
		Rotate(new TSVector(xAngle, yAngle, zAngle), Space.Self);
	}

	public void Rotate(FP xAngle, FP yAngle, FP zAngle, Space relativeTo)
	{
		Rotate(new TSVector(xAngle, yAngle, zAngle), relativeTo);
	}

	public void Rotate(TSVector eulerAngles)
	{
		Rotate(eulerAngles, Space.Self);
	}

	public void Rotate(TSVector axis, FP angle)
	{
		Rotate(axis, angle, Space.Self);
	}

	public void Rotate(TSVector axis, FP angle, Space relativeTo)
	{
		TSQuaternion identity = TSQuaternion.identity;
		identity = ((relativeTo != Space.Self) ? (TSQuaternion.AngleAxis(angle, axis) * rotation) : (rotation * TSQuaternion.AngleAxis(angle, axis)));
		identity.Normalize();
		rotation = identity;
	}

	public void Rotate(TSVector eulerAngles, Space relativeTo)
	{
		TSQuaternion identity = TSQuaternion.identity;
		identity = ((relativeTo != Space.Self) ? (TSQuaternion.Euler(eulerAngles) * rotation) : (rotation * TSQuaternion.Euler(eulerAngles)));
		identity.Normalize();
		rotation = identity;
	}

	public void Start()
	{
		if (Application.isPlaying)
		{
			shipscript = gameObject.GetComponent<_Ship>();
			Initialize();
			rb = GetComponent<TSRigidBody>();
		}
	}

	public void Initialize()
	{
		if (initialized)
		{
			return;
		}
		tsCollider = GetComponent<TSCollider>();
		if (transform.parent != null)
		{
			tsParent = transform.parent.GetComponent<TSTransform>();
		}
		if (!_serialized)
		{
			UpdateEditMode();
		}
		if (tsCollider != null)
		{
			if (tsCollider.IsBodyInitialized)
			{
				tsCollider.Body.TSPosition = _position + scaledCenter;
				tsCollider.Body.TSOrientation = TSMatrix.CreateFromQuaternion(_rotation);
			}
		}
		else
		{
			StateTracker.AddTracking(this);
		}
		initialized = true;
	}

	public void Update()
	{
		if (Application.isPlaying)
		{
			if (initialized)
			{
				UpdatePlayMode();
			}
		}
		else
		{
			UpdateEditMode();
		}
	}

	private void UpdateEditMode()
	{
		if (transform.hasChanged)
		{
			_position = transform.localPosition.ToTSVector();
			_rotation = transform.rotation.ToTSQuaternion();
			_serialized = true;
		}
	}

	private void UpdatePlayMode()
	{
		if (rb != null)
		{
			if (rb.interpolation == TSRigidBody.InterpolateMode.Interpolate)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, position.ToVector(), Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, rotation.ToQuaternion(), Time.deltaTime * 10f);
				return;
			}
			if (rb.interpolation == TSRigidBody.InterpolateMode.Extrapolate)
			{
				transform.localPosition = (position + rb.tsCollider.Body.TSLinearVelocity * Time.deltaTime * 10f).ToVector();
				transform.rotation = Quaternion.Lerp(transform.rotation, rotation.ToQuaternion(), Time.deltaTime * 10f);
				return;
			}
		}
		transform.localPosition = position.ToVector();
		transform.rotation = rotation.ToQuaternion();
	}
}
