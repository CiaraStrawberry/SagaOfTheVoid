using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class SmoothFollow : MonoBehaviour
{
	[Required]
	public Transform target;

	public float distance = 10f;

	public float height = 5f;

	public float heightDamping = 2f;

	public float rotationDamping = 3f;

	protected void LateUpdate()
	{
		Quaternion quaternion = Quaternion.Slerp(transform.rotation, target.rotation, rotationDamping * Time.deltaTime);
		float y = Mathf.Lerp(transform.position.y, target.position.y + target.up.y * height, heightDamping * Time.deltaTime);
		Vector3 position = target.position;
		position -= quaternion * target.forward * distance;
		position.y = y;
		transform.position = position;
		transform.rotation = quaternion;
	}
}
