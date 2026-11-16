using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
	public Transform target;

	public Transform[] layers;

	private void LateUpdate()
	{
		if (!(target == null))
		{
			Vector3 position = base.transform.position;
			position = target.transform.position;
			position.y = 10f;
			Vector3 vector = position - base.transform.position;
			base.transform.position = position;
			int num = 2;
			Transform[] array = layers;
			foreach (Transform transform in array)
			{
				transform.Translate(-vector / num, Space.World);
				num *= 2;
			}
		}
	}
}
