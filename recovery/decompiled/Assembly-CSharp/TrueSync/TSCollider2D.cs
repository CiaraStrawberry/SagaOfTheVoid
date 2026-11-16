using System;
using TrueSync.Physics2D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[Serializable]
[RequireComponent(typeof(TSTransform2D))]
[ExecuteInEditMode]
public abstract class TSCollider2D : MonoBehaviour, ICollider
{
	private Shape shape;

	[FormerlySerializedAs("isTrigger")]
	[SerializeField]
	private bool _isTrigger;

	public TSMaterial tsMaterial;

	[SerializeField]
	private TSVector2 center;

	private Vector3 scaledCenter;

	internal Body _body;

	private TSRigidBody2D tsRigidBody;

	[HideInInspector]
	public TSTransform2D tsTransform;

	[SerializeField]
	[HideInInspector]
	protected TSVector lossyScale = TSVector.one;

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
				return _body.IsSensor;
			}
			return _isTrigger;
		}
		set
		{
			_isTrigger = value;
			if (_body != null)
			{
				_body.IsSensor = value;
			}
		}
	}

	public IBody2D Body
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

	public TSVector2 Center
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

	public TSVector2 ScaledCenter => TSVector2.Scale(center, new TSVector2(lossyScale.x, lossyScale.y));

	public TSRigidBody2D attachedRigidbody => tsRigidBody;

	public bool IsBodyInitialized => _body != null;

	public virtual Shape CreateShape()
	{
		return null;
	}

	public virtual Shape[] CreateShapes()
	{
		return new Shape[0];
	}

	public void Awake()
	{
		tsTransform = GetComponent<TSTransform2D>();
		tsRigidBody = GetComponent<TSRigidBody2D>();
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

	private void CreateBodyFixture(Body body, Shape shape)
	{
		Fixture fixture = body.CreateFixture(shape);
		if (tsMaterial != null)
		{
			fixture.Friction = tsMaterial.friction;
			fixture.Restitution = tsMaterial.restitution;
		}
	}

	private void CreateBody(World world)
	{
		Body body = BodyFactory.CreateBody(world);
		if (tsMaterial == null)
		{
			tsMaterial = GetComponent<TSMaterial>();
		}
		Shape shape = Shape;
		if (shape != null)
		{
			CreateBodyFixture(body, shape);
		}
		else
		{
			Shape[] array = CreateShapes();
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				CreateBodyFixture(body, array[i]);
			}
		}
		if (tsRigidBody == null)
		{
			body.BodyType = BodyType.Static;
		}
		else
		{
			if (tsRigidBody.isKinematic)
			{
				body.BodyType = BodyType.Kinematic;
			}
			else
			{
				body.BodyType = BodyType.Dynamic;
				body.IgnoreGravity = !tsRigidBody.useGravity;
			}
			if (tsRigidBody.mass <= 0)
			{
				tsRigidBody.mass = 1;
			}
			body.FixedRotation = tsRigidBody.freezeZAxis;
			body.Mass = tsRigidBody.mass;
			body.TSLinearDrag = tsRigidBody.drag;
			body.TSAngularDrag = tsRigidBody.angularDrag;
		}
		body.IsSensor = isTrigger;
		body.CollidesWith = Category.All;
		_body = body;
	}

	public void Initialize(World world)
	{
		CreateBody(world);
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
			Vector3 pos = ((_body == null) ? (transform.position + ScaledCenter.ToVector()) : _body.TSPosition.ToVector());
			Quaternion q = ((_body == null) ? transform.rotation : Quaternion.Euler(0f, 0f, (_body.Rotation * FP.Rad2Deg).AsFloat()));
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
