using System;
using Apex.Common;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class HeightMapBoxThreePointProvider : IUnitHeightProvider
{
	private Vector3[] _points = new Vector3[3];

	private Vector3[] _samplePoints = new Vector3[3];

	private HighPointList _pendingHighMaxes;

	public HeightMapBoxThreePointProvider(Collider c)
	{
		Ensure.ArgumentNotNull(c, "c");
		Transform transform = c.transform;
		ColliderMeasurement colliderMeasurement = new ColliderMeasurement(c, inWorldSpace: false);
		ref Vector3 reference = ref _points[0];
		reference = colliderMeasurement.center + new Vector3(0f, 0f - colliderMeasurement.extents.y, 0f);
		ref Vector3 reference2 = ref _points[1];
		reference2 = colliderMeasurement.center + new Vector3(0f, 0f - colliderMeasurement.extents.y, colliderMeasurement.extents.z);
		ref Vector3 reference3 = ref _points[2];
		reference3 = colliderMeasurement.center + new Vector3(0f, 0f - colliderMeasurement.extents.y, 0f - colliderMeasurement.extents.z);
		_pendingHighMaxes = new HighPointList(2, transform.TransformPoint(_points[1]));
	}

	public float GetHeightDelta(SteeringInput input)
	{
		IUnitFacade unit = input.unit;
		IHeightMap heightMap = HeightMapManager.instance.GetHeightMap(unit.position);
		Vector3 vector = input.currentFullVelocity.OnlyXZ();
		if (vector.sqrMagnitude < 0.0001f && vector.y > 0f)
		{
			vector = input.currentPlanarVelocity;
		}
		float num = heightMap.granularity * 1.414214f;
		Vector3 vector2 = vector * input.deltaTime;
		Vector3 vector3 = ((!(vector2.sqrMagnitude < num * num)) ? vector2 : (vector.normalized * num));
		Transform transform = input.unit.transform;
		ref Vector3 reference = ref _samplePoints[0];
		reference = transform.TransformPoint(_points[0]) + vector2;
		ref Vector3 reference2 = ref _samplePoints[1];
		reference2 = transform.TransformPoint(_points[1]) + vector3;
		ref Vector3 reference3 = ref _samplePoints[2];
		reference3 = transform.TransformPoint(_points[2]) + vector2;
		float y = _samplePoints[0].y;
		float maxClimbHeight = unit.heightNavigationCapability.maxClimbHeight;
		float groundOffset = unit.groundOffset;
		float num2 = -10000f;
		int num3 = 0;
		for (int i = 0; i < 3; i++)
		{
			float num4 = heightMap.SampleHeight(_samplePoints[i]) + groundOffset;
			if (num4 > num2)
			{
				num2 = num4;
				num3 = i;
			}
			_samplePoints[i].y = num4;
		}
		_pendingHighMaxes.RegisterHighpoint(_samplePoints[1]);
		if (num3 != 1 && _pendingHighMaxes.count > 0 && Vector3.Dot(vector2, _pendingHighMaxes.current - _samplePoints[2]) < 0f)
		{
			_pendingHighMaxes.MoveNext();
		}
		Vector3 vector4 = _samplePoints[num3];
		if (_pendingHighMaxes.count > 0 && _pendingHighMaxes.currentHeight > num2)
		{
			vector4 = _pendingHighMaxes.current;
		}
		float num5 = vector4.y - y;
		Vector3 normalized = (vector4 - _samplePoints[0]).normalized;
		float num6 = Vector3.Dot(Vector3.up, normalized);
		float num7 = Mathf.Cos((90f - unit.heightNavigationCapability.maxSlopeAngle) * ((float)Math.PI / 180f));
		if (num6 > num7 && num5 > maxClimbHeight)
		{
			return 0f;
		}
		return num5;
	}
}
