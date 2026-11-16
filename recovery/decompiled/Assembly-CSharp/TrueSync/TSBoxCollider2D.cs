using TrueSync.Physics2D;
using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/BoxCollider2D", 0)]
public class TSBoxCollider2D : TSCollider2D
{
	[FormerlySerializedAs("size")]
	[SerializeField]
	private TSVector2 _size = TSVector2.one;

	public TSVector2 size
	{
		get
		{
			if (_body != null)
			{
				TSVector2 result = ((PolygonShape)_body.FixtureList[0].Shape).Vertices[0] * 2;
				result.x /= lossyScale.x;
				result.y /= -lossyScale.y;
				return result;
			}
			return _size;
		}
		set
		{
			_size = value;
			if (_body != null)
			{
				TSVector vecA = new TSVector(_size.x, _size.y, 1);
				TSVector tSVector = TSVector.Scale(vecA, lossyScale);
				((PolygonShape)_body.FixtureList[0].Shape).Vertices = PolygonTools.CreateRectangle(tSVector.x * FP.Half, tSVector.y * FP.Half);
			}
		}
	}

	public void Reset()
	{
		if (GetComponent<BoxCollider2D>() != null)
		{
			BoxCollider2D component = GetComponent<BoxCollider2D>();
			size = new TSVector2(component.size.x, component.size.y);
			Center = new TSVector2(component.offset.x, component.offset.y);
			isTrigger = component.isTrigger;
		}
		else if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component2 = GetComponent<BoxCollider>();
			size = component2.size.ToTSVector2();
			Center = component2.center.ToTSVector2();
			isTrigger = component2.isTrigger;
		}
	}

	public override Shape CreateShape()
	{
		TSVector vecA = new TSVector(size.x, size.y, 1);
		TSVector tSVector = TSVector.Scale(vecA, lossyScale);
		return new PolygonShape(PolygonTools.CreateRectangle(tSVector.x * FP.Half, tSVector.y * FP.Half), 1);
	}

	protected override void DrawGizmos()
	{
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	protected override Vector3 GetGizmosSize()
	{
		TSVector vecA = new TSVector(size.x, size.y, 1);
		return TSVector.Scale(vecA, lossyScale).ToVector();
	}
}
