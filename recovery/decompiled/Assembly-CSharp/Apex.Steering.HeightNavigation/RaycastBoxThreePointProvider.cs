using System;
using Apex.Common;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class RaycastBoxThreePointProvider : IUnitHeightProvider
{
	private Vector3[] _points = new Vector3[3];

	private Vector3[] _samplePoints = new Vector3[3];

	private HighPointList _pendingHighMaxes;

	public RaycastBoxThreePointProvider(Collider c)
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
		Vector3 vector = input.currentFullVelocity.OnlyXZ() * input.deltaTime;
		if (vector.sqrMagnitude < 0.0001f && input.currentFullVelocity.y > 0f)
		{
			vector = input.currentPlanarVelocity.normalized * 0.01f;
		}
		Transform transform = input.unit.transform;
		ref Vector3 reference = ref _samplePoints[0];
		reference = transform.TransformPoint(_points[0]) + vector;
		ref Vector3 reference2 = ref _samplePoints[1];
		reference2 = transform.TransformPoint(_points[1]) + vector;
		ref Vector3 reference3 = ref _samplePoints[2];
		reference3 = transform.TransformPoint(_points[2]) + vector;
		float y = _samplePoints[0].y;
		float maxClimbHeight = unit.heightNavigationCapability.maxClimbHeight;
		float groundOffset = unit.groundOffset;
		IGrid grid = input.grid;
		float y2 = ((grid == null) ? (_samplePoints[0].y + maxClimbHeight) : (grid.cellMatrix.origin.y + grid.cellMatrix.upperBoundary));
		float num = -10000f;
		int num2 = 0;
		Vector3 rhs = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			Vector3 origin = _samplePoints[i];
			origin.y = y2;
			if (Physics.Raycast(origin, Vector3.down, out var hitInfo, float.PositiveInfinity, Layers.terrain))
			{
				float num3 = hitInfo.point.y + groundOffset;
				if (num3 > num)
				{
					num = num3;
					rhs = hitInfo.normal;
					num2 = i;
				}
				_samplePoints[i].y = num3;
			}
		}
		_pendingHighMaxes.RegisterHighpoint(_samplePoints[1]);
		if (num2 != 1 && _pendingHighMaxes.count > 0 && Vector3.Dot(vector, _pendingHighMaxes.current - _samplePoints[2]) < 0f)
		{
			_pendingHighMaxes.MoveNext();
		}
		if (_pendingHighMaxes.count > 0 && _pendingHighMaxes.currentHeight > num)
		{
			num = _pendingHighMaxes.currentHeight;
		}
		float num4 = num - y;
		float num5 = Vector3.Dot(Vector3.up, rhs);
		float num6 = Mathf.Cos(unit.heightNavigationCapability.maxSlopeAngle * ((float)Math.PI / 180f));
		if (num5 < num6 && num4 > maxClimbHeight)
		{
			return 0f;
		}
		return num4;
	}
}
