using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/SphereCollider", 0)]
public class TSSphereCollider : TSCollider
{
	[FormerlySerializedAs("radius")]
	[SerializeField]
	private float _radius;

	public FP radius
	{
		get
		{
			if (_body != null)
			{
				return ((SphereShape)_body.Shape).Radius;
			}
			return _radius;
		}
		set
		{
			_radius = value.AsFloat();
			if (_body != null)
			{
				((SphereShape)_body.Shape).Radius = value;
			}
		}
	}

	public void Reset()
	{
		if (GetComponent<CircleCollider2D>() != null)
		{
			CircleCollider2D component = GetComponent<CircleCollider2D>();
			radius = component.radius;
			Center = new TSVector(component.offset.x, component.offset.y, 0);
			isTrigger = component.isTrigger;
		}
		else if (GetComponent<SphereCollider>() != null)
		{
			SphereCollider component2 = GetComponent<SphereCollider>();
			radius = component2.radius;
			Center = component2.center.ToTSVector();
			isTrigger = component2.isTrigger;
		}
	}

	public override Shape CreateShape()
	{
		return new SphereShape(radius);
	}

	protected override void DrawGizmos()
	{
		Gizmos.DrawWireSphere(Vector3.zero, 1f);
	}

	protected override Vector3 GetGizmosSize()
	{
		return Vector3.one * radius.AsFloat();
	}
}
