using UnityEngine;

public class SDP01_LookAtObject : MonoBehaviour
{
	private Transform myTransform;

	public Transform TargetToLookAt;

	private void Start()
	{
		myTransform = gameObject.transform;
	}

	private void Update()
	{
		if (TargetToLookAt != null)
		{
			myTransform.LookAt(TargetToLookAt.position);
		}
	}
}
