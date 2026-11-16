using UnityEngine;

namespace Apex.Steering.Behaviours;

[AddComponentMenu("")]
public class PatrolPointsComponent : MonoBehaviour
{
	public Color pointColor = new Color(0f, 150f / 255f, 150f / 255f);

	public Color textColor = Color.white;

	[Tooltip("Controls whether the patrol points are seen as relative to their parent transform. If relative they will move with the transform otherwise they will be static and remain where they were placed initially. It is recommended only to have this set to true when moving the parent and then set it back to false.")]
	public bool relativeToTransform;

	[Tooltip("The waypoints that make up the patrol route.")]
	public Vector3[] points = new Vector3[0];

	public Vector3[] worldPoints
	{
		get
		{
			if (relativeToTransform)
			{
				Vector3 position = transform.position;
				Vector3[] array = new Vector3[points.Length];
				for (int i = 0; i < array.Length; i++)
				{
					ref Vector3 reference = ref array[i];
					reference = points[i] + position;
				}
				return array;
			}
			return points;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = pointColor;
		Vector3[] array = worldPoints;
		foreach (Vector3 vector in array)
		{
			Vector3 vector2 = vector;
			vector2.y++;
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawSphere(vector2, 0.3f);
		}
	}
}
