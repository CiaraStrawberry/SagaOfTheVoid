using TrueSync.Physics2D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/CircleCollider2D", 0)]
public class TSCircleCollider2D : TSCollider2D
{
	[FormerlySerializedAs("radius")]
	[SerializeField]
	private FP _radius = 0.5f;

	public FP radius
	{
		get
		{
			if (_body != null)
			{
				return ((CircleShape)_body.FixtureList[0].Shape).Radius;
			}
			return _radius;
		}
		set
		{
			_radius = value;
			if (_body != null)
			{
				CircleShape circleShape = (CircleShape)_body.FixtureList[0].Shape;
				if (circleShape.Radius != _radius)
				{
					circleShape.Radius = _radius;
				}
			}
		}
	}

	public void Reset()
	{
		if (GetComponent<CircleCollider2D>() != null)
		{
			CircleCollider2D component = GetComponent<CircleCollider2D>();
			radius = component.radius;
			Center = new TSVector2(component.offset.x, component.offset.y);
			isTrigger = component.isTrigger;
		}
		else if (GetComponent<SphereCollider>() != null)
		{
			SphereCollider component2 = GetComponent<SphereCollider>();
			radius = component2.radius;
			Center = component2.center.ToTSVector2();
			isTrigger = component2.isTrigger;
		}
	}

	public override Shape CreateShape()
	{
		return new CircleShape(radius, 1);
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
