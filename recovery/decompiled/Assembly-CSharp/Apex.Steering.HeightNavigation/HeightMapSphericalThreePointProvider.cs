using System;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class HeightMapSphericalThreePointProvider : IUnitHeightProvider
{
	private Vector3[] _points = new Vector3[3];

	private Vector3[] _samplePoints = new Vector3[3];

	private float _radius;

	private HighPointList _pendingHighMaxes;

	public HeightMapSphericalThreePointProvider(Collider c)
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
		Vector3 vector4 = (reference = transform.TransformPoint(_points[0]) + vector2);
		ref Vector3 reference2 = ref _samplePoints[1];
		reference2 = transform.TransformPoint(_points[1]) + vector3;
		ref Vector3 reference3 = ref _samplePoints[2];
		reference3 = transform.TransformPoint(_points[2]) + vector2;
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
		if (num3 != 1 && _pendingHighMaxes.count > 0 && vector4.DirToXZ(_pendingHighMaxes.current).sqrMagnitude > _radius * _radius)
		{
			_pendingHighMaxes.MoveNext();
		}
		Vector3 vector5 = _samplePoints[num3];
		if (_pendingHighMaxes.count > 0 && (_pendingHighMaxes.currentHeight > num2 || (vector5 - vector4).sqrMagnitude > (_pendingHighMaxes.current - vector4).sqrMagnitude))
		{
			vector5 = _pendingHighMaxes.current;
		}
		else if (num3 == 0)
		{
			return num2 - (vector4.y - _radius);
		}
		Vector3 vector6 = _samplePoints[0];
		Vector3 normalized = (vector6 - vector5).normalized;
		Vector3 lhs = vector4 - vector5;
		float num5 = Vector3.Dot(lhs, normalized);
		Vector3 vector7 = ((!(num5 > 0f)) ? vector5 : (vector5 + normalized * num5));
		Vector3 vector8 = vector4 + (vector7 - vector4).normalized * _radius;
		Vector3 vector9 = new Vector3(vector5.z, vector5.y, 0f - vector5.x);
		Vector3 vector10 = Vector3.Cross(vector9 - vector5, vector5 - vector6);
		float num6 = (vector10.x * (vector8.x - vector5.x) + vector10.z * (vector8.z - vector5.z) - vector10.y * vector5.y) / (0f - vector10.y);
		float num7 = num6 - vector8.y;
		float num8 = Vector3.Dot(Vector3.up, -normalized);
		float num9 = Mathf.Cos((90f - unit.heightNavigationCapability.maxSlopeAngle) * ((float)Math.PI / 180f));
		if (num8 > num9 && num7 > maxClimbHeight)
		{
			return 0f;
		}
		return num7;
	}
}
