using UnityEngine;

public class PlaygroundRotateWithKeys : MonoBehaviour
{
	public float speed = 200f;

	private Transform thisTransform;

	private void Start()
	{
		thisTransform = transform;
	}

	private void Update()
	{
		thisTransform.RotateAround(Vector3.zero, thisTransform.up, (0f - Input.GetAxis("Horizontal")) * speed * Time.deltaTime);
		thisTransform.RotateAround(Vector3.zero, thisTransform.right, Input.GetAxis("Vertical") * speed * Time.deltaTime);
	}
}
