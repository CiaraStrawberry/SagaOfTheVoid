using UnityEngine;

public class F3DCockpitCamera : MonoBehaviour
{
	public Transform BackgroundCamera;

	public float RotationTime = 5f;

	private float mouseX;

	private float mouseY;

	private void Start()
	{
	}

	private void Update()
	{
		mouseX += Input.GetAxis("Mouse X");
		mouseY -= Input.GetAxis("Mouse Y");
		mouseY = Mathf.Clamp(mouseY, -90f, 90f);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(mouseY, mouseX, 0f), Time.deltaTime * RotationTime);
		if ((bool)BackgroundCamera)
		{
			BackgroundCamera.rotation = Quaternion.Euler(17.5f, 0f, 0f) * transform.rotation;
		}
	}
}
