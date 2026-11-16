using System;
using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[Serializable]
[RequireComponent(typeof(TSTransform))]
[ExecuteInEditMode]
public abstract class TSCollider : MonoBehaviour, ICollider
{
	private Shape shape;

	[FormerlySerializedAs("isTrigger")]
	[SerializeField]
	private bool _isTrigger;

	public TSMaterial tsMaterial;

	[SerializeField]
	private TSVector center;

	private Vector3 scaledCenter;

	internal RigidBody _body;

	private TSRigidBody tsRigidBody;

	[SerializeField]
	[HideInInspector]
	protected TSVector lossyScale = TSVector.one;

	[HideInInspector]
	public TSTransform tsTransform;

	public Shape Shape
	{
		get
		{
			if (shape == null)
			{
				shape = CreateShape();
			}
			return shape;
		}
		protected set
		{
			shape = value;
		}
	}

	public bool isTrigger
	{
		get
		{
			if (_body != null)
			{
				return _body.IsColliderOnly;
			}
			return _isTrigger;
		}
		set
		{
			_isTrigger = value;
			if (_body != null)
			{
				_body.IsColliderOnly = _isTrigger;
			}
		}
	}

	public TSVector Center
	{
		get
		{
			return center;
		}
		set
		{
			center = value;
		}
	}

	public TSVector ScaledCenter => TSVector.Scale(Center, lossyScale);

	public TSRigidBody attachedRigidbody => tsRigidBody;

	public TSBBox bounds => _body.BoundingBox;

	public IBody3D Body
	{
		get
		{
			if (_body == null)
			{
				CheckPhysics();
			}
			return _body;
		}
	}

	public bool IsBodyInitialized => _body != null;

	public abstract Shape CreateShape();

	public void Awake()
	{
		tsTransform = GetComponent<TSTransform>();
		tsRigidBody = GetComponent<TSRigidBody>();
		if (lossyScale == TSVector.one)
		{
			lossyScale = TSVector.Abs(transform.localScale.ToTSVector());
		}
	}

	public void Update()
	{
		if (!Application.isPlaying)
		{
			lossyScale = TSVector.Abs(transform.lossyScale.ToTSVector());
		}
	}

	private void CreateBody()
	{
		RigidBody rigidBody = new RigidBody(Shape);
		if (tsMaterial == null)
		{
			tsMaterial = GetComponent<TSMaterial>();
		}
		if (tsMaterial != null)
		{
			rigidBody.TSFriction = tsMaterial.friction;
			rigidBody.TSRestitution = tsMaterial.friction;
		}
		rigidBody.IsColliderOnly = isTrigger;
		rigidBody.IsKinematic = tsRigidBody != null && tsRigidBody.isKinematic;
		bool flag = tsRigidBody == null || tsRigidBody.isKinematic;
		if (tsRigidBody != null)
		{
			rigidBody.AffectedByGravity = tsRigidBody.useGravity;
			if (tsRigidBody.mass <= 0)
			{
				tsRigidBody.mass = 1;
			}
			rigidBody.Mass = tsRigidBody.mass;
			rigidBody.TSLinearDrag = tsRigidBody.drag;
			rigidBody.TSAngularDrag = tsRigidBody.angularDrag;
		}
		else
		{
			rigidBody.SetMassProperties();
		}
		if (flag)
		{
			rigidBody.AffectedByGravity = false;
			rigidBody.IsStatic = true;
		}
		_body = rigidBody;
	}

	public void Initialize()
	{
		CreateBody();
	}

	private void CheckPhysics()
	{
		if (_body == null && PhysicsManager.instance != null)
		{
			PhysicsManager.instance.AddBody(this);
		}
	}

	public virtual void OnDrawGizmos()
	{
		if (enabled)
		{
			Vector3 pos = ((_body == null) ? (transform.position + ScaledCenter.ToVector()) : _body.Position.ToVector());
			Quaternion q = ((_body == null) ? transform.rotation : _body.Orientation.ToQuaternion());
			Gizmos.color = Color.yellow;
			Matrix4x4 matrix4x = Matrix4x4.TRS(pos, q, GetGizmosSize());
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix *= matrix4x;
			DrawGizmos();
			Gizmos.matrix = matrix;
		}
	}

	protected abstract Vector3 GetGizmosSize();

	protected abstract void DrawGizmos();
}
