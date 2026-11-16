using UnityEngine;

[ExecuteInEditMode]
public class SgtRaycastDepth : SgtDepth
{
	[Tooltip("For the depth to return 1, the raycast must go through an object with this thickness in world space")]
	public float MaxThickness = 1f;

	public static SgtRaycastDepth CreateDepthRaycast(int layer = 0, Transform parent = null)
	{
		return CreateDepthRaycast(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtRaycastDepth CreateDepthRaycast(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Raycast Depth", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtRaycastDepth>();
	}

	protected override float DoCalculate(Vector3 eye, Vector3 target)
	{
		float result = 0f;
		if (MaxThickness > 0f)
		{
			Vector3 vector = Vector3.Normalize(target - eye);
			float maxDistance = Vector3.Distance(eye, target);
			RaycastHit hitInfo = default;
			if (Physics.Raycast(eye, vector, out hitInfo, maxDistance, Layers))
			{
				RaycastHit hitInfo2 = default;
				result = 1f;
				if (Physics.Raycast(target, -vector, out hitInfo2, maxDistance, Layers))
				{
					float num = Vector3.Distance(hitInfo.point, hitInfo2.point);
					if (num < MaxThickness)
					{
						result = num / MaxThickness;
					}
				}
			}
		}
		return result;
	}
}
