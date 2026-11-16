using System;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class CapsuleCastProvider : IUnitHeightProvider
{
	private float _radius;

	private float _centerOffsetY;

	private Vector3 _toEndPointOne;

	private Vector3 _toEndPointTwo;

	public CapsuleCastProvider(Collider c)
	{
		Ensure.ArgumentNotNull(c, "c");
		CapsuleCollider capsuleCollider = c as CapsuleCollider;
		if (capsuleCollider == null)
		{
			throw new ArgumentException("A capsule cast provider only works with capsule colliders (surprise)!");
		}
		Transform transform = c.transform;
		Vector3 lossyScale = transform.lossyScale;
		_radius = capsuleCollider.radius * Mathf.Max(lossyScale.x, lossyScale.z);
		float num = Mathf.Max(capsuleCollider.height * lossyScale.y * 0.5f - _radius, 0f);
		_toEndPointOne = (_toEndPointTwo = new Vector3(capsuleCollider.center.x * lossyScale.x, capsuleCollider.center.y * lossyScale.y, capsuleCollider.center.y * lossyScale.y));
		switch (capsuleCollider.direction)
		{
		case 0:
			_toEndPointOne.x -= num;
			_toEndPointTwo.x += num;
			_centerOffsetY = capsuleCollider.center.y * lossyScale.y;
			break;
		case 1:
			_toEndPointOne.y -= num;
			_toEndPointTwo.y += num;
			_centerOffsetY = capsuleCollider.center.y * lossyScale.y - num;
			break;
		case 2:
			_toEndPointOne.z -= num;
			_toEndPointTwo.z += num;
			_centerOffsetY = capsuleCollider.center.y * lossyScale.y;
			break;
		}
	}

	public float GetHeightDelta(SteeringInput input)
	{
		IUnitFacade unit = input.unit;
		float y = unit.basePosition.y;
		float maxClimbHeight = unit.heightNavigationCapability.maxClimbHeight;
		CellMatrix cellMatrix = input.grid?.cellMatrix;
		float y2 = ((cellMatrix == null) ? (y + Mathf.Max(unit.height, _radius + maxClimbHeight)) : (cellMatrix.origin.y + cellMatrix.upperBoundary));
		Vector3 vector = input.currentFullVelocity.OnlyXZ() * input.deltaTime;
		if (vector.sqrMagnitude < 0.0001f && input.currentFullVelocity.y > 0f)
		{
			vector = input.currentPlanarVelocity.normalized * 0.01f;
		}
		Vector3 point = unit.position + _toEndPointOne + vector;
		Vector3 point2 = unit.position + _toEndPointTwo + vector;
		point.y = (point2.y = y2);
		if (!Physics.CapsuleCast(point, point2, _radius, Vector3.down, out var hitInfo, float.PositiveInfinity, Layers.terrain))
		{
			return -10000f;
		}
		float num = hitInfo.point.y - y;
		float num2 = Vector3.Dot(Vector3.up, hitInfo.normal);
		float num3 = Mathf.Cos(unit.heightNavigationCapability.maxSlopeAngle * ((float)Math.PI / 180f));
		if (num2 > num3 || num <= maxClimbHeight)
		{
			return (hitInfo.point + hitInfo.normal * _radius).y - (unit.position.y + _centerOffsetY) + unit.groundOffset;
		}
		return 0f;
	}
}
