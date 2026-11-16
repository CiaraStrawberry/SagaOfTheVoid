using UnityEngine;

public class PlaygroundAutoRotate : MonoBehaviour
{
	public float rotationSpeed = 10f;

	private Transform thisTransform;

	private void Start()
	{
		thisTransform = transform;
	}

	private void Update()
	{
		thisTransform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
