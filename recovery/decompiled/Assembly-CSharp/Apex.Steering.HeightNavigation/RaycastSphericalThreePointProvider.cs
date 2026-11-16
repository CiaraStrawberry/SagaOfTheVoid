using System;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class RaycastSphericalThreePointProvider : IUnitHeightProvider
{
	private Vector3[] _points = new Vector3[3];

	private Vector3[] _samplePoints = new Vector3[3];

	private float _radius;

	private HighPointList _pendingHighMaxes;

	public RaycastSphericalThreePointProvider(Collider c)
	{
		Ensure.ArgumentNotNull(c, "c");
		Transform transform = c.transform;
		Vector3 lossyScale = transform.lossyScale;
		SphereCollider sphereCollider = c as SphereCollider;
		CapsuleCollider capsuleCollider = c as CapsuleCollider;
		if (sphereCollider != null)
		{
			float num = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
			_radius = sphereCollider.radius * num;
			ref Vector3 reference = ref _points[0];
			reference = sphereCollider.center;
			ref Vector3 reference2 = ref _points[1];
			reference2 = sphereCollider.center + new Vector3(0f, 0f, sphereCollider.radius * num / lossyScale.z);
			ref Vector3 reference3 = ref _points[2];
			reference3 = sphereCollider.center + new Vector3(0f, 0f, (0f - sphereCollider.radius) * num / lossyScale.z);
		}
		else
		{
			if (!(capsuleCollider != null) || capsuleCollider.direction != 1)
			{
				throw new ArgumentException("A Spherical provider only works with sphere or vertical capsule colliders!");
			}
			float num2 = Mathf.Max(lossyScale.x, lossyScale.z);
			_radius = capsuleCollider.radius * num2;
			ref Vector3 reference4 = ref _points[0];
			reference4 = capsuleCollider.center + new Vector3(0f, Mathf.Min((0f - capsuleCollider.height) * 0.5f + _radius / lossyScale.y, 0f), 0f);
			ref Vector3 reference5 = ref _points[1];
			reference5 = capsuleCollider.center + new Vector3(0f, 0f, capsuleCollider.radius * num2 / lossyScale.z);
			ref Vector3 reference6 = ref _points[2];
			reference6 = capsuleCollider.center + new Vector3(0f, 0f, (0f - capsuleCollider.radius) * num2 / lossyScale.z);
		}
		_pendingHighMaxes = new HighPointList(2, transform.TransformPoint(_points[0]));
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
		Vector3 vector2 = (reference = transform.TransformPoint(_points[0]) + vector);
		ref Vector3 reference2 = ref _samplePoints[1];
		reference2 = transform.TransformPoint(_points[1]) + vector;
		ref Vector3 reference3 = ref _samplePoints[2];
		reference3 = transform.TransformPoint(_points[2]) + vector;
		float maxClimbHeight = unit.heightNavigationCapability.maxClimbHeight;
		float groundOffset = unit.groundOffset;
		IGrid grid = input.grid;
		float y = ((grid == null) ? (vector2.y + maxClimbHeight) : (grid.cellMatrix.origin.y + grid.cellMatrix.upperBoundary));
		float num = -10000f;
		int num2 = 0;
		Vector3 rhs = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			Vector3 origin = _samplePoints[i];
			origin.y = y;
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
		if (num2 != 1 && _pendingHighMaxes.count > 0 && vector2.DirToXZ(_pendingHighMaxes.current).sqrMagnitude > _radius * _radius)
		{
			_pendingHighMaxes.MoveNext();
		}
		Vector3 vector3 = _samplePoints[num2];
		if (_pendingHighMaxes.count > 0 && (_pendingHighMaxes.currentHeight > num || (vector3 - vector2).sqrMagnitude > (_pendingHighMaxes.current - vector2).sqrMagnitude))
		{
			vector3 = _pendingHighMaxes.current;
		}
		else if (num2 == 0)
		{
			return num - (vector2.y - _radius);
		}
		Vector3 vector4 = _samplePoints[0];
		Vector3 normalized = (vector4 - vector3).normalized;
		Vector3 lhs = vector2 - vector3;
		float num4 = Vector3.Dot(lhs, normalized);
		Vector3 vector5 = ((!(num4 > 0f)) ? vector3 : (vector3 + normalized * num4));
		Vector3 vector6 = vector2 + (vector5 - vector2).normalized * _radius;
		Vector3 vector7 = new Vector3(vector3.z, vector3.y, 0f - vector3.x);
		Vector3 vector8 = Vector3.Cross(vector7 - vector3, vector3 - vector4);
		float num5 = (vector8.x * (vector6.x - vector3.x) + vector8.z * (vector6.z - vector3.z) - vector8.y * vector3.y) / (0f - vector8.y);
		float num6 = num5 - vector6.y;
		float num7 = Vector3.Dot(Vector3.up, rhs);
		float num8 = Mathf.Cos(unit.heightNavigationCapability.maxSlopeAngle * ((float)Math.PI / 180f));
		if (num7 < num8 && num6 > maxClimbHeight)
		{
			return 0f;
		}
		return num6;
	}
}
