using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/CapsuleCollider", 0)]
public class TSCapsuleCollider : TSCollider
{
	[FormerlySerializedAs("radius")]
	[SerializeField]
	private FP _radius;

	[FormerlySerializedAs("length")]
	[SerializeField]
	private FP _length;

	public FP radius
	{
		get
		{
			if (_body != null)
			{
				return ((CapsuleShape)_body.Shape).Radius;
			}
			return _radius;
		}
		set
		{
			_radius = value;
			if (_body != null)
			{
				((CapsuleShape)_body.Shape).Radius = _radius;
			}
		}
	}

	public FP length
	{
		get
		{
			if (_body != null)
			{
				return ((CapsuleShape)_body.Shape).Length;
			}
			return _length;
		}
		set
		{
			_length = value;
			if (_body != null)
			{
				((CapsuleShape)_body.Shape).Length = _length;
			}
		}
	}

	public override Shape CreateShape()
	{
		return new CapsuleShape(length, radius);
	}

	protected override void DrawGizmos()
	{
		Gizmos.DrawWireSphere(Vector3.zero, 1f);
		Gizmos.DrawWireSphere(new TSVector(0, length / radius - 2 * radius, 0).ToVector(), 1f);
		Gizmos.DrawWireSphere(new TSVector(0, -length / radius + 2 * radius, 0).ToVector(), 1f);
	}

	protected override Vector3 GetGizmosSize()
	{
		return Vector3.one * radius.AsFloat();
	}
}
