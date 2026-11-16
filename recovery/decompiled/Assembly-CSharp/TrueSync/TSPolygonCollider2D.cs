using System.Collections.Generic;
using TrueSync.Physics2D;
using UnityEngine;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/PolygonCollider2D", 0)]
public class TSPolygonCollider2D : TSCollider2D
{
	[SerializeField]
	private TSVector2[] _points;

	public TSVector2[] points
	{
		get
		{
			return _points;
		}
		set
		{
			if (_body == null)
			{
				_points = value;
			}
		}
	}

	public override Shape[] CreateShapes()
	{
		if (_points == null || _points.Length == 0)
		{
			return null;
		}
		TSVector2 value = new TSVector2(lossyScale.x, lossyScale.y);
		Vertices vertices = new Vertices();
		int i = 0;
		for (int num = _points.Length; i < num; i++)
		{
			vertices.Add(TSVector2.Scale(_points[i], value));
		}
		List<Vertices> list = BayazitDecomposer.ConvexPartition(vertices);
		Shape[] array = new Shape[list.Count];
		int j = 0;
		for (int num2 = array.Length; j < num2; j++)
		{
			array[j] = new PolygonShape(list[j], 1);
		}
		return array;
	}

	protected override void DrawGizmos()
	{
		DrawPolygon(_points);
	}

	private void DrawPolygon(TSVector2[] allPoints)
	{
		if (allPoints != null && allPoints.Length != 0)
		{
			int i = 0;
			for (int num = allPoints.Length - 1; i < num; i++)
			{
				Gizmos.DrawLine(allPoints[i].ToVector(), allPoints[i + 1].ToVector());
			}
			Gizmos.DrawLine(allPoints[allPoints.Length - 1].ToVector(), allPoints[0].ToVector());
		}
	}

	protected override Vector3 GetGizmosSize()
	{
		return lossyScale.ToVector();
	}
}
