using UnityEngine;

public class RelationPositionTransform : MonoBehaviour
{
	public Transform anchor;

	public Transform target;

	public float anchorToTargetDistance = 0.2f;

	public float slack;

	public bool rotateTowardsTarget;

	private Transform thisTransform;

	private void Start()
	{
		thisTransform = transform;
	}

	private void Update()
	{
		if (anchor == null || target == null)
		{
			return;
		}
		if (slack > 0f)
		{
			thisTransform.position = Vector3.Lerp(thisTransform.position, Vector3.Lerp(anchor.position, target.position, anchorToTargetDistance), Time.deltaTime / slack);
		}
		else
		{
			thisTransform.position = Vector3.Lerp(anchor.position, target.position, anchorToTargetDistance);
		}
		if (rotateTowardsTarget)
		{
			if (slack > 0f)
			{
				thisTransform.rotation = Quaternion.RotateTowards(thisTransform.rotation, Quaternion.LookRotation(thisTransform.position - target.position, Vector3.down) * new Quaternion(-90f, 0f, 90f, 1f), Time.deltaTime / slack);
			}
			else
			{
				thisTransform.rotation = Quaternion.LookRotation(thisTransform.position - target.position, Vector3.down) * new Quaternion(-90f, 0f, 90f, 1f);
			}
		}
	}
}
