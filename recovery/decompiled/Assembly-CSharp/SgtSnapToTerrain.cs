using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Snap To Terrain")]
public class SgtSnapToTerrain : MonoBehaviour
{
	[Tooltip("The terrain we want this GameObject to snap to")]
	public SgtTerrain Terrain;

	[Tooltip("Enable this if you want the position to be snapped")]
	public bool SnapPosition = true;

	[Tooltip("This allows you to set how far from the surface this GameObject will be snapped")]
	public float SnapOffset;

	[Tooltip("How fast the current GameObject moves to the target position (0 = instant)")]
	public float SnapMoveDampening;

	[Tooltip("Enable this if you want the rotation to be snapped")]
	public bool SnapRotation = true;

	[Tooltip("This allows you to set how far apart the right/left height samples will be. Increasing this can make the rotations smoother")]
	public float SnapRightDistance = 0.1f;

	[Tooltip("This allows you to set how far apart the forward/back height samples will be. Increasing this can make the rotations smoother")]
	public float SnapForwardDistance = 0.1f;

	[Tooltip("How fast the current GameObject rotates to the target position (0 = instant)")]
	public float SnapTurnDampening;

	public static void SnapTransformPosition(SgtTerrain terrain, Transform transform, float offset = 0f, float dampening = 0f)
	{
		if (!(terrain != null) || !(transform != null))
		{
			return;
		}
		Vector3 position = transform.position;
		Vector3 worldPoint = terrain.GetWorldPoint(position, offset);
		if (position != worldPoint)
		{
			if (dampening > 0f)
			{
				transform.position = SgtHelper.Dampen3(transform.position, worldPoint, dampening, Time.deltaTime);
			}
			else
			{
				transform.position = worldPoint;
			}
		}
	}

	public static void SnapTransformRotation(SgtTerrain terrain, Transform transform, float rightDistance = 1f, float forwardDistance = 1f, float dampening = 0f)
	{
		if (terrain != null && transform != null)
		{
			Vector3 vector = default;
			if (rightDistance != 0f && forwardDistance != 0f)
			{
				Vector3 right = transform.right * rightDistance;
				Vector3 forward = transform.forward * forwardDistance;
				vector = terrain.GetWorldNormal(transform.position, right, forward);
			}
			else
			{
				vector = terrain.GetWorldNormal(transform.position);
			}
			Quaternion rotation = transform.rotation;
			Quaternion quaternion = Quaternion.FromToRotation(transform.up, vector) * rotation;
			if (dampening > 0f)
			{
				transform.rotation = SgtHelper.Dampen(transform.rotation, quaternion, dampening, Time.deltaTime);
			}
			else
			{
				transform.rotation = quaternion;
			}
		}
	}

	public void UpdateSnap()
	{
		if (SnapPosition)
		{
			SnapTransformPosition(Terrain, transform, SnapOffset, SnapMoveDampening);
		}
		if (SnapRotation)
		{
			SnapTransformRotation(Terrain, transform, SnapRightDistance, SnapForwardDistance, SnapTurnDampening);
		}
	}

	protected virtual void Update()
	{
		UpdateSnap();
	}
}
