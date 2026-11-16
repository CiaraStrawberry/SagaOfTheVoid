using System;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

public class SphereCastProvider : IUnitHeightProvider
{
	private float _radius;

	private float _centerOffsetY;

	public SphereCastProvider(Collider c)
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
			_centerOffsetY = sphereCollider.center.y * num;
		}
		else
		{
			if (!(capsuleCollider != null) || capsuleCollider.direction != 1)
			{
				throw new ArgumentException("A sphere cast provider only works with sphere or vertical capsule colliders!");
			}
			_radius = capsuleCollider.radius * Mathf.Max(lossyScale.x, lossyScale.z);
			_centerOffsetY = capsuleCollider.center.y * lossyScale.y - Mathf.Max(capsuleCollider.height * lossyScale.y * 0.5f - _radius, 0f);
		}
	}

	public float GetHeightDelta(SteeringInput input)
	{
		IUnitFacade unit = input.unit;
		Vector3 basePosition = unit.basePosition;
		float y = basePosition.y;
		float maxClimbHeight = unit.heightNavigationCapability.maxClimbHeight;
		CellMatrix cellMatrix = input.grid?.cellMatrix;
		basePosition.y = ((cellMatrix == null) ? (basePosition.y + Mathf.Max(unit.height, _radius + maxClimbHeight)) : (cellMatrix.origin.y + cellMatrix.upperBoundary));
		Vector3 vector = input.currentFullVelocity.OnlyXZ() * input.deltaTime;
		if (vector.sqrMagnitude < 0.0001f && input.currentFullVelocity.y > 0f)
		{
			vector = input.currentPlanarVelocity.normalized * 0.01f;
		}
		if (!Physics.SphereCast(basePosition + vector, _radius, Vector3.down, out var hitInfo, float.PositiveInfinity, Layers.terrain))
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
