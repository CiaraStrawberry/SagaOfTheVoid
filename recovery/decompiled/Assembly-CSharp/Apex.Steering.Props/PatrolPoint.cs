using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Props;

[AddComponentMenu("Apex/Legacy/Navigation/Patrol Point (OLD)", 1021)]
public class PatrolPoint : MonoBehaviour, IPositioned
{
	public int orderIndex;

	public bool useTransformPosition = true;

	public Vector3 location;

	public Vector3 position
	{
		get
		{
			if (useTransformPosition)
			{
				return transform.position;
			}
			return location;
		}
	}

	private void OnDrawGizmos()
	{
		DrawGizmos(selected: false);
	}

	private void OnDrawGizmosSelected()
	{
		DrawGizmos(selected: true);
	}

	private void DrawGizmos(bool selected)
	{
		if (enabled)
		{
			Color yellow = Color.yellow;
			if (selected)
			{
				Gizmos.color = yellow;
				Gizmos.DrawSphere(position, 0.2f);
			}
		}
	}
}
