using UnityEngine;

namespace TrueSync;

[ExecuteInEditMode]
public class TSTransform2D : MonoBehaviour
{
	private const float DELTA_TIME_FACTOR = 10f;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private TSVector2 _position;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private FP _rotation;

	[SerializeField]
	[HideInInspector]
	[AddTracking]
	private TSVector _scale;

	[SerializeField]
	[HideInInspector]
	private bool _serialized;

	[HideInInspector]
	public TSCollider2D tsCollider;

	[HideInInspector]
	public TSTransform2D tsParent;

	private bool initialized;

	private TSRigidBody2D rb;

	public TSVector2 position
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

	public FP rotation
	{
		get
		{
			if (tsCollider != null && tsCollider.Body != null)
			{
				return tsCollider.Body.TSOrientation * FP.Rad2Deg;
			}
			return _rotation;
		}
		set
		{
			_rotation = value;
			if (tsCollider != null && tsCollider.Body != null)
			{
				tsCollider.Body.TSOrientation = _rotation * FP.Deg2Rad;
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

	private TSVector2 scaledCenter
	{
		get
		{
			if (tsCollider != null)
			{
				return tsCollider.ScaledCenter;
			}
			return TSVector2.zero;
		}
	}

	public void Start()
	{
		if (Application.isPlaying)
		{
			Initialize();
			rb = GetComponent<TSRigidBody2D>();
		}
	}

	public void Initialize()
	{
		if (initialized)
		{
			return;
		}
		tsCollider = GetComponent<TSCollider2D>();
		if (transform.parent != null)
		{
			tsParent = transform.parent.GetComponent<TSTransform2D>();
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
				tsCollider.Body.TSOrientation = _rotation * FP.Deg2Rad;
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
			_position = transform.position.ToTSVector2();
			_rotation = transform.rotation.eulerAngles.z;
			_scale = transform.localScale.ToTSVector();
			_serialized = true;
		}
	}

	private void UpdatePlayMode()
	{
		if (rb != null)
		{
			if (rb.interpolation == TSRigidBody2D.InterpolateMode.Interpolate)
			{
				transform.position = Vector3.Lerp(transform.position, position.ToVector(), Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rotation.AsFloat()), Time.deltaTime * 10f);
				transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * 10f);
				return;
			}
			if (rb.interpolation == TSRigidBody2D.InterpolateMode.Extrapolate)
			{
				transform.position = (position + tsCollider.Body.TSLinearVelocity * Time.deltaTime * 10f).ToVector();
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rotation.AsFloat()), Time.deltaTime * 10f);
				transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * 10f);
				return;
			}
		}
		transform.position = position.ToVector();
		Quaternion quaternion = transform.rotation;
		quaternion.eulerAngles = new Vector3(0f, 0f, rotation.AsFloat());
		transform.rotation = quaternion;
		transform.localScale = scale.ToVector();
	}
}
