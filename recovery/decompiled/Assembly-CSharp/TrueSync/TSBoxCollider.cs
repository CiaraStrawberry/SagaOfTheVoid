using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/BoxCollider", 0)]
public class TSBoxCollider : TSCollider
{
	[FormerlySerializedAs("size")]
	[SerializeField]
	private Vector3 _size = Vector3.one;

	public TSVector size
	{
		get
		{
			if (_body != null)
			{
				TSVector result = ((BoxShape)_body.Shape).Size;
				result.x /= lossyScale.x;
				result.y /= lossyScale.y;
				result.z /= lossyScale.z;
				return result;
			}
			return _size.ToTSVector();
		}
		set
		{
			_size = value.ToVector();
			if (_body != null)
			{
				((BoxShape)_body.Shape).Size = TSVector.Scale(value, lossyScale);
			}
		}
	}

	public void Reset()
	{
		if (GetComponent<BoxCollider2D>() != null)
		{
			BoxCollider2D component = GetComponent<BoxCollider2D>();
			size = new TSVector(component.size.x, component.size.y, 1);
			Center = new TSVector(component.offset.x, component.offset.y, 0);
			isTrigger = component.isTrigger;
		}
		else if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component2 = GetComponent<BoxCollider>();
			size = new TSVector(component2.size.x, component2.size.y, 1);
			Center = component2.center.ToTSVector();
			isTrigger = component2.isTrigger;
		}
	}

	public override Shape CreateShape()
	{
		return new BoxShape(TSVector.Scale(size, lossyScale));
	}

	protected override void DrawGizmos()
	{
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	protected override Vector3 GetGizmosSize()
	{
		return TSVector.Scale(size, lossyScale).ToVector();
	}
}
