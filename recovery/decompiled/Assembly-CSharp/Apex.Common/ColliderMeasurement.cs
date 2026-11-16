using UnityEngine;

namespace Apex.Common;

public class ColliderMeasurement
{
	public Vector3 size { get; private set; }

	public Vector3 extents { get; private set; }

	public Vector3 center { get; private set; }

	public Vector3 transformCenterOffset { get; private set; }

	public ColliderMeasurement(Collider c, bool inWorldSpace)
	{
		if (c is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = c as CapsuleCollider;
			if (inWorldSpace)
			{
				Transform transform = capsuleCollider.transform;
				Vector3 lossyScale = transform.lossyScale;
				center = transform.TransformPoint(capsuleCollider.center);
				transformCenterOffset = center - transform.position;
				float num = 2f * capsuleCollider.radius * Mathf.Max(lossyScale.x, lossyScale.z);
				float num2 = Mathf.Max(capsuleCollider.height * lossyScale.y, num);
				switch (capsuleCollider.direction)
				{
				case 0:
					size = new Vector3(num2, num, num);
					break;
				case 1:
					size = new Vector3(num, num2, num);
					break;
				case 2:
					size = new Vector3(num, num, num2);
					break;
				}
			}
			else
			{
				center = capsuleCollider.center;
				transformCenterOffset = center;
				float num3 = capsuleCollider.radius * 2f;
				float num4 = Mathf.Max(capsuleCollider.height, num3);
				switch (capsuleCollider.direction)
				{
				case 0:
					size = new Vector3(num4, num3, num3);
					break;
				case 1:
					size = new Vector3(num3, num4, num3);
					break;
				case 2:
					size = new Vector3(num3, num3, num4);
					break;
				}
			}
		}
		else if (c is SphereCollider)
		{
			SphereCollider sphereCollider = c as SphereCollider;
			if (inWorldSpace)
			{
				Transform transform2 = sphereCollider.transform;
				Vector3 lossyScale2 = transform2.lossyScale;
				float num5 = 2f * sphereCollider.radius * Mathf.Max(lossyScale2.x, lossyScale2.y, lossyScale2.z);
				size = new Vector3(num5, num5, num5);
				center = transform2.TransformPoint(sphereCollider.center);
				transformCenterOffset = center - transform2.position;
			}
			else
			{
				float num6 = 2f * sphereCollider.radius;
				size = new Vector3(num6, num6, num6);
				center = sphereCollider.center;
				transformCenterOffset = center;
			}
		}
		else if (c is BoxCollider)
		{
			BoxCollider boxCollider = c as BoxCollider;
			if (inWorldSpace)
			{
				Transform transform3 = boxCollider.transform;
				Vector3 lossyScale3 = transform3.lossyScale;
				size = new Vector3(boxCollider.size.x * lossyScale3.x, boxCollider.size.y * lossyScale3.y, boxCollider.size.z * lossyScale3.z);
				center = transform3.TransformPoint(boxCollider.center);
				transformCenterOffset = center - transform3.position;
			}
			else
			{
				size = boxCollider.size;
				center = boxCollider.center;
				transformCenterOffset = center;
			}
		}
		else if (c is MeshCollider)
		{
			MeshCollider meshCollider = c as MeshCollider;
			Transform transform4 = meshCollider.transform;
			Quaternion rotation = transform4.rotation;
			transform4.rotation = Quaternion.identity;
			if (inWorldSpace)
			{
				size = meshCollider.bounds.size;
				center = meshCollider.bounds.center;
				transformCenterOffset = center - transform4.position;
			}
			else
			{
				Vector3 lossyScale4 = transform4.lossyScale;
				Vector3 vector = meshCollider.bounds.size;
				size = new Vector3(vector.x / lossyScale4.x, vector.y / lossyScale4.y, vector.z / lossyScale4.z);
				center = transform4.InverseTransformPoint(meshCollider.bounds.center);
				transformCenterOffset = center;
			}
			transform4.rotation = rotation;
		}
		extents = size * 0.5f;
	}
}
